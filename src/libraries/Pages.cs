namespace JackTheVideoRipper;

public static class Pages
{
    public static DialogResult OpenPage<T>() where T : Form, new()
    {
        using Form form = new T();
        return form.ShowDialog();
    }
}