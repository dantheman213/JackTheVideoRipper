using System.Diagnostics;

namespace JackTheVideoRipper.interfaces;

public interface IProcessRunner
{
    public Process Process { get; set; }
    
    List<string> Results { get; set; }
    
    int Cursor { get; set; }
    
    bool Paused { get; }

    ProcessStatus ProcessStatus { get; }

    void AppendStatusLine();

    void TrackStandardOut();

    void AppendErrorLine();

    void TrackStandardError();

    void Update();

    void Start();

    void Stop();

    void Retry();

    void Cancel();

    void Pause();

    void Resume();

    void SkipToEnd();
}