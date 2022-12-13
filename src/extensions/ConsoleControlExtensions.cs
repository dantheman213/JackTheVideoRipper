using JackTheVideoRipper.models;

namespace JackTheVideoRipper.extensions;

public static class ConsoleControlExtensions
{
    private static readonly Color DEFAULT_COLOR = Color.White;

    public static void Write(this ConsoleControl.ConsoleControl consoleControl, string line)
    {
        consoleControl.WriteOutput(line, DEFAULT_COLOR);
    }
    
    public static void WriteLine(this ConsoleControl.ConsoleControl consoleControl, string line = "")
    {
        consoleControl.WriteOutput($"{line}\r\n", DEFAULT_COLOR);
    }
    
    public static void WriteLog(this ConsoleControl.ConsoleControl consoleControl, ProcessLogNode logNode)
    {
        consoleControl.Write("[");
        consoleControl.WriteOutput($"{logNode.Date:G}", Color.Aquamarine);
        if (logNode.LogType != ProcessLogType.Log)
        {
            consoleControl.Write(" | ");
            
            Color color = logNode.LogType switch
            {
                ProcessLogType.Info      => Color.Aqua,
                ProcessLogType.Warning   => Color.LightCoral,
                ProcessLogType.Error     => Color.LightPink,
                ProcessLogType.Exception => Color.IndianRed,
                ProcessLogType.Crash     => Color.DarkRed,
                _                        => Color.White
            };
            
            consoleControl.WriteOutput(logNode.LogType.ToString().ToUpper(), color);
        }
        consoleControl.Write("]: ");
        consoleControl.Write(logNode.Message);
        consoleControl.WriteLine();
    }
}