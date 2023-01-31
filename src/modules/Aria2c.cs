namespace JackTheVideoRipper.modules;

public static class Aria2c
{
    #region Data Members

    public static readonly string ExecutablePath = FileSystem.ProgramPath(Executables.Aria2c);
    
    private static readonly Command _Command = new(ExecutablePath);
    
    public const string DEFAULT_ARGS = "-x 16 -s 16 -k 1M";
    
    #endregion
    
    #region Attributes

    public static bool IsInstalled => File.Exists(ExecutablePath);

    #endregion

    #region Public Methods

    public static async Task DownloadLatest()
    {
        await FileSystem.GetWebResourceHandle(Urls.Aria2c, FileSystem.Paths.Install).Run();
    }

    #endregion
}