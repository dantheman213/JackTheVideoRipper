using System.Diagnostics;
using JackTheVideoRipper.extensions;

namespace JackTheVideoRipper;

public static class Statistics
{
    private static PerformanceCounter? _cpuCounter;
    private static PerformanceCounter? _ramCounter;
    private static List<PerformanceCounter>? _networkCounters;

    public static void InitializeCounters()
    {
        
    }
    
    public static string GetCpuUsagePercentage()
    {
        if (_cpuCounter is not null) 
            return $"{_cpuCounter.NextValue():0.00}%";
        
        try
        {
            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        }
        catch (Exception ex)
        {
            // TBA
            Console.WriteLine(ex);
            return Tags.NOT_APPLICABLE;
        }
        
        return $"{_cpuCounter.NextValue():0.00}%";
    }
    
    public static string GetAvailableMemory()
    {
        if (_ramCounter != null)
            return $"{_ramCounter.NextValue()} MB";
        
        try
        {
            _ramCounter = new PerformanceCounter("Memory", "% Available Megabytes");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return Tags.NOT_APPLICABLE;
        }
        
        return $"{_ramCounter.NextValue()} MB";
    }
    
    public static string GetNetworkTransfer()
    {
        try
        {
            // get the network transfer in kbps or mbps automatically

            _networkCounters ??= new List<PerformanceCounter>();

            if (!_networkCounters.Any())
            {
                PerformanceCounterCategory category = new("Network Interface");
                
                category.GetInstanceNames().ForEach(instance => 
                    _networkCounters.Add(new PerformanceCounter("Network Interface", "Bytes Received/sec", instance)));
            }

            double count = _networkCounters.Sum(counter => Math.Round(counter.NextValue() / 1024, 2));

            string suffix = "kbps";
            
            if (count >= 1000)
            {
                suffix = "mbps";
                count = Math.Round(count / 1000, 2);
            }

            return $"{count} {suffix}";
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return Tags.NOT_APPLICABLE;
        }           
    }
}