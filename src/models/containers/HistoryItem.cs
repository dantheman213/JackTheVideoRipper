using JackTheVideoRipper.interfaces;
using JackTheVideoRipper.models.enums;
using Newtonsoft.Json;

namespace JackTheVideoRipper.models.containers;

public class HistoryItem : IMediaItem
{
    // IMediaItem Properties
    
    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;
        
    [JsonProperty("url")]
    public string Url { get; set; } = string.Empty;

    [JsonIgnore]
    public IProcessParameters? ProcessParameters { get; init; }
        
    [JsonProperty("filepath")]
    public string Filepath { get; set; } = string.Empty;

    [JsonProperty("media_type")]
    public MediaType MediaType { get; set; } = MediaType.Video;

    [JsonProperty("parameters")]
    public string ParameterString => ProcessParameters?.ToString() ?? string.Empty;
    
    // Other properties

    [JsonProperty("tag")]
    public string? Tag;

    [JsonProperty("date_started")]
    public DateTime? DateStarted;
    
    [JsonProperty("date_finished")]
    public DateTime? DateFinished;

    [JsonProperty("download_duration")]
    public double DownloadDuration = -1;

    [JsonProperty("filesize")]
    public string Filesize = Text.NOT_APPLICABLE;

    [JsonProperty("website_name")]
    public string WebsiteName = string.Empty;

    [JsonProperty("download_result")]
    public ProcessStatus Result;

    [JsonConstructor]
    private HistoryItem()
    {
    }
    
    public HistoryItem(IMediaItem mediaItem, string tag)
    {
        Tag = tag;
        Title = mediaItem.Title;
        Url = mediaItem.Url;
        Filepath = mediaItem.Filepath;
        MediaType = mediaItem.MediaType;
        ProcessParameters = mediaItem.ProcessParameters;
    }
    
    public string[] ViewItemArray => new[]
    {
        Title,
        Url,
        ParameterString,
        Filepath,
        MediaType.ToString(),
        Tag!,
        DateStarted?.ToString("G") ?? Text.NOT_APPLICABLE,
        DateFinished?.ToString("G") ?? Text.NOT_APPLICABLE,
        $"{DownloadDuration:F2} s",
        Filesize,
        WebsiteName,
        Result.ToString()
    };

    public bool StartTimeSet => DateStarted is not null;
    
    public bool EndTimeSet => DateFinished is not null;

    public void SetDuration()
    {
        if (!EndTimeSet || !StartTimeSet)
            return;
        
        DownloadDuration = ((DateTime) DateFinished!).Subtract((DateTime) DateStarted!).TotalSeconds;
    }
}