namespace JackTheVideoRipper;

public class IndexableQueue<T> : List<T>
{
    private readonly ReaderWriterLockSlim _queueLock = new();

    private new int Count => base.Count;

    public int Length
    {
        get
        {
            int length;
            
            _queueLock.EnterReadLock();
            try
            {
                length = Count;
            }
            finally
            {
                _queueLock.ExitReadLock();
            }
            
            return length;
        }
    }
    
    public T Dequeue()
    {
        T nextProcess = Peek();
        Pop();
        return nextProcess;
    }

    public T Peek()
    {
        T nextProcess;
        
        _queueLock.EnterReadLock();
        try
        {
            nextProcess = this.First();
        }
        finally
        {
            _queueLock.ExitReadLock();
        }

        return nextProcess;
    }

    public void Pop()
    {
        _queueLock.EnterWriteLock();
        try
        {
            RemoveAt(0);
        }
        finally
        {
            _queueLock.ExitWriteLock();
        }
    }

    public bool Empty()
    {
        return Length == 0;
    }
}