using System.Diagnostics;
using JackTheVideoRipper.interfaces;

namespace JackTheVideoRipper;

public readonly struct Command
{
    private readonly string _executablePath;

    private readonly string _workingDirectory;

    public Command(string executablePath, string workingDirectory = "")
    {
        _executablePath = executablePath;
        _workingDirectory = workingDirectory;
    }

    public string RunCommand(string parameters, string? workingDirectory = null)
    {
        return FileSystem.RunCommand(_executablePath, parameters, workingDirectory ?? _workingDirectory);
    }
    
    public string RunCommand(IProcessParameters parameters, string? workingDirectory = null)
    {
        return FileSystem.RunCommand(_executablePath, parameters.ToString(), workingDirectory ?? _workingDirectory);
    }
    
    public string RunWebCommand(string url, string parameters, string? workingDirectory = null)
    {
        return FileSystem.RunCommand(_executablePath, $"{parameters} {url}", workingDirectory ?? _workingDirectory);
    }
    
    public string RunWebCommand(string url, IProcessParameters parameters, string? workingDirectory = null)
    {
        return FileSystem.RunCommand(_executablePath, $"{parameters.ToString()} {url}", workingDirectory ?? _workingDirectory);
    }

    public Process CreateCommand(string parameters, string? workingDirectory = null)
    {
        return FileSystem.CreateProcess(_executablePath, parameters, workingDirectory ?? _workingDirectory);
    }
    
    public Process CreateCommand(IProcessParameters parameters, string? workingDirectory = null)
    {
        return FileSystem.CreateProcess(_executablePath, parameters.ToString(), workingDirectory ?? _workingDirectory);
    }
    
    public T? ReceiveJsonResponse<T>(string url, string parameterString)
    {
        return FileSystem.ReceiveJsonResponse<T>(_executablePath, url, parameterString);
    }
    
    public T? ReceiveJsonResponse<T>(string url, IProcessParameters parameters)
    {
        return FileSystem.ReceiveJsonResponse<T>(_executablePath, url, parameters.ToString());
    }
        
    public IEnumerable<T> ReceiveMultiJsonResponse<T>(string url, string parameterString)
    {
        return FileSystem.ReceiveMultiJsonResponse<T>(_executablePath, url, parameterString);
    }
    
    public IEnumerable<T> ReceiveMultiJsonResponse<T>(string url, IProcessParameters parameters)
    {
        return FileSystem.ReceiveMultiJsonResponse<T>(_executablePath, url, parameters.ToString());
    }
}