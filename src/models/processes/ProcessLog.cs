using System.Diagnostics;
using JackTheVideoRipper.extensions;
using JackTheVideoRipper.interfaces;
using Newtonsoft.Json;

namespace JackTheVideoRipper.models;

[Serializable]
public class ProcessLog
{
    [JsonProperty("logs")]
    public readonly List<ProcessLogNode> Logs = new();

    [JsonIgnore]
    public readonly List<string> Messages = new();

    public void AddProcessInfo(Process process)
    {
        Add(ProcessLogType.Info, process.GetProcessInfo(), DateTime.Now);
    }
    
    public void Add(ProcessLogNode logNode)
    {
        Logs.Add(logNode);
        Messages.Add(logNode.ToString());
    }

    public void Add(ProcessLogType logType, string message, DateTime date)
    {
        Add(new ProcessLogNode(logType, message, date));
    }

    public void Clear()
    {
        Logs.Add(new ProcessLogNode(ProcessLogType.Info, "Process log was cleared", DateTime.Now));
        Messages.Clear();
    }
}

[Serializable]
public readonly struct ProcessLogNode : ILogNode
{
    public readonly ProcessLogType LogType;
    public readonly string Message;
    public readonly DateTime Date;

    [JsonConstructor]
    public ProcessLogNode(ProcessLogType logType, string message, DateTime date)
    {
        LogType = logType;
        Message = message;
        Date = date;
    }

    public override string ToString()
    {
        return LogType == ProcessLogType.Log ?
            $"[{Date:G}]: {Message}" :
            $"[{Date:G} | {LogType.ToString().ToUpper()}]: {Message}";
    }

    public IReadOnlyList<ConsoleLine> Serialize()
    {
        List<ConsoleLine> list = new()
        {
            new ConsoleLine("["),
            new ConsoleLine($"{Date:G}") { Color = Color.Aquamarine }
        };

        if (LogType != ProcessLogType.Log)
        {
            list.Add(new ConsoleLine(" | "));
            
            Color color = LogType switch
            {
                ProcessLogType.Info      => Color.Aqua,
                ProcessLogType.Warning   => Color.LightCoral,
                ProcessLogType.Error     => Color.LightPink,
                ProcessLogType.Exception => Color.IndianRed,
                ProcessLogType.Crash     => Color.DarkRed,
                _                        => Color.White
            };
            
            list.Add(new ConsoleLine(LogType.ToString().ToUpper()){Color = color });
        }
        
        list.Add(new ConsoleLine("]: "));
        list.Add(new ConsoleLine(Message, true));

        return list;
    }
}

[Serializable]
public enum ProcessLogType
{
    Log,
    Info,
    Warning,
    Error,
    Exception,
    Critical,
    Crash
}