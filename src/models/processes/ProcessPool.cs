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

    private readonly Task[] _clearTasks;

    #endregion

    #region Constructor

    public ProcessPool()
    {
        _clearTasks = new Task[]
        {
            new(_runningProcesses.Clear),
            new(_finishedProcesses.Clear),
            new(_pausedProcessQueue.Clear),
            new(_processQueue.Clear),
            new(_processTable.Clear)
        };
    }

    #endregion

    #region Events

    public event Action<IProcessUpdateRow> ProcessCompleted = delegate {  };
    
    public event Action ProcessStarted = delegate {  };

    #endregion

    #region Attributes

    public bool AnyActive => AnyRunning || AnyQueued || AnyPaused;

    public bool AnyRunning => _runningProcesses.HasCached;

    public bool AnyQueued => !_processQueue.IsEmpty;

    public bool AnyPaused => !_pausedProcessQueue.Empty();
    
    public bool QueueEmpty => _processQueue.IsEmpty && _pausedProcessQueue.Empty();
    
    public bool AtCapacity => _runningProcesses.Count == Settings.Data.MaxConcurrentDownloads;
    
    public string PoolStatus
    {
        get
        {
            if (AnyRunning)
                return "Processing Media";
            if (AnyQueued)
                return "Awaiting Process";
            if (QueueEmpty)
                return "Idle";
            return string.Empty;
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
        
    public IProcessUpdateRow Selected => _processTable.Get(Ripper.SelectedTag);
    
    public IEnumerable<IProcessUpdateRow> RunningProcesses => _runningProcesses.Processes;
    
    public IEnumerable<IProcessUpdateRow> CompletedProcesses => GetWhereStatus(ProcessStatus.Completed);
    
    public IEnumerable<IProcessUpdateRow> FailedProcesses => GetWhereStatus(ProcessStatus.Error);

    public IEnumerable<string> FailedUrls => FailedProcesses.Select(p => p.Url);

    #endregion

    #region Public Methods

    public async Task Update()
    {
        // Skip update if we have no running or we're currently updating
        if (!_runningProcesses.HasCached || _updating)
            return;
        
        _updating = true;
        await _runningProcesses.Cached.Update();
        _updating = false;
    }

    public bool Exists(IProcessUpdateRow processUpdateRow)
    {
        return _processTable.Contains(processUpdateRow.Tag);
    }
    
    public bool Exists(string tag)
    {
        return _processTable.Contains(tag);
    }

    public IEnumerable<T> GetOfType<T>()
    {
        // Select(r => r as T).Where(r => r is not null) ?
        return GetWhere(row => row is T).Cast<T>();
    }

    public IEnumerable<IProcessUpdateRow> GetWhere(Func<IProcessUpdateRow, bool> predicate)
    {
        return Processes.Where(predicate);
    }
    
    // TODO: Create overrides for different types of processes
    // Processes: Conversion, Validate, Repair
    
    public void QueueProcess(IProcessUpdateRow processUpdateRow, Action<IViewItem> queueCallback)
    {
        if (!_processTable.TryAdd(processUpdateRow))
            throw new ProcessPoolException($"Failed to add process with tag {processUpdateRow.Tag}");
        _processQueue.Enqueue(processUpdateRow);
        processUpdateRow.Enqueue();
        UpdateQueue();
        queueCallback(processUpdateRow.ViewItem);
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
        return GetProcess(tag) is { } processUpdateRow && RetryProcess(processUpdateRow);
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
        if (GetProcess(tag) is not { } processUpdateRow)
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
        if (GetProcess(tag) is not { } processUpdateRow)
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
        IProcessUpdateRow[] erroredProcesses = GetWhereStatus(ProcessStatus.Error).ToArray();
        List<bool> results = new(erroredProcesses.Length);

        Parallel.ForEach(erroredProcesses, process =>
        {
            lock (results)
            {
                results.Add(RetryProcess(process));
            }
        });
        
        return results.Any();
    }

    public IProcessUpdateRow? GetProcess(string tag)
    {
        return Exists(tag) ? _processTable[tag] : default;
    }
    
    public bool TryGetProcess(string tag, out IProcessUpdateRow? processUpdateRow)
    {
        if (Exists(tag))
        {
            return _processTable.TryGet(tag, out processUpdateRow);
        }
        
        processUpdateRow = default;
        return false;
    }

    public void RemoveSelected()
    {
        Remove(Selected);
    }

    public IEnumerable<IProcessUpdateRow> GetWhereStatus(ProcessStatus processStatus)
    {
        return GetWhere(p => p.ProcessStatus == processStatus);
    }
    
    public IEnumerable<IProcessUpdateRow> GetWhereStatus(params ProcessStatus[] statuses)
    {
        return GetWhereStatus(statuses.Aggregate(0u, (i, status) => i | (uint) status));
    }
    
    public IEnumerable<IProcessUpdateRow> GetWhereStatus(uint status)
    {
        return GetWhere(p => ((uint)p.ProcessStatus & status) > 0);
    }
    
    public IEnumerable<IProcessUpdateRow> GetFinished(ProcessStatus processStatus)
    {
        return _finishedProcesses.Where(p => p.ProcessStatus == processStatus);
    }

    public IViewItem? Remove(string tag)
    {
        if (GetProcess(tag) is not { } processUpdateRow)
            return default;

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
                _runningProcesses.Remove(processes);
                break;
            case ProcessStatus.Succeeded:
            case ProcessStatus.Completed:
            case ProcessStatus.Error:
            case ProcessStatus.Stopped:
            case ProcessStatus.Cancelled:
                processes = GetFinished(processStatus).ToArray();
                _finishedProcesses.Remove(processes);
                break;
            case ProcessStatus.Queued: // TODO:
                processes = GetWhereStatus(processStatus).ToArray();
                break;
            case ProcessStatus.Paused:
                processes = GetWhereStatus(processStatus).ToArray();
                Parallel.ForEach(processes, RemovePaused);
                break;
        }
        
        return processes;
    }
    
    private void RemoveFinished(IProcessUpdateRow processUpdateRow)
    {
        _finishedProcesses.Remove(processUpdateRow);
    }

    private static void RemoveQueued(IProcessRunner processUpdateRow)
    {
        processUpdateRow.Cancel();
    }

    private void RemovePaused(IProcessUpdateRow processUpdateRow)
    {
        _pausedProcessQueue.Remove(processUpdateRow);
        processUpdateRow.Cancel();
    }

    private bool _updatingQueue = false;

    public async void UpdateQueue()
    {
        if (QueueEmpty || AtCapacity || _updatingQueue)
            return;

        _updatingQueue = true;

        var processes = Enumerable
            .Range(_runningProcesses.Count, Settings.Data.MaxConcurrentDownloads)
            .Select(_ => NextProcess);

        await Parallel.ForEachAsync(processes, StartProcess);

        _updatingQueue = false;
    }

    #endregion

    #region Private Methods

    private void RunProcess(IProcessUpdateRow processUpdateRow)
    {
        _runningProcesses.Add(processUpdateRow);
    }

    private bool StopProcess(IProcessUpdateRow processUpdateRow)
    {
        return _runningProcesses.Remove(processUpdateRow);
    }
    
    private async ValueTask StartProcess(IProcessUpdateRow? processUpdateRow, CancellationToken cancellationToken)
    {
        if (processUpdateRow is null)
            return;
        
        RunProcess(processUpdateRow);

        // Starting Process Succeeded
        if (await processUpdateRow.Start())
        {
            ProcessStarted();
            return;
        }
        
        // Failed to Start Process
        StopProcess(processUpdateRow);
    }
    
    private IEnumerable<IViewItem> RemoveProcess(ProcessStatus processStatus, Action<IViewItem>? removeCallback = null)
    {
        IViewItem[] results = RemoveAll(processStatus).SelectViewItems().ToArray();
        if (removeCallback is not null)
            Parallel.ForEach(results, removeCallback);
        return results;
    }

    #endregion

    #region Bulk Actions

    public void PauseAll()
    {
        RunningProcesses.Pause();
    }

    public void ResumeAll()
    {
        _pausedProcessQueue.Resume();
    }

    public void KillAllRunning()
    {
        // kill all processes
        RunningProcesses.Kill();
    }

    public void StopAll()
    {
        RunningProcesses.Stop();
        _processQueue.Stop();
    }
    
    public void ClearAll()
    {
        StopAll();
        Task.WhenAll(_clearTasks);
    }

    public IEnumerable<IViewItem> RemoveCompleted(Action<IViewItem>? removeCallback = null)
    {
        return RemoveProcess(ProcessStatus.Completed, removeCallback);
    }
    
    public IEnumerable<IViewItem> RemoveFailed(Action<IViewItem>? removeCallback = null)
    {
        return RemoveProcess(ProcessStatus.Error, removeCallback);
    }

    #endregion

    #region Embedded Types

    public class ProcessPoolException : Exception
    {
        public ProcessPoolException()
        {
        }
        
        public ProcessPoolException(string message) : base(message)
        {
        }
        
        public ProcessPoolException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    #endregion
}