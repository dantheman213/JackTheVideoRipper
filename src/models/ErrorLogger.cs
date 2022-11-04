namespace JackTheVideoRipper.models;

public class ErrorLogger
{
    private readonly Dictionary<string, List<ProcessError>> _errorTable = new();
    
    #region Logging Methods

    private void InitializeLogging()
    {
        ProcessUpdateRow.ErrorLogEvent_Tag += LogError;
        ProcessUpdateRow.ErrorLogEvent_Error += LogError;
    }

    public void LogError(string processTag, Exception exception)
    {
        if (!_errorTable.ContainsKey(processTag))
        {
            _errorTable[processTag] = new List<ProcessError>();
        }

        _errorTable[processTag].Add(new ProcessError(processTag, exception));
    }
    
    public void LogError(ProcessError error)
    {
        if (!_errorTable.ContainsKey(error.ProcessTag))
        {
            _errorTable[error.ProcessTag] = new List<ProcessError>();
        }

        _errorTable[error.ProcessTag].Add(error);
    }

    #endregion
}