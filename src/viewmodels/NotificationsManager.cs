using JackTheVideoRipper.extensions;
using JackTheVideoRipper.models.DataStructures;
using JackTheVideoRipper.views;
using Timer = System.Threading.Timer;

namespace JackTheVideoRipper.models;

public class NotificationsManager
{
    #region Data Members

    private readonly Timer? _clearNotificationsTimer;

    private int _notificationClearTime = 5000; //< May want to be set by user later?

    private readonly Dictionary<Guid, List<Notification>> _notifications = new();
        
    private FrameNotifications? _frameNotifications;

    private readonly TimedQueue<Notification> _notificationQueue;

    #endregion

    #region Attributes

    private IEnumerable<Notification> Notifications => _notifications.Values.SelectMany(n => n);

    #endregion

    #region Events

    public static event Action<Notification> SendNotificationEvent = delegate {  };
    
    public static event Action ClearPushNotificationsEvent = delegate {  };
    
    private static event Action<Notification> QueueNotificationEvent = delegate {  };

    #endregion

    #region Constructor

    public NotificationsManager()
    {
        QueueNotificationEvent += QueueNotification;
        SendNotificationEvent += AppendNotification;
        _clearNotificationsTimer = new Timer(ClearPushNotifications, null, 0, _notificationClearTime);
        _notificationQueue = new TimedQueue<Notification>(PostNotification);
    }

    #endregion

    #region Public Methods

    public void Start()
    {
        _notificationQueue.Start();
    }

    public void Reset()
    {
        _clearNotificationsTimer?.Change(_notificationClearTime, _notificationClearTime);
    }

    public void Clear()
    {
        _notifications.Clear();
        _frameNotifications?.Clear();
    }

    public void OpenNotificationWindow()
    {
        bool alreadyOpened = CreateWindow();

        // Populate data
        Notifications.OrderBy(n => n.DateQueued).ForEach(_frameNotifications!.AddNotification);

        if (!alreadyOpened)
            _frameNotifications.Show();
        
        _frameNotifications.Activate();
    }

    #endregion

    #region Private Methods

    private void AppendNotification(Notification notification)
    {
        if (!_notifications.ContainsKey(notification.SenderGuid))
            _notifications.Add(notification.SenderGuid, new List<Notification>());
        notification.NotificationRow = new NotificationRow(notification);
        _notifications[notification.SenderGuid].Add(notification);
    }

    private void QueueNotification(Notification notification)
    {
        _notificationQueue.Enqueue(notification);
    }

    private bool CreateWindow()
    {
        if (_frameNotifications is not null)
            return true;
        
        _frameNotifications = new FrameNotifications();
        _frameNotifications.Closed += (_, _) => _frameNotifications = null;
        return false;
    }

    #endregion

    #region Static Methods
    
    public static void ClearPushNotifications(object? sender = null)
    {
        ClearPushNotificationsEvent();
    }

    public static void SendNotification(Notification notification)
    {
        notification.DateQueued = DateTime.Now;
        QueueNotificationEvent(notification);
    }

    private static void PostNotification(Notification notification)
    {
        notification.DatePosted = DateTime.Now;
        SendNotificationEvent(notification);
    }

    #endregion
}