using JackTheVideoRipper.interfaces;

namespace JackTheVideoRipper.models;

public readonly struct ViewSubItem : IViewSubItem
{
    private readonly ListViewItem.ListViewSubItem _listViewSubItem;

    public ListViewItem.ListViewSubItem ListViewSubItem => _listViewSubItem;

    public ViewSubItem(ListViewItem.ListViewSubItem listViewSubItem)
    {
        _listViewSubItem = listViewSubItem;
    }

    public string Text
    {
        get => _listViewSubItem.Text;
        set => _listViewSubItem.Text = value;
    }
}