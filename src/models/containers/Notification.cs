namespace JackTheVideoRipper.models;

public struct Notification
{
    public DateTime? DateQueued = null;
    public DateTime? DatePosted = null;
    public readonly string Message;
    public readonly string? ShortenedMessage;
    public readonly string SenderName;
    public readonly Guid SenderGuid;

    public NotificationRow? NotificationRow = null;
    
    public string[] ViewItemArray => new[] {DateQueued.ToString()!, SenderName, SenderGuid.ToString(), Message};

    public Notification(string message, Type T, string? shortenedMessage = null)
    {
        Message = message;
        SenderName = T.Name;
        SenderGuid = T.GUID;
        ShortenedMessage = shortenedMessage;
    }
    
    public Notification(string message, object sender, string? shortenedMessage = null)
    {
        Message = message;
        SenderName = sender.GetType().Name;
        SenderGuid = sender.GetType().GUID;
        ShortenedMessage = shortenedMessage;
    }
}