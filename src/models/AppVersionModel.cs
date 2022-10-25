using Newtonsoft.Json;

namespace JackTheVideoRipper.models
{
    public class AppVersionModel
    {
        [JsonProperty("version")]
        public string Version { get; set; } = string.Empty;
        
        [JsonProperty("is_newer_version_available")]
        public bool IsNewerVersionAvailable { get; set; }
    }
}
