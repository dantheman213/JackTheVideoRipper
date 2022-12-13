namespace JackTheVideoRipper;

public class IndexableQueue<T> : List<T>
{
    private readonly ReaderWriterLockSlim _queueLock = new();
    
    public T Dequeue()
    {
        _queueLock.EnterReadLock();
        T nextProcess = this.First();
        _queueLock.ExitReadLock();
        
        _queueLock.EnterWriteLock();
        RemoveAt(0);
        _queueLock.ExitWriteLock();
        
        return nextProcess;
    }

    public bool Empty()
    {
        _queueLock.EnterReadLock();
        bool isEmpty = Count == 0;
        _queueLock.ExitReadLock();
        return isEmpty;
    }
}