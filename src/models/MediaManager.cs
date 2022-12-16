using JackTheVideoRipper.extensions;
using JackTheVideoRipper.interfaces;
using JackTheVideoRipper.models.enums;
using JackTheVideoRipper.modules;
using JackTheVideoRipper.views;

namespace JackTheVideoRipper.models;

public class MediaManager
{
    #region Data Members

    private readonly ProcessPool _processPool;

    #endregion

    #region Attributes

    public IProcessUpdateRow? GetRow(string tag) => _processPool.GetProcess(tag);

    public IProcessUpdateRow Selected => _processPool.Selected;

    public string ToolbarStatus => $"{GetProgramStatus(),-20}"; // 20 chars

    public static string ToolbarCpu => $@"CPU: {Statistics.GetCpuUsagePercentage(),7}"; // 12 chars

    public static string ToolbarMemory => $@"Available Memory: {Statistics.GetAvailableMemory(),9}"; // 27 chars

    public static string ToolbarNetwork => $@"Network Usage: {Statistics.GetNetworkTransfer(),10}"; // 25 chars

    #endregion

    #region Events

    public event Action QueueUpdated = delegate { };

    public event Action<ListViewItem> ProcessAdded = delegate { };

    public event Action<ListViewItem> ProcessRemoved = delegate { };

    #endregion

    #region Constructor

    public MediaManager()
    {
        _processPool = new ProcessPool();
        _processPool.ProcessCompleted += ProcessCompletionCallback;
        _processPool.ProcessStarted += ProcessStartedCallback;
    }

    #endregion

    #region Bulk Actions

    public void ClearAll()
    {
        _processPool.ClearAll();
    }

    public void StopAll()
    {
        _processPool.StopAll();
    }

    public void RetryAll()
    {
        _processPool.RetryAllProcesses();
    }

    public async Task UpdateListItemRows()
    {
        await _processPool.Update();
    }

    public void PauseAll()
    {
        _processPool.PauseAll();
    }

    public void ResumeAll()
    {
        _processPool.ResumeAll();
    }

    #endregion

    #region Public Methods

    public string GetProgramStatus()
    {
        return _processPool.PoolStatus;
    }

    public void OnProcessRemoved(ListViewItem processViewItem)
    {
        ProcessRemoved(processViewItem);
    }

    public void OnProcessAdded(ListViewItem processViewItem)
    {
        ProcessAdded(processViewItem);
    }

    public void RemoveCompleted()
    {
        Parallel.ForEach(_processPool.RemoveCompleted(), OnProcessRemoved);
    }

    public void RemoveFailed()
    {
        Parallel.ForEach(_processPool.RemoveFailed(), OnProcessRemoved);
    }

    public void AddRow(IMediaItem row)
    {
        OnProcessAdded(_processPool.QueueDownloadProcess(row));
    }

    public async Task QueueProcess(IMediaItem row)
    {
        await Core.RunTaskInMainThread(() => AddRow(row));
    }
    
    public async ValueTask QueueProcessAsync(IMediaItem row, CancellationToken cancellationToken)
    {
        await Core.RunTaskInMainThread(() => AddRow(row));
    }

    public void QueueBatchDownloads()
    {
        Common.RepeatInvoke(UpdateProcessQueue, Settings.Data.MaxConcurrentDownloads);
    }

    public async Task DownloadBatch(IEnumerable<string>? urls = null)
    {
        if (FrameNewMediaBatch.GetMedia(urls?.MergeReturn() ?? string.Empty) is not { } items)
            return;

        HashSet<string> existingUrls = _processPool
            .GetOfType<DownloadProcessUpdateRow>()
            .Select(r => r.Url)
            .ToHashSet();

        IEnumerable<DownloadMediaItem> uniqueUrls = items.Where(i => !existingUrls.Contains(i.Url));
        await Parallel.ForEachAsync(uniqueUrls, QueueProcessAsync);
        QueueBatchDownloads();
    }

    public async Task BatchDocument()
    {
        if (FileSystem.ReadFileUsingDialog() is not { } fileContent)
            return;

        await DownloadBatch(Import.GetAllUrlsFromPayload(fileContent));
    }

