using System.Diagnostics;

namespace JackTheVideoRipper
{
    internal static class Program
    {
        [STAThread]
        private static async Task Main()
        {
            using Mutex singleInstanceMutex = new(true, FileSystem.PROGRAM_NAME, out bool isOnlyInstance);

            if (!isOnlyInstance)
            {
                Modals.Notification(@"Application already running!");
                return;
            }
            
            await StartBackgroundTasks();
            ConfigureExceptionHandling();
            ConfigureGraphics();
            Application.Run(new FrameMain());
            Application.Exit();
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        private static async Task StartBackgroundTasks()
        {
            // Allows us to read out the console values
            if (Debugger.IsAttached)
                Input.OpenConsole();
            await Task.WhenAll(Core.LoadConfigurationFiles(), Statistics.InitializeCounters(), Core.CheckForUpdates());
        }

        private static void ConfigureGraphics()
        {
            // fixes blurry text on some screens
            if (Environment.OSVersion.Version.Major >= 6)
                SetProcessDPIAware();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
        }

        private static void ConfigureExceptionHandling()
        {
            // Add the event handler for handling UI thread exceptions to the event.
            //Application.ThreadException += new ThreadExceptionEventHandler(ErrorHandlerForm.Form1_UIThreadException);

            // Set the unhandled exception mode to force all Windows Forms errors to go through our handler.
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            // Add the event handler for handling non-UI thread exceptions to the event.
            //AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }
    }
}
