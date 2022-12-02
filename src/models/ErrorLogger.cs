using JackTheVideoRipper.interfaces;

namespace JackTheVideoRipper.models;

public class ErrorLogger
{
    #region Data Members

    private readonly Dictionary<string, List<ProcessError>> _errorTable = new();

    #endregion

    #region Logging Methods

    public void LogError(string processGuid, Exception exception)
    {
        if (!_errorTable.ContainsKey(processGuid))
        {
            _errorTable[processGuid] = new List<ProcessError>();
        }

        _errorTable[processGuid].Add(new ProcessError(processGuid, exception));
    }
    
    public void LogError(IProcessRunner processRunner, Exception exception)
    {
        string guid = processRunner.Guid.ToString();
        
        if (!_errorTable.ContainsKey(guid))
        {
            _errorTable[guid] = new List<ProcessError>();
        }

        _errorTable[guid].Add(new ProcessError(guid, exception));
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