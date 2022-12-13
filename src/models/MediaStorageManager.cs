namespace JackTheVideoRipper.models;

public class MediaStorageManager
{
    private static readonly string Directory = Path.Combine(FileSystem.Paths.Settings, "data");
    private static readonly string Filepath = Path.Combine(Directory, "media_table.json");
    
    private MediaTable MediaTable = new();

    public void LoadFromDisk()
    {
        if (FileSystem.Deserialize<MediaTable>(Filepath) is { } mediaTable)
            MediaTable = mediaTable;
    }

    public void AddMedia()
    {
        // Open folder select to select file
        
        /**
         * WE WILL NEED:
         * Title
         * Description
         * Link
         * OriginalPath
         * ProjectPath (Optional)
         * Tags = tags (Optional)
         * TableIndex = NodeCount
         **/
    }
}