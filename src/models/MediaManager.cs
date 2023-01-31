using JackTheVideoRipper.extensions;
using JackTheVideoRipper.interfaces;
using JackTheVideoRipper.models.containers;
using JackTheVideoRipper.models.enums;
using JackTheVideoRipper.models.parameters;
using JackTheVideoRipper.models.rows;
using JackTheVideoRipper.modules;
using JackTheVideoRipper.views;

namespace JackTheVideoRipper.models;

public class MediaManager
{
    #region Data Members

    private readonly ProcessPool _processPool = new();

    private readonly ConcurrentHashSet<string> _downloadedUrls = new();

    #endregion

    #region Attributes

    public IProcessUpdateRow? GetRow(string tag) => _processPool.GetProcess(tag);

    public IProcessUpdateRow Selected => _processPool.Selected;
    
    public IEnumerable<string> DownloadedUrls => _downloadedUrls;

    #endregion

    #region Events

    public event Action QueueUpdated = delegate { };

    public event Action<IViewItem> ProcessAdded = delegate { };
    
    public event Action<IEnumerable<IViewItem>> ProcessesAdded = delegate { };

    public event Action<IViewItem> ProcessRemoved = delegate { };
    
    public event Action<IEnumerable<IViewItem>> ProcessesRemoved = delegate { };

    #endregion

    #region Constructor

