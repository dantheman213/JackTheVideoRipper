namespace JackTheVideoRipper
{
    internal static class AtomicParsley
    {
        private const string binName = "AtomicParsley.exe";
        private static readonly string binPath = $"{Common.InstallDirectory}\\{binName}";

        public static bool IsInstalled()
        {
            return File.Exists(binPath);
        }
    }
}
