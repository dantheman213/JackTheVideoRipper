using JackTheVideoRipper.models;
using JackTheVideoRipper.Properties;

namespace JackTheVideoRipper;

public static class Modals
{
    public static bool Confirmation(string text, string caption, MessageBoxIcon icon = MessageBoxIcon.Warning, 
        MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
    {
        return MessageBox.Show(text, caption, MessageBoxButtons.YesNo, icon, defaultButton:defaultButton) == DialogResult.Yes;
    }
        
    public static DialogResult Notification(string text, string caption, MessageBoxIcon icon = MessageBoxIcon.Information)
    {
        return MessageBox.Show(text, caption, MessageBoxButtons.OK, icon);
    }
    
    public static DialogResult Warning(string text, string caption = "Warning", MessageBoxIcon icon = MessageBoxIcon.Warning)
    {
        return MessageBox.Show(text, caption, MessageBoxButtons.OK, icon);
    }
    
    public static DialogResult Error(string text, string caption = "Error", MessageBoxIcon icon = MessageBoxIcon.Error)
    {
        return MessageBox.Show(text, caption, MessageBoxButtons.OK, icon);
    }
    
    public static bool Update(AppVersionModel? result)
    {
        return Confirmation(string.Format(Resources.NewUpdate, result?.Version), "New Version Available",
            MessageBoxIcon.Information);
    }

    public static DialogResult UpToDate()
    {
        return Notification(Resources.UpToDate, "Version Current");
    }
}