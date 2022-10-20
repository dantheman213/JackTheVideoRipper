using System.Diagnostics;

namespace JackTheVideoRipper
{
    internal static class CLI
    {
        public static Process RunCommand(string command, string workingDir = "")
        {
            Process process = new();
            try
            {
                ProcessStartInfo startInfo = new()
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Arguments = "/C " + command,
                    WorkingDirectory = workingDir != "" ? workingDir : Common.AppPath,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };
                process.StartInfo = startInfo;
                process.Start();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }

            return process;
        }

        public static Process RunYouTubeCommand(string bin, string opts)
        {
            Process process = new();

            if (string.IsNullOrEmpty(bin))
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
            Process process = new();
            try
            {
                ProcessStartInfo startInfo = new()
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Arguments = $"/C {command}",
                    WorkingDirectory = workingDir != "" ? workingDir : Common.AppPath,
                    UseShellExecute = true,
                    CreateNoWindow = true,
                    Verb = "runas"
                };
                process.StartInfo = startInfo;
                process.Start();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
       
            return process;
        }
    }
}
