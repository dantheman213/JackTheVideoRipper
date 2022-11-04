using JackTheVideoRipper.Properties;

namespace JackTheVideoRipper;

public static class Core
{
    public static string ApplicationTitle => $@"JackTheVideoRipper {Common.GetAppVersion()}";
    
    public static event Action<string> NotificationEvent = delegate {  };
    
    public static void Startup()
    {
        FileSystem.ValidateInstallDirectory();
        CheckDependencies();
        CheckForYouTubeDLUpdates();
    }

    public static void SendNotification(string notification)
    {
        NotificationEvent(notification);
    }
    
    public static void CheckDependencies()
    {
        // Verify YouTube-DL
        if (!YouTubeDL.IsInstalled)
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
        if (!FFMPEG.IsInstalled)
        {
            Modals.Warning(Resources.FfmpegMissing, Captions.REQUIRED_NOT_INSTALLED);
        }

        // Verify Atomic Parsley
        if (!AtomicParsley.IsInstalled)
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

    public static void CheckForYouTubeDLUpdates()
    {
        Task.Run(YouTubeDL.CheckForUpdates);
    }

    public static bool ConfirmExit()
    {
        return Modals.Confirmation(Resources.ExitWarning, Captions.VERIFY_EXIT, MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2);
    }
    
    public static void CopyToClipboard(string url)
    {
        FileSystem.SetClipboardText(url);
    }

    public static void OpenInstallFolder()
    {
        FileSystem.OpenFileExplorer(FileSystem.Paths.Install);
    }
    
    public static void DownloadDependency(Dependencies dependency)
    {
        switch (dependency)
        {
            case Dependencies.YouTubeDL:
                FileSystem.GetWebResourceHandle(YouTubeDL.UPDATE_URL);
                break;
            case Dependencies.FFMPEG:
                FFMPEG.DownloadLatest();
                break;
            case Dependencies.Handbrake:
                FileSystem.GetWebResourceHandle(URLs.HANDBRAKE);
                break;
            case Dependencies.VLC:
                FileSystem.GetWebResourceHandle(URLs.VLC);
                break;
            case Dependencies.AtomicParsley:
                AtomicParsley.DownloadLatest();
                break;
            case Dependencies.Redistributables:
                FileSystem.GetWebResourceHandle(URLs.REDISTRIBUTABLES);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(dependency), dependency, null);
        }
    }
    
    public static void OpenSettings()
    {
        Pages.OpenPage<FrameSettings>();
    }

    public static void OpenAbout()
    {
        Pages.OpenPage<FrameAbout>();
    }

    public static void OpenConvert()
    {
        Pages.OpenPage<FrameConvert>();
    }
    
    public static void CheckForUpdates(bool isStartup = true)
    {
        AppUpdate.CheckForNewAppVersion(isStartup);
    }
    
    [System.Runtime.InteropServices.DllImport("wininet.dll")]
    private static extern bool InternetGetConnectedState(out int description, int reservedValue);

    public static bool IsConnectedToInternet()
    {
        return InternetGetConnectedState(out int _, 0);         
    }
}