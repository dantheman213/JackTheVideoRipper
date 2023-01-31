using JackTheVideoRipper.extensions;
using JackTheVideoRipper.interfaces;
using JackTheVideoRipper.models;
using JackTheVideoRipper.models.enums;
using JackTheVideoRipper.views;

namespace JackTheVideoRipper;

/**
 * Serves as interface between view and model
 */
public class Ripper
{
    public static Ripper Instance = null!;
    
    #region Data Members

    private readonly MediaManager _mediaManager = new();
    private readonly NotificationsManager _notificationsManager = new();
    public readonly FrameMain FrameMain;

    private readonly IViewItemProvider _viewItemProvider;

    #endregion

    #region Attributes

    public static string SelectedTag => Instance.FrameMain.CachedSelectedTag;
    
    public static IViewItemProvider ViewItemProvider => Instance._viewItemProvider;

    #endregion

    public static IViewItem CreateViewItem(string? tag = null)
    {
        return ViewItemProvider.CreateViewItem(tag);
    }
    
    public static IViewItem CreateViewItem(string[] items, string? tag = null)
    {
        return ViewItemProvider.CreateViewItem(items, tag);
    }
    
    public static IViewItem CreateMediaViewItem(IMediaItem mediaItem, string? tag = null)
    {
        return ViewItemProvider.CreateMediaViewItem(mediaItem, tag);
    }

    public Ripper(IViewItemProvider viewItemProvider)
    {
        _viewItemProvider = viewItemProvider;
        FrameMain = new FrameMain(this);
        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        NotificationsManager.SendNotificationEvent += _ => _notificationsManager.Reset();
        FrameMain.ContextActionEvent += OnContextAction;
        FrameMain.DependencyActionEvent += OnUpdateDependency;
        FrameMain.Shown += (_, _) => _notificationsManager.Start();
    }

    public string GetProgramStatus()
    {
        return _mediaManager.GetStatus();
    }

    public ProcessStatus? GetSelectedStatus()
    {
        return Instance.GetStatus(FrameMain.CachedSelectedTag);
    }

    public ProcessStatus? GetStatus(string tag)
    {
        return _mediaManager.GetRow(tag)?.ProcessStatus;
    }

    public async Task Update()
    {
        await _mediaManager.UpdateListItemRows();
    }

    public void SubscribeMediaManagerEvents(Action updateEventHandler,
        Action<IViewItem> addAction,
        Action<IEnumerable<IViewItem>> addMultiAction,
        Action<IViewItem> removeAction,
        Action<IEnumerable<IViewItem>> removeMultiAction)
    {
        _mediaManager.QueueUpdated += updateEventHandler;
        _mediaManager.ProcessAdded += addAction;
        _mediaManager.ProcessesAdded += addMultiAction;
        _mediaManager.ProcessRemoved += removeAction;
        _mediaManager.ProcessesRemoved += removeMultiAction;
    }

    #region Event Handlers

    public async Task OnPasteContent()
    {
        string url = FileSystem.GetClipboardText();
            
        if (url.Invalid(FileSystem.IsValidUrl))
            return;

        await _mediaManager.DownloadFromUrl(url);
    }

    public async void OnCompressVideo(object? sender, EventArgs e)
    {
        if (FileSystem.SelectFile() is not { } filepath)
            return;
             
        await _mediaManager.CompressVideo(filepath);
    }
        
    public async Task OnCompressBulk(object? sender, EventArgs e)
    {
        if (FileSystem.SelectFolder() is not { } directoryPath)
            return;

        await _mediaManager.CompressBulk(directoryPath);
    }
        
    public async void OnRecodeVideo(object? sender, EventArgs e)
    {
        if (FileSystem.SelectFile() is not { } filepath)
            return;

        await _mediaManager.RecodeVideo(filepath);
    }
        
    public async void OnRepairVideo(object? sender, EventArgs e)
    {
        if (FileSystem.SelectFile() is not { } filepath)
            return;

        await _mediaManager.RepairVideo(filepath);
    }
    
    public async void OnDownloadVideo(object? sender, EventArgs e)
    {
        await _mediaManager.DownloadMediaDialog(MediaType.Video);
    }
    
    public async void OnDownloadAudio(object? sender, EventArgs e)
    {
        await _mediaManager.DownloadMediaDialog(MediaType.Audio);
    }
    
