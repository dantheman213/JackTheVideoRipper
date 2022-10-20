namespace JackTheVideoRipper
{
    class FFmpeg
    {
        private const string executableName = "ffmpeg.exe";
        private static readonly string binPath = Path.Combine(Common.InstallDirectory, executableName);

        public static bool IsInstalled()
        {
            return File.Exists(binPath);
        }

        public static void ConvertImageToJpg(string inputPath, string outputPath)
        {
            CLI.RunCommand($"{binPath} -nostats -loglevel error -hide_banner -i {inputPath} -vf \"scale=1920:-1\" {outputPath}");
            Thread.Sleep(1000); // allow the file to exist in the OS before querying for it otherwise sometimes its missed by app...
        }
    }
}
