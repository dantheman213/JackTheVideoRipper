using System.Diagnostics;
using System.Text;
using Newtonsoft.Json;
using static System.Environment;

namespace JackTheVideoRipper
{
    internal static class YouTubeDl
    {
        public static readonly string DefaultDownloadPath = Path.Combine(ExpandEnvironmentVariables("%userprofile%"), "Downloads");
        private const string binName = "yt-dlp.exe";
        public static readonly string binPath = Path.Combine(Common.InstallDirectory, binName);
        private const string downloadURL = "https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp.exe";

        public static bool IsInstalled()
        {
            return File.Exists(binPath);
        }

        public static void CheckDownload()
        {
            if (!IsInstalled())
            {
                DownloadAndInstall();
            }
        }

        public static void DownloadAndInstall()
        {
            if (File.Exists(binPath))
                return;
            
            Directory.CreateDirectory(Common.InstallDirectory);
                
            // Download binary to directory
            Common.DownloadFile(downloadURL, binPath);
        }

        public static void CheckForUpdates()
        {
            if (!IsInstalled())
                return;
            
            CLI.RunCommand($"{binName} -U", Common.InstallDirectory);
            string previousVersion = Settings.Data.LastVersionYouTubeDL;
            string currentVersion = GetVersion();

            if (string.IsNullOrEmpty(previousVersion))
            {
                Settings.Data.LastVersionYouTubeDL = currentVersion;
                Settings.Save();
                return;
            }

            if (previousVersion != currentVersion)
            {
                Settings.Data.LastVersionYouTubeDL = currentVersion;
                Settings.Save();
                MessageBox.Show($@"Dependency yt-dlp has been upgraded from {previousVersion} to {currentVersion}!",
                    @"yt-dlp update", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public static Process Run(string opts)
        {
            return CLI.RunYouTubeCommand(binPath, opts);
        }

        private static string TimeStampDate => DateTime.Now.ToString("yyyyMMddhmmsstt");

        private static readonly string TempPath = Path.GetTempPath();
        
        private static string GetTempFilename(string ext)
        {
            return $"{TempPath}jtvr_thumbnail_{TimeStampDate}.{ext}";
        }
        
        public static string? DownloadThumbnail(string thumbnailUrl)
        {
            if (!Common.IsValidUrl(thumbnailUrl))
                return null;
            
            string urlExt = thumbnailUrl[(thumbnailUrl.LastIndexOf(".", StringComparison.Ordinal) + 1)..].ToLower();
            
            // allow jpg and png but don't allow webp since we'll convert that below
            if (urlExt == "webp")
                urlExt = "jpg";
            
            // TODO: get extension from URL rather than hard coding
            string tmpFilePath = GetTempFilename(urlExt);
                
            // popular format for saving thumbnails these days but PictureBox in WinForms can't handle it :( so we'll convert to jpg
            if (thumbnailUrl.EndsWith("webp"))
            {
                string tmpWebpFilePath = GetTempFilename("webp");
                Common.DownloadFile(thumbnailUrl, tmpWebpFilePath);
                FFmpeg.ConvertImageToJpg(tmpWebpFilePath, tmpFilePath);
            }
            else
            {
                Common.DownloadFile(thumbnailUrl, tmpFilePath);
            }

            return tmpFilePath;
        }

        public static List<PlaylistInfoItem>? GetPlaylistMetadata(string url)
        {
            string json = RunCommand($"-i --no-warnings --no-cache-dir --dump-json --flat-playlist --skip-download --yes-playlist{url}");
            // youtube-dl returns an individual json object per line

            StringBuilder buffer = new();
            buffer.Append('[');
            buffer.Append(string.Join(",\n", json.Split("\n")));
            buffer.Append(']');
            
            return JsonConvert.DeserializeObject<List<PlaylistInfoItem>>(json);
        }

        public static MediaInfoData? GetMediaData(string url)
        {
            string json = RunCommand($"-s --no-warnings --no-cache-dir --print-json {url}");
            return JsonConvert.DeserializeObject<MediaInfoData>(json);
        }

        public static string GetExtractors()
        {
            return RunCommand("--list-extractors");
        }

        public static string GetVersion()
        {
            return RunCommand("--version");
        }

        public static string GetTitle(string url)
        {
            return RunCommand($"--get-title {url}");
        }

        private static string RunCommand(string paramString)
        {
            Process process = CLI.RunYouTubeCommand(binPath, paramString);
            process.Start();
            return process.StandardOutput.ReadToEnd().Trim();
        }
    }
}
