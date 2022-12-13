using JackTheVideoRipper.models.containers;

namespace JackTheVideoRipper;

internal static class History
{
    public static HistoryModel Data { get; private set; } = new();
    
    public static void Save()
    {
        Data.WriteToDisk();
    }

    public static void Load()
    {
        // If it returns null, it does not exist on disk
        if (Data.CreateOrLoadFromDisk<HistoryModel>() is not { } loadedData)
            return;

        Data = loadedData;
    }
}