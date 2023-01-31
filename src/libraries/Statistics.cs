using System.Diagnostics;
using JackTheVideoRipper.extensions;
using JackTheVideoRipper.models;

namespace JackTheVideoRipper;

public static class Statistics
{
    private static readonly Stopwatch _StartupTimer = new();
    
    private static PerformanceCounter? _cpuCounter;
    private static PerformanceCounter? _ramCounter;
    private static readonly List<PerformanceCounter> _NetworkCounters = new();

    private static readonly Notification _InitializedNotification = new(Messages.CountersInitialized, typeof(Statistics));

    public static void BeginStartup()
    {
        _StartupTimer.Start();
    }

    public static void EndStartup()
    {
        _StartupTimer.Stop();
    }

    public static string StartupMessage => string.Format(Notifications.StartupTime, _StartupTimer.ElapsedMilliseconds);

    private static readonly Task[] _CounterInitializationTasks =
    {
        Task.Run(InitializeCpuCounter),
        Task.Run(InitializeMemoryCounter),
        Task.Run(InitializeNetworkTrackers)
    };

    public static async Task InitializeCounters()
    {
        await Task.WhenAll(_CounterInitializationTasks).ContinueWith(_ =>
        {
            NotificationsManager.SendNotification(_InitializedNotification);
        });
    }

    private static void InitializeCpuCounter()
    {
        _cpuCounter = InitializePerformanceCounter("Processor", "% Processor Time", "_Total");
    }

    private static void InitializeMemoryCounter()
    {
        _ramCounter = InitializePerformanceCounter("Memory", "Available MBytes");
    }

    private static PerformanceCounter? InitializePerformanceCounter(string categoryName, string counterName, 
        string instanceName = "")
    {
        try
        {
            return instanceName.HasValue() ?
                new PerformanceCounter(categoryName, counterName, instanceName) :
                new PerformanceCounter(categoryName, counterName);
        }
        catch (Exception ex)
        {
            FileSystem.LogException(ex);
            return null;
        }
    }

    private static void InitializeNetworkTrackers()
    {
        try
        {
            PerformanceCounterCategory category = new("Network Interface");
            Parallel.ForEach(category.GetInstanceNames(), instance =>
            {
                PerformanceCounter counter = new(category.CategoryName, "Bytes Received/sec", instance);
                _NetworkCounters.Add(counter);
            });
        }
        catch (Exception ex)
        {
            FileSystem.LogException(ex);
        }
    }
    
    public static string GetCpuUsagePercentage()
    {
        return _cpuCounter is not null ? $"{_cpuCounter.NextValue():0.00}%" : Text.NotApplicable;
    }
    
    public static string GetAvailableMemory()
    {
        return _ramCounter is not null ? GetFormattedSize(_ramCounter.NextValue()*1E6) : Text.NotApplicable;
    }
    
    public static string GetNetworkTransfer()
    {
        return _NetworkCounters.Any() ? CalculateNetworkUsage() : Text.NotApplicable;
    }

    private static string CalculateNetworkUsage()
    {
        return $"{GetFormattedSize(NetworkUsage)}/s";
    }

    private static double NetworkUsage => _NetworkCounters.Sum(counter => counter.NextValue());

    private static double RoundBytes(double initialValue)
    {
        return Math.Round(initialValue / 1024, 2);
    }

    private static string GetFormattedSize(double value)
    {
        return FileSystem.GetFileSizeFormatted((long) Math.Floor(value), 2);
    }

    public static class Toolbar
    {
        public static string ToolbarStatus => $"{Ripper.Instance.GetProgramStatus(),-20}"; // 20 chars

        public static string ToolbarCpu => $@"CPU: {GetCpuUsagePercentage(),7}"; // 12 chars

        public static string ToolbarMemory => $@"Available Memory: {GetAvailableMemory(),9}"; // 27 chars

        public static string ToolbarNetwork => $@"Network Usage: {GetNetworkTransfer(),10}"; // 25 chars
    }
}