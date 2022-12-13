using System.Text.RegularExpressions;
using JackTheVideoRipper.extensions;

namespace JackTheVideoRipper
{
    internal static class Common
    {
        #region Properties

        private static readonly Random _Random = new();

        private static readonly Regex _TitlePattern = new("[^a-zA-Z0-9]", RegexOptions.Compiled);
        
        private static readonly Regex _NumericPattern = new(@"[^\d]", RegexOptions.Compiled);
        
        private static readonly Regex _SpaceSplitPattern = new(@"\s+");
        
        private const string _CHARACTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        #endregion

        #region Public Methods

        public static string GetAppVersion()
        {
            return $"v{FileSystem.VersionInfo}";
        }

        public static string FormatTitleForFileName(string title)
        {
            return _TitlePattern.Remove(title).Trim().ValueOrDefault();
        }

        public static string RandomString(int length)
        {
            return Enumerable.Repeat(_CHARACTERS, length).Select(s => s[_Random.Next(s.Length)]).Merge();
        }

        public static string RemoveAllNonNumericValuesFromString(string str)
        {
            return _NumericPattern.Remove(str).ValueOrDefault("0");
        }

        public static void OpenInBrowser(string url)
        {
            if (url.HasValue())
            {
                FileSystem.GetWebResourceHandle(url);
            }
        }

        public static void OpenFileInMediaPlayer(string filepath)
        {
            if (filepath.Valid(File.Exists))
            {
                FileSystem.GetWebResourceHandle(filepath);
            }
        }
        
        public static void RepeatInvoke(Action action, int n, int sleepTime = 300)
        {
            for (int i = 0; i < n; i++)
            {
                Application.DoEvents();
                Thread.Sleep(sleepTime);
                action.Invoke();
            }
        }

        public static string[] Tokenize(string line)
        {
            return _SpaceSplitPattern.Split(line);
        }

        public static string CreateTag()
        {
            //return $"{RandomString(5)}{DateTime.UtcNow.Ticks}";
            return Guid.NewGuid().ToString();
        }

        #endregion
    }
}
