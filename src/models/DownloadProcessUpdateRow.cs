using System.Diagnostics;
using System.Net;
using JackTheVideoRipper.extensions;
using JackTheVideoRipper.interfaces;
using JackTheVideoRipper.models.enums;
using JackTheVideoRipper.Properties;
using Nager.PublicSuffix;

namespace JackTheVideoRipper.models;

public class DownloadProcessUpdateRow : ProcessUpdateRow
{
    #region Data Members

    private bool _redirected;

    private string RedirectedUrl = string.Empty;

    private DownloadStage DownloadStage = DownloadStage.None;
    
    private const string ALREADY_DOWNLOADED = "has already been downloaded";

    public string OriginalUrl => base.Url;

    public new string Url
    {
        get => _redirected ? RedirectedUrl : OriginalUrl;
        set
        {
            _redirected = true;
            RedirectedUrl = value;
        }
    }

    public new string ParameterString => _redirected ?
        base.ParameterString.Replace(OriginalUrl, Url) :
        base.ParameterString;

    #endregion

    #region Constructor

    public DownloadProcessUpdateRow(IMediaItem mediaItem, Action<IProcessRunner> completionCallback) :
        base(mediaItem, completionCallback)
    {
    }
        
    #endregion

    #region Overrides
    
    protected override Process CreateProcess()
    {
        return YouTubeDL.CreateCommand(ParameterString);
    }

    // Extract elements of CLI output from YouTube-DL
    protected override void SetProgressText(IReadOnlyList<string> tokens)
    {
        if (tokens.Count < 8 || DownloadStage != DownloadStage.Downloading)
            return;
        Progress = tokens[1];
        FileSize = FormatSize(tokens[3]);
        DownloadSpeed = tokens[5];
        Eta = tokens[7];
    }

    protected override string? GetStatus()
    {
        if (Buffer.ProcessLine is not { } line || line.IsNullOrEmpty())
            return string.Empty;

        DownloadStage = YouTubeDL.GetDownloadStage(line);

        return DownloadStage switch
        {
            DownloadStage.Waiting       => Messages.WAITING,
            DownloadStage.Retrieving    => Messages.RETRIEVING,
            DownloadStage.Transcoding   => Messages.TRANSCODING,
            DownloadStage.Downloading   => Messages.DOWNLOADING,
            DownloadStage.Metadata      => Messages.METADATA,
            DownloadStage.Error         => null,
            _                           => string.Empty
        };
    }

    public override async Task<bool> Start()
    {
        // Called before process starts
        if (!await PreDownloadTasks())
            return false;

        if (!YouTubeDL.IsSupported(Url))
        {
            string? domain = FileSystem.ParseUrl(Url)?.Domain.WrapQuotes();
            Fail(new YouTubeDL.LinkNotSupportedException(string.Format(Resources.LinkNotSupported, domain)));
            return false;
        }
        
        return await base.Start();
    }

    public override void OnProcessExit(object? o, EventArgs eventArgs)
    {
        base.OnProcessExit(o, eventArgs);
        
        // Editing the GUI elements must be in the main thread context to avoid errors
        Core.RunTaskInMainThread(PostDownloadTasks);
    }

    #endregion

    #region Public Methods

    public string GetFilepath()
    {
        if (Buffer.GetResultWhichContains(Tags.DOWNLOAD) is not { } download)
            return Tag;

        string line = download.After(Tags.DOWNLOAD).Trim();

        return line.Contains(ALREADY_DOWNLOADED, StringComparison.OrdinalIgnoreCase) ?
            line.Before(ALREADY_DOWNLOADED).Trim() :
            line.After("Destination: ").Trim();
    }

    #endregion

    #region Private Methods

