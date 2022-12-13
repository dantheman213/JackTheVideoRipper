namespace JackTheVideoRipper.extensions;

public static class StringExtensions
{
    private static bool Contains(this string line, string word)
    {
        return line.IndexOf(word, StringComparison.Ordinal) > -1;
    }

    public static string Before(this string str, string element)
    {
        return str.Contains(element) ? str[..str.IndexOf(element, StringComparison.Ordinal)] : str;
    }

    public static string After(this string str, string element)
    {
        return str.Contains(element) ? str[(str.IndexOf(element, StringComparison.Ordinal) + element.Length)..] : str;
    }
    
    public static string BeforeLast(this string str, string element)
    {
        return str.Contains(element) ? str[..str.LastIndexOf(element, StringComparison.Ordinal)] : str;
    }

    public static string AfterLast(this string str, string element)
    {
        return str.Contains(element) ? str[(str.LastIndexOf(element, StringComparison.Ordinal) + element.Length)..] : str;
    }

    public static bool HasValue(this string? str)
    {
        return !string.IsNullOrEmpty(str);
    }
    
    public static bool HasValueAndNot(this string? str, string other)
    {
        return !string.IsNullOrEmpty(str) && !string.Equals(str, other);
    }
    
    public static bool HasValueAndNotIgnoreCase(this string? str, string other)
    {
        return !string.IsNullOrEmpty(str) && !string.Equals(str, other, StringComparison.OrdinalIgnoreCase);
    }
    
    public static bool HasValueAndNot(this string? str, params string[] others)
    {
        return !string.IsNullOrEmpty(str) && others.All(other => !string.Equals(str, other));
    }
    
    public static bool HasValueAndNotIgnoreCase(this string? str, params string[] others)
    {
        return !string.IsNullOrEmpty(str) && others.All(other => !string.Equals(str, other, StringComparison.OrdinalIgnoreCase));
    }

    public static string EvaluateOrDefault(this string? str, Func<string, string> func, string defaultValue = "")
    {
        return str.HasValue() ? func(str!) : defaultValue;
    }
    
    public static void EvaluateIfValue(this string? str, Action<string> action)
    {
        if (str.HasValue())
            action(str!);
    }
    
    public static bool IsNullOrEmpty(this string? str)
    {
        return string.IsNullOrEmpty(str);
    }

    public static bool Valid(this string? str, Func<string, bool> predicate)
    {
        return str.HasValue() && predicate(str!);
    }

    public static bool Invalid(this string? str, Func<string, bool> isValidPredicate)
    {
        return str.IsNullOrEmpty() || !isValidPredicate(str!);
    }

    public static string Remove(this string str, string element, StringComparison? stringComparison = null)
    {
        return stringComparison is null ? str.Replace(element, string.Empty) :
            str.Replace(element, string.Empty, (StringComparison) stringComparison);
    }

    public static string RemoveAll(this string str, params string[] elements)
    {
        elements.ForEach(e => str.Remove(e));
        return str;
    }

    public static string ValueOrDefault(this string? str, string defaultValue = "")
    {
        return str.HasValue() ? str! : defaultValue;
    }

    public static string FormattedOrDefault(this string? str, string formatString, string defaultValue = "")
    {
        return str.HasValue() ? string.Format(formatString, str) : defaultValue;
    }

    public static string Truncate(this string str, int length)
    {
        return str.Length <= length ? str : str[..length];
    }
    
    public static string TruncateEllipse(this string str, int length)
    {
        return str.Length <= length ? str : length <= 3 ? str[..length] : $"{str[..(length - 3)]}...";
    }

    public static IEnumerable<string> SplitNewline(this string str, StringSplitOptions options = StringSplitOptions.None)
    {
        return str.Split("\n", options);
    }
    
    public static IEnumerable<string> SplitReturn(this string str, StringSplitOptions options = StringSplitOptions.None)
    {
        return str.Split("\r\n", options);
    }

    public static string WrapQuotes(this string str)
    {
        return $"\"{str}\"";
    }
}