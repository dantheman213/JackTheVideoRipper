namespace JackTheVideoRipper;

public static class Pages
{
    public static DialogResult OpenPage<T>() where T : Form, new()
    {
        using Form form = new T();
        return form.ShowDialog();
    }
    
    public static bool OpenPageConfirm<T>() where T : Form, new()
    {
        using Form form = new T();
        return form.ShowDialog() is DialogResult.OK or DialogResult.Yes or DialogResult.Continue;
    }
}