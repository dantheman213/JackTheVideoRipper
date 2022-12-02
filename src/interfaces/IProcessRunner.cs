using System.Diagnostics;
using JackTheVideoRipper.models;

namespace JackTheVideoRipper.interfaces;

public interface IProcessRunner
{
    Process Process { get; }
    
    bool Paused { get; }

    ProcessStatus ProcessStatus { get; }
    
    bool Started { get; }
                
    bool Finished { get; }

    bool Completed { get; }
    
    bool Failed { get; }
    
    Guid Guid { get; }
    
    ProcessBuffer Buffer { get; }

    void Update();

    void Start();

    void Stop();

    void Retry();

    void Cancel();

    void Pause();

    void Resume();

    void Kill();
    
    void TryKillProcess();
}