using System.Diagnostics;
using JackTheVideoRipper.extensions;
using ProcessResult = System.Threading.Tasks.Task<(int ExitCode, string Output)>;

namespace JackTheVideoRipper.models;

public class AsyncProcess : Process
{
    private readonly TaskCompletionSource<(int, string)> _taskCompletionSource = new();

    private readonly List<string> _output = new();
    private readonly List<string> _errors = new();

    public new bool EnableRaisingEvents
    {
        get => base.EnableRaisingEvents;
        init => base.EnableRaisingEvents = value;
    }
    
    public AsyncProcess()
    {
        // Process Args
        EnableRaisingEvents = true;
        StartInfo.RedirectStandardOutput = true;
        StartInfo.RedirectStandardError = true;
        
        // Events
        OutputDataReceived += OnOutputDataReceived;
        ErrorDataReceived += OnErrorDataReceived;
        Exited += OnProcessExit;
    }
    
    private void OnOutputDataReceived(object? sender, DataReceivedEventArgs args)
    {
        if (args.Data is not null)
            _output.Add(args.Data);
    }

    private void OnErrorDataReceived(object? sender, DataReceivedEventArgs args)
    {
        if (args.Data is not null)
            _errors.Add(args.Data);
    }

    private void OnProcessExit(object? sender, EventArgs args)
    {
        _taskCompletionSource.TrySetResult((ExitCode, _output.MergeNewline()));
    }

    public async ProcessResult Run()
    {
        Start();
        BeginOutputReadLine();
        BeginErrorReadLine();
        return await _taskCompletionSource.Task;
    }
}