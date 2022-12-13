namespace JackTheVideoRipper.modules;

public static class ExifTool
{
    #region Data Members

    private const string _EXECUTABLE_NAME = "exiftool.exe";
    
    public static readonly string ExecutablePath = FileSystem.ProgramPath(_EXECUTABLE_NAME);
    
    private const string _DOWNLOAD_URL = "https://exiftool.sourceforge.net"; // TODO: ...
    
    private static readonly Command _Command = new(ExecutablePath);
    
    #endregion

    #region Attributes

    public static bool IsInstalled => File.Exists(ExecutablePath);

    #endregion

    #region Public Methods

    public static void DownloadLatest()
    {
        FileSystem.GetWebResourceHandle(_DOWNLOAD_URL);
    }

    #endregion
}