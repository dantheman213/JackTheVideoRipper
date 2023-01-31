using JackTheVideoRipper.extensions;
using JackTheVideoRipper.models.enums;

namespace JackTheVideoRipper.models.processes;

public record ProcessResult
{
    public ProcessResult(int exitCode = -1, string output = "")
    {
        ExitCode = exitCode;
        Output = output;
    }

    public ProcessResult(ExitCode exitCode, string output = "")
    {
        ExitCode = exitCode.Cast<int>();
        Output = output;
    }
    
    public readonly int ExitCode;
    public readonly string Output;

    public bool Failed => ExitCode > 0;

    public bool Succeeded => ExitCode == 0;

    public bool Invalid => ExitCode < 0;

    public static readonly ProcessResult Timeout = new(enums.ExitCode.TimeoutExceeded);
    
    public static readonly ProcessResult FailureGeneric = new(enums.ExitCode.Generic);
}