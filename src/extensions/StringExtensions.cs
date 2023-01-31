using System.ComponentModel;
using System.Text.RegularExpressions;

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
    
    public static string Before(this string str, char element)
    {
        return str.Contains(element) ? str[..str.IndexOf(element, StringComparison.Ordinal)] : str;
    }

    public static string After(this string str, string element)
    {
        return str.Contains(element) ? str[(str.IndexOf(element, StringComparison.Ordinal) + element.Length)..] : str;
    }
    
    public static string After(this string str, char element)
    {
        return str.Contains(element) ? str[(str.IndexOf(element, StringComparison.Ordinal) + 1)..] : str;
    }
    
    public static string BeforeLast(this string str, string element)
    {
        return str.Contains(element) ? str[..str.LastIndexOf(element, StringComparison.Ordinal)] : str;
    }
    
    public static string BeforeLast(this string str, char element)
    {
        return str.Contains(element) ? str[..str.LastIndexOf(element)] : str;
    }

    public static string AfterLast(this string str, string element)
    {
        return str.Contains(element) ? str[(str.LastIndexOf(element, StringComparison.Ordinal) + element.Length)..] : str;
    }
    
    public static string AfterLast(this string str, char element)
    {
        return str.Contains(element) ? str[(str.LastIndexOf(element) + 1)..] : str;
    }

    public static bool HasValue(this string? str)
    {
        return !string.IsNullOrEmpty(str);
    }

    public static string Between(this string str, string start, string end)
    {
        return str.After(start).Before(end);
    }
    
    public static string Between(this string str, string enclosingString)
    {
        return str.AfterFirst(enclosingString).BeforeLast(enclosingString);
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
    
    public static string? ValidOrDefault(this string? str, Func<string, bool> predicate, string? defaultValue = default)
    {
        return str.Valid(predicate) ? str : defaultValue;
    }

    public static bool Invalid(this string? str, Func<string, bool> isValidPredicate)
    {
        return str.IsNullOrEmpty() || !isValidPredicate(str!);
    }
    
    public static string? InvalidOrDefault(this string? str, Func<string, bool> predicate, string? defaultValue = default)
    {
        return str.Invalid(predicate) ? str : defaultValue;
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
    
    public static string Wrap(this string str, string prefix, string suffix)
    {
        return $"{prefix}{str}{suffix}";
    }
    
    public static string Wrap(this string str, string wrapper)
    {
        return $"{wrapper}{str}{wrapper}";
    }

    public static string WrapQuotes(this string str)
    {
        return str.Wrap("\"");
    }
    
    public static bool ContainsLetter(this string str)
    {
        return str.Any(char.IsLetter);
    }

    public static bool ContainsNumber(this string str)
    {
        return str.Any(char.IsDigit);
    }
    
    public static bool ContainsSymbol(this string str)
    {
        return str.Any(char.IsSymbol);
    }

    private static readonly char[] _Numbers =
    {
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
    };

    private static readonly char[] _Letters =
    {
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v',
            'w', 'x', 'y', 'z',
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V',
            'W', 'X', 'Y', 'Z'
    };

    public static string BeforeFirstNumber(this string str, bool inclusive = false)
    {
        return str.ContainsLetter() ? str[..(str.IndexOfAny(_Numbers) + (inclusive ? 1 : 0))] : str;
    }
    
    public static string BeforeLastNumber(this string str, bool inclusive = false)
    {
        return str.ContainsNumber() ? str[..(str.Length - str.LastIndexOfAny(_Numbers) - 2)] : str;
    }

    public static string BeforeFirstLetter(this string str, bool inclusive = false)
    {
        return str.ContainsLetter() ? str[..(str.IndexOfAny(_Letters) + (inclusive ? 1 : 0))] : str;
    }
    
    public static string BeforeLastLetter(this string str, bool inclusive = false)
    {
        return str.ContainsLetter() ? str[..(str.Length - str.LastIndexOfAny(_Letters) - 2)] : str;
    }
    
    public static string AfterFirstNumber(this string str, bool inclusive = false)
    {
        return str.ContainsNumber() ? str[str.IndexOfAny(_Numbers)..] : str;
    }
    
    public static string AfterLastNumber(this string str, bool inclusive = false)
    {
        return str.ContainsNumber() ? str[str.LastIndexOfAny(_Numbers)..] : str;
    }

    public static string AfterFirstLetter(this string str, bool inclusive = false)
    {
        return str.ContainsLetter() ? str[str.IndexOfAny(_Letters)..] : str;
    }
    
    public static string AfterLastLetter(this string str, bool inclusive = false)
    {
        return str.ContainsLetter() ? str[str.LastIndexOfAny(_Letters)..] : str;
    }
    
    public static string AfterFirst(this string str, string element, bool inclusive = false)
    {
        return str.Contains(element) ? str[str.IndexOf(element, StringComparison.Ordinal)..] : str;
    }
    
    public static string BeforeFirst(this string str, string element, bool inclusive = false)
    {
        return str.Contains(element) ? str[..(str.Length - str.IndexOf(element, StringComparison.Ordinal) - 2)] : str;
    }
    
    public static T? Convert<T>(this string input)
    {
        try
        {
            return TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(input) is T output ? output : default;
        }
        catch (NotSupportedException)
        {
            return default;
        }
    }

    private static readonly Regex _SplitPattern1 = new(@"(\P{Ll})(\P{Ll}\p{Ll})", RegexOptions.Compiled);
    
    private static readonly Regex _SplitPattern2 = new(@"(\p{Ll})(\P{Ll})", RegexOptions.Compiled);

    private const string _SPLIT_REPLACEMENT = "$1 $2";

    // https://stackoverflow.com/questions/5796383/insert-spaces-between-words-on-a-camel-cased-token
    public static string SplitCamelCase(this string str)
    {
        return _SplitPattern2.Replace(_SplitPattern1.Replace(str, _SPLIT_REPLACEMENT), _SPLIT_REPLACEMENT);
    }
    
    private static readonly Regex _UnderScorePattern = new(@"_", RegexOptions.Compiled);

    public static string ReplaceUnderscore(this string str)
    {
        return _UnderScorePattern.Replace(str, " ");
    }
    
    private static readonly Regex _RemoveMultiSpacePattern = new(@"\s\s+", RegexOptions.Compiled);
    
    public static string RemoveMultiSpace(this string str)
    {
        return _RemoveMultiSpacePattern.Replace(str, " ");
    }
}