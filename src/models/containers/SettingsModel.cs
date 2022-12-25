using Newtonsoft.Json;

namespace JackTheVideoRipper
{
    [Serializable]
    public class SettingsModel : ConfigModel
    {
        #region Static Data Members
        
        public static readonly string SettingsFilepath = Path.Combine(ConfigDirectory, "settings.json");

        #endregion

        #region Attributes

        public override string Filepath => SettingsFilepath;

        #endregion

        #region Data Members

        [JsonProperty("default_download_path")]
        public string DefaultDownloadPath { get; set; } = FileSystem.Paths.Download;

        [JsonProperty("max_concurrent_downloads")]
        public int MaxConcurrentDownloads { get; set; } = 5;

        [JsonProperty("last_version_youtube-dl")]
        public string LastVersionYouTubeDL { get; set; } = string.Empty;
        
        [JsonProperty("skip_metadata")]
        public bool SkipMetadata { get; set; }

        [JsonProperty("store_history")]
        public bool StoreHistory { get; set; }

        [JsonProperty("enable_developer_mode")]
        public bool EnableDeveloperMode { get; set; }
        
        [JsonProperty("enable_multi_threaded_downloads")]
        public bool EnableMultiThreadedDownloads { get; set; }
        
        [JsonProperty("last_opened_filepath")]
        public string LastOpenedFilepath { get; set; } = FileSystem.Paths.Download;
        
        #endregion

        #region Public Methods

        public override void Validate()
        {
            if (!Directory.Exists(DefaultDownloadPath))
                DefaultDownloadPath = FileSystem.Paths.Download;
            
            if (!Directory.Exists(LastOpenedFilepath))
                LastOpenedFilepath = FileSystem.Paths.Download;
        }

        #endregion
    }
}
