using JackTheVideoRipper.models.enums;

namespace JackTheVideoRipper.interfaces;

public interface IMediaItem
{
    public string Title { get; }
        
    public string Url { get; }
        
    public IProcessParameters? ProcessParameters { get; }
        
    public string Filepath { get; }
        
    public MediaType MediaType { get; }

    public string ParametersString => ProcessParameters?.ToString() ?? string.Empty;
}