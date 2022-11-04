namespace JackTheVideoRipper
{
    internal static class FFMPEG
    {
        private const string _EXECUTABLE_NAME = "ffmpeg.exe";
        private static readonly string _ExecutablePath = FileSystem.ProgramPath(_EXECUTABLE_NAME);
        private const string _DOWNLOAD_URL = "https://www.ffmpeg.org/download.html";

        private const string _PARAMETERS = "-nostats -loglevel error -hide_banner";

        public static bool IsInstalled => File.Exists(_ExecutablePath);

        public static void ConvertImageToJpg(string inputPath, string outputPath)
        {
            if (!IsInstalled)
                return;
            
            CLI.RunCommand($"{_ExecutablePath} {_PARAMETERS} -i {inputPath} -vf \"scale=1920:-1\" {outputPath}");
            
            // allow the file to exist in the OS before querying for it otherwise sometimes its missed by app...
            Thread.Sleep(1000);
        }

        public static void DownloadLatest()
        {
            FileSystem.GetWebResourceHandle(_DOWNLOAD_URL);
        }
    }
}
