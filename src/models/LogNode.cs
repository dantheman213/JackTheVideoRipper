namespace JackTheVideoRipper.models;

public readonly struct LogNode
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
}