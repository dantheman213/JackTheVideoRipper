using System.Runtime.InteropServices;

namespace JackTheVideoRipper.extensions;

public static class RichTextBoxExtensions
{
    private const int WM_VSCROLL = 0x115;
    private const int WM_MOUSEWHEEL = 0x20A;
    private const int WM_USER = 0x400;
    private const int SB_VERT = 1;
    private const int EM_SETSCROLLPOS = WM_USER + 222;
    private const int EM_GETSCROLLPOS = WM_USER + 221;

    [DllImport("user32.dll")]
    private static extern bool GetScrollRange(IntPtr hWnd, int nBar, out int lpMinPos, out int lpMaxPos);

    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, int wParam, ref Point lParam);

    public static bool IsAtMaxScroll(this RichTextBox richTextBox)
    {
        Point rtfPoint = Point.Empty;
        GetScrollRange(richTextBox.Handle, SB_VERT, out int minScroll, out int maxScroll);
        SendMessage(richTextBox.Handle, EM_GETSCROLLPOS, 0, ref rtfPoint);
        return rtfPoint.Y + richTextBox.ClientSize.Height >= maxScroll;
    }

    public static bool HasSelected(this RichTextBox richTextBox)
    {
        return richTextBox.SelectionLength > 0;
    }
}