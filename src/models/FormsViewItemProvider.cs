using JackTheVideoRipper.interfaces;
using JackTheVideoRipper.models;

namespace JackTheVideoRipper;

public class FormsViewItemProvider : IViewItemProvider
{
    public IViewItem CreateViewItem(string? tag = null)
    {
        return new ViewItem { Tag = tag ?? Common.CreateTag() };
    }
    
    public IViewItem CreateViewItem(string[] items, string? tag = null)
    {
        return new ViewItem(items) { Tag = tag ?? Common.CreateTag() };
    }
    
    public IViewItem CreateMediaViewItem(IMediaItem mediaItem, string? tag = null)
    {
        return new ViewItem(DefaultRow(mediaItem))
        {
            Tag = tag ?? Common.CreateTag(),
            BackColor = Color.LightGray,
            ImageIndex = (int) mediaItem.MediaType
        };
    }

    private static string[] DefaultRow(IMediaItem mediaItem)
    {
        return new[]
        {
            mediaItem.Title,
            Statuses.Waiting,
            mediaItem.MediaType.ToString(),
            Text.DefaultSize,
            Text.DefaultProgress,
            Text.DefaultSpeed,
            Text.DefaultTime,
            mediaItem.Url,
            mediaItem.Filepath
        };
    }
}