using System.Runtime.InteropServices;

namespace System.Windows.Forms;

public static class ControlExtensions
{
    [DllImport("user32.dll")]
    private static extern bool LockWindowUpdate(IntPtr hWndLock);

    public static void Suspend(this Control control)
    {
        LockWindowUpdate(control.Handle);
    }

    public static void Resume(this Control control)
    {
        LockWindowUpdate(IntPtr.Zero);
    }
}