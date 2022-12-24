using System.Diagnostics;
using JackTheVideoRipper.extensions;
using JackTheVideoRipper.models;
using JackTheVideoRipper.models.enums;
using JackTheVideoRipper.modules;
using JackTheVideoRipper.Properties;
using Nager.PublicSuffix;

namespace JackTheVideoRipper
{
    internal static class YouTubeDL
    {
        #region Data Members

        private const string _EXECUTABLE_NAME = "yt-dlp.exe";
        public static readonly string ExecutablePath = FileSystem.ProgramPath(_EXECUTABLE_NAME);
        public const string UPDATE_URL = "https://github.com/yt-dlp/yt-dlp";
        private const string _DOWNLOAD_URL =  $"{UPDATE_URL}/releases/latest/download/yt-dlp.exe";

        public const string DEFAULT_FORMAT = "%(title)s.%(ext)s";
        
        public const StringComparison DEFAULT_COMPARISON = StringComparison.OrdinalIgnoreCase;
        
        private static readonly Command _Command = new(ExecutablePath);

        #endregion

        #region Attributes

        public static string DefaultFilename => Path.Combine(Settings.Data.DefaultDownloadPath, DEFAULT_FORMAT);

        public static bool IsInstalled => File.Exists(ExecutablePath);
        
        public static bool UpToDate => PreviousVersion == CurrentVersion;
        
        public static string GetYouTubeLink(string id) => $"https://www.youtube.com/watch?v={id}";
        
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

        private static string _currentVersion = string.Empty;

        public static string CurrentVersion
        {
            get
            {
                if (_currentVersion.IsNullOrEmpty())
                    _currentVersion = GetVersion();
                return _currentVersion;
            }
        }

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
                
            await FileSystem.InstallProgram(_DOWNLOAD_URL, _EXECUTABLE_NAME);
        }

        public static void CheckForUpdates()
        {
            if (!IsInstalled || UpToDate)
                return;

            UpdateInstallation();

            Modals.Notification(string.Format(Resources.YouTubeDLUpdated, PreviousVersion, CurrentVersion),
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
            if (line.Contains(Text.ERROR, DEFAULT_COMPARISON) || 
                line[..21].Contains(Text.Usage, DEFAULT_COMPARISON))
                return DownloadStage.Error;
            if (line.Contains('%'))
                return DownloadStage.Waiting;
            return DownloadStage.None;
        }

        public static bool IsSupported(string url)
        {
            if (FileSystem.ParseUrl(url) is not { } domainInfo)
                return false;

            return _supportedServicesString.Contains(domainInfo.Domain, DEFAULT_COMPARISON);
        }

        #endregion

        #region Private Methods

        private static void UpdateInstallation()
        {
            _Command.RunCommand(Params.Update, FileSystem.Paths.Install);
            PreviousVersion = CurrentVersion;
        }
        
        private static async Task<string> GetExtractors()
        {
            return await _Command.RunCommandAync(Params.Extractors);
        }

        private static string GetVersion()
        {
            return _Command.RunCommand(Params.Version);
        }
        
        private static async Task<string> GetTitleYouTubeDL(string url)
        {
            return await _Command.RunWebCommandAsync(url, Params.Title);
        }

        private static async Task<string> GetTitleWebQuery(string url)
        {
            HttpResponseMessage response = await FileSystem.SimpleWebQueryAsync(url);

            if (!response.IsSuccessStatusCode)
                return string.Empty;

            string title = Web.GetTitle(response.GetResponse());

            DomainInfo? domainInfo = FileSystem.ParseUrl(url);

            if (domainInfo is null)
                return title;

            string domain = $"{domainInfo.Domain}";
            string domainExtended = $"{domainInfo.Domain}.{domainInfo.TLD}";

            if (RemoveIfEndsWith(title, "-", domain) is { } domainWithDash)
                return domainWithDash;
            if (RemoveIfEndsWith(title, "|", domain) is { } domainWithPipe)
                return domainWithPipe;
            if (RemoveIfEndsWith(title, "-", domainExtended) is { } domainExtendedWithDash)
                return domainExtendedWithDash;
            if (RemoveIfEndsWith(title, "|", domainExtended) is { } domainExtendedWithPipe)
                return domainExtendedWithPipe;
            
            return title;
        }
        
        private static string? RemoveIfEndsWith(string title, string separator, string domainText)
        {
            string text = $" {separator} {domainText}";
            return title.EndsWith(text, DEFAULT_COMPARISON) ?
                title.Remove(text, DEFAULT_COMPARISON) : null;
        }

        #endregion

        #region Embedded Types

        public class LinkNotSupportedException : Exception
        {
            public LinkNotSupportedException(string message) : base(message)
            {
            }
        }
        
        private static class Params
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
            public const string Extractors = "--list-extractors";
            public const string Version = "--version";
            public const string Title = "--get-title";
            public const string Update = "-U";
        }

        public class YouTubeParameters : ProcessParameters<YouTubeParameters>
        {
            public YouTubeParameters AddMetadata()
            {
                return AddNoValue("add-metadata");
            }
            