    public async Task BatchPlaylist()
    {
        if (await FrameImportPlaylist.GetMetadata() is not { } links)
            return;

        await DownloadBatch(links);
    }

    public async Task DownloadMediaDialog(MediaType type)
    {
        if (Settings.Data.SkipMetadata)
        {
            await GetNewMediaSimple(type);
        }
        else
        {
            await GetNewMedia(type);
        }
    }

    private async Task GetNewMedia(MediaType type)
    {
        if (FrameNewMedia.GetMedia(type) is not { } mediaItemRow)
            return;
            
        await QueueProcess(mediaItemRow);
    }

    private async Task GetNewMediaSimple(MediaType type)
    {
        if (FrameNewMediaSimple.GetMedia(type) is not { } mediaItemRow)
            return;
            
        await QueueProcess(mediaItemRow);
    }

    public void RetryProcess(string tag)
    {
        _processPool.RetryProcess(tag);
        UpdateProcessQueue();
    }

    public void RemoveProcess(string tag)
    {
        if (_processPool.Remove(tag) is not { } result)
            return;

        ProcessRemoved(result);
    }

    public void ResumeProcess(string tag)
    {
        if (_processPool.GetProcess(tag) is not { } result)
            return;

        result.Resume();

        ProcessStartedCallback();
    }

    public void RemoveSelectedProcess()
    {
        _processPool.RemoveSelected();
    }

    public void CopyFailedUrls()
    {
        FileSystem.SetClipboardText(_processPool.GetAllFailedUrls().MergeNewline());
    }

    public bool SelectedHasStatus(ProcessStatus processStatus)
    {
        return Selected.ProcessStatus == processStatus;
    }

    public async Task PerformContextAction(ContextActions contextAction)
    {
        switch (contextAction)
        {
            case ContextActions.OpenMedia:
                if (Selected.Completed)
                    await Task.Run(() => Common.OpenFileInMediaPlayer(Selected.Path));
                break;
            case ContextActions.Copy:
                Core.CopyToClipboard(Selected.Url);
                break;
            case ContextActions.Delete:
                RemoveProcess(Selected.Tag);
                break;
            case ContextActions.Stop:
                if (!Selected.Completed)
                    StopSelectedProcess();
                break;
            case ContextActions.Retry:
                if (Selected.Failed)
                    RetryProcess(Selected.Tag);
                break;
            case ContextActions.OpenUrl:
                Common.OpenInBrowser(Selected.Url);
                break;
            case ContextActions.Reveal:
                await FileSystem.OpenFolder(Selected.Path);
                break;
            case ContextActions.Resume:
                if (Selected.Paused)
                    ResumeProcess(Selected.Tag);
                break;
            case ContextActions.Remove:
                if (Selected.Finished)
                    FileSystem.DeleteFileIfExists(Selected.Path);
                break;
            case ContextActions.OpenConsole:
                await Selected.OpenInConsole();
                break;
            case ContextActions.SaveLogs:
                Selected.SaveLogs();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(contextAction), contextAction, null);
        }
    }

    #endregion

    #region Private Methods

    private void UpdateProcessQueue()
    {
        QueueUpdated();
    }

    private void ProcessStartedCallback()
    {
        UpdateProcessQueue();
    }

    private void ProcessCompletionCallback(IProcessUpdateRow processUpdateRow)
    {
        UpdateProcessQueue();
    }

    #endregion

    #region Event Handlers

    public bool OnFormClosing()
    {
        if (!_processPool.AnyActive)
            return false;

        if (!Core.ConfirmExit())
            return true;

        _processPool.KillAllRunning();
        return false;
    }

    #endregion

    #region Static Methods

    public void StopProcess(string tag)
    {
        _processPool.GetProcess(tag)?.Stop();
    }

    public void StopSelectedProcess()
    {
        Selected.Stop();
    }

    public static void VerifyIntegrity()
    {
        if (FileSystem.GetFilePathUsingDialog() is not { } filepath)
            return;
        
        Output.WriteLine($"Verifying file: {filepath.WrapQuotes()}");
        Output.WriteLine(FFMPEG.VerifyIntegrity(filepath), sendAsNotification:true);
    }

    #endregion
}