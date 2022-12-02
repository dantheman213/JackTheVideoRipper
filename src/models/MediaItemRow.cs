using JackTheVideoRipper.interfaces;
using JackTheVideoRipper.models.enums;

namespace JackTheVideoRipper.models;

public struct MediaItemRow : IMediaItem
{
    public string Title { get; }

    public string Url { get; }
    
    public MediaParameters MediaParameters { get; } = new();
    
    public string Filepath { get; }

    public MediaType MediaType { get; }

    public MediaItemRow(string title, string url, string filepath, MediaType mediaMediaType = MediaType.Video,
        MediaParameters? mediaParameters = null)
    {
        Title = title;
        Url = url;
        Filepath = filepath;
        MediaType = mediaMediaType;
        MediaParameters = mediaParameters ?? new MediaParameters();
    }
}