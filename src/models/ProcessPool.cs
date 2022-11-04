using JackTheVideoRipper.extensions;
using JackTheVideoRipper.models;

namespace JackTheVideoRipper;

public class ProcessPool
{
    #region Data Members

    private readonly Dictionary<string, ProcessUpdateRow> _processTable = new();
    
    private readonly Queue<ProcessUpdateRow> _processQueue = new();
    private readonly Dictionary<string, ProcessUpdateRow> _activeProcesses = new();
    private readonly HashSet<ProcessUpdateRow> _finishedProcesses = new();

    public static readonly ErrorLogger ErrorLogger = new();

    private readonly Action<ProcessUpdateRow> _processCompletionCallback;

    private readonly Action _processStartedCallback;

    #endregion

    #region Properties

    public bool AnyActive => AnyRunning || AnyQueued;

    public bool AnyRunning => _activeProcesses.Count > 0;

    public bool AnyQueued => _processQueue.Count > 0;
    
    public bool QueueEmpty => _processQueue.Count <= 0;
    
    public IEnumerable<ProcessUpdateRow> ActiveProcesses => _activeProcesses.Values;
    
    private ProcessUpdateRow NextProcess => _processQueue.Peek();

    private ProcessUpdateRow GetNextProcess() => _processQueue.Dequeue();

    #endregion

    #region Constructor

    public ProcessPool(Action<ProcessUpdateRow> processCompletionCallback, Action processStartedCallback)
    {
        _processCompletionCallback = processCompletionCallback;
        _processStartedCallback = processStartedCallback;
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
    
    public bool Exists(ListViewItem listViewItem)
    {
        return _processTable.ContainsKey(listViewItem.Tag.As<string>());
    }
    
    public void QueueProcess(MediaItemRow row, ListViewItem listViewItem)
    {
        if (row.Parameters is not { } parameters)
            return;
        
        QueueProcess(row.Tag, parameters.ToString(), listViewItem);
    }
    
    public void QueueProcess(string tag, Parameters parameters, ListViewItem listViewItem)
    {
        QueueProcess(parameters.ToString(), tag, listViewItem);
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
    
    public void OnCompleteProcess(ProcessUpdateRow processUpdateRow)
    {
        _activeProcesses.Remove(processUpdateRow.Tag);
        _finishedProcesses.Add(processUpdateRow);
        _processCompletionCallback.Invoke(processUpdateRow);
    }

    public void RetryProcess(ListViewItem listViewItem)
    {
        if (Get(listViewItem) is not { } processUpdateRow)
            return;
        
        RetryProcess(processUpdateRow);
    }
    
    public void RetryProcess(ProcessUpdateRow processUpdateRow)
    {
        _finishedProcesses.Remove(processUpdateRow);
        _processQueue.Enqueue(processUpdateRow);
        processUpdateRow.Retry();
    }
    
    public void RetryAllProcesses()
    {
        GetAll(ProcessStatus.Error).ForEach(RetryProcess);
    }

    public ProcessUpdateRow? Get(ListViewItem listViewItem)
    {
        return Exists(listViewItem) ? _processTable[listViewItem.Tag.As<string>()] : null;
    }
    
    public IEnumerable<ProcessUpdateRow> GetAll(ProcessStatus processStatus)
    {
        return _processTable.Values.Where(p => p.ProcessStatus == processStatus);
    }
    
    public IEnumerable<ProcessUpdateRow> GetAllRunning()
    {
        return _activeProcesses.Values;
    }
    
    public IEnumerable<ProcessUpdateRow> GetAllFinished(ProcessStatus processStatus)
    {
        return _finishedProcesses.Where(p => p.ProcessStatus == processStatus);
    }

    public void Remove(ListViewItem listViewItem)
    {
        if (Get(listViewItem) is not { } processUpdateRow)
            return;
        
        switch (processUpdateRow.ProcessStatus)
        {
            default:
            case ProcessStatus.Created:
                return;
            case ProcessStatus.Running:
                _activeProcesses.Remove(processUpdateRow.Tag);
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
                GetAllRunning().ForEach(p => _activeProcesses.Remove(p.Tag));
                return;
            case ProcessStatus.Succeeded:
            case ProcessStatus.Completed:
            case ProcessStatus.Error:
            case ProcessStatus.Stopped:
            case ProcessStatus.Cancelled:
                GetAllFinished(processStatus).ForEach(p => _finishedProcesses.Remove(p));
                return;
            case ProcessStatus.Queued:
                GetAll(ProcessStatus.Queued).ForEach(p => p.Cancel());
                return;
        }
    }

    public void KillAllActive()
    {
        // kill all processes
        _activeProcesses.Values.ForEach(p => FileSystem.TryKillProcessAndChildren(p.Process.Id));
    }

    public void StopAll()
    {
        _activeProcesses.Values.ForEach(p => p.Stop());
    }

    public bool UpdateQueue()
    {
        if (QueueEmpty)
            return false;

        bool dirtyFlag = false;

        for (int i = _activeProcesses.Count; i < Settings.Data.MaxConcurrentDownloads; i++)
        {
            if (QueueEmpty)
                break;
            RunProcess(GetNextProcess());
            dirtyFlag = true;
        }

        return dirtyFlag;
    }

    private void RunProcess(ProcessUpdateRow processUpdateRow)
    {
        _activeProcesses.Add(processUpdateRow.Tag, processUpdateRow);
        processUpdateRow.Start();
        _processStartedCallback.Invoke();
    }

    public void ClearAll()
    {
        StopAll();
        _finishedProcesses.Clear();
        _activeProcesses.Clear();
        _processQueue.Clear();
        _processTable.Clear();
    }

    public IEnumerable<string> GetAllFailedUrls()
    #region Bulk Actions
    {
        return _finishedProcesses.Where(p => p.Failed).Select(p => p.Url);
    }

    {
    }

    {

    }
    
    {
    }
    
    {
    }

    #endregion
}