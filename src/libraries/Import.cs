using System.Text.RegularExpressions;

namespace JackTheVideoRipper
{
    internal static class Import
    {
        private static readonly Regex _UrlPattern =
            new(@"(http|ftp|https):\/\/([\w\-_]+(?:(?:\.[\w\-_]+)+))([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?", RegexOptions.Compiled);
        
        public static IEnumerable<string> GetAllUrlsFromPayload(string s)
        {
            return _UrlPattern.Matches(s).Select(i => i.Value);
        }
    }
}
