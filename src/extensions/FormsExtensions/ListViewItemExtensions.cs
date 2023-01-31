namespace JackTheVideoRipper.extensions;

public static class ListViewItemExtensions
{
    public static bool InBounds(this ListViewItem listViewItem, Point point)
    {
        return listViewItem.Bounds.Contains(point);
    }
}