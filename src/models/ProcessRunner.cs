using System.Diagnostics;
using JackTheVideoRipper.extensions;
using JackTheVideoRipper.interfaces;
using JackTheVideoRipper.models;

namespace JackTheVideoRipper;

public abstract class ProcessRunner : IProcessRunner
{
    #region Data Members

    public Process Process { get; private set; } = null!;

    public ProcessStatus ProcessStatus { get; private set; } = ProcessStatus.Created;

    protected readonly string ParameterString;
    
    protected readonly Action<IProcessRunner> CompletionCallback;

    public Guid Guid { get; } = new();

    public ProcessBuffer Buffer { get; } = new();
    
    public bool Completed { get; private set; }
    
    public int ExitCode { get; private set; }

    #endregion

    #region Attributes

    public bool Failed => ExitCode > 0;

    public bool Started => ProcessStatus is not ProcessStatus.Created;
                
    public bool Finished => ProcessStatus is ProcessStatus.Completed or ProcessStatus.Error or 
        ProcessStatus.Cancelled or ProcessStatus.Stopped;
    
    public bool Paused => ProcessStatus is ProcessStatus.Paused;

    // public bool Completed => Started && Process.HasExited;

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
        // Don't run updates after we've completed
        if (Paused || Finished)
            return;
        
        Buffer.Update();
    }

    public virtual void Start()
    {
        if (IsProcessStatus(ProcessStatus.Running))
            return;
        
        InitializeProcess();
        
        SetProcessStatus(ProcessStatus.Running);
    }
    
    public virtual void Stop()
    {
        if (!SetProcessStatus(ProcessStatus.Stopped))
            return;

        CloseProcess();
    }
    
    public virtual void Retry()
    {
        if (!IsProcessStatus(ProcessStatus.Error, ProcessStatus.Stopped, ProcessStatus.Cancelled))
            return;
        
        SetProcessStatus(ProcessStatus.Created);

        Completed = false;
            
        InitializeProcess();
        
        Buffer.Clear();
    }

    public virtual void Cancel()
    {
        if (!SetProcessStatus(ProcessStatus.Cancelled))
            return;
            
        CloseProcess();
    }
    
    protected virtual void Complete()
    {
        if (Completed || Finished)
            return;

        ExitCode = Process.ExitCode;

        // TODO: Give more information about exit
        /*switch (ExitCode)
        {
            
        }*/

        SetProcessStatus(Failed ? ProcessStatus.Error : ProcessStatus.Completed);
        
        Completed = true;
    }
    
    public virtual void Pause()
    {
        if (!SetProcessStatus(ProcessStatus.Paused))
            return;
        Process.Suspend();
    }

    public virtual void Resume()
    {
        if (!IsProcessStatus(ProcessStatus.Paused))
            return;
        SetProcessStatus(ProcessStatus.Running);
        Process.Resume();
    }

    public virtual void OnProcessExit(object? o, EventArgs eventArgs)
    {
        Core.InvokeInMainContext(Complete);
        CloseProcess();
    }
    
    protected virtual bool SetProcessStatus(ProcessStatus processStatus)
    {
        if (IsProcessStatus(processStatus))
            return false;
        ProcessStatus = processStatus;
        return true;
    }

    protected virtual void NotifyCompletion()
    {
        CompletionCallback.Invoke(this);
    }

    #endregion

    #region Public Methods

    public void Kill()
    {
        if (Process.HasExited)
            return;
        
        FileSystem.TryKillProcessAndChildren(Process.Id);
    }
    
    public void TryKillProcess()
    {
        if (Process.HasExited)
            return;
        
        try { Process.Kill(); }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
    }

    #endregion

    #region Protected Methods
    
    protected bool IsProcessStatus(ProcessStatus processStatus) => ProcessStatus == processStatus;
    
    protected bool IsProcessStatus(params ProcessStatus[] processStatuses) => processStatuses.Any(IsProcessStatus);

    #endregion

    #region Private Methods

    private void InitializeProcess()
    {
        Process = CreateProcess();

        Process.Exited += OnProcessExit;
        Process.OutputDataReceived += (sender, args) => Buffer.AppendResult(args.Data);
        Process.ErrorDataReceived += (sender, args) => Buffer.AppendError(args.Data);
        Process.EnableRaisingEvents = true;

        Process.Start();
        Process.BeginOutputReadLine();
        Process.BeginErrorReadLine();
    }
    
    private void CloseProcess()
    {
        Process.Close();
        NotifyCompletion();
    }

    #endregion

    #region Abstract Methods

    protected abstract Process CreateProcess();

    #endregion
}