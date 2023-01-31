using JackTheVideoRipper.interfaces;

namespace JackTheVideoRipper.models;

public readonly struct LogNode : ILogNode
{
    public readonly DateTime DateTime;
    public readonly string Message;
    public readonly Color Color;

    public LogNode(DateTime dateTime, string message, Color color)
    {
        DateTime = dateTime;
        Message = message;
        Color = color;
    }

    public IReadOnlyList<ConsoleLine> Serialize()
    {
        return new[]
        {
            new ConsoleLine($"[{DateTime:G}]: {Message}") { Color = Color }   
        };
    }
}