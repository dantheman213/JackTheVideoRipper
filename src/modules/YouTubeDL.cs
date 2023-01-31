using System.Diagnostics;
using JackTheVideoRipper.extensions;
using JackTheVideoRipper.models.enums;
using JackTheVideoRipper.models.parameters;
using Nager.PublicSuffix;

namespace JackTheVideoRipper
{
    internal static class YouTubeDL
    {
        #region Data Members
        
        public static readonly string ExecutablePath = FileSystem.ProgramPath(Executables.YouTubeDL);

        public const string DEFAULT_FORMAT = "%(title)s.%(ext)s";
        
        public const StringComparison DEFAULT_COMPARISON = StringComparison.OrdinalIgnoreCase;
        
        private static readonly Command _Command = new(ExecutablePath, throwOnValidationFailed:true);
        
        private static FileVersionInfo _fileVersionInfo = GetVersion();

        #endregion

        #region Attributes

        public static string DefaultFilename => FileSystem.CreateDownloadPath(DEFAULT_FORMAT);

        public static bool IsInstalled => File.Exists(ExecutablePath);
        
        public static bool UpToDate => PreviousVersion == CurrentVersion;

        public static string GetYouTubeLink(string id) => string.Format(Urls.YouTubeVideo, id);
        
        public static IEnumerable<string> SupportedServices => _supportedServices ?? Array.Empty<string>();

        private static string _supportedServicesString = string.Empty;

        private static string[]? _supportedServices;
        
        private static string PreviousVersion
        {
            get => Settings.Data.LastVersionYouTubeDL;
            set
            {
                Settings.Data.LastVersionYouTubeDL = value;
                Settings.Save();
            }
        }

        public static string CurrentVersion => _fileVersionInfo.FileVersion ?? "N/A";

        #endregion

        #region Public Methods

        public static async Task StartupTasks()
        {
            _supportedServicesString = await GetExtractors();
            _supportedServices = _supportedServicesString.SplitNewline().ToArray();
        }

        public static async void CheckDownload()
        {
            if (!IsInstalled)
                await DownloadAndInstall();
        }

        public static async Task DownloadAndInstall()
        {
            if (IsInstalled)
                return;
                
            await FileSystem.InstallProgram(Urls.YouTubeDLDownload, Executables.YouTubeDL);
        }

        public static async Task CheckForUpdates()
        {
            if (!IsInstalled || UpToDate)
                return;

            await UpdateInstallation();

            Modals.Notification(string.Format(Messages.YouTubeDLUpdated, PreviousVersion, CurrentVersion),
                @"yt-dlp update");
        }

        public static async Task<IEnumerable<PlaylistInfoItem>> GetPlaylistMetadata(string url)
        {
            return await _Command.ReceiveMultiJsonResponse<PlaylistInfoItem>(url, Params.PlaylistMetadata);
        }

        public static async Task<MediaInfoData?> GetMediaData(string url)
        {
            return await _Command.ReceiveJsonResponse<MediaInfoData>(url, Params.MediaData);
        }

        public static async Task<string> GetTitle(string url, bool useCommand = true)
        {
            if (useCommand && url.Valid(FileSystem.IsValidUrl) && url.Contains("youtube", DEFAULT_COMPARISON))
                return await GetTitleYouTubeDL(url);

            return await GetTitleWebQuery(url);
        }

        public static Process CreateCommand(string parameters)
        {
            return _Command.CreateCommand(parameters);
        }
        
        public static DownloadStage GetDownloadStage(string line)
        {
            if (line.IsNullOrEmpty())
                return DownloadStage.None;
            if (line.Contains(Tags.YOUTUBE) || line.Contains(Tags.YOUTUBE_TAB) || line.Contains(Tags.INFO))
                return DownloadStage.Retrieving;
            if (line.Contains(Tags.METADATA))
                return DownloadStage.Metadata;
            if (line.Contains(Tags.FFMPEG))
                return DownloadStage.Transcoding;
            if (line.Contains(Tags.DOWNLOAD))
                return DownloadStage.Downloading;
            if (line.Contains(Text.Error, DEFAULT_COMPARISON) || 
                line[..21].Contains(Text.Usage, DEFAULT_COMPARISON))
                return DownloadStage.Error;
            if (line.Contains('%'))
                return DownloadStage.Waiting;
            return DownloadStage.None;
        }

