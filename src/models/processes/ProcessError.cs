using JackTheVideoRipper.extensions;
using JackTheVideoRipper.interfaces;

namespace JackTheVideoRipper;

public readonly struct ProcessError
{
    public readonly string ProcessTag;
    public readonly Exception Exception;
    public readonly DateTime Timestamp;
    public readonly string AdditionalDetails;

    public ProcessError(IListViewItemRow updateRow, Exception exception, string additionalDetails = "")
    {
        ProcessTag = updateRow.Tag;
        Exception = exception;
        Timestamp = DateTime.Now;
        AdditionalDetails = additionalDetails;
    }
    
    public ProcessError(ListViewItem listViewItem, Exception exception, string additionalDetails = "")
    {
        ProcessTag = listViewItem.Tag.ToString().ValueOrDefault();
        Exception = exception;
        Timestamp = DateTime.Now;
        AdditionalDetails = additionalDetails;
    }
    
    public ProcessError(string processTag, Exception exception, string additionalDetails = "")
    {
        ProcessTag = processTag;
        Exception = exception;
        Timestamp = DateTime.Now;
        AdditionalDetails = additionalDetails;
    }
}