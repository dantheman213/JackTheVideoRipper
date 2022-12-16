namespace JackTheVideoRipper;

public static class Config
{
    public static async Task Save()
    {
        await Task.Run(Settings.Save);
        await Task.Run(History.Save);
    }

    public static async Task Load()
    {
        await Task.Run(Settings.Load);
        await Task.Run(History.Load);
    }
}