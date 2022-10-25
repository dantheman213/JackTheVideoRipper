using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace JackTheVideoRipper;

public static class Input
{
    [DllImport("kernel32.dll", SetLastError = true)]
    //public static extern bool AttachConsole(uint dwProcessId);
    public static extern bool AttachConsole(int pid);
        
    [DllImport("kernel32")]
    public static extern bool AllocConsole();
        
    [DllImport("kernel32.dll")]
    public static extern IntPtr GetStdHandle(int nStdHandle);
        
    public static bool RunAsConsole()
    {
        // Check if Console exists, if not, attach it
        return AttachConsole(-1) || AllocConsole();
    }

    public static IntPtr StandardOutputHandle => GetStdHandle(-11);
        
    public static SafeFileHandle StandardOutputHandleSafe => new(StandardOutputHandle, false);

    public static StreamWriter GetConsoleWriter()
    {
        return new StreamWriter(new FileStream(StandardOutputHandleSafe, FileAccess.Write));
    }
}