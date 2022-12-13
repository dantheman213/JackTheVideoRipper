using System.Text;
using JackTheVideoRipper.interfaces;

namespace JackTheVideoRipper.models;

public class ProcessParameters<T> : IProcessParameters where T : IProcessParameters
{
    private readonly StringBuilder _buffer = new();

    #region Constructor

    public ProcessParameters()
    {
    }

    public ProcessParameters(string initialParameters)
    {
        Append(initialParameters);
    }

    #endregion

    #region Public Methods

    public T Append(IProcessParameters parameters)
    {
        return Append(parameters.ToString());
    }
    
    public T Append(string parameter)
    {
        _buffer.Append($" {parameter}");
        return (T)(IProcessParameters) this;
    }

    public T Prepend(ProcessParameters<T> parameters)
    {
        return parameters.Append(ToString());
    }

    #endregion
    
    #region Protected Methods
    
    protected T AddNoValue(string parameter)
    {
        _buffer.Append($" {(parameter.Length == 1 ? "-" : "--")}{parameter}");
        return (T)(IProcessParameters) this;
    }
    
    protected T AddNoValue(char parameter)
    {
        _buffer.Append($" -{parameter}");
        return (T)(IProcessParameters) this;
    }
    
    protected T Add(string paramName, object paramValue)
    {
        _buffer.Append($" {(paramName.Length == 1 ? "-" : "--")}{paramName} {paramValue}");
        return (T)(IProcessParameters) this;
    }
    
    protected T Add(char paramChar, object paramValue)
    {
        _buffer.Append($" -{paramChar} {paramValue}");
        return (T)(IProcessParameters) this;
    }

    #endregion

    #region Overrides

    public override string ToString()
    {
        return _buffer.ToString();
    }

    #endregion
}