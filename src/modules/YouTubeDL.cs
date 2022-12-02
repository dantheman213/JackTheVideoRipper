using System.Diagnostics;
using System.Text.RegularExpressions;
using JackTheVideoRipper.extensions;
using JackTheVideoRipper.models;
using JackTheVideoRipper.models.enums;
using JackTheVideoRipper.Properties;

namespace JackTheVideoRipper
{
    internal static class YouTubeDL
    {
        private const string _EXECUTABLE_NAME = "yt-dlp.exe";
        public static readonly string ExecutablePath = FileSystem.ProgramPath(_EXECUTABLE_NAME);
        public const string UPDATE_URL = "https://github.com/yt-dlp/yt-dlp";
        private const string _DOWNLOAD_URL =  $"{UPDATE_URL}/releases/latest/download/yt-dlp.exe";

        public const string DEFAULT_FORMAT = "%(title)s.%(ext)s";

        public static string DefaultFilename => Path.Combine(Settings.Data.DefaultDownloadPath, DEFAULT_FORMAT);
        
        private static readonly Command _Command = new(ExecutablePath);

        public static bool IsInstalled => File.Exists(ExecutablePath);
        
        public static bool UpToDate => PreviousVersion == CurrentVersion;
        
        public static string GetYouTubeLink(string id) => $"https://www.youtube.com/watch?v={id}";
        
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

        public static async void CheckDownload()
        {
            if (!IsInstalled)
                await DownloadAndInstall();
        }

        public static async Task DownloadAndInstall()
        {
            if (IsInstalled)
                return;
                
            await FileSystem.Install(_DOWNLOAD_URL, _EXECUTABLE_NAME);
        }

        public static void CheckForUpdates()
        {
            if (!IsInstalled || UpToDate)
                return;

            UpdateInstallation();

            Modals.Notification(string.Format(Resources.YouTubeDLUpdated, PreviousVersion, CurrentVersion),
                @"yt-dlp update");
        }

        public static IEnumerable<PlaylistInfoItem> GetPlaylistMetadata(string url)
        {
            return _Command.ReceiveMultiJsonResponse<PlaylistInfoItem>(url, Params.PlaylistMetadata);
        }

        public static MediaInfoData? GetMediaData(string url)
        {
            return _Command.ReceiveJsonResponse<MediaInfoData>(url, Params.MediaData);
        }

        public static IEnumerable<string> GetSupportedServices() => GetExtractors().SplitNewline();

        private static readonly Regex _GetTitlePattern =
            new(@"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase);

        public static string GetTitle(string url)
        {
            if (url.Valid(FileSystem.IsValidUrl) && url.Contains("youtube", StringComparison.OrdinalIgnoreCase))
                return _Command.RunWebCommand(url, Params.Title);
            
            HttpResponseMessage response = FileSystem.SimpleWebQuery(url);

            return response.IsSuccessStatusCode ? 
                _GetTitlePattern.Match(response.GetResponse()).Groups["Title"].Value.Remove("- YouTube") :
                string.Empty;
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
            if (line.Contains(Text.ERROR, StringComparison.OrdinalIgnoreCase) || 
                line[..21].Contains(Text.Usage, StringComparison.OrdinalIgnoreCase))
                return DownloadStage.Error;
            if (line.Contains('%'))
                return DownloadStage.Waiting;
            return DownloadStage.None;
        }

        #region Private Methods

        private static void UpdateInstallation()
        {
            _Command.RunCommand(Params.Update, FileSystem.Paths.Install);
            PreviousVersion = CurrentVersion;
        }
        
        private static string GetExtractors()
        {
            return _Command.RunCommand(Params.Extractors);
        }

        private static string GetVersion()
        {
            return _Command.RunCommand(Params.Version);
        }

        #endregion

        #region Embedded Types
        
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
            public const string Extractors = "--list-extractors";
            public const string Version = "--version";
            public const string Title = "--get-title";
            public const string Update = "-U";
        }

        public class YouTubeParameters : ProcessParameters
        {
            public YouTubeParameters AddMetadata()
            {
                return Add<YouTubeParameters>("--add-metadata");
            }
            
            public YouTubeParameters IncludeAds()
            {
                return Add<YouTubeParameters>("--include-ads");
            }
            
