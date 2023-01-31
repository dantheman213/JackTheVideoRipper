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
        
        private static readonly Regex _SpaceSplitPattern = new(@"\s+", RegexOptions.Compiled);
        
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

        public static async Task OpenInBrowser(string url)
        {
            if (url.Valid(FileSystem.IsValidUrl))
                await FileSystem.GetWebResourceHandle(url).Run();
        }

        public static async Task OpenFileInMediaPlayer(string filepath)
        {
            if (filepath.Valid(File.Exists))
                await FileSystem.GetWebResourceHandle(filepath).Run();
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
        
        public static string TimeString(float timeInSeconds)
        {
            if (timeInSeconds <= 0.01f)
                return Text.DefaultTime;
            double hours = Math.Floor(timeInSeconds / 360);
            double minutes = Math.Floor(timeInSeconds % 360 / 60);
            double seconds = MathF.Floor(timeInSeconds % 60);
            return $"{hours:00.}:{minutes:00.}:{seconds:00.}";
        }

        #endregion
    }
}
