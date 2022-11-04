namespace JackTheVideoRipper
{
    internal static class AtomicParsley
    {
        private const string _EXECUTABLE_NAME = "AtomicParsley.exe";
        private static readonly string _ExecutablePath = FileSystem.ProgramPath(_EXECUTABLE_NAME);
        private const string _DOWNLOAD_URL = "https://atomicparsley.sourceforge.net";

        public static bool IsInstalled => File.Exists(_ExecutablePath);

        public static void DownloadLatest()
        {
            FileSystem.GetWebResourceHandle(_DOWNLOAD_URL);
        }
    }
}
