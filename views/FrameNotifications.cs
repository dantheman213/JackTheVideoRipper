using JackTheVideoRipper.extensions;
using JackTheVideoRipper.interfaces;
using JackTheVideoRipper.models;

namespace JackTheVideoRipper.views
{
   public partial class FrameNotifications : Form
   {
      private readonly HashSet<IViewItem> _notifications = new();

      public FrameNotifications()
      {
         InitializeComponent();
      }

      public void Clear()
      {
         listItems.Items.Clear();  
         _notifications.Clear();
      }

      private bool NotificationExists(IListViewItemRow notificationRow)
      {
         return _notifications.Contains(notificationRow.ViewItem);
      }

      public void AddNotification(NotificationRow notificationRow)
      {
         if (NotificationExists(notificationRow))
            return;
         IViewItem viewItem = notificationRow.ViewItem;
         AddViewItem(viewItem);
      }
      
      public void AddNotification(Notification notification)
      {
         if (NotificationExists(notification.NotificationRow!))
            return;
         IViewItem viewItem = notification.NotificationRow!.ViewItem;
         AddViewItem(viewItem);
      }

      private void AddViewItem(IViewItem viewItem)
      {
         listItems.Items.Add(viewItem.As<ListViewItem>());
         _notifications.Add(viewItem);
      }
   }
}
