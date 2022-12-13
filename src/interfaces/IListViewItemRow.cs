namespace JackTheVideoRipper.interfaces;

public interface IListViewItemRow
{
    ListViewItem ViewItem { get; }

    string Tag { get; }
}