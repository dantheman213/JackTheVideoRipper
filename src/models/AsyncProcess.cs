using System.Diagnostics;
using JackTheVideoRipper.extensions;
using JackTheVideoRipper.models.processes;

namespace JackTheVideoRipper.models;

public class AsyncProcess : Process
{
    public readonly List<string> Output = new();
    
    public readonly List<string> Errors = new();

    public string OutputString => Output.MergeNewline();

    // Protect from repeated writes
    public new bool EnableRaisingEvents
    {
        get => base.EnableRaisingEvents;
        init => base.EnableRaisingEvents = value;
    }

    private readonly int _timeoutPeriod = -1; //< Milliseconds
    
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    
    private readonly TaskCompletionSource<ProcessResult> _taskCompletionSource = new();

    public new ProcessStartInfo StartInfo
    {
        get => base.StartInfo;
        init
        {
            value.RedirectStandardOutput = true;
            value.RedirectStandardError = true;
            value.UseShellExecute = false;
            base.StartInfo = value;
        }
    }

    #region Constructor

    public AsyncProcess(int timeoutPeriod = -1)
    {
        if (timeoutPeriod > 0)
        {
            _timeoutPeriod = timeoutPeriod;
            _cancellationTokenSource.Token.Register(OnCancelTask, false);
        }

        // Process Args
        EnableRaisingEvents = true;
        InitializeStartInfo();
        
        // Events
        SubscribeEvents();
    }

    #endregion

    #region Public Methods

    public async Task<ProcessResult> Run()
    {
        BeginTimeout();
        try
        {
            Start();
            BeginOutputReadLine();
            BeginErrorReadLine();
            return await _taskCompletionSource.Task;
        }
        catch (OperationCanceledException)
        {
            return ProcessResult.Timeout;
        }
    }

    #endregion

    #region Private Methods
    
    private void SubscribeEvents()
    {
        OutputDataReceived += OnOutputDataReceived;
        ErrorDataReceived += OnErrorDataReceived;
        Exited += OnProcessExit;
    }

    private void InitializeStartInfo()
    {
        StartInfo.RedirectStandardOutput = true;
        StartInfo.RedirectStandardError = true;
        StartInfo.UseShellExecute = false;
    }

    private void BeginTimeout()
    {
        if (_timeoutPeriod <= 0)
            return;
        _cancellationTokenSource.CancelAfter(_timeoutPeriod);
    }

    #endregion

    #region Event Handlers

    private void OnOutputDataReceived(object? sender, DataReceivedEventArgs args)
    {
        if (args.Data is not null)
            Output.Add(args.Data);
    }

    private void OnErrorDataReceived(object? sender, DataReceivedEventArgs args)
    {
        if (args.Data is not null)
            Errors.Add(args.Data);
    }

    private void OnProcessExit(object? sender, EventArgs args)
    {
        _taskCompletionSource.TrySetResult(new ProcessResult(ExitCode, OutputString));
    }
    
    private void OnCancelTask()
    {
        if (!_taskCompletionSource.TrySetCanceled())
            return;
        Output.Add(Messages.TaskCancelled);
    }

    #endregion
}