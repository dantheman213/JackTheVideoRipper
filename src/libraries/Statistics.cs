using System.Diagnostics;
using JackTheVideoRipper.extensions;
using JackTheVideoRipper.models;

namespace JackTheVideoRipper;

public static class Statistics
{
    private static PerformanceCounter? _cpuCounter;
    private static PerformanceCounter? _ramCounter;
    private static readonly List<PerformanceCounter> _NetworkCounters = new();

    private static readonly Notification _InitializedNotification = new("Counters successfully initialized",
        typeof(Statistics));

    public static async Task InitializeCounters()
    {
        Task[] tasks =
        {
            Task.Run(InitializeCpuCounter),
            Task.Run(InitializeMemoryCounter),
            Task.Run(InitializeNetworkTrackers)
        };

        await Task.WhenAll(tasks).ContinueWith(_ =>
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

    private static PerformanceCounter? InitializePerformanceCounter(string categoryName, string counterName, string instanceName = "")
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
                _NetworkCounters.Add(new PerformanceCounter(category.CategoryName, "Bytes Received/sec", instance));
            });
        }
        catch (Exception ex)
        {
            FileSystem.LogException(ex);
        }
    }
    
    public static string GetCpuUsagePercentage()
    {
        return _cpuCounter is not null ? $"{_cpuCounter.NextValue():0.00}%" : Text.NOT_APPLICABLE;
    }
    
    public static string GetAvailableMemory()
    {
        return _ramCounter is not null ? GetFormattedSize(_ramCounter.NextValue()*1E6) : Text.NOT_APPLICABLE;
    }
    
    public static string GetNetworkTransfer()
    {
        return _NetworkCounters.Any() ? CalculateNetworkUsage() : Text.NOT_APPLICABLE;
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
        return FileSystem.GetSizeWithSuffix((long) Math.Floor(value), 2);
    }
}