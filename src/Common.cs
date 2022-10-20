using System.Diagnostics;
using System.Management;
using System.Security.Principal;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace JackTheVideoRipper
{
    internal static class Common
    {
        public static string AppPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName) ?? "";
        private static PerformanceCounter? _cpuCounter;
        private static PerformanceCounter? _ramCounter;
        private static List<PerformanceCounter>? networkCounters;
        private static readonly Random _random = new();

        private static readonly string commonDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        public static readonly string RootDirectory = Path.Combine(commonDirectory, "JackTheVideoRipper");
        public static readonly string InstallDirectory = Path.Combine(RootDirectory, "bin");

        public static void OpenFolderWithFileSelect(string filePath)
        {
            Process.Start("explorer.exe", $"/select, \"{filePath}\"");
        }

        public static void OpenFolder(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                OpenFileExplorer(folderPath);
            }
            else
            {
                MessageBox.Show($@"{folderPath} Directory does not exist!");
            }
        }

        public static string GetAppVersion()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            return $"v{fvi.FileVersion}";
        }
        
        private static readonly Regex _urlPattern = new(@"^(http|http(s)?://)?([\w-]+\.)+[\w-]+[.com|.in|.org]+(\[\?%&=]*)?",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static bool IsValidUrl(string url)
        {
            return _urlPattern.IsMatch(url);
        }

        private static readonly Regex TitlePattern = new("[^a-zA-Z0-9]", RegexOptions.Compiled);

        public static string FormatTitleForFileName(string title)
        {
            return string.IsNullOrEmpty(title) ? "" : TitlePattern.Replace(title, "").Trim();
        }
        
        public static string GetCpuUsagePercentage()
        {
            if (_cpuCounter is not null) 
                return $"{_cpuCounter.NextValue():0.00}%";
            
            try
            {
                _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            } catch(Exception ex)
            {
                // TBA
                return "Unable to get CPU usage...";
            }
            return $"{_cpuCounter.NextValue():0.00}%";
        }
        
        public static string GetAvailableMemory()
        {
            if (_ramCounter != null)
                return $"{_ramCounter.NextValue()} MB";
            
            try
            {
                _ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            }
            catch (Exception ex)
            {
                return "Unable to get RAM usage...";
            }
            
            return $"{_ramCounter.NextValue()} MB";
        }
        
        public static string GetNetworkTransfer()
        {
            try
            {
                // get the network transfer in kbps or mbps automatically

                if (networkCounters == null)
                {
                    networkCounters = new List<PerformanceCounter>();

                    PerformanceCounterCategory category = new("Network Interface");
                    foreach (string? instance in category.GetInstanceNames())
                    {
                        networkCounters.Add(new PerformanceCounter("Network Interface", "Bytes Received/sec", instance));
                    }
                }

                double count = networkCounters.Sum(counter => Math.Round(counter.NextValue() / 1024, 2));

                string suffix = "kbps";
                
                if (count >= 1000)
                {
                    suffix = "mbps";
                    count = Math.Round(count / 1000, 2);
                }

                return $"{count} {suffix}";
            }
            catch (Exception ex)
            {
                return "Unable to get network transfer...";
            }           
        }

        public static bool IsAdministrator()
        {
            WindowsPrincipal principal = new(WindowsIdentity.GetCurrent());
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static void KillProcessAndChildren(int pid)
        {
            try
            {
                ManagementObjectSearcher searcher = new(@$"Select * From Win32_Process Where ParentProcessID={pid}");
                ManagementObjectCollection objectCollection = searcher.Get();
                foreach (ManagementBaseObject? obj in objectCollection)
                {
                    ManagementObject? managementObject = (ManagementObject) obj;
                    try
                    {
                        KillProcessAndChildren(Convert.ToInt32(managementObject["ProcessID"]));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }

            try
            {
                Process proc = Process.GetProcessById(pid);
                proc.Kill();
            }
            catch (ArgumentException)
            {
                // Process already exited.
            }
        }

        public static string StripIllegalFileNameChars(string str)
        {
            Regex rgx = new("[^a-zA-Z0-9 -]");
            return rgx.Replace(str, "_").Replace(' ', '_');

            // return string.Join("_", str.Split(Path.GetInvalidFileNameChars()));
        }
        
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        public static string RemoveAllNonNumericValuesFromString(string str)
        {
            string nums = Regex.Replace(str, @"[^\d]", string.Empty);
            return string.IsNullOrEmpty(nums) ? "0" : nums;
        }

        public static Process? GetWebResourceHandle(string url, bool useShellExecute = true)
        {
            return Process.Start(new ProcessStartInfo(url) { UseShellExecute = useShellExecute });
        }
        
        public static void WriteJsonToFile(string filepath, object obj)
        {
            File.WriteAllText(filepath, JsonConvert.SerializeObject(obj));
        }

        public static T? GetObjectFromJsonFile<T>(string url)
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(url));
        }

        public static Process? OpenFileExplorer(string directory)
        {
            ProcessStartInfo startInfo = new()
            {
                Arguments = directory,
                FileName = "explorer.exe"
            };

            return Process.Start(startInfo);
        }

        public static Process? OpenTaskManager()
        {
            ProcessStartInfo startInfo = new()
            {
                CreateNoWindow = false, //just hides the window if set to true
                UseShellExecute = true, //use shell (current programs privillage)
                FileName = Path.Combine(Environment.SystemDirectory, "taskmgr.exe"), //The file path and file name
                Arguments = "" //Add your arguments here
            }; //a processstartinfo object

            return Process.Start(startInfo);
        }

        public static void DownloadFile(string url, string localPath)
        {
            if (File.Exists(localPath))
            {
                File.Delete(localPath);
            }
            
            HttpClient client = new();
            HttpResponseMessage response = client.GetAsync(new Uri(url)).Result;

            if (response.IsSuccessStatusCode)
            {
                using FileStream fileStream = new(localPath, FileMode.CreateNew);
                response.Content.CopyToAsync(fileStream).Wait();
            }
            else
            {
                Console.WriteLine(@$"Failed to download {(int)response.StatusCode} ({response.ReasonPhrase})");
            }
        }
    }
}
