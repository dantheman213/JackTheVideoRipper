using JackTheVideoRipper.interfaces;
using JackTheVideoRipper.models.enums;
using Newtonsoft.Json;

namespace JackTheVideoRipper
{
    public class DownloadMediaItem : IMediaItem
    {
        [JsonProperty("title")]
        public string Title { get; set; } = string.Empty;
        
        [JsonProperty("url")]
        public string Url { get; set; } = string.Empty;
        
        [JsonIgnore]
        public MediaParameters MediaParameters { get; init; }
        
        [JsonProperty("filepath")]
        public string Filepath { get; set; } = string.Empty;
        
        [JsonProperty("media_type")]
        public MediaType MediaType { get; set; }

        [JsonProperty("parameters")]
        public string ParameterString => MediaParameters.ToString();
    }
}
