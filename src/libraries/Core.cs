using JackTheVideoRipper.Properties;

namespace JackTheVideoRipper;

public static class Core
{
    public static void Startup()
    {
        FileSystem.ValidateInstallDirectory();
        CheckDependencies();
        CheckForUpdates();
    }
    
    public static void CheckDependencies()
    {
        // Verify YouTube-DL
        if (!YouTubeDL.IsInstalled())
        {
            if (Modals.Confirmation(Resources.InstallMessage, Captions.REQUIRED_INSTALLED))
            {
                InstallDependencies();
            }
            else
            {
                Modals.Error(Resources.InstallationError, Captions.APPLICATION_ERROR);
                return;
            }
        }

        // Verify FFMPEG
        if (!FFMPEG.IsInstalled())
        {
            Modals.Warning(Resources.FfmpegMissing, Captions.REQUIRED_NOT_INSTALLED);
        }

        // Verify Atomic Parsley
        if (!AtomicParsley.IsInstalled())
        {
            Modals.Warning(Resources.AtomicParsleyMissing, Captions.REQUIRED_NOT_INSTALLED);
        }
    }

    private static void InstallDependencies()
    {
        FrameYTDLDependencyInstall frameDependencyInstall = new();
        frameDependencyInstall.ShowDialog();
        // TODO ?
        Modals.Notification(Resources.InstallationSuccess, Captions.REQUIRED_INSTALLED);
        frameDependencyInstall.Close();
    }

    public static void CheckForUpdates()
    {
        Task.Run(YouTubeDL.CheckForUpdates);
    }

    public static bool ConfirmExit()
    {
        return Modals.Confirmation(Resources.ExitWarning, Captions.VERIFY_EXIT, MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2);
    }
}