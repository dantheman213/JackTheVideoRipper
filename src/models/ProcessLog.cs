using System.Diagnostics;
using JackTheVideoRipper.extensions;
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
public readonly struct ProcessLogNode
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