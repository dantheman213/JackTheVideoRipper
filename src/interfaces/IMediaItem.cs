using JackTheVideoRipper.models.enums;

namespace JackTheVideoRipper.interfaces;

public interface IMediaItem
{
    public string Title { get; }
        
    public string Url { get; }
        
    public MediaParameters MediaParameters { get; }
        
    public string Filepath { get; }
        
    public MediaType MediaType { get; }
}