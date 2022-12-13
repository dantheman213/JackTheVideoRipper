using JackTheVideoRipper.models.containers;

namespace JackTheVideoRipper.models;

public class HistoryTable
{
    private readonly Dictionary<string, HistoryItem> _historyItems = new();
    
    public List<HistoryItem> HistoryItems => _historyItems.Values.ToList();

    public HistoryTable(IEnumerable<HistoryItem> historyItems)
    {
        IEnumerable<KeyValuePair<string, HistoryItem>> collection = historyItems.Where(h => h.Tag is not null)
            .Select(h => new KeyValuePair<string, HistoryItem>(h.Tag!, h));
        _historyItems = new Dictionary<string, HistoryItem>(collection);
    }

    public HistoryTable()
    {
    }

    public IEnumerable<HistoryItem> GetOrderedItems => HistoryItems.OrderBy(h => h.DateFinished);
    
    public HistoryItem? GetByTag(string tag)
    {
        lock (_historyItems)
        {
            return _historyItems.ContainsKey(tag) ? _historyItems[tag] : null;
        }
    }

    public void Remove(string tag)
    {
        lock (_historyItems)
        {
            if (_historyItems.ContainsKey(tag))
                _historyItems.Remove(tag);
        }
    }

    public void Add(string tag, HistoryItem historyItem)
    {
        lock (_historyItems)
        {
            _historyItems.Add(tag, historyItem);
        }
    }
    
    public void MarkStarted(string tag, DateTime startTime)
    {
        if (!_historyItems.ContainsKey(tag))
            return;
        
        _historyItems[tag].DateStarted = startTime;
    }

    public void MarkCompleted(string tag, DateTime endTime, ProcessStatus result)
    {
        if (!_historyItems.ContainsKey(tag))
            return;
        
        _historyItems[tag].DateFinished = endTime;
        _historyItems[tag].SetDuration();
        _historyItems[tag].Result = result;
    }

    public void UpdateFileInformation(string tag, string filepath, string filesize)
    {
        if (!_historyItems.ContainsKey(tag))
            return;

        _historyItems[tag].Filepath = filepath;
        _historyItems[tag].Filesize = filesize;
    }
}