namespace JackTheVideoRipper.interfaces;

public interface IListViewItemRow
{
    IViewItem ViewItem { get; }

    string Tag { get; }
}