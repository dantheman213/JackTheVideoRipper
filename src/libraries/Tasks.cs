namespace JackTheVideoRipper;

public static class Tasks
{
    public static async Task WaitUntil(Func<bool> predicate, int delayInMilliseconds = 300)
    {
        while (!predicate())
        {
            Application.DoEvents();
            await Task.Delay(delayInMilliseconds);
        }
    }
}