            public YouTubeParameters IncludeAds()
            {
                return AddNoValue("include-ads");
            }
            
            public YouTubeParameters RestrictFilenames()
            {
                return AddNoValue("restrict-filenames");
            }
            
            public YouTubeParameters PreferFfmpeg()
            {
                return AddNoValue("prefer-ffmpeg");
            }
            
            public YouTubeParameters NoCheckCertificate()
            {
                return AddNoValue("no-check-certificate");
            }
            
            public YouTubeParameters Source(string url)
            {
                return Append(url.WrapQuotes());
            }
            
            public YouTubeParameters Output(string? filename = null)
            {
                // youtube-dl doesn't like it when you provide --audio-format and extension in -o together
                string outputFilename = filename.HasValue() ? ReplaceExtension(filename!) : DefaultFilename;
                return Add('o', outputFilename.WrapQuotes());
            }

            private static string ReplaceExtension(string filename)
            {
                return $"{(filename.Contains('.') ? filename.BeforeLast(".") : filename)}.%(ext)s";
            }
            
            public YouTubeParameters AudioQuality(string qualitySpecifier = "0")
            {
                return Add("audio-quality", qualitySpecifier);
            }
            
            public YouTubeParameters AudioFormat(string format)
            {
                return Add("audio-format", format);
            }
            
            public YouTubeParameters ExtractAudio()
            {
                return AddNoValue('x');
            }
            
            public YouTubeParameters EmbedSubtitles()
            {
                return AddNoValue("embed-subs");
            }
            
            public YouTubeParameters EmbedThumbnail()
            {
                return AddNoValue("embed-thumbnail");
            }
            
            public YouTubeParameters RecodeVideo(string videoFormat)
            {
                return Add("recode-video", videoFormat);
            }

            public YouTubeParameters Format(string? videoFormat = null, string? audioFormat = null, bool useBest = false)
            {
                string formatSpecifier = videoFormat switch
                {
                    not null when audioFormat is not null   => $"{videoFormat}+{audioFormat}",
                    not null                                => videoFormat,
                    null when audioFormat is not null       => audioFormat,
                    _                                       => "best"
                };

                string bestSpecifier = useBest && formatSpecifier is not "best" ? "/best" : string.Empty;
                
                return Add('f', $"{formatSpecifier}{bestSpecifier}");
            }
            
            public YouTubeParameters Username(string username)
            {
                return Add("username", username);
            }
            
            public YouTubeParameters Password(string password)
            {
                return Add("password", password);
            }
            
            public YouTubeParameters YesPlaylist()
            {
                return AddNoValue("yes-playlist");
            }
            
            public YouTubeParameters FlatPlaylist()
            {
                return AddNoValue("flat-playlist");
            }
            
            public YouTubeParameters DumpJson()
            {
                return AddNoValue("dump-json");
            }
            
            public YouTubeParameters PrintJson()
            {
                return AddNoValue("print-json");
            }
            
            public YouTubeParameters SkipDownload()
            {
                return AddNoValue("skip-download");
            }
            
            // Do not download or write to disk
            public YouTubeParameters Simulate()
            {
                return AddNoValue('s');
            }
            
            public YouTubeParameters IgnoreErrors()
            {
                return AddNoValue('i');
            }
            
            public YouTubeParameters NoWarnings()
            {
                return AddNoValue("no-warnings");
            }

            public YouTubeParameters NoCache()
            {
                return AddNoValue("no-cache-dir");
            }
            
            public YouTubeParameters ListExtractors()
            {
                return Append(Params.Extractors);
            }
            
            public YouTubeParameters GetTitle()
            {
                return Append(Params.Title);
            }
            
            public YouTubeParameters Version()
            {
                return Append(Params.Version);
            }

            public YouTubeParameters FfmpegLocation()
            {
                return Add("ffmpeg-location", FFMPEG.ExecutablePath);
            }
            
            public YouTubeParameters AllSubtitles()
            {
                return AddNoValue("all-subs");
            }

            public YouTubeParameters MergeOutputFormat(string format)
            {
                return Add("merge-output-format", format);
            }

            public YouTubeParameters ExternalDownloader(string downloaderFilepath)
            {
                return Add("external-downloader", downloaderFilepath.WrapQuotes());
            }
            
            public YouTubeParameters ExternalDownloader(string downloaderFilepath, string downloaderArgs)
            {
                return ExternalDownloader(downloaderFilepath).ExternalDownloaderArgs(downloaderArgs);
            }
            
            public YouTubeParameters ExternalDownloaderArgs(string downloaderArgs)
            {
                return Add("external-downloader-args", downloaderArgs.WrapQuotes());
            }

            public YouTubeParameters DownloadArchive(string archivePath)
            {
                return Add("download-archive", archivePath.WrapQuotes());
            }

            public YouTubeParameters Links(string linksPath)
            {
                return Add('a', linksPath.WrapQuotes());
            }
        }

        #endregion
    }
}
