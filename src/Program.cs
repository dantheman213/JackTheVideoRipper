namespace JackTheVideoRipper
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            using (new Mutex(true, "JackTheVideoRipper", out bool firstRun))
            {
                if (firstRun)
                {
                    if (Environment.OSVersion.Version.Major >= 6)
                        SetProcessDPIAware(); // fixes blurry text on some screens

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new FrameMain());
                }
                else
                {
                    MessageBox.Show(@"Application already running!", @"Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
    }
}
