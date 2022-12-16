using JackTheVideoRipper.extensions;
using JackTheVideoRipper.interfaces;
using JackTheVideoRipper.views;

namespace JackTheVideoRipper.models;

public class Console
{
    #region Data Members

    private FrameConsole? _frameConsole;

    public readonly string InstanceName = string.Empty;
    
    public ConsoleControl.ConsoleControl? Control { get; private set; }
    
    public bool Opened { get; private set; }
    
    private readonly List<ILogNode> _logHistory = new();
    
    private readonly Queue<ILogNode> _messageQueue = new();
    
    private bool _paused;

    #endregion

    #region Attributes

    private bool Visible => _frameConsole?.Visible ?? false;

    private bool Active => Opened && !_paused;

    #endregion

    #region Constructor

    public Console(string instanceName)
    {
        InstanceName = instanceName;
    }
    
    public Console()
    {
    }

    #endregion

    #region Public Methods

    public async Task Open(string? instanceName = null)
    {
        if (_frameConsole is not null && Visible)
        {
            await Core.RunTaskInMainThread(_frameConsole.Activate);
            return;
        }

        await Core.RunTaskInMainThread(() => InitializeFrame(instanceName)).ContinueWith(async _ =>
        {
            await _frameConsole!.OpenConsole();
            InitializeMessageQueue();
        });
    }

    public void QueueLog(ILogNode logNode)
    {
        _logHistory.Add(logNode);
        
        if (!Active)
        {
            _messageQueue.Enqueue(logNode);
        }
    }

    public void WriteOutput(ILogNode logNode)
    {
        Core.RunTaskInMainThread(() => Control?.WriteLog(logNode));
    }

    public async Task LockOutput(Task task)
    {
        await Task.Run(PauseQueue);
        await task;
        UnpauseQueue();
    }

    public async Task PauseQueue()
    {
        _paused = true;
        await Tasks.WaitUntil(() => !_paused);
        WriteFromQueue();
    }

    public void UnpauseQueue()
    {
        _paused = false;
    }

    #endregion

    #region Private Methods
    
    private void InitializeFrame(string? instanceName = null)
    {
        _frameConsole = new FrameConsole(instanceName ?? InstanceName, OnCloseConsole);
        Control = _frameConsole!.ConsoleControl;
        Opened = true;
    }

    private void InitializeMessageQueue()
    {
        _messageQueue.Clear();
        _messageQueue.Extend(_logHistory);
        WriteFromQueue();
    }

    private void WriteFromQueue()
    {
        while (Active && _messageQueue.Count > 0)
        {
            WriteOutput(_messageQueue.Dequeue());
        }
    }
    
    private void OnCloseConsole(object? sender, FormClosedEventArgs args)
    {
        _frameConsole = null;
        Control = null;
        Opened = false;
    }

    #endregion
}