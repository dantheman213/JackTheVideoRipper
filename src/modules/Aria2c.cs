namespace JackTheVideoRipper.modules;

public static class Aria2c
{
    #region Data Members
    
    private const string _EXECUTABLE_NAME = "aria2c.exe";
    
    public static readonly string ExecutablePath = FileSystem.ProgramPath(_EXECUTABLE_NAME);
    
    private const string _DOWNLOAD_URL = "https://github.com/aria2/aria2/releases/latest";
    
    private static readonly Command _Command = new(ExecutablePath);
    
    public const string DEFAULT_ARGS = "-x 16 -s 16 -k 1M";
    
    #endregion
    
    #region Attributes

    public static bool IsInstalled => File.Exists(ExecutablePath);

    #endregion

    #region Public Methods

    public static async Task DownloadLatest()
    {
        await FileSystem.GetWebResourceHandle(_DOWNLOAD_URL, FileSystem.Paths.Install).Run();
    }

    #endregion
}