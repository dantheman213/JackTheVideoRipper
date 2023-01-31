using System.Runtime.Serialization;
using JackTheVideoRipper.interfaces;

namespace JackTheVideoRipper.models;

public class ViewItem : ListViewItem, IViewItem
{
    public new string Tag { get; init; } = string.Empty;

    #region Constructor

    public ViewItem()
    {
    }

    protected ViewItem(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public ViewItem(string text) : base(text)
    {
    }

    public ViewItem(string text, int imageIndex) : base(text, imageIndex)
    {
        
    }

    public ViewItem(string[] items) : base(items)
    {
    }

    public ViewItem(string[] items, int imageIndex) : base(items, imageIndex)
    {
    }

    public ViewItem(string[] items, int imageIndex, Color foreColor, Color backColor, Font font) : 
        base(items, imageIndex, foreColor, backColor, font)
    {
    }

    public ViewItem(ListViewSubItem[] subItems, int imageIndex) : base(subItems, imageIndex)
    {
    }

    public ViewItem(ListViewGroup group) : base(group)
    {
    }

    public ViewItem(string text, ListViewGroup group) : base(text, group)
    {
    }

    public ViewItem(string text, int imageIndex, ListViewGroup group) : base(text, imageIndex, group)
    {
    }

    public ViewItem(string[] items, ListViewGroup group) : base(items, group)
    {
    }

    public ViewItem(string[] items, int imageIndex, ListViewGroup group) : base(items, imageIndex, group)
    {
    }

    public ViewItem(string[] items, int imageIndex, Color foreColor, Color backColor, Font font, ListViewGroup group) :
        base(items, imageIndex, foreColor, backColor, font, group)
    {
    }

    public ViewItem(ListViewSubItem[] subItems, int imageIndex, ListViewGroup group) : base(subItems, imageIndex, group)
    {
    }

    public ViewItem(string text, string imageKey) : base(text, imageKey)
    {
    }

    public ViewItem(string[] items, string imageKey) : base(items, imageKey)
    {
    }

    public ViewItem(string[] items, string imageKey, Color foreColor, Color backColor, Font font) :
        base(items, imageKey, foreColor, backColor, font)
    {
    }

    public ViewItem(ListViewSubItem[] subItems, string imageKey) : base(subItems, imageKey)
    {
    }

    public ViewItem(string text, string imageKey, ListViewGroup group) : base(text, imageKey, group)
    {
    }

    public ViewItem(string[] items, string imageKey, ListViewGroup group) : base(items, imageKey)
    {
    }

    public ViewItem(string[] items, string imageKey, Color foreColor, Color backColor, Font font, ListViewGroup group) :
        base(items, imageKey, foreColor, backColor, font, group)
    {
    }

    public ViewItem(ListViewSubItem[] subItems, string imageKey, ListViewGroup group) : base(subItems, imageKey, group)
    {
    }

    #endregion

    public new IViewSubItemCollection SubItems => new ViewSubItemCollection(base.SubItems);

    public IViewSubItem this[int index] => SubItems[index];

    public void Suspend()
    {
        ListView.Suspend();
    }

    public void Resume()
    {
        ListView.Resume();
    }
}