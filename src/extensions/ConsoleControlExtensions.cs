using JackTheVideoRipper.interfaces;
using JackTheVideoRipper.models;

namespace JackTheVideoRipper.extensions;

public static class ConsoleControlExtensions
{
    private static readonly Color DEFAULT_COLOR = Color.White;

    public static void Write(this ConsoleControl.ConsoleControl consoleControl, string line)
    {
        if (!consoleControl.Visible)
            return;
        consoleControl.WriteOutput(line, DEFAULT_COLOR);
    }
    
    public static void WriteLine(this ConsoleControl.ConsoleControl consoleControl, string line = "")
    {
        if (!consoleControl.Visible)
            return;
        consoleControl.WriteOutput($"{line}\r\n", DEFAULT_COLOR);
    }
    
    public static void WriteLog(this ConsoleControl.ConsoleControl consoleControl, ILogNode logNode)
    {
        if (!consoleControl.Visible)
            return;
        foreach (ConsoleLine consoleLine in logNode.Serialize())
        {
            consoleControl.WriteOutput(consoleLine.Linebreak ? consoleLine.Message : $"{consoleLine.Message}\r\n",
                consoleLine.Color ?? DEFAULT_COLOR);
        }
    }

    public static void WriteLog(this ConsoleControl.ConsoleControl consoleControl, IEnumerable<ConsoleLine> consoleLines)
    {
        if (!consoleControl.Visible)
            return;
        foreach (ConsoleLine consoleLine in consoleLines)
        {
            consoleControl.WriteOutput(consoleLine.Linebreak ? consoleLine.Message : $"{consoleLine.Message}\r\n",
                consoleLine.Color ?? DEFAULT_COLOR);
        }
    }
}