using JackTheVideoRipper.models;
using JackTheVideoRipper.models.enums;
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
        
        [JsonProperty("media_type")]
        public MediaType MediaType { get; set; }
        
        public static implicit operator MediaItemRow (DownloadMediaItem item)
        {
            return new MediaItemRow(item.Title!, item.Url!, item.Filepath!, item.MediaType)
            {
                Parameters = new Parameters()
            };
        }
    }
}
