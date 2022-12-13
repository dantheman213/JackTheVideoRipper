using JackTheVideoRipper.extensions;
using JackTheVideoRipper.views;

namespace JackTheVideoRipper.models;

public class NotificationsManager
{
    public static event Action<Notification> SendNotificationEvent = delegate {  };

    private readonly Dictionary<Guid, List<Notification>> _notifications = new();
    
    private FrameNotifications? _frameNotifications;

    private IEnumerable<Notification> Notifications => _notifications.Values.SelectMany(n => n);

    public NotificationsManager()
    {
        SendNotificationEvent += AppendNotification;
    }

    private void AppendNotification(Notification notification)
    {
        if (!_notifications.ContainsKey(notification.SenderGuid))
            _notifications.Add(notification.SenderGuid, new List<Notification>());
        notification.NotificationRow = new NotificationRow(notification);
        _notifications[notification.SenderGuid].Add(notification);
    }
    
    public void ClearNotifications()
    {
        _notifications.Clear();
    }

    public static void SendNotification(Notification notification)
    {
        notification.DatePosted = DateTime.Now;
        SendNotificationEvent.Invoke(notification);
    }

    public void OpenNotificationWindow()
    {
        if (_frameNotifications is not null)
        {
            _frameNotifications.Focus();
            return;
        }
        
        _frameNotifications = new FrameNotifications();

        // Populate data
        Notifications.OrderBy(n => n.DatePosted).ForEach(_frameNotifications.AddNotification);

        _frameNotifications.FormClosed += (_, _) =>
        {
            _frameNotifications = null;
        };
        
        _frameNotifications.Show();
    }
}