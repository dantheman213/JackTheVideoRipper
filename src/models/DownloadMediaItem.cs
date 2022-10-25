using Newtonsoft.Json;

namespace JackTheVideoRipper
{
    public class DownloadMediaItem
    {
        [JsonProperty("title")]
        public string? Title { get; set; }
        
        [JsonProperty("url")]
        public string? Url { get; set; }
        
        [JsonProperty("parameters")]
        public string? Parameters { get; set; }
        
        [JsonProperty("filepath")]
        public string? Filepath { get; set; }
    }
}
