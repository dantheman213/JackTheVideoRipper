using JackTheVideoRipper.extensions;

namespace JackTheVideoRipper.models;

public struct ConsoleLine
{
    public readonly string Message;
    public Color? Color = null;
    public readonly bool Linebreak;

    public ConsoleLine(string message, bool linebreak = false)
    {
        Message = message;
        Linebreak = linebreak;
    }

    public void WriteToConsole(ConsoleControl.ConsoleControl consoleControl)
    {
        if (Linebreak)
        {
            consoleControl.WriteLine(Message, Color);
        }
        else
        {
            consoleControl.Write(Message, Color);
        }
    }
}