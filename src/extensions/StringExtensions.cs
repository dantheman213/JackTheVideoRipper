namespace JackTheVideoRipper.extensions;

public static class StringExtensions
{
    private static bool Contains(this string line, string word)
    {
        return line.IndexOf(word, StringComparison.Ordinal) > -1;
    }

    public static string Before(this string str, string element)
    {
        return str[..str.IndexOf(element, StringComparison.Ordinal)];
    }

    public static string After(this string str, string element)
    {
        return str[(str.IndexOf(element, StringComparison.Ordinal) + 1)..];
    }
    
    public static string BeforeLast(this string str, string element)
    {
        return str[..str.LastIndexOf(element, StringComparison.Ordinal)];
    }

    public static string AfterLast(this string str, string element)
    {
        return str[(str.LastIndexOf(element, StringComparison.Ordinal) + 1)..];
    }

    public static bool HasValue(this string? str)
    {
        return !string.IsNullOrEmpty(str);
    }
    
    public static bool IsNullOrEmpty(this string? str)
    {
        return string.IsNullOrEmpty(str);
    }

    public static string Remove(this string str, string element)
    {
        return str.Replace(element, string.Empty);
    }
}