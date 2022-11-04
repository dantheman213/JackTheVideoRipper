using Timer = System.Threading.Timer;

namespace JackTheVideoRipper.models;

public class NotificationBar
{
    public readonly Timer Timer;

    public NotificationBar()
    {
        Timer = new Timer(ClearNotification, null, 0, 5000);
    }

    public void SetNotification(string notification)
    {
        Core.SendNotification(notification);
    }

    private void ClearNotification(object? state)
    {
        Core.SendNotification(string.Empty);
    }
}