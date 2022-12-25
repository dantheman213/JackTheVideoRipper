using JackTheVideoRipper.extensions;
using JackTheVideoRipper.models.containers;

namespace JackTheVideoRipper.modules;

public static class ExifTool
{
    #region Data Members

    private const string _EXECUTABLE_NAME = "exiftool.exe";
    
    public static readonly string ExecutablePath = FileSystem.ProgramPath(_EXECUTABLE_NAME);
    
    private const string _DOWNLOAD_URL = "https://sourceforge.net/projects/exiftool/files/latest/download";
    
    private static readonly Command _Command = new(ExecutablePath, validationHandler:ValidationHandler);
    
    #endregion

    #region Attributes

    public static bool IsInstalled => File.Exists(ExecutablePath);

    #endregion

    #region Public Methods
    
    public static async Task DownloadLatest()
    {
        string? result = await FileSystem.ExtractResourceFromUrl(_DOWNLOAD_URL, FileSystem.Paths.Install);
        
        //await FileSystem.GetWebResourceHandle(_DOWNLOAD_URL, FileSystem.Paths.Install).Run();
    }

    private static class Parameters
    {
        // Removes tab aligned spacing that makes the usual output formatted like a table
        public const string CompactFormat = "-s -s";
    
        // Returns only the value of the requested tags, no keys or messages
        public const string ReturnValueOnly = "-s -s -s";
    }

    public static async Task<string> GetMetadataString(string filepath)
    {
        return await _Command.RunCommandAsync($"{Parameters.CompactFormat} {filepath.WrapQuotes()}");
    }

    public static async Task<ExifData> GetMetadata(string filepath)
    {
        return new ExifData(await GetMetadataString(filepath));
    }

    public static async Task<bool> AddTag(string filepath, string key, object value)
    {
        return (await _Command.RunCommandAsync($"-{key}={value.ToString()?.WrapQuotes()} {filepath.WrapQuotes()}")).HasValue();
    }
    
    public static async Task<string> GetTag(string filepath, string key)
    {
        return await _Command.RunCommandAsync($"{Parameters.ReturnValueOnly} -{key} {filepath.WrapQuotes()}");
    }
    
    public static async Task<string> GetTags(string filepath, params string[] keys)
    {
        return await _Command.RunCommandAsync($"{Parameters.CompactFormat} {MergeKeys(keys)} {filepath.WrapQuotes()}");
    }

    public static async Task<Dictionary<string, string>> GetTagsAsDict(string filepath, params string[] keys)
    {
        return ExifData.BuildFromData(await GetTags(filepath, keys));
    }
    
    public static async Task<int> GetTotalFrames(string filepath)
    {
        return new ExifData(await GetTags(filepath, "Duration", "VideoFrameRate")).Frames;
    }

    private static bool ValidationHandler(string output)
    {
        //return output.Contains("files updated") && !output.Contains("No file specified");
        return !output.Contains("No file specified");
    }
    
    private static string MergeKeys(params string[] keys)
    {
        return keys.Select(k => $"-{k}").Merge(" ");
    }

    #endregion
}