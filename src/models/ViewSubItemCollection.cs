using JackTheVideoRipper.extensions.FormsExtensions;
using JackTheVideoRipper.interfaces;

namespace JackTheVideoRipper.models;

public readonly struct ViewSubItemCollection : IViewSubItemCollection
{
    private readonly ListViewItem.ListViewSubItemCollection _listViewSubItemCollection;

    public ViewSubItemCollection(ListViewItem.ListViewSubItemCollection listViewSubItemCollection)
    {
        _listViewSubItemCollection = listViewSubItemCollection;
    }
    
    public IViewSubItem this[int index] => new ViewSubItem(_listViewSubItemCollection[index]);

    public IEnumerable<IViewSubItem> Items => _listViewSubItemCollection.ToEnumerable()
        .Select(i => new ViewSubItem(i) as IViewSubItem);
}