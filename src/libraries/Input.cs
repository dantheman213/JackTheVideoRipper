using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace JackTheVideoRipper;

public static class Input
{
    #region Data Members

    private static readonly List<Task<bool>> _TaskTypeQueue = new();
    
    private const int _TYPING_PING = 1800;

    #endregion

    #region Attributes
    
    public static IntPtr StandardInputHandle => GetStdHandle(-10);

    public static IntPtr StandardOutputHandle => GetStdHandle(-11);
    
    public static IntPtr StandardErrorHandle => GetStdHandle(-12);

    public static SafeFileHandle StandardOutputHandleSafe => new(StandardOutputHandle, false);

    public static bool ConsoleAttached => AttachConsole(-1);

    #endregion
    
    #region Imports

    [DllImport("kernel32.dll", SetLastError = true)]
    //public static extern bool AttachConsole(uint dwProcessId);
    private static extern bool AttachConsole(int pid);
        
    [DllImport("kernel32")]
    private static extern bool AllocConsole();
        
    [DllImport("kernel32.dll")]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    #endregion
    
    #region Public Methods

    public static bool OpenConsole()
    {
        // Check if Console exists, if not, attach it
        return ConsoleAttached || AllocConsole();
    }

    public static StreamWriter GetConsoleWriter()
    {
        return new StreamWriter(new FileStream(StandardOutputHandleSafe, FileAccess.Write));
    }
    
    // https://stackoverflow.com/questions/33776387/dont-raise-textchanged-while-continuous-typing
    public static async Task WaitForFinishTyping<T>(Func<T> valueGenerator) where T : IComparable
    {
        async Task<bool> IsStillTyping()
        {
            Application.DoEvents();

            int taskCount = _TaskTypeQueue.Count;
            T oldValue = valueGenerator.Invoke();
            await Task.Delay(_TYPING_PING);

            return oldValue.Equals(valueGenerator.Invoke()) || taskCount != _TaskTypeQueue.Count - 1;
        }

        _TaskTypeQueue.Add(IsStillTyping());
        if (await _TaskTypeQueue[^1])
            return;

        // typing appears to have stopped, continue
        _TaskTypeQueue.Clear();
    }

    #endregion
}