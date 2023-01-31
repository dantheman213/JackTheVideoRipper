using System.Diagnostics;

namespace JackTheVideoRipper
{
    internal static class CLI
    {
        private static Process CommandProcess(string command, string workingDir = "", bool runAsAdmin = false)
        {
            return FileSystem.CreateProcess(Executables.Command, $"/C {command}", workingDir, runAsAdmin);
        }
        
        public static Process RunElevatedSystemCommand(string command, string workingDir = "")
        {
            return FileSystem.TryStartProcess(CommandProcess(command, workingDir, true));
        }
        
        public static Process RunCommand(string command, string workingDir = "")
        {
            return FileSystem.TryStartProcess(CommandProcess(command, workingDir));
        }
    }
}