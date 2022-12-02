using JackTheVideoRipper.extensions;
using JackTheVideoRipper.interfaces;
using JackTheVideoRipper.models;

namespace JackTheVideoRipper;

public class ProcessPool
{
    #region Data Members

    private readonly ProcessTable _processTable = new();
    private readonly Queue<IProcessUpdateRow> _processQueue = new();
    private readonly List<IProcessUpdateRow> _pausedProcessQueue = new();
    private readonly ProcessTable _runningProcesses = new();
    private readonly HashSet<IProcessUpdateRow> _finishedProcesses = new();

    public static readonly ErrorLogger ErrorLogger = new();

    #endregion

    #region Events

    public event Action<IProcessUpdateRow> ProcessCompleted = delegate {  };
    
    public event Action ProcessStarted = delegate {  };

    #endregion

    #region Properties

    public bool AnyActive => AnyRunning || AnyQueued || AnyPaused;

    public bool AnyRunning => !_runningProcesses.Empty();

    public bool AnyQueued => _processQueue.Count > 0;

    public bool AnyPaused => _pausedProcessQueue.Count > 0;
    
    public bool QueueEmpty => _processQueue.Count <= 0 && _pausedProcessQueue.Count <= 0;
    
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

    private IProcessUpdateRow NextProcess
    {
        get
        {
            if (!AnyPaused)
                return _processQueue.Dequeue();
            IProcessUpdateRow nextProcess = _pausedProcessQueue.First();
            _pausedProcessQueue.RemoveAt(0);
            return nextProcess;
        }
    }

    private IEnumerable<IProcessUpdateRow> Processes => _processTable.Processes;

    public IProcessUpdateRow? Selected => Processes.FirstOrDefault(p => p.ViewItem.Selected);
    
    public IEnumerable<IProcessUpdateRow> RunningProcesses => _runningProcesses.Processes;
    
    public IEnumerable<IProcessUpdateRow> CompletedProcesses => GetAll(ProcessStatus.Completed);
    
    public IEnumerable<IProcessUpdateRow> FailedProcesses => GetAll(ProcessStatus.Error);

    #endregion

    #region Public Methods

    public void Update()
    {
        lock (_runningProcesses)
        {
            // TODO: Is it performant to make a copy every time? Maybe cache last?
            RunningProcesses.ForEach(p => p.Update());
        }
    }

    public bool Exists(IProcessUpdateRow processUpdateRow)
    {
        return _processTable.Contains(processUpdateRow);
    }
    
    public bool Exists(string tag)
    {
        return _processTable.Contains(tag);
    }
    
    public ListViewItem QueueDownloadProcess(IMediaItem mediaItem)
    {
        return QueueProcess(new DownloadProcessUpdateRow(mediaItem, OnCompleteProcess));
    }
    
