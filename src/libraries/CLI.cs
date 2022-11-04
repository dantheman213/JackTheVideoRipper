using System.Diagnostics;

namespace JackTheVideoRipper
{
    internal static class CLI
    {
        private const string _COMMAND_EXECUTABLE = "cmd.exe";
        
        private static Process CommandProcess(string command, string workingDir = "", bool runAsAdmin = false)
        {
            return FileSystem.CreateProcess(_COMMAND_EXECUTABLE, $"/C {command}", workingDir, runAsAdmin);
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