            public YouTubeParameters RestrictFilenames()
            {
                return Add<YouTubeParameters>("--restrict-filenames");
            }
            
            public YouTubeParameters PreferFfmpeg()
            {
                return Add<YouTubeParameters>("--prefer-ffmpeg");
            }
            
            public YouTubeParameters NoCheckCertificate()
            {
                return Add<YouTubeParameters>("--no-check-certificate");
            }
            
            public YouTubeParameters Source(string url)
            {
                return Add<YouTubeParameters>($"\"{url}\"");
            }
            
            public YouTubeParameters Output(string? filename = null)
            {
                // youtube-dl doesn't like it when you provide --audio-format and extension in -o together
                return Add<YouTubeParameters>(filename is not null
                    ? $" -o \"{(filename.Contains('.') ? filename.BeforeLast(".") : filename)}.%(ext)s\""
                    : $" -o\"{DefaultFilename}\"");
            }
            
            public YouTubeParameters AudioQuality(string qualitySpecifier = "0")
            {
                return Add<YouTubeParameters>($"--audio-quality {qualitySpecifier}");
            }
            
            public YouTubeParameters AudioFormat(string format)
            {
                return Add<YouTubeParameters>($"--audio-format {format}");
            }
            
            public YouTubeParameters ExtractAudio()
            {
                return Add<YouTubeParameters>("-x");
            }
            
            public YouTubeParameters EmbedSubtitles()
            {
                return Add<YouTubeParameters>("--embed-subs");
            }
            
            public YouTubeParameters EmbedThumbnail()
            {
                return Add<YouTubeParameters>("--embed-thumbnail");
            }
            
            public YouTubeParameters RecodeVideo(string videoFormat)
            {
                return Add<YouTubeParameters>($"--recode-video {videoFormat}");
            }
            
            public YouTubeParameters Format(string? videoFormat = null, string? audioFormat = null, bool useBest = false)
            {
                string formatSpecifier;
                
                if (videoFormat is not null && audioFormat is not null)
                {
                    formatSpecifier = $"{videoFormat}+{audioFormat}";
                }
                else if (videoFormat is not null)
                {
                    formatSpecifier = videoFormat;
                }
                else if (audioFormat is not null)
                {
                    formatSpecifier = audioFormat;
                }
                else
                {
                    return this;
                }
                
                return Add<YouTubeParameters>($"-f {formatSpecifier}{(useBest ? "/best" : string.Empty)}");
            }
            
            public YouTubeParameters Username(string username)
            {
                return Add<YouTubeParameters>($"--username {username}");
            }
            
            public YouTubeParameters Password(string password)
            {
                return Add<YouTubeParameters>($"--password {password}");
            }
            
            public YouTubeParameters YesPlaylist()
            {
                return Add<YouTubeParameters>("--yes-playlist");
            }
            
            public YouTubeParameters FlatPlaylist()
            {
                return Add<YouTubeParameters>("--flat-playlist");
            }
            
            public YouTubeParameters DumpJson()
            {
                return Add<YouTubeParameters>("--dump-json");
            }
            
            public YouTubeParameters PrintJson()
            {
                return Add<YouTubeParameters>("--print-json");
            }
            
            public YouTubeParameters SkipDownload()
            {
                return Add<YouTubeParameters>("--skip-download");
            }
            
            // Do not download or write to disk
            public YouTubeParameters Simulate()
            {
                return Add<YouTubeParameters>("-s");
            }
            
            public YouTubeParameters IgnoreErrors()
            {
                return Add<YouTubeParameters>("-i");
            }
            
            public YouTubeParameters NoWarnings()
            {
                return Add<YouTubeParameters>("--no-warnings");
            }

            public YouTubeParameters NoCache()
            {
                return Add<YouTubeParameters>("--no-cache-dir");
            }
            
            public YouTubeParameters ListExtractors()
            {
                return Add<YouTubeParameters>(Params.Extractors);
            }
            
            public YouTubeParameters GetTitle()
            {
                return Add<YouTubeParameters>(Params.Title);
            }
            
            public YouTubeParameters Version()
            {
                return Add<YouTubeParameters>(Params.Version);
            }
        }

        #endregion
    }
}
