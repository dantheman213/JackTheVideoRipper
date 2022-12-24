namespace JackTheVideoRipper.extensions;

public static class FormExtensions
{
    public static bool Confirm(this Form form)
    {
        return form.ShowDialog() is DialogResult.OK or DialogResult.Yes;
    }
    
    public static bool Confirm(this CommonDialog dialog)
    {
        return dialog.ShowDialog() is DialogResult.OK or DialogResult.Yes;
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