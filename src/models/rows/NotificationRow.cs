using JackTheVideoRipper.interfaces;

namespace JackTheVideoRipper.models;

public class NotificationRow : IListViewItemRow
{
    #region Data Members

    public ListViewItem ViewItem { get; }

    public string Tag { get; } = Common.CreateTag();

    #endregion
    
    #region View Item Accessors

    public string DatePosted
    {
        get => ViewItem.SubItems[0].Text;
        init => ViewItem.SubItems[0].Text = value;
    }
    
    public string SenderName
    {
        get => ViewItem.SubItems[1].Text;
        init => ViewItem.SubItems[1].Text = value;
    }
    
    public string SenderGuid
    {
        get => ViewItem.SubItems[2].Text;
        init => ViewItem.SubItems[2].Text = value;
    }
    
    public string Message
    {
        get => ViewItem.SubItems[3].Text;
        init => ViewItem.SubItems[3].Text = value;
    }
    
    #endregion

    public NotificationRow(Notification notification)
    {
        ViewItem = new ListViewItem(notification.ViewItemArray) {Tag = Tag};
    }
}