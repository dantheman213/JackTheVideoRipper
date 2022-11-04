using System.Diagnostics;
using JackTheVideoRipper.extensions;
using JackTheVideoRipper.Properties;

namespace JackTheVideoRipper
{
    internal static class YouTubeDL
    {
        private const string _EXECUTABLE_NAME = "yt-dlp.exe";
        public static readonly string ExecutablePath = FileSystem.ProgramPath(_EXECUTABLE_NAME);
        public const string UPDATE_URL = "https://github.com/yt-dlp/yt-dlp";
        private const string _DOWNLOAD_URL =  $"{UPDATE_URL}/releases/latest/download/yt-dlp.exe";
        
        private static readonly Command _Command = new(ExecutablePath);

        private static class Params
        {
            private const string NoWarningsNoCache = "--no-warnings --no-cache-dir";
            public const string PlaylistMetadata = $"-i {NoWarningsNoCache} --dump-json --flat-playlist --skip-download --yes-playlist";
            public const string MediaData = $"-s {NoWarningsNoCache} --print-json";
            public const string Extractors = "--list-extractors";
            public const string Version = "--version";
            public const string Title = "--get-title";
        }

        public static bool IsInstalled => File.Exists(ExecutablePath);
        
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

        public static void CheckDownload()
        {
            if (!IsInstalled)
                DownloadAndInstall();
        }

        public static void DownloadAndInstall()
        {
            if (IsInstalled)
                return;
                
            // Download binary to directory
            FileSystem.DownloadWebFile(_DOWNLOAD_URL, ExecutablePath);
        }

        private static void UpdateInstallation()
        {
            CLI.RunCommand($"{ExecutablePath} -U", FileSystem.Paths.Install);
        }

        public static void CheckForUpdates()
        {
            if (!IsInstalled)
                return;

            UpdateInstallation();

            if (PreviousVersion.IsNullOrEmpty())
            {
                PreviousVersion = CurrentVersion;
                return;
            }
            
            if (PreviousVersion == CurrentVersion)
                return;

            PreviousVersion = CurrentVersion;
                
            Modals.Notification(string.Format(Resources.YouTubeDLUpdated, PreviousVersion, CurrentVersion),
                @"yt-dlp update");
        }
        
        public static string GetYouTubeLink(string id) => $"https://www.youtube.com/watch?v={id}";
        
        public static IEnumerable<PlaylistInfoItem> GetPlaylistMetadata(string url)
        {
            return FileSystem.ReceiveMultiJsonResponse<PlaylistInfoItem>(ExecutablePath, url, Params.PlaylistMetadata);
        }

        public static MediaInfoData? GetMediaData(string url)
        {
            return FileSystem.ReceiveJsonResponse<MediaInfoData>(ExecutablePath, url, Params.MediaData);
        }

        public static string GetExtractors()
        {
            return _Command.RunCommand(Params.Extractors);
        }

        public static string GetVersion()
        {
            return _Command.RunCommand(Params.Version);
        }

        public static string GetTitle(string url)
        {
            return _Command.RunWebCommand(Params.Title, url);
        }

        public static Process CreateCommand(string parameters)
        {
            return _Command.CreateCommand(parameters);
        }
    }
}
