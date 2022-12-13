using JackTheVideoRipper.interfaces;
using JackTheVideoRipper.models.containers;

namespace JackTheVideoRipper.models;

public class HistoryRow : IListViewItemRow
{
    #region Data Members

    public ListViewItem ViewItem { get; }

    public string Tag { get; } = Common.CreateTag();

    #endregion
    
    #region View Item Accessors

    public string Title
    {
        get => ViewItem.SubItems[0].Text;
        init => ViewItem.SubItems[0].Text = value;
    }
    
    public string Url
    {
        get => ViewItem.SubItems[1].Text;
        init => ViewItem.SubItems[1].Text = value;
    }
    
    public string Parameters
    {
        get => ViewItem.SubItems[2].Text;
        init => ViewItem.SubItems[2].Text = value;
    }
    
    public string Filepath
    {
        get => ViewItem.SubItems[3].Text;
        init => ViewItem.SubItems[3].Text = value;
    }
    
    public string MediaType
    {
        get => ViewItem.SubItems[4].Text;
        init => ViewItem.SubItems[4].Text = value;
    }

    public string HistoryTag
    {
        get => ViewItem.SubItems[5].Text;
        init => ViewItem.SubItems[5].Text = value;
    }
    
    public string DateStarted
    {
        get => ViewItem.SubItems[6].Text;
        init => ViewItem.SubItems[6].Text = value;
    }
    
    public string DateFinished
    {
        get => ViewItem.SubItems[7].Text;
        init => ViewItem.SubItems[7].Text = value;
    }
    
    public string Duration
    {
        get => ViewItem.SubItems[8].Text;
        init => ViewItem.SubItems[8].Text = value;
    }
    
    public string Filesize
    {
        get => ViewItem.SubItems[9].Text;
        init => ViewItem.SubItems[9].Text = value;
    }
    
    public string WebsiteName
    {
        get => ViewItem.SubItems[10].Text;
        init => ViewItem.SubItems[10].Text = value;
    }
    
    public string Result
    {
        get => ViewItem.SubItems[11].Text;
        init => ViewItem.SubItems[11].Text = value;
    }
    
    #endregion
    
    public HistoryRow(HistoryItem historyItem)
    {
        ViewItem = new ListViewItem(historyItem.ViewItemArray) { Tag = Tag };
    }
}