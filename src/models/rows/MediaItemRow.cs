using JackTheVideoRipper.extensions;
using JackTheVideoRipper.interfaces;
using JackTheVideoRipper.models.enums;

namespace JackTheVideoRipper.models;

public struct MediaItemRow<T> : IMediaItem where T : IProcessParameters, new()
{
    public string Title { get; }

    public string Url { get; }
    
    public IProcessParameters ProcessParameters { get; }
    
    public string Filepath { get; }

    public MediaType MediaType { get; }

    public MediaItemRow(string url = "", string title = "", string filepath = "",
        MediaType mediaMediaType = MediaType.Video,
        T? mediaParameters = default)
    {
        if (mediaParameters is IRequiresUrlParameters && url.IsNullOrEmpty())
        {
            throw new ArgumentNullException(nameof(url));
        }

        if (mediaParameters is IRequiresFileParameters && filepath.IsNullOrEmpty())
        {
            throw new ArgumentNullException(nameof(filepath));
        }
        
        Url = url;
        Title = title;
        Filepath = filepath;
        MediaType = mediaMediaType;
        ProcessParameters = mediaParameters ?? new T();
    }
}