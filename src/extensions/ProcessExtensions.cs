using System.Diagnostics;
using System.Runtime.InteropServices;

namespace JackTheVideoRipper.extensions;

public static class ProcessExtensions
{
    [Flags]
    public enum ThreadAccess : uint
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
    
    [DllImport("kernel32.dll")]
    private static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
    
    [DllImport("kernel32.dll")]
    private static extern uint SuspendThread(IntPtr hThread);
    
    [DllImport("kernel32.dll")]
    private static extern int ResumeThread(IntPtr hThread);

    public static void Suspend(this Process process)
    {
        foreach (ProcessThread thread in process.Threads)
        {
            IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)thread.Id);
            if (pOpenThread == IntPtr.Zero)
                break;
            uint result = SuspendThread(pOpenThread);
            if (result != 0)
            {
                throw new ApplicationException($"Failed to suspend thread (id = {thread.Id}! (Error code: {result}");
            }
        }
    }
    public static void Resume(this Process process)
    {
        foreach (ProcessThread thread in process.Threads)
        {
            IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)thread.Id);
            if (pOpenThread == IntPtr.Zero)
                break;
            int result = ResumeThread(pOpenThread);
            if (result != 0)
            {
                throw new ApplicationException($"Failed to suspend thread (id = {thread.Id}! (Error code: {result}");
            }
        }
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
        return new []
        {
            $"Program: {Path.GetFileName(process.StartInfo.FileName).WrapQuotes()}",
            $" > Path: {process.StartInfo.FileName.WrapQuotes()}",
            $" > Arguments: {process.StartInfo.Arguments}",
            $" > Working Directory: {process.StartInfo.WorkingDirectory.WrapQuotes()}",
        }.MergeNewline();
    }
}