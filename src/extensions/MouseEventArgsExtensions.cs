namespace JackTheVideoRipper.extensions;

public static class MouseEventArgsExtensions
{
    public static bool IsRightClick(this MouseEventArgs e)
    {
        return e.Button == MouseButtons.Right;
    }
}