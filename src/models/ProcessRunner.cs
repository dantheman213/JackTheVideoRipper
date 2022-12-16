using System.Diagnostics;
using JackTheVideoRipper.extensions;
using JackTheVideoRipper.interfaces;

namespace JackTheVideoRipper.models;

public abstract class ProcessRunner : IProcessRunner
{
    #region Data Members

    public Process? Process { get; private set; }

    public ProcessStatus ProcessStatus { get; private set; } = ProcessStatus.Created;

    protected readonly string ParameterString;
    
    protected readonly Action<IProcessRunner> CompletionCallback;

    public Guid Guid { get; } = new();

    public ProcessBuffer Buffer { get; } = new();
    
    public bool Completed { get; private set; }

    public int ExitCode { get; private set; }
    
    public bool Succeeded { get; private set; }
    
    public string FileName { get; private set; } = string.Empty;

    #endregion

    #region Attributes

    public bool ProcessExited => Process?.HasExited ?? true;

    public bool Failed => !Succeeded;

    public bool Started => ProcessStatus is not ProcessStatus.Created;
                
    public bool Finished => ProcessStatus is ProcessStatus.Completed
        or ProcessStatus.Error
        or ProcessStatus.Cancelled
        or ProcessStatus.Stopped;
    
    public bool Paused => ProcessStatus is ProcessStatus.Paused;

    public bool Errored => ProcessStatus is ProcessStatus.Cancelled
        or ProcessStatus.Error
        or ProcessStatus.Stopped;
    
    private static Task<bool> TrueTask => Task.FromResult(true);
    
    private static Task<bool> FalseTask => Task.FromResult(false);

    #endregion

    #region Constructor

    public ProcessRunner(string parameterString, Action<IProcessRunner> completionCallback)
    {
        ParameterString = parameterString;
        CompletionCallback = completionCallback;
    }

    #endregion

    #region Process States

    public virtual Task<bool> Update()
    {
        // Don't run updates after we've completed
        if (Paused || Finished)
            return FalseTask;
        
        Buffer.Update();

        return TrueTask;
    }

    public virtual Task<bool> Start()
    {
        if (ProcessStatus is ProcessStatus.Running)
            return FalseTask;
        
        InitializeProcess();

        StartProcess();
        
        SetProcessStatus(ProcessStatus.Running);

        return TrueTask;
    }
    
    public virtual void Stop()
    {
        if (!SetProcessStatus(ProcessStatus.Stopped))
            return;
        
        Process?.Kill();
    }
    
    public virtual void Retry()
    {
        if (!Errored)
            return;
        
        SetProcessStatus(ProcessStatus.Created);

        Completed = false;
        
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

        SetProcessStatus(Failed ? ProcessStatus.Error : ProcessStatus.Completed);
        
        Completed = true;
    }

    public virtual void Enqueue()
    {
        SetProcessStatus(ProcessStatus.Queued);
    }

    // Returns if the process succeeded
    public virtual bool HandleExitCode(int exitCode)
    {
        return exitCode == 0;
    }
    
    public virtual void Pause()
    {
        if (ProcessStatus is not ProcessStatus.Running)
            return;
        Process?.Suspend();
    }

    public virtual void Resume()
    {
        if (ProcessStatus is not ProcessStatus.Paused)
            return;
        SetProcessStatus(ProcessStatus.Running);
        Process?.Resume();
    }

    public virtual void OnProcessExit(object? o, EventArgs eventArgs)
    {
        CloseProcess();
        Core.RunTaskInMainThread(Complete);
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

    public void Fail<T>(T exception) where T : Exception
    {
        Buffer.AddLog($"Process failed with exception of type {typeof(T)}: {exception}", ProcessLogType.Exception);
        if (!Finished)
            Stop();
    }

    public void Kill()
    {
        if (ProcessExited)
            return;
        
        FileSystem.TryKillProcessAndChildren(Process!.Id);
    }
    
    public void TryKillProcess()
    {
        if (ProcessExited)
            return;
        
        try { Process!.Kill(); }
        catch (Exception exception)
        {
            Output.WriteLine(exception);
        }
    }

    public string GetError()
    {
        return Buffer.GetError();
    }

    public void SaveLogs()
    {
        Buffer.SaveLogs();
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
        Process.EnableRaisingEvents = true;

        FileName = Process.StartInfo.FileName;
        
        Buffer.Initialize(Process);
    }

    private void StartProcess()
    {
        if (Process is null)
            return;
        Process.Start();
        Process.BeginOutputReadLine();
        Process.BeginErrorReadLine();
    }
    
    private void CloseProcess(bool notifyCompletion = true)
    {
        if (Process is null)
            return;
        ExitCode = Process.ExitCode;
        Succeeded = HandleExitCode(ExitCode);
        Process.Close();
        if (notifyCompletion)   
            NotifyCompletion();
    }

    #endregion

    #region Abstract Methods

    protected abstract Process CreateProcess();

    #endregion
}