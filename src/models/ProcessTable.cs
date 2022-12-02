using JackTheVideoRipper.interfaces;

namespace JackTheVideoRipper.models;

public class ProcessTable
{
    private readonly Dictionary<string, IProcessUpdateRow> _processUpdateRowDictionary = new();
    
    public IEnumerable<IProcessUpdateRow> Processes => _processUpdateRowDictionary.Values.ToArray();
    
    
    public IProcessUpdateRow? this[string tag] => Contains(tag) ? _processUpdateRowDictionary[tag] : null;

    public int Count => _processUpdateRowDictionary.Count;

    public bool Contains(IProcessUpdateRow processUpdateRow)
    {
        return _processUpdateRowDictionary.ContainsKey(processUpdateRow.Tag);
    }
    
    public bool Contains(string tag)
    {
        return _processUpdateRowDictionary.ContainsKey(tag);
    }

    public void Add(IProcessUpdateRow processUpdateRow)
    {
        _processUpdateRowDictionary.Add(processUpdateRow.Tag, processUpdateRow);
    }

    public void Remove(IProcessUpdateRow processUpdateRow)
    {
        _processUpdateRowDictionary.Remove(processUpdateRow.Tag);
    }

    public void Remove(string tag)
    {
        _processUpdateRowDictionary.Remove(tag);
    }

    public void Clear()
    {
        _processUpdateRowDictionary.Clear();
    }

    public bool Empty()
    {
        return _processUpdateRowDictionary.Count < 1;
    }
}