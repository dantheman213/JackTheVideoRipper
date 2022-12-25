using System.Diagnostics;
using System.Runtime.InteropServices;

namespace JackTheVideoRipper.extensions;

public static class ProcessExtensions
{
    #region Embedded Types

    [Flags]
    private enum ThreadAccess : uint
    {
        TERMINATE            = 0x0001,
        SUSPEND_RESUME       = 0x0002,
        GET_CONTEXT          = 0x0008,
        SET_CONTEXT          = 0x0010,
        SET_INFORMATION      = 0x0020,
        QUERY_INFORMATION    = 0x0040,
        SET_THREAD_TOKEN     = 0x0080,
        IMPERSONATE          = 0x0100,
        DIRECT_IMPERSONATION = 0x0200
    }

    #endregion

    #region Imports

    [DllImport("kernel32.dll")]
    private static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
    
    [DllImport("kernel32.dll")]
    private static extern int SuspendThread(IntPtr hThread);
    
    [DllImport("kernel32.dll")]
    private static extern int ResumeThread(IntPtr hThread);
    
    [DllImport("kernel32.dll")]
    private static extern int ExitProcess(uint uExitCode);
    
    [DllImport("kernel32.dll")]
    private static extern int TerminateProcess(IntPtr hProcess, uint uExitCode);
    
    [DllImport("kernel32.dll")]
    private static extern IntPtr OpenProcess(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

    #endregion

    #region Private Methods

    private static IntPtr GetProcessThread(uint id) => OpenThread(ThreadAccess.SUSPEND_RESUME, false, id);

    #endregion

    #region Public Methods
    
    public static int Exit(this Process process, int exitCode)
    {
        return ExitProcess((uint) exitCode);
    }

    public static int Terminate(this Process process, int exitCode)
    {
        return TerminateProcess((IntPtr) process.Id, (uint) exitCode);
    }

    public static void Suspend(this Process process)
    {
        foreach (uint threadId in process.GetThreads().Select(t => t.Id))
        {
            ThreadOperation(threadId, SUSPEND);
        }
    }
    
    public static void Resume(this Process process)
    {
        foreach (uint threadId in process.GetThreads().Select(t => t.Id))
        {
            ThreadOperation(threadId, RESUME);
        }
    }

    private static void ThreadOperation(uint threadId, string operation)
    {
        IntPtr pOpenThread = GetProcessThread(threadId);
        if (pOpenThread == IntPtr.Zero)
            return;
        int result = MethodDict[operation].Invoke(pOpenThread);
        if (result != 0)
            ErrorHandler(operation, threadId, result);
    }

    public const string SUSPEND = "suspend";
    
    public const string RESUME = "resume";

    public static readonly Dictionary<string, Func<IntPtr, int>> MethodDict = new()
    {
        { SUSPEND, SuspendThread },
        { RESUME, ResumeThread }
    };

    public static void ErrorHandler(string operation, uint threadId, int result)
    {
        throw new ApplicationException($"Failed to {operation} thread (id = {threadId}! (Error code: {result}");
    }

    public static string GetOutput(this Process process)
    {
        return process.StandardOutput.ReadToEnd().Trim();
    }
    
    public static async Task<string> GetOutputAsync(this Process process)
    {
        return (await process.StandardOutput.ReadToEndAsync()).Trim();
    }

    public static string GetProcessInfo(this Process process)
    {
        ProcessStartInfo processStartInfo = process.StartInfo;
        return new []
        {
            $"Program: {Path.GetFileName(processStartInfo.FileName).WrapQuotes()}",
            $" > Path: {processStartInfo.FileName.WrapQuotes()}",
            $" > Arguments: {processStartInfo.Arguments}",
            $" > Working Directory: {processStartInfo.WorkingDirectory.WrapQuotes()}",
        }.MergeNewline();
    }
    
    public static Task WaitForExitAsync(this Process process, CancellationToken cancellationToken = default)
    {
        if (process.HasExited)
            return Task.CompletedTask;

        TaskCompletionSource<object?> tcs = new();
        process.EnableRaisingEvents = true;
        process.Exited += (_, _) => tcs.TrySetResult(null);
        if (cancellationToken != default)
            cancellationToken.Register(() => tcs.SetCanceled(cancellationToken));

        return process.HasExited ? Task.CompletedTask : tcs.Task;
    }

    public static IEnumerable<ProcessThread> GetThreads(this Process process)
    {
        return process.HasStarted() ? 
            Enumerable.Empty<ProcessThread>() : 
            Enumerable.Cast<ProcessThread>(process.Threads).ToArray();
    }

    public static bool HasStarted(this Process process)
    {
        return GetProcessThread((uint) process.Id) != IntPtr.Zero;
    }
    
    #endregion
}