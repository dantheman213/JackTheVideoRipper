namespace JackTheVideoRipper.interfaces;

public interface IViewItemProvider
{
    IViewItem CreateViewItem(string? tag = null);
    
    IViewItem CreateViewItem(string[] items, string? tag = null);
    
    IViewItem CreateMediaViewItem(IMediaItem mediaItem, string? tag = null);
}