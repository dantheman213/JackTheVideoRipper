using System.Runtime.ExceptionServices;
using JackTheVideoRipper.extensions;
using JackTheVideoRipper.modules;
using JackTheVideoRipper.Properties;
using JackTheVideoRipper.views;

namespace JackTheVideoRipper;

public static class Core
{
    public static string ApplicationTitle => $@"{AppInfo.ProgramName} {Common.GetAppVersion()}";

    public static TaskScheduler Scheduler { get; private set; } = null!;
    
    private static TaskFactory TaskFactory { get; set; } = null!;

    public static readonly CancellationTokenSource FormClosingCancellationTokenSource = new();
    
    private static CancellationToken FormClosingCancellationToken => FormClosingCancellationTokenSource.Token;

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
        FormClosingCancellationTokenSource.Cancel();
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
            if (Modals.Confirmation(Messages.InstallMessage, Captions.RequiredInstalled))
            {
                InstallDependencies();
            }
            else
            {
                Modals.Error(Messages.InstallationError, Captions.ApplicationError);
                return;
            }
        }

        // Verify FFMPEG
        if (!FFMPEG.IsInstalled)
        {
            MissingDependencyModal("FFMPEG");
        }

        // Verify Atomic Parsley
        if (!AtomicParsley.IsInstalled)
        {
            MissingDependencyModal("Atomic Parsley");
        }
        
        // Verify Aria2C
        if (!Aria2c.IsInstalled)
        {
            MissingDependencyModal("Aria2C");
        }

        // Verify ExifTool
        if (!ExifTool.IsInstalled)
        {
            MissingDependencyModal("ExifTool");
        }
    }

    private static void MissingDependencyModal(string name)
    {
        Modals.Warning(string.Format(Messages.DependencyMissing, name), Captions.RequiredNotInstalled);
    }

    private static void InstallDependencies()
    {
        FrameYTDLDependencyInstall frameDependencyInstall = new();
        frameDependencyInstall.ShowDialog();
        frameDependencyInstall.Close();
        Modals.Notification(Messages.InstallationSuccess, Captions.RequiredInstalled);
    }

    public static async Task CheckForYouTubeDLUpdates()
    {
        await Task.Run(YouTubeDL.CheckForUpdates);
    }

    public static void CopyToClipboard(string url)
    {
        FileSystem.SetClipboardText(url);
    }

    public static async Task OpenInstallFolder()
    {
        await FileSystem.OpenFileExplorer(FileSystem.Paths.Install);
    }
    
    public static async Task DownloadDependency(Dependencies dependency)
    {
        switch (dependency)
        {
            case Dependencies.YouTubeDL:
                await FileSystem.InstallProgram(Urls.YouTubeDL, Executables.YouTubeDL);
                //await FileSystem.GetWebResourceHandle(Urls.YouTubeDL, FileSystem.Paths.Install).Run();
                break;
            case Dependencies.FFMPEG:
                await FileSystem.InstallProgram(Urls.FFMPEG, Executables.FFProbe);
                //await FFMPEG.DownloadLatest();
                break;
            case Dependencies.Handbrake:
                FileSystem.OpenWebPage(Urls.HandbrakeDownload);
                //await FileSystem.InstallProgram(Urls.HandbrakeDownload, Executables.Handbrake);
                break;
            case Dependencies.VLC:
                FileSystem.OpenWebPage(Urls.VLC);
                //await FileSystem.InstallProgram(Urls.VLC, Executables.VLC);
                break;
            case Dependencies.AtomicParsley:
                await FileSystem.InstallProgram(Urls.AtomicParsley, Executables.AtomicParsley);
                break;
            case Dependencies.Redistributables:
                // TODO:
                await FileSystem.GetWebResourceHandle(Urls.VS2010Redistributables).Run();
                break;
            case Dependencies.Aria2c:
                await FileSystem.InstallProgram(Urls.Aria2c, Executables.Aria2c);
                break;
            case Dependencies.ExifTool:
                await FileSystem.InstallProgram(Urls.ExifTool, Executables.ExifTool);
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
    
    public static void Crash(string message, Exception? exception = null)
    {
        Environment.FailFast(message, exception);
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
    
    public static void OpenExceptionHandler(object? sender, FirstChanceExceptionEventArgs args)
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
            Crash("Application closed due to unhandled exception.", exception);
    }

    #endregion

    #region Task Scheduling
    
    // https://stackoverflow.com/questions/15428604/how-to-run-a-task-on-a-custom-taskscheduler-using-await

    public static Task RunTaskInMainThread(Func<Task> func, CancellationToken? cancellationToken = null,
        TaskCreationOptions taskCreationOptions = TaskCreationOptions.None)
    {
        return TaskFactory.StartNew(func, cancellationToken ?? FormClosingCancellationToken,
            taskCreationOptions, Scheduler).Unwrap();
    }
    
    public static Task<T> RunTaskInMainThread<T>(Func<Task<T>> func, CancellationToken? cancellationToken = null,
        TaskCreationOptions taskCreationOptions = TaskCreationOptions.None)
    {
        return TaskFactory.StartNew(func, cancellationToken ?? FormClosingCancellationToken,
            taskCreationOptions, Scheduler).Unwrap();
    }
    
    public static Task RunTaskInMainThread(Action func, CancellationToken? cancellationToken = null,
        TaskCreationOptions taskCreationOptions = TaskCreationOptions.None)
    {
        return TaskFactory.StartNew(func, cancellationToken ?? FormClosingCancellationToken, 
            taskCreationOptions, Scheduler);
    }
    
    public static Task<T> RunTaskInMainThread<T>(Func<T> func, CancellationToken? cancellationToken = null,
        TaskCreationOptions taskCreationOptions = TaskCreationOptions.None)
    {
        return TaskFactory.StartNew(func, cancellationToken ?? FormClosingCancellationToken,
            taskCreationOptions, Scheduler);
    }

    #endregion

    #region Imports

    [System.Runtime.InteropServices.DllImport("wininet.dll")]
    private static extern bool InternetGetConnectedState(out int description, int reservedValue);

    #endregion
}