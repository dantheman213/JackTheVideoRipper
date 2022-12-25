using System.Management;

namespace JackTheVideoRipper.extensions;

public static class ManagementObjectExtensions
{
    public static int GetProcessId(this ManagementObject manager)
    {
        return Convert.ToInt32(manager["ProcessID"]);
    }

    public static void TryKillProcessAndChildren(this ManagementObject manager)
    {
        FileSystem.TryKillProcessAndChildren(manager.GetProcessId());
    }
}