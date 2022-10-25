namespace JackTheVideoRipper
{
    internal static class AtomicParsley
    {
        private const string executableName = "AtomicParsley.exe";
        private static readonly string binPath = FileSystem.ProgramPath(executableName);
        public const string DOWNLOAD_URL = "http://atomicparsley.sourceforge.net";

        public static bool IsInstalled()
        {
            return File.Exists(binPath);
        }
    }
}
