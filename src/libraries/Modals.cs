using JackTheVideoRipper.models;
using JackTheVideoRipper.Properties;

namespace JackTheVideoRipper;

public static class Modals
{
    public static bool Confirmation(string text, string caption = "Confirm",
        MessageBoxIcon icon = MessageBoxIcon.Warning,
        MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
    {
        return MessageBox.Show(text, caption, MessageBoxButtons.YesNo, icon, defaultButton: defaultButton)
               == DialogResult.Yes;
    }

    public static DialogResult Notification(string text, string caption = "Info",
        MessageBoxIcon icon = MessageBoxIcon.Information)
    {
        return MessageBox.Show(text, caption, MessageBoxButtons.OK, icon);
    }

    public static DialogResult Warning(string text, string caption = "Warning",
        MessageBoxIcon icon = MessageBoxIcon.Warning)
    {
        return MessageBox.Show(text, caption, MessageBoxButtons.OK, icon);
    }

    public static DialogResult Error(string text, string caption = "Error",
        MessageBoxIcon icon = MessageBoxIcon.Error)
    {
        return MessageBox.Show(text, caption, MessageBoxButtons.OK, icon);
    }

    public static bool Update(AppVersionModel? result)
    {
        return Confirmation(string.Format(Messages.NewUpdate, AppInfo.ProgramName, result?.VersionString),
            Captions.NewVersion, MessageBoxIcon.Information);
    }

    public static DialogResult UpToDate()
    {
        return Notification(Messages.UpToDate, Captions.CurrentVersion);
    
    public static bool ConfirmExit()
    {
        return Confirmation(Messages.ExitWarning, Captions.VerifyExit, MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2);
    }
}