        public static bool IsSupported(string url)
        {
            return FileSystem.ParseUrl(url) is { } domainInfo &&
                   _supportedServicesString.Contains(domainInfo.Domain, DEFAULT_COMPARISON);
        }

        #endregion

        #region Private Methods
        
        private static void UpdateVersionInfo()
        {
            PreviousVersion = CurrentVersion;
            _fileVersionInfo = GetVersion();
        }

        private static async Task UpdateInstallation()
        {
            await _Command.RunCommandAsync(Params.UPDATE, FileSystem.Paths.Install);
            UpdateVersionInfo();
        }
        
        private static async Task<string> GetExtractors()
        {
            return await _Command.RunCommandAsync(Params.EXTRACTORS);
        }
        
        private static FileVersionInfo GetVersion()
        {
            return FileVersionInfo.GetVersionInfo(ExecutablePath);
        }

        private static async Task<string> GetVersionCLI()
        {
            return await _Command.RunCommandAsync(Params.VERSION);
        }
        
        private static async Task<string> GetTitleYouTubeDL(string url)
        {
            return await _Command.RunWebCommandAsync(url, Params.TITLE);
        }

        private static async Task<string> GetTitleWebQuery(string url)
        {
            HttpResponseMessage response = await FileSystem.SimpleWebQueryAsync(url);

            if (!response.IsSuccessStatusCode)
                return string.Empty;

            string title = Web.GetTitle(await response.GetResponseAsync());

            DomainInfo? domainInfo = FileSystem.ParseUrl(url);

            if (domainInfo is null)
                return title;

            string domain = $"{domainInfo.Domain}";
            string domainExtended = $"{domainInfo.Domain}.{domainInfo.TLD}";

            if (RemoveIfStartsWith(title, "-", domain) is { } domainWithDashStart)
                return domainWithDashStart;
            if (RemoveIfStartsWith(title, "|", domain) is { } domainWithPipeStart)
                return domainWithPipeStart;
            if (RemoveIfStartsWith(title, "-", domainExtended) is { } domainExtendedWithDashStart)
                return domainExtendedWithDashStart;
            if (RemoveIfStartsWith(title, "|", domainExtended) is { } domainExtendedWithPipeStart)
                return domainExtendedWithPipeStart;
            if (RemoveIfEndsWith(title, "-", domain) is { } domainWithDashEnd)
                return domainWithDashEnd;
            if (RemoveIfEndsWith(title, "|", domain) is { } domainWithPipeEnd)
                return domainWithPipeEnd;
            if (RemoveIfEndsWith(title, "-", domainExtended) is { } domainExtendedWithDashEnd)
                return domainExtendedWithDashEnd;
            if (RemoveIfEndsWith(title, "|", domainExtended) is { } domainExtendedWithPipeEnd)
                return domainExtendedWithPipeEnd;
            
            return title;
        }
        
        private static string? RemoveIfStartsWith(string title, string separator, string domainText)
        {
            string text = $"{domainText} {separator}";
            return title.StartsWith(text, DEFAULT_COMPARISON) ?
                title.Remove(text, DEFAULT_COMPARISON).Trim() : null;
        }
        
        private static string? RemoveIfEndsWith(string title, string separator, string domainText)
        {
            string text = $"{separator} {domainText}";
            return title.EndsWith(text, DEFAULT_COMPARISON) ?
                title.Remove(text, DEFAULT_COMPARISON).Trim() : null;
        }

        #endregion

        #region Embedded Types

        public class LinkNotSupportedException : Exception
        {
            public LinkNotSupportedException(string message) : base(message)
            {
            }
        }
        
        public static class Params
        {
            public static readonly YouTubeParameters NoWarningsNoCache = new YouTubeParameters()
                .NoWarnings()
                .NoCache();
            public static readonly YouTubeParameters PlaylistMetadata = NoWarningsNoCache
                .IgnoreErrors()
                .DumpJson()
                .FlatPlaylist()
                .SkipDownload()
                .YesPlaylist();
            public static readonly YouTubeParameters MediaData = NoWarningsNoCache
                .Simulate()
                .PrintJson();
            public static readonly YouTubeParameters Default = NoWarningsNoCache
                .PreferFfmpeg()
                .FfmpegLocation();
            public const string EXTRACTORS = "--list-extractors";
            public const string VERSION = "--version";
            public const string TITLE = "--get-title";
            public const string UPDATE = "-U";
        }

        #endregion
    }
}
