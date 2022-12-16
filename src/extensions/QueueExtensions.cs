namespace JackTheVideoRipper.extensions;

public static class QueueExtensions
{
    // https://stackoverflow.com/questions/19141259/how-to-enqueue-a-list-of-items-in-c
    public static void Extend<T>(this Queue<T> queue, IEnumerable<T> enumerable)
    {
        enumerable.ForEach(queue.Enqueue);
    }
}