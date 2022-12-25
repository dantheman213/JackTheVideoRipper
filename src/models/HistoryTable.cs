using JackTheVideoRipper.models.containers;

namespace JackTheVideoRipper.models;

public class HistoryTable
{
    #region Data Members

    private readonly Dictionary<string, HistoryItem> _historyItems = new();
    
    private readonly ReaderWriterLockSlim _tableLock = new();

    #endregion

    #region Attributes

    public List<HistoryItem> HistoryItems => _historyItems.Values.ToList();

    #endregion

    #region Constructor

    public HistoryTable(IEnumerable<HistoryItem> historyItems)
    {
        var collection = historyItems.Where(h => h.Tag is not null)
            .Select(h => new KeyValuePair<string, HistoryItem>(h.Tag!, h));
        _historyItems = new Dictionary<string, HistoryItem>(collection);
    }

    public HistoryTable()
    {
    }

    #endregion

    public IEnumerable<HistoryItem> GetOrderedItems
    {
        get
        {
            _tableLock.EnterReadLock();
            var result = HistoryItems.OrderBy(h => h.DateFinished);
            _tableLock.ExitReadLock();
            return result;
        }
    }

    public bool ContainsUrl(string url)
    {
        _tableLock.EnterReadLock();
        bool result = HistoryItems.FindIndex(h => h.Url == url) != -1;
        _tableLock.ExitReadLock();
        return result;
    }

    public HistoryItem? GetByUrl(string url)
    {
        _tableLock.EnterReadLock();
        HistoryItem? result = HistoryItems.FirstOrDefault(h => h.Url == url);
        _tableLock.ExitReadLock();
        return result;
    }
    
    public HistoryItem? GetByTag(string tag)
    {
        _tableLock.EnterReadLock();
        HistoryItem? result = Exists(tag) ? _historyItems[tag] : null;
        _tableLock.ExitReadLock();
        return result;
    }

    private bool Exists(string tag)
    {
        _tableLock.EnterReadLock();
        bool result = _historyItems.ContainsKey(tag);
        _tableLock.ExitReadLock();
        return result;
    }

    public void Remove(string tag)
    {
        if (!Exists(tag))
            return;
        _tableLock.EnterWriteLock();
        _historyItems.Remove(tag);
        _tableLock.ExitWriteLock();
    }

    public void Add(string tag, HistoryItem historyItem)
    {
        if (!Exists(tag))
            return;
        _tableLock.EnterWriteLock();
        _historyItems.Add(tag, historyItem);
        _tableLock.ExitWriteLock();
    }
    
    public void MarkStarted(string tag, DateTime startTime)
    {
        if (!Exists(tag))
            return;
        
        _tableLock.EnterWriteLock();
        _historyItems[tag].DateStarted = startTime;
        _tableLock.ExitWriteLock();
    }

    public void MarkCompleted(string tag, DateTime endTime, ProcessStatus result)
    {
        if (!Exists(tag))
            return;
        
        _tableLock.EnterWriteLock();
        _historyItems[tag].DateFinished = endTime;
        _historyItems[tag].SetDuration();
        _historyItems[tag].Result = result;
        _tableLock.ExitWriteLock();
    }

    public void UpdateFileInformation(string tag, string filepath, string filesize)
    {
        if (!Exists(tag))
            return;

        _tableLock.EnterWriteLock();
        _historyItems[tag].Filepath = filepath;
        _historyItems[tag].Filesize = filesize;
        _tableLock.ExitWriteLock();
    }
}