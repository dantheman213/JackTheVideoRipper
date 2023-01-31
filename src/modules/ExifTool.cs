using JackTheVideoRipper.extensions;
using JackTheVideoRipper.models.containers;

namespace JackTheVideoRipper.modules;

public static class ExifTool
{
    #region Data Members
    
    public static readonly string ExecutablePath = FileSystem.ProgramPath(Executables.ExifTool);
    
    private static readonly Command _Command = new(ExecutablePath,
        validationHandler:ValidationHandler, 
        throwOnValidationFailed:true);
    
    #endregion

    #region Attributes

    public static bool IsInstalled => File.Exists(ExecutablePath);

    #endregion

    #region Public Methods
    
    public static async Task DownloadLatest()
    {
        await FileSystem.ExtractResourceFromUrl(Urls.ExifTool, FileSystem.Paths.Install);
        
        //await FileSystem.GetWebResourceHandle(_DOWNLOAD_URL, FileSystem.Paths.Install).Run();
    }

    private static class Parameters
    {
        // Removes tab aligned spacing that makes the usual output formatted like a table
        public const string COMPACT_FORMAT = "-s -s";
    
        // Returns only the value of the requested tags, no keys or messages
        public const string RETURN_VALUE_ONLY = "-s -s -s";
    }

    public static async Task<string> GetMetadataString(string filepath)
    {
        return await _Command.RunCommandAsync($"{Parameters.COMPACT_FORMAT} {filepath.WrapQuotes()}");
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
        return await _Command.RunCommandAsync($"{Parameters.RETURN_VALUE_ONLY} -{key} {filepath.WrapQuotes()}");
    }
    
    public static async Task<string> GetTags(string filepath, params string[] keys)
    {
        return await _Command.RunCommandAsync($"{Parameters.COMPACT_FORMAT} {MergeKeys(keys)} {filepath.WrapQuotes()}");
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