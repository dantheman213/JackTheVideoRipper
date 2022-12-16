using JackTheVideoRipper.extensions;
using JackTheVideoRipper.Properties;
using JackTheVideoRipper.views;

namespace JackTheVideoRipper;

public static class Core
{
    public static string ApplicationTitle => $@"JackTheVideoRipper {Common.GetAppVersion()}";

    public static TaskScheduler Scheduler { get; private set; } = null!;
    
    private static TaskFactory TaskFactory { get; set; } = null!;

    public static async Task Startup()
    {
        FileSystem.ValidateInstallDirectory();
        await Task.Run(CheckDependencies);
        await CheckForYouTubeDLUpdates();
    }

    public static async Task LoadConfigurationFiles()
    {
        await Config.Load();
    }

    public static async Task Shutdown()
    {
        await Config.Save();
    }

    public static void InitializeScheduler()
    {
        Scheduler = TaskScheduler.FromCurrentSynchronizationContext();
        TaskFactory = new TaskFactory(CancellationToken.None, TaskCreationOptions.DenyChildAttach,
            TaskContinuationOptions.None, Scheduler);
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
        if (!modules.FFMPEG.IsInstalled)
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

    public static async Task CheckForYouTubeDLUpdates()
    {
        await Task.Run(YouTubeDL.CheckForUpdates);
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

    public static async Task OpenInstallFolder()
    {
        await FileSystem.OpenFileExplorer(FileSystem.Paths.Install);
    }
    
    public static void DownloadDependency(Dependencies dependency)
    {
        switch (dependency)
        {
            case Dependencies.YouTubeDL:
                FileSystem.GetWebResourceHandle(YouTubeDL.UPDATE_URL);
                break;
            case Dependencies.FFMPEG:
                modules.FFMPEG.DownloadLatest();
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
    
    public static async Task CheckForUpdates()
    {
        await AppUpdate.CheckForNewAppVersion();
    }
    
    public static Form MainForm => Application.OpenForms[0];
        
    public static FrameMain FrameMain => (MainForm as FrameMain)!;

    public static bool IsConnectedToInternet()
    {
        return InternetGetConnectedState(out int _, 0);         
    }
    
    private static void DownloadImages()
    {
        Settings.Load();
            
        string[] ids = 
        {   
            "JO-Q73f3yPi5rWjE-w",
            "IO_B6Cemwqjtqz2f9g",
            "J-XBvSSvyvrv-jiWqQ",
            "J-zF73Tznq_s_zWU_w",
            "JriSuHCvm66_-T6QrQ",
            "Ju-UuST0zK_u_zmR-A",
            "JOjAuCCunKzkrG2R_A",
            "Je2Sv36lnPi6rDzBqQ",
            "IrzCuXX1nq_pqjvCqg",
            "J-iQu3f3m_zq_jjB_A",
            "Jb7H7nChz6rpqziTrg",
            "J-uXvCegnqi9rjWR_w",
            "ILuW63Kjm_jl-zSf-w"
        };
            
        string[] prefixes = { "main", "w320h240", "common" };

        var counts = new Dictionary<string, int>
        {
            { prefixes[0], 1 },
            { prefixes[1], 10 },
            { prefixes[2], 3 }
        };
        
        counts.Zip(ids)
            .Select(t => (t.Second, t.First.Key, t.First.Value))
            .ForEach<(string id,string prefix,int i)>(link =>
        {
            string filename = $"{link.id}_{link.prefix}_{link.i}.jpg";

            string resourceUrl = $"https://static-cache.k2s.cc/thumbnail/{link.id}/{link.prefix}/{link.i}.jpeg";

            FileSystem.DownloadWebFile(resourceUrl, FileSystem.CreateDownloadPath(filename, "Thumbnails"));
                        
            Output.WriteLine($"Downloaded: {filename.WrapQuotes()} to disk!");
        });
            
        Output.WriteLine("Downloads completed!");
    }

    #region Exception Handling

    public static void OpenExceptionHandler(object? sender, ThreadExceptionEventArgs args)
    {
        OpenExceptionHandler(args.Exception);
    }
    
    public static void OpenExceptionHandler(object? sender, UnhandledExceptionEventArgs args)
    {
        if (args.ExceptionObject is not Exception exception)
            return;
        OpenExceptionHandler(exception);
    }

    public static void OpenExceptionHandler(Exception exception)
    {
        if (new FrameErrorHandler(exception).ShowDialog() == DialogResult.Abort)
            Application.Exit();
    }

    #endregion

    #region Task Scheduling
    
    // https://stackoverflow.com/questions/15428604/how-to-run-a-task-on-a-custom-taskscheduler-using-await

    public static Task RunTaskInMainThread(Func<Task> func)
    {
        return TaskFactory.StartNew(func).Unwrap();
    }
    
    public static Task<T> RunTaskInMainThread<T>(Func<Task<T>> func)
    {
        return TaskFactory.StartNew(func).Unwrap();
    }
    
    public static Task RunTaskInMainThread(Action func)
    {
        return TaskFactory.StartNew(func);
    }
    
    public static Task<T> RunTaskInMainThread<T>(Func<T> func)
    {
        return TaskFactory.StartNew(func);
    }

    #endregion

    #region Imports

    [System.Runtime.InteropServices.DllImport("wininet.dll")]
    private static extern bool InternetGetConnectedState(out int description, int reservedValue);

    #endregion
}