using System.Collections.Concurrent;
using JackTheVideoRipper.extensions;
using JackTheVideoRipper.interfaces;
using JackTheVideoRipper.models;

namespace JackTheVideoRipper;

public class ProcessPool
{
    #region Data Members

    private readonly ProcessTable _processTable = new();
    private readonly ConcurrentQueue<IProcessUpdateRow> _processQueue = new();
    private readonly IndexableQueue<IProcessUpdateRow> _pausedProcessQueue = new();
    private readonly ProcessTable _runningProcesses = new();
    private readonly ConcurrentHashSet<IProcessUpdateRow> _finishedProcesses = new();

    public static readonly ErrorLogger ErrorLogger = new();
    
    private bool _updating;

    #endregion

    #region Events

    public event Action<IProcessUpdateRow> ProcessCompleted = delegate {  };
    
    public event Action ProcessStarted = delegate {  };

    #endregion

    #region Properties

    public bool AnyActive => AnyRunning || AnyQueued || AnyPaused;

    public bool AnyRunning => _runningProcesses.HasCached;

    public bool AnyQueued => !_processQueue.IsEmpty;

    public bool AnyPaused => !_pausedProcessQueue.Empty();
    
    public bool QueueEmpty => _processQueue.IsEmpty && _pausedProcessQueue.Empty();
    
    public string PoolStatus
    {
        get
        {
            if (AnyRunning)
                return "Downloading Media";
            if (AnyQueued)
                return "Awaiting Download";
            if (QueueEmpty)
                return "Idle";
            return "";
        }
    }

    private IProcessUpdateRow? NextProcess
    {
        get
        {
            if (QueueEmpty)
                return null;
            
            if (AnyPaused)
                return _pausedProcessQueue.Dequeue();

            if (!_processQueue.IsEmpty && _processQueue.TryDequeue(out IProcessUpdateRow? result))
                return result;

            return null;
        }
    }

    private IEnumerable<IProcessUpdateRow> Processes => _processTable.Processes;
        
    public IProcessUpdateRow Selected => _processTable.Selected;
    
    public IEnumerable<IProcessUpdateRow> RunningProcesses => _runningProcesses.Processes;
    
    public IEnumerable<IProcessUpdateRow> CompletedProcesses => GetAll(ProcessStatus.Completed);
    
    public IEnumerable<IProcessUpdateRow> FailedProcesses => GetAll(ProcessStatus.Error);

    #endregion

    #region Public Methods

    public async Task Update()
    {
        // Skip update if we have no running or we're currently updating
        if (!_runningProcesses.HasCached || _updating)
            return;
        
        PerformAtomicOperation(async () =>
        {
            _updating = true;
            await _runningProcesses.Cached.Update();
            _updating = false;
        });
    }

    public bool Exists(IProcessUpdateRow processUpdateRow)
    {
        return _processTable.Contains(processUpdateRow);
    }
    
    public bool Exists(string tag)
    {
        return _processTable.Contains(tag);
    }

    public IEnumerable<T> GetOfType<T>()
    {
        return GetWhere(row => row is T).Cast<T>();
    }

    public IEnumerable<IProcessUpdateRow> GetWhere(Func<IProcessUpdateRow, bool> predicate)
    {
        return _processTable.Processes.Where(predicate);
    }

    public ListViewItem QueueDownloadProcess(IMediaItem mediaItem)
    {
        DownloadProcessUpdateRow processUpdateRow = new(mediaItem, OnCompleteProcess);
        if (mediaItem.Title.IsNullOrEmpty())
        {
            StartBackgroundTask(async () =>
            {
                processUpdateRow.Title = await YouTubeDL.GetTitle(processUpdateRow.Url);
            });
        }
        return QueueProcess(processUpdateRow);
    }
    
    // TODO: Create overrides for different types of processes
    // Processes: Conversion, Validate, Compress, Repair
    
    public ListViewItem QueueProcess(IProcessUpdateRow processUpdateRow)
    {
        _processTable.Add(processUpdateRow);
        _processQueue.Enqueue(processUpdateRow);
        processUpdateRow.Enqueue();
        UpdateQueue();
        return processUpdateRow.ViewItem;
    }
    
    public void OnCompleteProcess(IProcessRunner processRunner)
    {
        if (processRunner is not IProcessUpdateRow processUpdateRow)
            return;
        StopProcess(processUpdateRow);
        _finishedProcesses.Add(processUpdateRow);
        ProcessCompleted(processUpdateRow);
        UpdateQueue();
    }

