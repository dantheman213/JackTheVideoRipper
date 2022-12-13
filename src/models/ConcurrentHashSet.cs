using System.Collections.Concurrent;

namespace JackTheVideoRipper.models;

public class ConcurrentHashSet<T> where T : notnull
{
    private readonly ConcurrentDictionary<T, byte> _data = new();

    public void Add(T value)
    {
        _data.TryAdd(value, 0);
    }

    public bool Remove(T value)
    {
        return _data.TryRemove(new KeyValuePair<T, byte>(value, 0));
    }

    public IEnumerable<T> Where(Func<T, bool> predicate)
    {
        return _data.Keys.Where(predicate);
    }

    public void Clear()
    {
        _data.Clear();
    }
}