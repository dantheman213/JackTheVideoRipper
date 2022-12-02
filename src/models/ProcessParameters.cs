using System.Text;
using JackTheVideoRipper.interfaces;

namespace JackTheVideoRipper.models;

public class ProcessParameters : IProcessParameters
{
    private readonly StringBuilder _buffer = new();

    #region Constructor

    public ProcessParameters()
    {
    }

    public ProcessParameters(string initialParameters)
    {
        Add<ProcessParameters>(initialParameters);
    }

    #endregion

    #region Public Methods

    public T Append<T>(T parameters) where T : ProcessParameters
    {
        return Add<T>(parameters.ToString());
    }

    public T Prepend<T>(T parameters) where T : ProcessParameters
    {
        return parameters.Add<T>(ToString());
    }

    #endregion
    
    #region Protected Methods

    protected T Add<T>(string parameter) where T : ProcessParameters
    {
        _buffer.Append($" {parameter}");
        return (this as T)!;
    }

    #endregion

    #region Overrides

    public override string ToString()
    {
        return _buffer.ToString();
    }

    #endregion
}