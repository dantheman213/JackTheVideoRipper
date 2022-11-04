using JackTheVideoRipper.extensions;
using JackTheVideoRipper.interfaces;
using JackTheVideoRipper.models;

namespace JackTheVideoRipper;

public class ProcessPool
{
    #region Data Members

    private readonly Dictionary<string, ProcessUpdateRow> _processTable = new();
    
    private readonly Queue<ProcessUpdateRow> _processQueue = new();
    private readonly List<ProcessUpdateRow> _pausedProcessQueue = new();
    private readonly Dictionary<string, ProcessUpdateRow> _runningProcesses = new();
    private readonly HashSet<ProcessUpdateRow> _finishedProcesses = new();

    public static readonly ErrorLogger ErrorLogger = new();

    #endregion

    #region Events

    public event Action<ProcessUpdateRow> ProcessCompleted = delegate {  };
    
    public event Action ProcessStarted = delegate {  };

    #endregion

    #region Properties

    public bool AnyActive => AnyRunning || AnyQueued || AnyPaused;

    public bool AnyRunning => _runningProcesses.Count > 0;

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

    private ProcessUpdateRow NextProcess
    {
        get
        {
            if (!AnyPaused)
                return _processQueue.Dequeue();
            ProcessUpdateRow nextProcess = _pausedProcessQueue.First();
            _pausedProcessQueue.RemoveAt(0);
            return nextProcess;
        }
    }

    private IEnumerable<ProcessUpdateRow> Processes => _processTable.Values;

    public ProcessUpdateRow? Selected => Processes.FirstOrDefault(p => p.ViewItem.Selected);
    
    public IEnumerable<ProcessUpdateRow> RunningProcesses => _runningProcesses.Values;
    
    public IEnumerable<ProcessUpdateRow> CompletedProcesses => GetAll(ProcessStatus.Completed);
    
    public IEnumerable<ProcessUpdateRow> FailedProcesses => GetAll(ProcessStatus.Error);

    #endregion

    #region Public Methods

    public void Update()
    {
        RunningProcesses.ForEach(p => p.Update());
    }

    #endregion

    public bool Exists(ProcessUpdateRow processUpdateRow)
    {
        return _processTable.ContainsKey(processUpdateRow.Tag);
    }
    
    public bool Exists(string tag)
    {
        return _processTable.ContainsKey(tag);
    }
    
    public void QueueProcess(MediaItemRow row)
    {
        if (row.Parameters is not { } parameters)
            return;
        
        QueueProcess(row.Tag, parameters.ToString(), row.ListViewItem);
    }
    
    public void QueueProcess(string tag, string parameterString, ListViewItem listViewItem)
    {
        ProcessUpdateRow processUpdateRow = new(parameterString, OnCompleteProcess)
        {
            ViewItem = listViewItem,
            Tag = tag
        };

        _processTable.Add(tag, processUpdateRow);
        _processQueue.Enqueue(processUpdateRow);

        UpdateQueue();
    }
    
    public void OnCompleteProcess(IProcessRunner processRunner)
    {
        ProcessUpdateRow processUpdateRow = processRunner.As<ProcessUpdateRow>();
        _runningProcesses.Remove(processUpdateRow.Tag);
        _finishedProcesses.Add(processUpdateRow);
        ProcessCompleted(processUpdateRow);
    }

    public void RetryProcess(string tag)
    {
        if (Get(tag) is not { } processUpdateRow)
            return;
        
        RetryProcess(processUpdateRow);
    }
    
    public void RetryProcess(ProcessUpdateRow processUpdateRow)
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
    
    public void PauseProcess(ProcessUpdateRow processUpdateRow)
    {
        _runningProcesses.Remove(processUpdateRow.Tag);
        _pausedProcessQueue.Add(processUpdateRow);
        processUpdateRow.Pause();
    }
    
    public void ResumeProcess(string tag)
    {
        if (Get(tag) is not { } processUpdateRow)
            return;
        
        ResumeProcess(processUpdateRow);
    }
    
    public void ResumeProcess(ProcessUpdateRow processUpdateRow)
    {
        _pausedProcessQueue.Remove(processUpdateRow);
        _runningProcesses.Add(processUpdateRow.Tag, processUpdateRow);
        processUpdateRow.Resume();
    }
    
    public void RetryAllProcesses()
    {
        GetAll(ProcessStatus.Error).ForEach(RetryProcess);
    }

    public ProcessUpdateRow? Get(string tag)
    {
        return Exists(tag) ? _processTable[tag] : null;
    }

    public void RemoveSelected()
    {
        if (Selected is not null)
            Remove(Selected);
    }

    public IEnumerable<ProcessUpdateRow> GetAll(ProcessStatus processStatus)
    {
        return Processes.Where(p => p.ProcessStatus == processStatus);
    }
    
    public IEnumerable<ProcessUpdateRow> GetAll(params ProcessStatus[] statuses)
    {
        return Processes.Where(p => statuses.Any(status => p.ProcessStatus == status));
    }
    
    public IEnumerable<ProcessUpdateRow> GetAll(int status)
    {
        return Processes.Where(p => (p.ProcessStatus.As<int>() & status) > 0);
    }
    
    public IEnumerable<ProcessUpdateRow> GetAllFinished(ProcessStatus processStatus)
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
    
    public void Remove(ProcessUpdateRow processUpdateRow)
    {
        switch (processUpdateRow.ProcessStatus)
        {
            default:
            case ProcessStatus.Created:
                return;
            case ProcessStatus.Running:
                _runningProcesses.Remove(processUpdateRow.Tag);
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
                RunningProcesses.ForEach(p => _runningProcesses.Remove(p.Tag));
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

    private void StartNextProcess()
    {
        ProcessUpdateRow nextProcess = NextProcess;
        _runningProcesses.Add(nextProcess.Tag, nextProcess);
        nextProcess.Start();
        ProcessStarted();
    }

    public IEnumerable<string> GetAllFailedUrls()
    {
        return FailedProcesses.Select(p => p.Url);
    }

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
        RunningProcesses.ForEach(p => FileSystem.TryKillProcessAndChildren(p.Process.Id));
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
        var completed = CompletedProcesses.Select(p => p.ViewItem);
        RemoveAll(ProcessStatus.Completed);
        return completed;
    }
    
    public IEnumerable<ListViewItem> RemoveFailed()
    {
        var failed = FailedProcesses.Select(p => p.ViewItem);
        RemoveAll(ProcessStatus.Error);
        return failed;
    }

    #endregion
}