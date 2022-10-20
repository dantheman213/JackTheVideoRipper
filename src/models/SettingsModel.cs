namespace JackTheVideoRipper
{
    public class SettingsModel
    {
        public static readonly string Directory = Path.Combine(Common.RootDirectory, "settings");
        public static readonly string Filepath = Path.Combine(Directory, "settings.json");
        public string DefaultDownloadPath { get; set; }
        public int MaxConcurrentDownloads { get; set; }
        public string LastVersionYouTubeDL { get; set; }

        public static bool Exists()
        {
            return File.Exists(Filepath);
        }

        public static SettingsModel GenerateDefaultSettings()
        {
            return new SettingsModel
            {
                DefaultDownloadPath = YouTubeDl.DefaultDownloadPath,
                MaxConcurrentDownloads = 5,
                LastVersionYouTubeDL = ""
            };;
        }
    }
}
