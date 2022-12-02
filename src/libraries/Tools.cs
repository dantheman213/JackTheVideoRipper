using System.Diagnostics;

namespace JackTheVideoRipper;

public static class Tools
{
    public static void RunBenchmark(Action action1, Action action2)
    {
        Stopwatch stopwatch = new();
            
        stopwatch.Restart();
        action1();
        stopwatch.Stop();

        long result1 = stopwatch.ElapsedMilliseconds;
            
        stopwatch.Restart();
        action2();
        stopwatch.Stop();

        long result2 = stopwatch.ElapsedMilliseconds;
            
        Modals.Notification($"Method 1 in {result1:F3} ms | Method 2 in {result2:F3} ms");
    }
}