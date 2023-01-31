using System.Collections;
using System.Collections.Concurrent;
using JackTheVideoRipper.extensions;

namespace JackTheVideoRipper.models;

public class ConcurrentHashSet<T> : IEnumerable<T> where T : notnull
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
    
    public bool Remove(IEnumerable<T> values)
    {
        return values.Select(Remove).All();
    }

    public IEnumerable<T> Where(Func<T, bool> predicate)
    {
        return _data.Keys.Where(predicate);
    }

    public void Clear()
    {
        _data.Clear();
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _data.Keys.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _data.Keys.GetEnumerator();
    }
}