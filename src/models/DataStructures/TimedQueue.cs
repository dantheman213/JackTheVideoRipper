using Timer = System.Threading.Timer;

namespace JackTheVideoRipper.models.DataStructures;

public class TimedQueue<T>
{
    private readonly Timer _updateTimer;
    
    private int _updatePeriod;

    private readonly Action<T> _updateAction;
    
    private bool _started;

    private bool _updating;
    
    private bool _paused;

    private readonly Queue<T> _notificationQueue = new();

    public TimedQueue(Action<T> updateAction, int updatePeriod = 500)
    {
        _updateAction = updateAction;
        _updatePeriod = updatePeriod;
        _updateTimer = new Timer(Update, null, 0, updatePeriod);
    }
    
    public void Start()
    {
        if (_started)
            return;
        _started = true;
    }

    public void Pause()
    {
        if (_paused)
            return;
        _paused = true;
    }

    public void Resume()
    {
        if (!_paused)
            return;
        _paused = false;
    }

    public void Enqueue(T item)
    {
        _notificationQueue.Enqueue(item);
    }

    private async void Update(object? sender = null)
    {
        if (_updating)
            return;
        
        _updating = true;
        
        if (!_started || _paused)
            await Tasks.WaitUntil(() => _started && !_paused);
        
        while (_notificationQueue.Count > 0)
        {
            _updateAction(_notificationQueue.Dequeue());
            await Task.Delay(200);
            if (_paused)
                break;
        }

        _updating = false;
    }
}