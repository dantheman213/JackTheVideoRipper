using System.Diagnostics;
using JackTheVideoRipper.extensions;
using JackTheVideoRipper.Properties;
using Newtonsoft.Json;
using static System.Environment;

namespace JackTheVideoRipper
{
    internal static class YouTubeDL
    {
        public static readonly string DefaultDownloadPath = Path.Combine(ExpandEnvironmentVariables("%userprofile%"), "Downloads");
        private const string binName = "yt-dlp.exe";
        public static readonly string binPath = FileSystem.ProgramPath(binName);
        private const string downloadURL = "https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp.exe";
        public const string UPDATE_URL = "https://github.com/yt-dlp/yt-dlp";

        public static bool IsInstalled()
        {
            return File.Exists(binPath);
        }

        public static void CheckDownload()
        {
            if (!IsInstalled())
                DownloadAndInstall();
        }

        public static void DownloadAndInstall()
        {
            if (File.Exists(binPath))
                return;
                
            // Download binary to directory
            FileSystem.DownloadFile(downloadURL, binPath);
        }

        public static void CheckForUpdates()
        {
            if (!IsInstalled())
                return;
            
            CLI.RunCommand($"{binPath} -U", Common.Paths.Install);
            string previousVersion = Settings.Data.LastVersionYouTubeDL;
            string currentVersion = GetVersion();

            if (previousVersion.IsNullOrEmpty())
            {
                Settings.Data.LastVersionYouTubeDL = currentVersion;
                Settings.Save();
                return;
            }

            if (previousVersion != currentVersion)
            {
                Settings.Data.LastVersionYouTubeDL = currentVersion;
                Settings.Save();
                Modals.Notification(string.Format(Resources.YouTubeDLUpdated, previousVersion, currentVersion),
                    @"yt-dlp update");
            }
        }
        
        public static string GetYouTubeLink(string id) => $"https://www.youtube.com/watch?v={id}";

        public static string? DownloadThumbnail(string thumbnailUrl)
        {
            if (!Common.IsValidUrl(thumbnailUrl))
                return null;
            
            string urlExt = FileSystem.GetExtension(thumbnailUrl).ToLower();
            
            // allow jpg and png but don't allow webp since we'll convert that below
            if (urlExt == "webp")
                urlExt = "jpg";
            
            // TODO: get extension from URL rather than hard coding
            string tmpFilePath = Common.GetTempFilename(urlExt);
                
            // popular format for saving thumbnails these days but PictureBox in WinForms can't handle it :( so we'll convert to jpg
            if (thumbnailUrl.EndsWith("webp"))
            {
                string tmpWebpFilePath = Common.GetTempFilename("webp");
                FileSystem.DownloadFile(thumbnailUrl, tmpWebpFilePath);
                FFMPEG.ConvertImageToJpg(tmpWebpFilePath, tmpFilePath);
            }
            else
            {
                FileSystem.DownloadFile(thumbnailUrl, tmpFilePath);
            }

            return tmpFilePath;
        }

        // youtube-dl returns an individual json object per line
        public static IEnumerable<PlaylistInfoItem> GetPlaylistMetadata(string url)
        {
            return FileSystem.ReceiveMultiJsonResponse<IEnumerable<PlaylistInfoItem>>(binPath, url, Params.PlaylistMetadata) 
                   ?? Array.Empty<PlaylistInfoItem>();
        }

        private static class Params
        {
            public const string PlaylistMetadata = "-i --no-warnings --no-cache-dir --dump-json --flat-playlist --skip-download --yes-playlist";
            public const string MediaData = "-s --no-warnings --no-cache-dir --print-json";
            public const string Extractors = "--list-extractors";
            public const string Version = "--version";
            public const string Title = "--get-title";
        }

        public static MediaInfoData? GetMediaData(string url)
        {
            return FileSystem.ReceiveJsonResponse<MediaInfoData>(binPath, url, Params.MediaData);
        }

        private static readonly Command _Command = new(binPath);

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
