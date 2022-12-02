using System.Diagnostics;
using JackTheVideoRipper.extensions;
using JackTheVideoRipper.interfaces;
using JackTheVideoRipper.models.enums;

namespace JackTheVideoRipper.models;

public class DownloadProcessUpdateRow : ProcessUpdateRow
{
    #region Attributes

    private DownloadStage DownloadStage = DownloadStage.None;

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

    public override void OnProcessExit(object? o, EventArgs eventArgs)
    {
        base.OnProcessExit(o, eventArgs);
        
        Core.InvokeInMainContext(() =>
        {
            if (Path.IsNullOrEmpty())
                Path = GetFilepath();

            if (FileSize.IsNullOrEmpty() || FileSize is Text.DEFAULT_SIZE or "~" || IsFilesizeNegligible())
                FileSize = FileSystem.GetFileSizeFormatted(Path);
        });
    }

    #endregion

    #region Public Methods

    private const string ALREADY_DOWNLOADED = "has already been downloaded";

    public string GetFilepath()
    {
        if (Buffer.Results.FirstOrDefault(r => r.Contains(Tags.DOWNLOAD)) is not { } download)
            return Tag;

        string line = download.After(Tags.DOWNLOAD).Trim();

        return line.Contains(ALREADY_DOWNLOADED, StringComparison.OrdinalIgnoreCase) ?
            line.Before(ALREADY_DOWNLOADED) :
            line.After("Destination: ");
    }

    #endregion

    #region Private Methods

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
    

    #endregion
}