    // TODO: Should it just not wait and notify user of reconnection?
    public async Task OnConnectionLost()
    {
        Modals.Notification(Messages.LostConnection);
        _mediaManager.PauseAll();
        await Tasks.WaitUntil(Core.IsConnectedToInternet); //< Delay processes until connected
        _mediaManager.ResumeAll();
    }

    public async void OnContextAction(object? sender, ContextActionEventArgs e)
    {
        await _mediaManager.PerformContextAction(e.ContextAction);
    }

    public void OnNotificationBarClicked(object? sender, MouseEventArgs e)
    {
        if (e.IsRightClick())
        {
            NotificationsManager.ClearPushNotifications();
        }
        else
        {
            _notificationsManager.OpenNotificationWindow();
        }
    }

    public async void OnApplicationClosing(object? sender, FormClosingEventArgs e)
    {
        if (_mediaManager.OnFormClosing())
        {
            e.Cancel = true;
            return;
        }
        
        // Allow normal shutdown for all subscribers
        await Core.Shutdown();
    }
    
    // Edit Menu
    public void OnCopyFailedUrls(object? sender, EventArgs e)
    {
        _mediaManager.CopyFailedUrls();
    }

    public void OnRetryAll(object? sender, EventArgs e)
    {
        _mediaManager.RetryAll();
    }

    public void OnStopAll(object? sender, EventArgs e)
    {
        _mediaManager.StopAll();
    }

    public void OnRemoveFailed(object? sender, EventArgs e)
    {
        _mediaManager.RemoveFailed();
    }

    public void OnClearAll(object? sender, EventArgs e)
    {
        _mediaManager.ClearAll();
    }

    public void OnRemoveCompleted(object? sender, EventArgs e)
    {
        _mediaManager.RemoveCompleted();
    }

    public void OnPauseAll(object? sender, EventArgs e)
    {
        _mediaManager.PauseAll();
    }

    public void OnResumeAll(object? sender, EventArgs e)
    {
        _mediaManager.ResumeAll();
    }
    
    public async void OnBatchPlaylist(object? sender, EventArgs e)
    {
        await _mediaManager.BatchPlaylist();
    }

    public async void OnBatchDocument(object? sender, EventArgs e)
    {
        await _mediaManager.BatchDocument();
    }

    public async void OnDownloadBatch(object? sender, EventArgs e)
    {
        await _mediaManager.DownloadBatch();
    }

    public static void OnVerifyIntegrity(object? sender, EventArgs e)
    {
        MediaManager.VerifyIntegrity();
    }

    public static async void OnOpenConsole(object? sender, EventArgs e)
    {
        await Output.OpenMainConsoleWindow();
    }

    public static void OnOpenHistory(object? sender, EventArgs e)
    {
        Pages.OpenPageBackground<FrameHistory>();
    }
    
    public static async void OnUpdateDependency(object? sender, DependencyActionEventArgs e)
    {
        await Core.UpdateDependency(e.Dependency);
    }

    public static void OnEndStartup(object? sender, EventArgs e)
    {
        Statistics.EndStartup();
        Output.WriteLine(Statistics.StartupMessage, sendAsNotification:true);
    }
    
    public void OnClearAllViewItems(object? sender, EventArgs e)
    {
        _mediaManager.ClearAll();
    }

    public static void OnOpenDownloads(object? sender, EventArgs e)
    {
        Task.Run(FileSystem.OpenDownloads);
    }

    public static void OnOpenTaskManager(object? sender, EventArgs e)
    {
        Task.Run(FileSystem.OpenTaskManager);
    }

    public static void OnCheckForUpdates(object? sender, EventArgs e)
    {
        Task.Run(Core.CheckForUpdates);
    }

    public static void OnOpenSettings(object? sender, EventArgs e)
    {
        Pages.OpenPage<FrameSettings>();
    }

    public static void OnOpenInstallFolder(object? sender, EventArgs e)
    {
        Task.Run(() => FileSystem.OpenFileExplorer(FileSystem.Paths.Install));
    }

    public static void OnOpenAbout(object? sender, EventArgs e)
    {
        Pages.OpenPage<FrameAbout>();
    }

    public static void OnOpenConvert(object? sender, EventArgs e)
    {
        Pages.OpenPage<FrameConvert>();
    }

    public static async Task OnCheckForApplicationUpdates()
    {
        await AppUpdate.CheckForNewAppVersion(false);
    }
    
    #endregion
}