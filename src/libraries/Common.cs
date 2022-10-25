using System.Diagnostics;
using System.Management;
using System.Security.Principal;
using System.Text.RegularExpressions;
using JackTheVideoRipper.extensions;

namespace JackTheVideoRipper
{
    internal static class Common
    {
        public const string PROGRAM_NAME = "JackTheVideoRipper";
        
        private static readonly Random _random = new();

        public static class Paths
        {
            public static string AppPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName) ?? string.Empty;
            public static readonly string Local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            public static readonly string Common = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            public static readonly string Root = Path.Combine(Common, PROGRAM_NAME);
            public static readonly string Install = Path.Combine(Root, "bin");
            public static readonly string Settings = Path.Combine(Local, PROGRAM_NAME);
        }

        private static readonly Regex _urlPattern = new(@"^(http|http(s)?://)?([\w-]+\.)+[\w-]+[.com|.in|.org]+(\[\?%&=]*)?",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
        
        private static readonly Regex TitlePattern = new("[^a-zA-Z0-9]", RegexOptions.Compiled);

        public static string GetAppVersion()
        {
            return $"v{FileSystem.VersionInfo}";
        }

        public static bool IsValidUrl(string url)
        {
            return _urlPattern.IsMatch(url);
        }

        public static string FormatTitleForFileName(string title)
        {
            return title.HasValue() ? TitlePattern.Remove(title).Trim() : string.Empty;
        }

        public static bool IsAdministrator()
        {
            return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        }

        private static string GetProcessQuery(int pid) => @$"Select * From Win32_Process Where ParentProcessID={pid}";

        public static void KillProcessAndChildren(int pid)
        {
            try
            {
                ManagementObjectSearcher searcher = new(GetProcessQuery(pid));
                foreach (ManagementObject managementObject in searcher.Get().Cast<ManagementObject>())
                {
                    try { KillProcessAndChildren(Convert.ToInt32(managementObject["ProcessID"])); }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void KillProcess(int pid)
        {
            Process process = Process.GetProcessById(pid);
            if (process.HasExited)
                return;
            
            try { process.Kill(); }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex);
            }
        }
        
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        private static readonly Regex NumericPattern = new Regex(@"[^\d]", RegexOptions.Compiled);

        public static string RemoveAllNonNumericValuesFromString(string str)
        {
            string nums = NumericPattern.Remove(str);
            return nums.HasValue() ? nums : "0";
        }

        public static string TimeStampDate => DateTime.Now.ToString("yyyyMMddhmmsstt");

        public static readonly string TempPath = Path.GetTempPath();
        
        public static string GetTempFilename(string ext)
        {
            return $"{TempPath}jtvr_thumbnail_{TimeStampDate}.{ext}";
        }
        
        public const string DO_NOT_TRANSCODE = "(do not transcode)";
    }
}