    public bool RetryProcess(string tag)
    {
        return Get(tag) is { } processUpdateRow && RetryProcess(processUpdateRow);
    }
    
    public bool RetryProcess(IProcessUpdateRow processUpdateRow)
    {
        if (!_finishedProcesses.Remove(processUpdateRow))
            return false;
        _processQueue.Enqueue(processUpdateRow);
        processUpdateRow.Retry();
        return true;
    }
    
    public void PauseProcess(string tag)
    {
        if (Get(tag) is not { } processUpdateRow)
            return;
        
        PauseProcess(processUpdateRow);
    }
    
    public void PauseProcess(IProcessUpdateRow processUpdateRow)
    {
        StopProcess(processUpdateRow);
        _pausedProcessQueue.Add(processUpdateRow);
        processUpdateRow.Pause();
    }
    
    public void ResumeProcess(string tag)
    {
        if (Get(tag) is not { } processUpdateRow)
            return;
        
        ResumeProcess(processUpdateRow);
    }
    
    public void ResumeProcess(IProcessUpdateRow processUpdateRow)
    {
        _pausedProcessQueue.Remove(processUpdateRow);
        RunProcess(processUpdateRow);
        processUpdateRow.Resume();
    }
    
    public bool RetryAllProcesses()
    {
        bool result = true;
        
        Parallel.ForEach(GetAll(ProcessStatus.Error), process =>
        {
            if (!RetryProcess(process))
                result = false;
        });
        
        return result;
    }

    public IProcessUpdateRow? Get(string tag)
    {
        return Exists(tag) ? _processTable[tag] : null;
    }

    public void RemoveSelected()
    {
        Remove(Selected);
    }

    public IEnumerable<IProcessUpdateRow> Where(Func<IProcessUpdateRow, bool> predicate)
    {
        return Processes.Where(predicate);
    }

    public IEnumerable<IProcessUpdateRow> GetAll(ProcessStatus processStatus)
    {
        return Where(p => p.ProcessStatus == processStatus);
    }
    
    public IEnumerable<IProcessUpdateRow> GetAll(params ProcessStatus[] statuses)
    {
        return GetAll(statuses.Aggregate(0u, (i, status) => i | (uint) status));
    }
    
    public IEnumerable<IProcessUpdateRow> GetAll(uint status)
    {
        return Where(p => ((uint)p.ProcessStatus & status) > 0);
    }
    
    public IEnumerable<IProcessUpdateRow> GetAllFinished(ProcessStatus processStatus)
    {
        return _finishedProcesses.Where(p => p.ProcessStatus == processStatus);
    }

    public ListViewItem? Remove(string tag)
    {
        if (Get(tag) is not { } processUpdateRow)
            return null;

        Remove(processUpdateRow);

        return processUpdateRow.ViewItem;
    }
    
    public void Remove(IProcessUpdateRow processUpdateRow)
    {
        switch (processUpdateRow.ProcessStatus)
        {
            default:
            case ProcessStatus.Created:
                return;
            case ProcessStatus.Running:
                StopProcess(processUpdateRow);
                return;
            case ProcessStatus.Succeeded:
            case ProcessStatus.Completed:
            case ProcessStatus.Error:
            case ProcessStatus.Stopped:
            case ProcessStatus.Cancelled:
                RemoveFinished(processUpdateRow);
                return;
            case ProcessStatus.Queued:
                RemoveQueued(processUpdateRow);
                return;
            case ProcessStatus.Paused:
                RemovePaused(processUpdateRow);
                return;
        }
    }
    
    public IEnumerable<IProcessUpdateRow> RemoveAll(ProcessStatus processStatus)
    {
        IEnumerable<IProcessUpdateRow> processes = Array.Empty<IProcessUpdateRow>();
        
        switch (processStatus)
        {
            default:
            case ProcessStatus.Created:
                break;
            case ProcessStatus.Running:
                processes = RunningProcesses.ToArray();
                PerformAtomicOperationLoop(processes, RemoveRunning);
                UpdateCachedRunning();
                break;
            case ProcessStatus.Succeeded:
            case ProcessStatus.Completed:
            case ProcessStatus.Error:
            case ProcessStatus.Stopped:
            case ProcessStatus.Cancelled:
                processes = GetAllFinished(processStatus).ToArray();
                Parallel.ForEach(processes, RemoveFinished);
                break;
            case ProcessStatus.Queued: // TODO:
            case ProcessStatus.Paused:
                processes = GetAll(processStatus).ToArray();
                Parallel.ForEach(processes, RemovePaused);
                break;
        }
        
        return processes;
    }

