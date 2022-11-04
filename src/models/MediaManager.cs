using JackTheVideoRipper.extensions;
using JackTheVideoRipper.models;
using JackTheVideoRipper.models.enums;

namespace JackTheVideoRipper;

public class MediaManager
{
    #region Data Members

    private readonly ProcessPool _processPool;

    #endregion

    #region Attributes

    public ProcessUpdateRow? GetRow(string tag) => _processPool.Get(tag);

    public ProcessUpdateRow? Selected => _processPool.Selected;

    public string ToolbarStatus => $"{GetProgramStatus(),-20}";

    public static string ToolbarCpu => $@"CPU: {Statistics.GetCpuUsagePercentage(),7}";

    public static string ToolbarMemory => $@"Available Memory: {Statistics.GetAvailableMemory(),9}";

    public static string ToolbarNetwork => $@"Network Usage: {Statistics.GetNetworkTransfer(),10}";

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

    public void UpdateListItemRows()
    {
        _processPool.Update();
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

    public void RemoveCompleted()
    {
        _processPool.RemoveCompleted().ForEach(ProcessRemoved);
    }

    public void RemoveFailed()
    {
        _processPool.RemoveFailed().ForEach(ProcessRemoved);
    }

    public void QueueProcess(MediaItemRow row)
    {
        _processPool.QueueProcess(row);
        ProcessAdded(row.ListViewItem);
    }

    private void QueueProcess(DownloadMediaItem item)
    {
        QueueProcess(item.As<MediaItemRow>());
    }

    public void QueueBatchDownloads()
    {
        Common.RepeatInvoke(UpdateProcessQueue, Settings.Data.MaxConcurrentDownloads);
    }

    public void DownloadBatch(string urls = "")
    {
        if (FrameNewMediaBatch.GetMedia(urls) is not { } items)
            return;

        items.ForEach(QueueProcess);
        QueueBatchDownloads();
    }

    public void BatchDocument()
    {
        if (FileSystem.GetFileUsingDialog() is not { } fileContent)
            return;

        DownloadBatch(Import.GetAllUrlsFromPayload(fileContent).MergeReturn());
    }

    public void BatchPlaylist()
    {
        if (FrameImportPlaylist.GetMetadata() is not { } youTubeLinks)
            return;

        DownloadBatch(youTubeLinks.MergeReturn());
    }

    public void DownloadMediaDialog(MediaType type)
    {
        if (FrameNewMedia.GetMedia(type) is not { } mediaItemRow)
            return;

        QueueProcess(mediaItemRow);
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
        if (_processPool.Get(tag) is not { } result)
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
        return Selected?.ProcessStatus == processStatus;
    }

    public void PerformContextAction(ContextActions contextAction)
    {
        if (Selected is null)
            return;

        switch (contextAction)
        {
            case ContextActions.OpenMedia when Selected.Completed:
                Common.OpenFileInMediaPlayer(Selected.Path);
                break;
            case ContextActions.Copy:
                Core.CopyToClipboard(Selected.Url);
                break;
            case ContextActions.Delete:
                RemoveProcess(Selected.Tag);
                break;
            case ContextActions.Stop when !Selected.Completed:
                StopSelectedProcess();
                break;
            case ContextActions.Retry when Selected.Failed:
                RetryProcess(Selected.Tag);
                break;
            case ContextActions.OpenUrl:
                Common.OpenInBrowser(Selected.Url);
                break;
            case ContextActions.Reveal:
                FileSystem.OpenFolder(Selected.Path);
                break;
            case ContextActions.Resume when Selected.Paused:
                ResumeProcess(Selected.Tag);
                break;
            case ContextActions.Remove when Selected.Finished:
                FileSystem.DeleteIfExists(Selected.Path);
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

    private void ProcessCompletionCallback(ProcessUpdateRow processUpdateRow)
    {
        Core.SendNotification($"\"{processUpdateRow.Title.TruncateEllipse(35)}\" finished downloading!");
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
        _processPool.Get(tag)?.Stop();
    }

    public void StopProcess(ProcessUpdateRow? processUpdateRow)
    {
        processUpdateRow?.Stop();
    }

    public void StopSelectedProcess()
    {
        Selected?.Stop();
    }

    #endregion
}