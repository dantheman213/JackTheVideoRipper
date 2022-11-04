namespace JackTheVideoRipper
{
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            using (new Mutex(true, FileSystem.PROGRAM_NAME, out bool firstRun))
            {
                if (firstRun)
                {
                    // fixes blurry text on some screens
                    if (Environment.OSVersion.Version.Major >= 6)
                        SetProcessDPIAware();

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Statistics.InitializeCounters();
                    Application.Run(new FrameMain());
                }
                else
                {
                    Modals.Notification(@"Application already running!");
                }
            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
    }
}
