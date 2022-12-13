using JackTheVideoRipper.models;

namespace JackTheVideoRipper.views
{
   public partial class FrameNotifications : Form
   {
      public FrameNotifications()
      {
         InitializeComponent();
      }

      public void AddNotification(NotificationRow notificationRow)
      {
         listItems.Items.Add(notificationRow.ViewItem);
      }
      
      public void AddNotification(Notification notification)
      {
         listItems.Items.Add(notification.NotificationRow!.ViewItem);
      }
   }
}
