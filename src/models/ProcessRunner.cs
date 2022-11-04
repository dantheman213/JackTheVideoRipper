using System.Diagnostics;
using JackTheVideoRipper.extensions;
using JackTheVideoRipper.interfaces;

namespace JackTheVideoRipper;

public abstract class ProcessRunner : IProcessRunner
{
    #region Data Members

    public Process Process { get; set; } = null!;

    public List<string> Results { get; set; } = new() { string.Empty }; // Placeholder result
    
    public int Cursor { get; set; } // where in message buffer are we
    
    public bool Paused { get; private set; }
    
    public ProcessStatus ProcessStatus { get; private set; } = ProcessStatus.Created;

    protected readonly string ParameterString;
    
    protected readonly Action<IProcessRunner> CompletionCallback;

    #endregion
    
    #region Events

    public static event Action<string, Exception> ErrorLogEvent_Tag = delegate {  };
        
    public static event Action<ProcessRunner, Exception> ErrorLogEvent_Process = delegate {  };
        
    public static event Action<ProcessError> ErrorLogEvent_Error = delegate {  };

    #endregion

    #region Attributes

    public bool AtEndOfBuffer => Cursor >= Results.Count - 1;
    
    public bool Failed => Process.ExitCode > 0;

    protected string ProcessLine => Results[Cursor];
    
    public string[] TokenizedProcessLine => Common.Tokenize(ProcessLine);

    public bool Started => ProcessStatus is not ProcessStatus.Created;
                
    public bool Finished => ProcessStatus is ProcessStatus.Completed or ProcessStatus.Error;

    public bool Completed => Started && Process.HasExited;

    #endregion

    #region Constructor

    public ProcessRunner(string parameterString, Action<IProcessRunner> completionCallback)
    {
        ParameterString = parameterString;
        CompletionCallback = completionCallback;
    }

    #endregion

    #region Process States
    
    public virtual void Update()
    {
        if (Completed)
        {
            Complete();
            return;
        }

        if (AtEndOfBuffer)
            return;

        Cursor++;
    }

    public virtual void Start()
    {
        Process = CreateProcess();
        
        Process.Start();
        
        SetProcessStatus(ProcessStatus.Running);
        
        TrackStandardOut();
        
        TrackStandardError();
    }
    
    public virtual void Stop()
    {
        SetProcessStatus(ProcessStatus.Stopped);

        TryKillProcess();

        NotifyCompletion();
    }
    
    public virtual void Retry()
    {
        SetProcessStatus(ProcessStatus.Created);
            
        CreateProcess();
    }

    public virtual void Cancel()
    {
        SetProcessStatus(ProcessStatus.Cancelled);
            
        TryKillProcess();

        NotifyCompletion();
    }
    
    protected virtual void Complete()
    {
        // Switch exit code here to determine more information and set status

        if (Failed)
        {
            SetErrorState();
            return;
        }

        SetProcessStatus(ProcessStatus.Completed);

        NotifyCompletion();
    }
    
    public virtual void Pause()
    {
        if (Paused) return;
        Process.Suspend();
        Paused = true;
        SetProcessStatus(ProcessStatus.Paused);
    }

    public virtual void Resume()
    {
        if (!Paused) return;
        Process.Resume();
        Paused = false;
        SetProcessStatus(ProcessStatus.Running);
    }

    #endregion

    #region Public Methods
    
    public void SkipToEnd()
    {
        if (Cursor + 10 < Results.Count)
            Cursor = Results.Count;
    }

    public void AppendStatusLine()
    {
        if (Process.StandardOutput.ReadLine() is { } line)
            Results.Add(line);
    }
    
    public void TrackStandardOut()
    {
        RunWhileProcessActive(AppendStatusLine);
    }
    
    public void AppendErrorLine()
    {
        if (Process.StandardError.ReadLine() is { } line)
            Results.Add(line);
    }

    public void TrackStandardError()
    {
        RunWhileProcessActive(AppendErrorLine);
    }

    #endregion

    #region Protected Methods
    
    protected void TryKillProcess()
    {
        try { Process.Kill(); }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
    }

    protected virtual void SetProcessStatus(ProcessStatus processStatus)
    {
        ProcessStatus = processStatus;
    }

    protected virtual void NotifyCompletion()
    {
        CompletionCallback.Invoke(this);
    }
    
    protected void SetErrorState(Exception? exception = null)
    {
        if (ProcessStatus == ProcessStatus.Error)
            return;

        SetProcessStatus(ProcessStatus.Error);

        WriteErrorMessage(exception);
            
        TryKillProcess();
            
        NotifyCompletion();
    }
    
    protected void WriteErrorMessage(Exception? exception = null)
    {
        ErrorLogEvent_Process(this, exception ?? new ApplicationException(Results.MergeNewline()));
        Console.Write(Results);
    }
    
    protected void RunWhileProcessActive(Action action)
    {
        Task.Run(() => { while (Process is { HasExited: false }) { action(); } });
    }

    #endregion

    #region Abstract Methods

    protected abstract Process CreateProcess();

    #endregion
}