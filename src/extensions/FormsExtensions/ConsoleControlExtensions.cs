using JackTheVideoRipper.interfaces;
using JackTheVideoRipper.models;

namespace JackTheVideoRipper.extensions;

public static class ConsoleControlExtensions
{
    private static readonly Color DEFAULT_COLOR = Color.White;

    public static void Write(this ConsoleControl.ConsoleControl consoleControl, string line, Color? color = null)
    {
        if (!consoleControl.Visible)
            return;
        consoleControl.WriteOutput(line, color ?? DEFAULT_COLOR);
    }
    
    public static void WriteLine(this ConsoleControl.ConsoleControl consoleControl, string line = "", Color? color = null)
    {
        if (!consoleControl.Visible)
            return;
        consoleControl.WriteOutput($"{line}\r\n", color ?? DEFAULT_COLOR);
    }
    
    public static void WriteLog(this ConsoleControl.ConsoleControl consoleControl, ILogNode logNode)
    {
        if (!consoleControl.Visible)
            return;
        logNode.Serialize().WriteToConsole(consoleControl);
    }

    public static void WriteToConsole(this IEnumerable<ConsoleLine> consoleLines, ConsoleControl.ConsoleControl consoleControl)
    {
        consoleLines.ForEach(consoleLine => consoleLine.WriteToConsole(consoleControl));
    }

    public static void WriteLog(this ConsoleControl.ConsoleControl consoleControl, IEnumerable<ConsoleLine> consoleLines)
    {
        if (!consoleControl.Visible)
            return;
        consoleLines.ForEach(consoleLine => consoleLine.WriteToConsole(consoleControl));
    }
}