namespace JackTheVideoRipper.extensions;

public static class IEnumerableExtensions
{
    public static string Merge<T>(this IEnumerable<T> enumerable, string separator = "")
    {
        return string.Join(separator, enumerable);
    }
    
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        foreach (T element in enumerable)
        {
            action.Invoke(element);
        }
    }

    public static IEnumerable<T> Reversed<T>(this IEnumerable<T> enumerable)
    {
        return enumerable.ToArray().Reverse();
    }
}