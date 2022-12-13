namespace JackTheVideoRipper;

public static class Pages
{
    public static DialogResult OpenPage<T>() where T : Form, new()
    {
        using Form form = new T();
        return form.ShowDialog();
    }

    public static T OpenPageBackground<T>() where T : Form, new()
    {
        T form = new();
        Task.Run(() => form.ShowDialog());
        return form;
    }
    
    public static bool OpenPageConfirm<T>() where T : Form, new()
    {
        using Form form = new T();
        return form.ShowDialog() is DialogResult.OK or DialogResult.Yes or DialogResult.Continue;
    }
}