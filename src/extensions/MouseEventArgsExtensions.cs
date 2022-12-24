namespace JackTheVideoRipper.extensions;

public static class MouseEventArgsExtensions
{
    public static bool IsRightClick(this MouseEventArgs e)
    {
        return e.Button == MouseButtons.Right;
    }
    
    public static bool IsLeftClick(this MouseEventArgs e)
    {
        return e.Button == MouseButtons.Left;
    }
    
    public static bool IsMiddleClick(this MouseEventArgs e)
    {
        return e.Button == MouseButtons.Middle;
    }
}