using System.Diagnostics;
using JackTheVideoRipper.extensions;

namespace JackTheVideoRipper;

public static class Statistics
{
    private static PerformanceCounter? _cpuCounter;
    private static PerformanceCounter? _ramCounter;
    private static readonly List<PerformanceCounter> _NetworkCounters = new();

    public static void InitializeCounters()
    {
        _cpuCounter = InitializePerformanceCounter("Processor", "% Processor Time", "_Total");
        _ramCounter = InitializePerformanceCounter("Memory", "% Available Megabytes");
        InitializeNetworkTrackers();
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
            Console.WriteLine(ex);
            return null;
        }
    }

    private static void InitializeNetworkTrackers()
    {
        try
        {
            PerformanceCounterCategory category = new("Network Interface");
            category.GetInstanceNames().ForEach(instance =>
            {
                _NetworkCounters.Add(new PerformanceCounter(category.CategoryName, "Bytes Received/sec", instance));
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
    
    public static string GetCpuUsagePercentage()
    {
        return _cpuCounter is not null ? $"{_cpuCounter.NextValue():0.00}%" : Tags.NOT_APPLICABLE;
    }
    
    public static string GetAvailableMemory()
    {
        return _ramCounter is not null ? $"{_ramCounter.NextValue()} MB" : Tags.NOT_APPLICABLE;
    }
    
    public static string GetNetworkTransfer()
    {
        return _NetworkCounters.Any() ? CalculateNetworkUsage() : Tags.NOT_APPLICABLE;
    }

    private static string CalculateNetworkUsage()
    {
        double count = NetworkUsage;
        return $"{(count >= 1024 ? RoundBytes(count) : count)} {(count >= 1024 ? "mbps" : "kbps")}";
    }

    private static double NetworkUsage => _NetworkCounters.Sum(counter => RoundBytes(counter.NextValue()));

    private static double RoundBytes(double initialValue)
    {
        return Math.Round(initialValue / 1024, 2);
    }
}