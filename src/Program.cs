using System.Diagnostics;

namespace JackTheVideoRipper
{
    internal static class Program
    {
        [STAThread]
        private static async Task Main(string[] args)
        {
            using (new Mutex(true, FileSystem.PROGRAM_NAME, out bool firstRun))
            {
                if (firstRun)
                {
                    // fixes blurry text on some screens
                    if (Environment.OSVersion.Version.Major >= 6)
                        SetProcessDPIAware();

                    await StartBackgroundTasks();
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
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

        private static async Task StartBackgroundTasks()
        {
            // Allows us to read out the console values
            if (Debugger.IsAttached)
                Input.OpenConsole();
            
            await Task.Run(Settings.Load);
            await Task.Run(Statistics.InitializeCounters);
            await Core.CheckForUpdates();
        }

        private static void DownloadImages()
        {
            Settings.Load();
            
            string[] ids = { "JO-Q73f3yPi5rWjE-w",
                "IO_B6Cemwqjtqz2f9g",
                "J-XBvSSvyvrv-jiWqQ",
                "J-zF73Tznq_s_zWU_w",
                "JriSuHCvm66_-T6QrQ",
                "Ju-UuST0zK_u_zmR-A",
                "JOjAuCCunKzkrG2R_A",
                "Je2Sv36lnPi6rDzBqQ",
                "IrzCuXX1nq_pqjvCqg",
                "J-iQu3f3m_zq_jjB_A",
                "Jb7H7nChz6rpqziTrg",
                "J-uXvCegnqi9rjWR_w",
                "ILuW63Kjm_jl-zSf-w"
            };
            
            string[] prefixes = {"main", "w320h240", "common"};

            var counts = new Dictionary<string, int>
            {
                { prefixes[0], 1 },
                { prefixes[1], 10 },
                { prefixes[2], 3 }
            };

            foreach (string id in ids)
            {
                foreach (string prefix in prefixes)
                {
                    for (int i = 0; i < counts[prefix]; i++)
                    {
                        string filename = $"{id}_{prefix}_{i}.jpg";
                        
                        FileSystem.DownloadWebFile($"https://static-cache.k2s.cc/thumbnail/{id}/{prefix}/{i}.jpeg",
                            Path.Combine(Settings.Data.DefaultDownloadPath, "Thumbnails", filename));
                        
                        Console.WriteLine($"Downloaded: \"{filename}\" to disk!");
                    }
                }
            }
            
            Console.WriteLine("Downloads completed!");
        }
    }
}
