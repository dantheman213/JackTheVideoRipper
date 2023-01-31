namespace JackTheVideoRipper
{
    internal static class AtomicParsley
    {
        private static readonly string _ExecutablePath = FileSystem.ProgramPath(Executables.AtomicParsley);

        public static bool IsInstalled => File.Exists(_ExecutablePath);

        public static async Task DownloadLatest()
        {
            await FileSystem.GetWebResourceHandle(Urls.AtomicParsley, FileSystem.Paths.Install).Run();
        }
    }
}