    public MediaManager()
    {
        Core.ShutdownEvent += OnProgramShutdown;
        _processPool.ProcessCompleted += OnProcessCompleted;
        _processPool.ProcessStarted += OnProcessStarted;
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
    
    public void OnProgramShutdown()
    {
        History.Data.DownloadedUrls = DownloadedUrls.ToArray();
    }

    public string GetStatus()
    {
        return _processPool.PoolStatus;
    }

    public void RemoveCompleted() => ProcessesRemoved(_processPool.RemoveCompleted());

    public void RemoveFailed() => ProcessesRemoved(_processPool.RemoveFailed());

    private void AddRow(IMediaItem mediaItem, ProcessRowType processRowType)
    {
        switch (processRowType)
        {
            case ProcessRowType.Download:
                _downloadedUrls.Add(mediaItem.Url);
                AddProcess(new DownloadProcessUpdateRow(mediaItem, _processPool.OnCompleteProcess));
                break;
            case ProcessRowType.Compress:
                AddProcess(new CompressProcessUpdateRow(mediaItem, _processPool.OnCompleteProcess));
                break;
            default:
            case ProcessRowType.Convert:
            case ProcessRowType.Recode:
            case ProcessRowType.Repair:
                break;
        }
    }

    private void AddProcess(IProcessUpdateRow processUpdateRow)
    {
        _processPool.QueueProcess(processUpdateRow, ProcessAdded);
    }
    
    private async ValueTask QueueProcessAsync(IMediaItem row, ProcessRowType processRowType, 
        CancellationToken? cancellationToken = null)
    {
        void AddProcessTask()
        {
            AddRow(row, processRowType);
        }
        
        await Threading.RunInMainContext(AddProcessTask, cancellationToken);
    }

    public IEnumerable<DownloadMediaItem> FilterExistingUrls(IEnumerable<DownloadMediaItem> items)
    {
        return items.Where(UrlExists);
    }

    public async Task DownloadBatch(IEnumerable<string> urls)
    {
        await DownloadBatch(urls.MergeReturn());
    }
    
    public async Task DownloadBatch(string urlString = "")
    {
        if (FrameNewMediaBatch.GetMedia(urlString) is not { } items)
            return;

        async ValueTask QueueDownloadTask(DownloadMediaItem row, CancellationToken token)
        {
            await QueueProcessAsync(row, ProcessRowType.Download, token);
        }

        await Parallel.ForEachAsync(FilterExistingUrls(items), QueueDownloadTask);
        
        QueueUpdated();
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
    
    private bool UrlExists(DownloadMediaItem item)
    {
        return !DownloadedUrls.Contains(item.Url);
    }

    private async Task GetNewMedia(MediaType type)
    {
        if (FrameNewMedia.GetMedia(type) is not { } mediaItemRow)
            return;
            
        await QueueProcessAsync(mediaItemRow, ProcessRowType.Download);
    }

    private async Task GetNewMediaSimple(MediaType type)
    {
        if (FrameNewMediaSimple.GetMedia(type) is not { } mediaItemRow)
            return;
            
        await QueueProcessAsync(mediaItemRow, ProcessRowType.Download);
    }

    public void RetryProcess(string tag)
    {
        _processPool.RetryProcess(tag);
        QueueUpdated();
    }

    public void RemoveProcess(string tag)
    {
        if (_processPool.Remove(tag) is not { } result)
            return;

        ProcessRemoved(result);
    }

    public void ResumeProcess(string tag)
    {
        if (_processPool.TryGetProcess(tag, out IProcessUpdateRow? result) || result is not { })
            return;

        result.Resume();

        QueueUpdated();
    }

    public void RemoveSelectedProcess()
    {
        _processPool.RemoveSelected();
    }

    public void CopyFailedUrls()
    {
        FileSystem.SetClipboardText(_processPool.FailedUrls.MergeNewline());
    }

    public bool SelectedHasStatus(ProcessStatus? processStatus)
    {
        return Selected.ProcessStatus == processStatus;
    }

    public async Task PerformContextAction(ContextActions contextAction)
    {
        if (Ripper.SelectedTag.IsNullOrEmpty())
            return;
        
        switch (contextAction)
        {
            case ContextActions.OpenMedia:
                if (Selected.Completed)
                    await Common.OpenFileInMediaPlayer(Selected.Path);
                break;
            case ContextActions.Copy:
                Core.CopyToClipboard(Selected.Url);
                break;
            case ContextActions.Delete:
                if (Selected.Finished)
                    FileSystem.DeleteFileIfExists(Selected.Path);
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
                await Common.OpenInBrowser(Selected.Url);
                break;
            case ContextActions.Reveal:
                FileSystem.OpenFolder(Selected.Path);
                break;
            case ContextActions.Resume:
                if (Selected.Paused)
                    ResumeProcess(Selected.Tag);
                break;
            case ContextActions.Remove:
                RemoveProcess(Selected.Tag);
                break;
            case ContextActions.OpenConsole:
                await Selected.OpenInConsole();
                break;
            case ContextActions.SaveLogs:
                Selected.SaveLogs();
                break;
            default:
                ArgumentOutOfRangeException innerException = new(nameof(contextAction), contextAction, null);
                throw new MediaManagerException(Messages.ContextActionFailed, innerException);
        }
    }
    
    public async Task DownloadFromUrl(string url)
    {
        MediaItemRow<DownloadMediaParameters> row = new(url, mediaParameters: new DownloadMediaParameters(url));
        await QueueProcessAsync(row, ProcessRowType.Download);
    }

    public async Task CompressVideo(string filepath)
    {
        string newFilepath = FFMPEG.GetOutputFilename(filepath, FFMPEG.Operation.Compress);
        if (!FileSystem.WarnAndDeleteIfExists(newFilepath))
            return;
        
        //FFMPEG.VideoInformation videoInformation = await FFMPEG.ExtractVideoInformation(filepath);
             
        ExifData metadata = await ExifTool.GetMetadata(filepath);

        MediaItemRow<FfmpegParameters> row = new(title:metadata.Title, filepath: filepath,
            mediaParameters: FFMPEG.Compress(filepath));
             
        await QueueProcessAsync(row, ProcessRowType.Compress);
    }

    public async Task CompressBulk(string directoryPath)
    {
        async ValueTask Compress(string filepath, CancellationToken token)
        {
            await CompressVideo(filepath);
        }

        string[] filepaths = Directory.GetFiles(directoryPath, FileFilters.VideoFiles);
        
        await Parallel.ForEachAsync(filepaths, Compress);
    }

    public async Task RecodeVideo(string filepath)
    {
        MediaItemRow<FfmpegParameters> row = new(filepath: filepath, mediaParameters: FFMPEG.Recode(filepath));
        await QueueProcessAsync(row, ProcessRowType.Recode);
    }

    public async Task RepairVideo(string filepath)
    {
        // Order list of parameters for each task necessary
        IEnumerable<FfmpegParameters> repairTaskParameters = await FFMPEG.RepairVideo(filepath);
            
        MediaItemRow<FfmpegParameters> CreateRepairRow(FfmpegParameters parameters)
        {
            return new MediaItemRow<FfmpegParameters>(filepath: parameters.OutputFilepath, mediaParameters: parameters);
        }

        // Rows for each process required
        IEnumerable<MediaItemRow<FfmpegParameters>> mediaItemRows = repairTaskParameters.Select(CreateRepairRow);

        async void Repair(MediaItemRow<FfmpegParameters> row)
        {
            await QueueProcessAsync(row, ProcessRowType.Repair);
        }

        mediaItemRows.ForEach(Repair);
    }

    #endregion
    
    #region Event Handlers
    
    private void OnProcessCompleted(IProcessUpdateRow completedProcessRow)
    {
        QueueUpdated();
    }

    private void OnProcessStarted()
    {
        QueueUpdated();
    }

    public bool OnFormClosing()
    {
        if (!_processPool.AnyActive)
            return false;

        if (!Modals.ConfirmExit())
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
        if (FileSystem.SelectFile() is not { } filepath)
            return;

        string filepathQuotes = filepath.WrapQuotes();
        
        Output.WriteLine(string.Format(Messages.VerifyingFile, filepathQuotes));
        Output.WriteLine(FFMPEG.VerifyIntegrity(filepath), sendAsNotification:true);

        string logFilepath = FileSystem.TempFile;
        string result = File.ReadAllText(logFilepath).IsNullOrEmpty()
            ? string.Format(Messages.VerifyNoErrors, filepathQuotes)
            : string.Format(Messages.VerifyErrorsDetected, filepathQuotes, logFilepath.WrapQuotes());

        Output.WriteLine(result);
    }

    #endregion
    
    #region Embedded Types

    public class MediaManagerException : Exception
    {
        public MediaManagerException()
        {
        }
        
        public MediaManagerException(string message) : base(message)
        {
        }
        
        public MediaManagerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    #endregion
}