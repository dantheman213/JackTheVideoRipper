using System.Text.RegularExpressions;

namespace JackTheVideoRipper
{
    internal static class Import
    {
        private static readonly Regex urlPattern =
            new(@"(http|ftp|https):\/\/([\w\-_]+(?:(?:\.[\w\-_]+)+))([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?", RegexOptions.Compiled);
        
        public static IEnumerable<string> GetAllUrlsFromPayload(string s)
        {
            return urlPattern.Matches(s).Select(i => i.Value);
        }
    }
}
