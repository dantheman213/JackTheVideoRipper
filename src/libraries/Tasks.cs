namespace JackTheVideoRipper;

public static class Tasks
{
    private const int _DEFAULT_TICK = 300;
    
    public static async Task WaitUntil(Func<bool> predicate, int tickInMilliseconds = _DEFAULT_TICK)
    {
        while (!predicate())
        {
            Application.DoEvents();
            await Task.Delay(tickInMilliseconds);
        }
    }

    public static async Task StartAfter(Action action, int tickInMilliseconds = _DEFAULT_TICK)
    {
        await Task.Delay(tickInMilliseconds);
        await Threading.RunInMainContext(action);
    }
}