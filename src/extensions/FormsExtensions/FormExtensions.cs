using System.Runtime.InteropServices;

namespace JackTheVideoRipper.extensions;

public static class FormExtensions
{
    [DllImport("user32.dll")]
    private static extern bool LockWindowUpdate(IntPtr hWndLock);

    public static void Suspend(this Form form)
    {
        LockWindowUpdate(form.Handle);
    }

    public static void Resume(this Form form)
    {
        LockWindowUpdate(IntPtr.Zero);
    }
    
    public static bool Confirm(this Form form)
    {
        return form.ShowDialog().IsSuccess();
    }
    
    public static bool Confirm(this CommonDialog dialog)
    {
        return dialog.ShowDialog().IsSuccess();
    }

    public static void Close(this Form form, DialogResult result)
    {
        form.DialogResult = result;
        form.Close();
    }

    public static bool AtScrollBottom(this Form form, int scrollValue)
    {
        VScrollProperties vs = form.VerticalScroll;
        return scrollValue == vs.Maximum - vs.LargeChange + 1;
    }
    
    public static bool AtScrollBottom(this UserControl control, int scrollValue)
    {
        VScrollProperties vs = control.VerticalScroll;
        return scrollValue == vs.Maximum - vs.LargeChange + 1;
    }
}