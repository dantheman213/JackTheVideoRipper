using JackTheVideoRipper.models.containers;
using JackTheVideoRipper.modules;

namespace JackTheVideoRipper.models;

public class MediaStorageManager
{
    private static readonly string _Directory = FileSystem.Paths.Data;
    
    private static readonly string _Filepath = FileSystem.MergePaths(_Directory, "media_table.json");
    
    private MediaTable _mediaTable = new();

    public void LoadFromDisk()
    {
        if (FileSystem.Deserialize<MediaTable>(_Filepath) is { } mediaTable)
            _mediaTable = mediaTable;
    }
    
    public void SaveToDisk()
    {
        FileSystem.WriteJsonToFile(_Filepath, _mediaTable);
    }

    public async Task AddMedia()
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

        if (FileSystem.SelectFolder("H:\\Orno\\Projekte") is not { } projectRoot)
            return;

        if (FileSystem.SelectFile(projectRoot) is not { } originalFile)
            return;

        ExifData metadata = await ExifTool.GetMetadata(originalFile);
        
        _mediaTable.AddMedia(metadata.Title, "", metadata.Comments, originalFile, projectRoot);
    }
}