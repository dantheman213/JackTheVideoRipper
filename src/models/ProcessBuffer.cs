using JackTheVideoRipper.extensions;

namespace JackTheVideoRipper.models;

public class ProcessBuffer
{
    protected readonly List<string> Log = new();
    
    public List<string> Results { get; private set; } = new() { string.Empty }; // Placeholder result
    
    public int Cursor { get; set; } // where in message buffer are we
    
    public bool AtEndOfBuffer => Cursor >= Results.Count - 1;
    
    public string ProcessLine => Cursor < Results.Count ? Results[Cursor] : string.Empty;
    
    public string[] TokenizedProcessLine => Common.Tokenize(ProcessLine);
    
    private int _resultCount;

    public void Update()
    {
        // No new updates to process (or ship to the view)
        if (AtEndOfBuffer)
            return;

        Cursor++;
    }
    
    public void SkipToEnd()
    {
        if (Cursor + 10 < Results.Count)
            Cursor = Results.Count;
    }
    
    public void AppendResult(string? line)
    {
        if (line != null && line.HasValue())
            AddResultLine(line, "Info");
    }

    public void AppendError(string? line)
    {
        if (line != null && line.HasValue())
            AddResultLine(line, "Error");
    }

    public void Clear()
    {
        Log.Clear();
        Results = new List<string> { string.Empty };
        Cursor = 0;
        _resultCount = 0;
    }

    #region Private Methods

    private void AddResultLine(string message, string? header = null)
    {
        Results.Add(message);
        string body = header is null ? $"{message}" : $"[{header.ToUpper()}] {message}";
        Log.Add($"{_resultCount} > {body}");
        _resultCount++;
    }

    #endregion
}