    private void RemoveRunning(IProcessUpdateRow processUpdateRow)
    {
        _runningProcesses.Remove(processUpdateRow);
    }

    private void RemoveFinished(IProcessUpdateRow processUpdateRow)
    {
        _finishedProcesses.Remove(processUpdateRow);
    }

    private void RemoveQueued(IProcessRunner processUpdateRow)
    {
        processUpdateRow.Cancel();
    }

    private void RemovePaused(IProcessUpdateRow processUpdateRow)
    {
        _pausedProcessQueue.Remove(processUpdateRow);
        processUpdateRow.Cancel();
    }

    public void UpdateQueue()
    {
        if (QueueEmpty)
            return;

        IEnumerable<IProcessUpdateRow?> processes = Enumerable
            .Range(_runningProcesses.Count, Settings.Data.MaxConcurrentDownloads)
            .Select(_ => NextProcess);

        Parallel.ForEachAsync(processes, StartProcess);
    }
    
    public IEnumerable<string> GetAllFailedUrls()
    {
        return FailedProcesses.Select(p => p.Url);
    }
    
    #endregion

    #region Private Methods

    private void RunProcess(IProcessUpdateRow processUpdateRow)
    {
        PerformAtomicOperation(processUpdateRow, _runningProcesses.Add);
        UpdateCachedRunning();
    }

    private bool StopProcess(IProcessUpdateRow processUpdateRow)
    {
        if (!PerformAtomicOperation(processUpdateRow, _runningProcesses.Remove))
            return false;
        UpdateCachedRunning();
        return true;
    }
    
    private async ValueTask StartProcess(IProcessUpdateRow? processUpdateRow, CancellationToken cancellationToken)
    {
        if (processUpdateRow is null)
            return;
        
        RunProcess(processUpdateRow);

        if (await processUpdateRow.Start())
        {
            ProcessStarted();
            return;
        }
        
        StopProcess(processUpdateRow);
    }
    
    // Modifies the running processes in a thread safe context
    // Because we are updating the running processes asynchronously/outside the main thread, we need to lock this resource
    // TODO: These are obsolete with thread safety in place...
    private void PerformAtomicOperation<TSource>(TSource source, Action<TSource> action)
    {
        lock (_runningProcesses) { action(source); }
    }
    
    private T PerformAtomicOperation<T,TSource>(TSource source, Func<TSource, T> action)
    {
        lock (_runningProcesses) { return action(source); }
    }
    
    private void PerformAtomicOperation(Action action)
    {
        lock (_runningProcesses) { action(); }
    }
    
    private void PerformAtomicOperationLoop<TSource>(IEnumerable<TSource> source, Action<TSource> body)
    {
        lock (_runningProcesses) { Parallel.ForEach(source, body); }
    }
    
    private void UpdateCachedRunning()
    {
        _runningProcesses.UpdateCache();
    }

    private Task StartBackgroundTask(Action action)
    {
        return Task.Run(action);
    }
    
    private Task StartBackgroundTask(Func<Task> action)
    {
        return Task.Run(action);
    }

    #endregion

    #region Bulk Actions

    public void PauseAll()
    {
        PerformAtomicOperation(RunningProcesses.Pause);
    }

    public void ResumeAll()
    {
        PerformAtomicOperation(RunningProcesses.Resume);
    }

    public void KillAllRunning()
    {
        // kill all processes
        PerformAtomicOperation(RunningProcesses.Kill);
    }

    public void StopAll()
    {
        PerformAtomicOperation(RunningProcesses.Stop);
    }
    
    public void ClearAll()
    {
        StopAll();
        PerformAtomicOperation(() => _runningProcesses.Clear());
        _finishedProcesses.Clear();
        _pausedProcessQueue.Clear();
        _processQueue.Clear();
        _processTable.Clear();
    }
    
    public IEnumerable<ListViewItem> RemoveCompleted()
    {
        return RemoveAll(ProcessStatus.Completed).SelectViewItems();
    }
    
    public IEnumerable<ListViewItem> RemoveFailed()
    {
        return RemoveAll(ProcessStatus.Error).SelectViewItems();
    }

    #endregion
}