    private async Task<bool> PreDownloadTasks()
    {
        if (FileSystem.ParseUrl(OriginalUrl) is not { } domainInfo || !VideoProxy.IsProxyLink(domainInfo))
            return true;

        try
        {
            RedirectedUrl = await GetRedirectedLink(VideoProxy.GetProxyType(domainInfo.Domain), OriginalUrl);
        }
        catch (Exception exception)
        {
            Fail(exception);
            return false;
        }

        HttpResponseMessage resourceStatus = await Web.GetResourceStatus(RedirectedUrl);

        switch (resourceStatus.StatusCode)
        {
            case HttpStatusCode.NotFound:
                Fail(new WebException($"Resource could not be found at: {RedirectedUrl.WrapQuotes()} (Error {resourceStatus.ResponseCode()})"));
                return false;
        }
        
        _redirected = RedirectedUrl != OriginalUrl;
        if (_redirected)
        {
            string oldSite = domainInfo.Domain.WrapQuotes();
            string newSite = FileSystem.ParseUrl(RedirectedUrl)?.Domain.WrapQuotes().ValueOrDefault()!;
            Buffer.AddLog($"Redirected from video proxy site {oldSite} to {newSite}", ProcessLogType.Info);
        }
        
        return true;
    }

    private static async Task<string> GetRedirectedLink(VideoProxyType proxyType, string url)
    {
        return proxyType switch
        {
            VideoProxyType.Pornkai => await GetPornkaiRedirect(url),
            _ => url
        };
    }

    private static async Task<string> GetXHamsterLink(string id)
    {
        string redirectedUrl = await Web.GetRedirectedUrl($"https://xhamster.com/embed/{id}");
        string redirectedId = new Uri(redirectedUrl).Segments.Last();
        return $"https://xhamster.com/videos?id={redirectedId}";
    }

    private static async Task<string> GetPornkaiRedirect(string url)
    {
        string id = url.After("key=").Before("&");
        switch (id.Length)
        {
            // XHamster
            case 4:
            case 5:
                return await GetXHamsterLink(id);
            case > 2:
                switch (id[..2])
                {
                    case "xh":
                        return await GetXHamsterLink(id);
                    case "ph":
                        return $"https://www.pornhub.com/view_video.php?viewkey={id}";
                    case "xv":
                        // xvideos won't redirect without a character after the slash?
                        return $"https://www.xvideos.com/video{id.After("xv")}/a";
                }
                break;
        }

        throw new NotImplementedException($"Pornkai link not supported! ({url.WrapQuotes()})");
    }
    
    private async Task PostDownloadTasks()
    {
        if (Path.IsNullOrEmpty())
            Path = GetFilepath();

        if (FileSize.IsNullOrEmpty() || FileSize is Text.DEFAULT_SIZE or "~" || IsFilesizeNegligible())
            FileSize = FileSystem.GetFileSizeFormatted(Path);
        
        if (Title.IsNullOrEmpty())
            Title = await YouTubeDL.GetTitle(Url, false);

        SendProcessCompletedNotification();
            
        // Update history information
        History.Data.UpdateFileInformation(Tag, Path, FileSize);
    }

    private static string FormatSize(string size)
    {
        if (size.Contains("MiB"))
            return size.Replace("MiB", " MB");
        if (size.Contains("GiB"))
            return size.Replace("GiB", " GB");
        if (size.Contains("TeB"))
            return size.Replace("TeB", " TB");
        return size;
    }
    
    private bool IsFilesizeNegligible()
    {
        return float.TryParse(FileSize.Split()[0], out float size) && size < 0.001;
    }

    private void SendProcessCompletedNotification()
    {
        if (Succeeded)
        {
            string notificationMessage = $"{Title.WrapQuotes()} finished downloading!";
            string shortenedMessage = $"{Title.TruncateEllipse(35).WrapQuotes()} finished downloading!";
            NotificationsManager.SendNotification(new Notification(notificationMessage, this, shortenedMessage));
        }
        else if (GetError() is { } errorMessage && errorMessage.HasValue())
        {
            NotificationsManager.SendNotification(new Notification($"Failed to download: {errorMessage}", this));
        }
    }

    #endregion
}