    public ListViewItem QueueProcess(IProcessUpdateRow processUpdateRow)
    {
        _processTable.Add(processUpdateRow);
        _processQueue.Enqueue(processUpdateRow);
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

    public void RetryProcess(string tag)
    {
        if (Get(tag) is not { } processUpdateRow)
            return;
        
        RetryProcess(processUpdateRow);
    }
    
    public void RetryProcess(IProcessUpdateRow processUpdateRow)
    {
        _finishedProcesses.Remove(processUpdateRow);
        _processQueue.Enqueue(processUpdateRow);
        processUpdateRow.Retry();
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
    
    public void RetryAllProcesses()
    {
        GetAll(ProcessStatus.Error).ForEach(RetryProcess);
    }

    public IProcessUpdateRow? Get(string tag)
    {
        return Exists(tag) ? _processTable[tag] : null;
    }

    public void RemoveSelected()
    {
        if (Selected is not null)
            Remove(Selected);
    }

    public IEnumerable<IProcessUpdateRow> GetAll(ProcessStatus processStatus)
    {
        return Processes.Where(p => p.ProcessStatus == processStatus);
    }
    
    public IEnumerable<IProcessUpdateRow> GetAll(params ProcessStatus[] statuses)
    {
        return Processes.Where(p => statuses.Any(status => p.ProcessStatus == status));
    }
    
    public IEnumerable<IProcessUpdateRow> GetAll(int status)
    {
        return Processes.Where(p => ((int)p.ProcessStatus & status) > 0);
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
                _finishedProcesses.Remove(processUpdateRow);
                return;
            case ProcessStatus.Queued:
                processUpdateRow.Cancel();
                return;
            case ProcessStatus.Paused:
                _pausedProcessQueue.Remove(processUpdateRow);
                return;
        }
    }
    
    public void RemoveAll(ProcessStatus processStatus)
    {
        switch (processStatus)
        {
            default:
            case ProcessStatus.Created:
                return;
            case ProcessStatus.Running:
                lock (_runningProcesses)
                {
                    RunningProcesses.ForEach(p => _runningProcesses.Remove(p.Tag));
                }
                return;
            case ProcessStatus.Succeeded:
            case ProcessStatus.Completed:
            case ProcessStatus.Error:
            case ProcessStatus.Stopped:
            case ProcessStatus.Cancelled:
                GetAllFinished(processStatus).ForEach(p => _finishedProcesses.Remove(p));
                return;
            case ProcessStatus.Queued: // TODO:
                GetAll(ProcessStatus.Queued).ForEach(p => p.Cancel());
                return;
            case ProcessStatus.Paused:
                GetAll(ProcessStatus.Paused).ForEach(p => p.Cancel());
                return;
        }
    }
    
    public bool UpdateQueue()
    {
        if (QueueEmpty)
            return false;

        bool dirtyFlag = false;

        for (int i = _runningProcesses.Count; i < Settings.Data.MaxConcurrentDownloads; i++)
        {
            if (QueueEmpty)
                break;
            StartNextProcess();
            dirtyFlag = true;
        }

        return dirtyFlag;
    }
    
    public IEnumerable<string> GetAllFailedUrls()
    {
        return FailedProcesses.Select(p => p.Url);
    }
    
    #endregion

    #region Private Methods

    private void RunProcess(IProcessUpdateRow processUpdateRow)
    {
        lock (_runningProcesses)
        {
            _runningProcesses.Add(processUpdateRow);
        }
    }

    private void StopProcess(IProcessUpdateRow processUpdateRow)
    {
        lock (_runningProcesses)
        {
            _runningProcesses.Remove(processUpdateRow);
        }
    }
    
    private void StartNextProcess()
    {
        IProcessUpdateRow nextProcess = NextProcess;
        RunProcess(nextProcess);
        nextProcess.Start();
        ProcessStarted();
    }
    
    #endregion

    #region Bulk Actions

    public void PauseAll()
    {
        RunningProcesses.ForEach(p => p.Pause());
    }

    public void ResumeAll()
    {
        RunningProcesses.ForEach(p => p.Resume());
    }

    public void KillAllRunning()
    {
        // kill all processes
        RunningProcesses.ForEach(p => p.Kill());
    }

    public void StopAll()
    {
        RunningProcesses.ForEach(p => p.Stop());
    }
    
    public void ClearAll()
    {
        StopAll();
        _finishedProcesses.Clear();
        _runningProcesses.Clear();
        _processQueue.Clear();
        _processTable.Clear();
    }
    
    public IEnumerable<ListViewItem> RemoveCompleted()
    {
        IEnumerable<ListViewItem> completedProcesses = CompletedProcesses.Select(p => p.ViewItem);
        RemoveAll(ProcessStatus.Completed);
        return completedProcesses;
    }
    
    public IEnumerable<ListViewItem> RemoveFailed()
    {
        IEnumerable<ListViewItem> failedProcesses = FailedProcesses.Select(p => p.ViewItem);
        RemoveAll(ProcessStatus.Error);
        return failedProcesses;
    }

    #endregion
}