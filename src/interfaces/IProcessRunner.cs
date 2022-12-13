using System.Diagnostics;
using JackTheVideoRipper.models;

namespace JackTheVideoRipper.interfaces;

public interface IProcessRunner
{
    Process? Process { get; }
    
    ProcessStatus ProcessStatus { get; }
    
    Guid Guid { get; }
    
    ProcessBuffer Buffer { get; }

    int ExitCode { get; }

    bool Completed { get; }
    
    string FileName { get; }
    
    bool Failed { get; }
    
    bool Succeeded { get; }
    
    bool Started { get; }
                
    bool Finished { get; }
    
    bool Paused { get; }

    Task<bool> Update();

    Task<bool> Start();

    void Stop();

    void Retry();

    void Cancel();

    void Pause();

    void Resume();

    void Enqueue();

    void Kill();
    
    void TryKillProcess();
}