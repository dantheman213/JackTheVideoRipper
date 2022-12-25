namespace JackTheVideoRipper;

internal static class Settings
{
    public static SettingsModel Data { get; private set; } = new();

    public static void Save()
    {
        Data.WriteToDisk();
    }

    public static void Load()
    {
        // If it returns null, it does not exist on disk
        if (Data.CreateOrLoadFromDisk<SettingsModel>() is not { } loadedData)
            return;
        
        loadedData.Validate();

        Data = loadedData;
    }
}
