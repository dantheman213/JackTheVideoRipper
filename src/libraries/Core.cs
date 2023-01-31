using JackTheVideoRipper.extensions;
using JackTheVideoRipper.modules;
using JackTheVideoRipper.Properties;

namespace JackTheVideoRipper;

public static class Core
{
    public static string ApplicationTitle => $@"{AppInfo.ProgramName} {Common.GetAppVersion()}";

    public static event Action ShutdownEvent = delegate { };

    //static Core() { }

    public static FrameMain FrameMain => Ripper.Instance.FrameMain;

    public static async Task LoadConfigurationFiles()
    {
        await Config.Load();
    }

    public static async Task Shutdown()
    {
        ShutdownEvent();
        await Config.Save();
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
        Pages.OpenPage<FrameYTDLDependencyInstall>();
        Modals.Notification(Messages.InstallationSuccess, Captions.RequiredInstalled);
    }

    public static async Task CheckForYouTubeDLUpdates()
    {
        await YouTubeDL.CheckForUpdates();
    }

    public static void CopyToClipboard(string url)
    {
        FileSystem.SetClipboardText(url);
    }

    public static async Task UpdateDependencies()
    {
        await Task.Delay(100);
        /*await Parallel.ForEachAsync(Enum.GetValues<Dependencies>(), async (d, _) =>
        {
            await UpdateDependency(d);
        });*/
    }

    public static async Task UpdateDependency(Dependencies dependency)
    {
        switch (dependency)
        {
            case Dependencies.YouTubeDL when !YouTubeDL.UpToDate:
            case Dependencies.FFMPEG:
            case Dependencies.Handbrake:
            case Dependencies.VLC:
            case Dependencies.AtomicParsley:
            case Dependencies.Redistributables:
            case Dependencies.Aria2c:
            case Dependencies.ExifTool:
                await DownloadDependency(dependency);
                break;
            default:
                return;
        }
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
    
    public static async Task CheckForUpdates()
    {
        await AppUpdate.CheckForNewAppVersion();
    }
    
    public static Form MainForm => Application.OpenForms[0];

    public static bool IsConnectedToInternet()
    {
        return InternetGetConnectedState(out int _, 0);         
    }
    
    public static void Crash(string message, Exception? exception = null)
    {
        Environment.FailFast(message, exception);
    }
    
    private static void DownloadImages(IEnumerable<string> ids)
    {
        Settings.Load();
            
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

    #region Imports

    [System.Runtime.InteropServices.DllImport("wininet.dll")]
    private static extern bool InternetGetConnectedState(out int description, int reservedValue);

    #endregion
}