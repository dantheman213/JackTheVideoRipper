using System.Diagnostics;
using System.Text.RegularExpressions;
using JackTheVideoRipper.interfaces;

namespace JackTheVideoRipper;

public class ProcessRunner : IProcessRunner
{
    public Process Process { get; set; }
    
    public List<string> Results { get; set; } = new() { string.Empty }; // Placeholder result
    
    public int Cursor { get; set; } // where in message buffer are we
    
    public bool AtEndOfBuffer => Cursor >= Results.Count - 1;
    
    public bool Failed => Process.ExitCode > 0;
    
    private static readonly Regex _SpaceSplitPattern = new(@"\s+");
    
    protected string ProcessLine => Results[Cursor];
    
    public string[] TokenizedProcessLine => _SpaceSplitPattern.Split(ProcessLine);

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

    private void RunWhileProcessActive(Action action)
    {
        Task.Run(() => { while (Process is { HasExited: false }) { action(); } });
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

    protected void TryKillProcess()
    {
        try { Process.Kill(); }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
    }
}