﻿using Newtonsoft.Json;

namespace JackTheVideoRipper
{
    public class SettingsModel
    {
        public static readonly string Directory = Path.Combine(FileSystem.Paths.Settings, "settings");
        public static readonly string Filepath = Path.Combine(Directory, "settings.json");

        [JsonProperty("default_download_path")]
        public string DefaultDownloadPath { get; set; } = FileSystem.Paths.Download;

        [JsonProperty("max_concurrent_downloads")]
        public int MaxConcurrentDownloads { get; set; } = 5;

        [JsonProperty("last_version_youtube-dl")]
        public string LastVersionYouTubeDL { get; set; } = string.Empty;

        public static bool Exists()
        {
            return File.Exists(Filepath);
        }

        public static SettingsModel GenerateDefaultSettings()
        {
            return new SettingsModel
            {
                DefaultDownloadPath = FileSystem.Paths.Download,
                MaxConcurrentDownloads = 5,
                LastVersionYouTubeDL = string.Empty
            };
        }
    }
}