using JackTheVideoRipper.interfaces;

namespace JackTheVideoRipper.models;

public class ProcessTable
{
    private readonly Dictionary<string, IProcessUpdateRow> _processUpdateRowDictionary = new();
    
    public IProcessUpdateRow? this[string tag] => Contains(tag) ? Get(tag) : null;
    
    public IProcessUpdateRow Selected => Get((string) Core.FrameMain.FirstSelected.Tag);
    
    private readonly ReaderWriterLockSlim _tableLock = new();
    
    public IProcessUpdateRow[] Processes
    {
        get
        {
            _tableLock.EnterReadLock();
            try
            {
                return _processUpdateRowDictionary.Values.ToArray();
            }
            finally
            {
                _tableLock.ExitReadLock();
            }
        }
    }

    public int Count
    {
        get
        {
            _tableLock.EnterReadLock();
            try
            {
                return _processUpdateRowDictionary.Count;
            }
            finally
            {
                _tableLock.ExitReadLock();
            }
        }
    }

    public IProcessUpdateRow Get(string tag)
    {
        _tableLock.EnterReadLock();
        try
        {
            return _processUpdateRowDictionary[tag];
        }
        finally
        {
            _tableLock.ExitReadLock();
        }
    }

    public bool Contains(IProcessUpdateRow processUpdateRow)
    {
        return Contains(processUpdateRow.Tag);
    }
    
    public bool Contains(string tag)
    {
        _tableLock.EnterReadLock();
        try
        {
            return _processUpdateRowDictionary.ContainsKey(tag);
        }
        finally
        {
            _tableLock.ExitReadLock();
        }
    }

    public void Add(IProcessUpdateRow processUpdateRow)
    {
        _tableLock.EnterWriteLock();
        try
        {
            _processUpdateRowDictionary.Add(processUpdateRow.Tag, processUpdateRow);
        }
        finally
        {
            _tableLock.ExitWriteLock();
        }
    }

    public bool Remove(IProcessUpdateRow processUpdateRow)
    {
        return Remove(processUpdateRow.Tag);
    }

    public bool Remove(string tag)
    {
        _tableLock.EnterWriteLock();
        try
        {
            return _processUpdateRowDictionary.Remove(tag);
        }
        finally
        {
            _tableLock.ExitWriteLock();
        }
    }

    public void Clear()
    {
        _tableLock.EnterWriteLock();
        try
        {
            _processUpdateRowDictionary.Clear();
        }
        finally
        {
            _tableLock.ExitWriteLock();
        }
    }

    public bool Empty() => Count <= 0;

    private IProcessUpdateRow[] _cachedProcesses = Array.Empty<IProcessUpdateRow>();

    public void UpdateCache()
    {
        lock (_cachedProcesses)
        {
            _cachedProcesses = Processes;
        }
    }

    public IEnumerable<IProcessUpdateRow> Cached
    {
        get
        {
            lock (_cachedProcesses)
            {
                return _cachedProcesses;
            }
        }
    }

    public bool HasCached
    {
        get
        {
            lock (_cachedProcesses)
            {
                return _cachedProcesses.Length > 0;
            } 
        }
    }
}