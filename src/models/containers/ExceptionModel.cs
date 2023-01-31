using Newtonsoft.Json;

namespace JackTheVideoRipper.models;

[Serializable]
public class ExceptionModel
{
    [JsonProperty("message")]
    public string Message;
    
    [JsonProperty("source")]
    public string? Source;
    
    [JsonProperty("caller")]
    public string Caller;
    
    [JsonProperty("type")]
    public string Type;
    
    [JsonProperty("stack_trace")]
    public string? StackTrace;

    [JsonConstructor]
    public ExceptionModel(string message, string? source, string caller, string type, string? stackTrace)
    {
        Message = message;
        Source = source;
        Caller = caller;
        Type = type;
        StackTrace = stackTrace;
    }

    public ExceptionModel(Exception exception)
    {
        Message = exception.Message;
        Source = exception.Source;
        Caller = nameof(exception.TargetSite);
        Type = exception.GetBaseException().GetType().ToString();
        StackTrace = exception.StackTrace;
    }
}