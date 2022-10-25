using System.Diagnostics;
using JackTheVideoRipper.extensions;

namespace JackTheVideoRipper
{
    internal static class CLI
    {
        public static Process RunCommand(string command, string workingDir = "")
        {
            Process process = CommandProcess(command, workingDir);
            try
            {
                process.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return process;
        }

        public static Process RunYouTubeCommand(string bin, string opts)
        {
            Process process = new();

            if (bin.IsNullOrEmpty())
                return process;
            
            try
            {
                ProcessStartInfo startInfo = new()
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = bin,
                    Arguments = opts,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };
                process.StartInfo = startInfo;
                //process.Start(); // TODO: remove?
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return process;
        }

        public static Process RunElevatedSystemCommand(string command, string workingDir = "")
        {
            Process process = CommandProcess(command, workingDir, true);
            try
            {
                process.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
       
            return process;
        }

        private static Process CommandProcess(string command, string workingDir = "", bool runAsAdmin = false)
        {
            return new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Arguments = $"/C {command}",
                    WorkingDirectory = workingDir.HasValue() ? workingDir : Common.Paths.AppPath,
                    UseShellExecute = runAsAdmin,
                    RedirectStandardError = !runAsAdmin,
                    RedirectStandardOutput = !runAsAdmin,
                    CreateNoWindow = true,
                    Verb = runAsAdmin ? "runas" : ""
                }
            };
        }
    }
}
