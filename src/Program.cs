using System.Diagnostics;

namespace JackTheVideoRipper
{
    internal static class Program
    {
        #region Data Members

        private static readonly Task[] _BackgroundTasks =
        {
            Core.LoadConfigurationFiles(),
            Statistics.InitializeCounters(),
            Core.CheckForUpdates(),
            YouTubeDL.StartupTasks(),
            Core.UpdateDependencies(),
            Task.Run(FileSystem.ValidateInstallDirectory),
            Task.Run(Core.CheckDependencies),
            Core.CheckForYouTubeDLUpdates()
        };

        #endregion

        #region Constructor

        static Program()
        {
            Statistics.BeginStartup();
            ConfigureExceptionHandling();
            ConfigureGraphics();
            Ripper.Instance = new Ripper(new FormsViewItemProvider());
        }

        #endregion

        #region Main

        [STAThread]
        private static void Main()
        {
            using Mutex singleInstanceMutex = FileSystem.CreateSingleInstanceLock(out bool isOnlyInstance);

            if (!isOnlyInstance)
            {
                Modals.Notification(Messages.AlreadyRunning);
                Environment.Exit(1056);  //< An instance of the service is already running.
                return;
            }
            
            Task.Run(StartBackgroundTasks);
            Application.Run(Ripper.Instance.FrameMain);
            Application.Exit();
        }

        #endregion

        #region Private Methods

        private static async Task StartBackgroundTasks()
        {
            // Allows us to read out the console values
            if (Debugger.IsAttached)
                Input.OpenConsole();

            await Task.WhenAll(_BackgroundTasks);
        }

        private static void ConfigureGraphics()
        {
            // fixes blurry text on some screens
            if (FileSystem.OSVersion.Major >= 6)
                SetProcessDPIAware();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
        }

        private static void ConfigureExceptionHandling()
        {
            // We want our IDE to access the exceptions
            if (Debugger.IsAttached)
                return;
            
            // Add the event handler for handling UI thread exceptions to the event.
            Application.ThreadException += Pages.OpenExceptionHandler;

            //AppDomain.CurrentDomain.FirstChanceException += Pages.OpenExceptionHandler;

            // Set the unhandled exception mode to force all Windows Forms errors to go through our handler.
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            // Add the event handler for handling non-UI thread exceptions to the event.
            AppDomain.CurrentDomain.UnhandledException += Pages.OpenExceptionHandler;
        }        

        #endregion

        #region Imports

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        #endregion
    }
}
