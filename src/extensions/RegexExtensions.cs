using System.Text.RegularExpressions;

namespace JackTheVideoRipper.extensions;

public static class RegexExtensions
{
    public static string Remove(this Regex regex, string element)
    {
        return regex.Replace(element, string.Empty);
    }
}