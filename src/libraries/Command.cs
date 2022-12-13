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
    
    public string RunCommand(Process process)
    {
        return FileSystem.RunProcess(process);
    }
    
    public string RunWebCommand(string url, string parameters, string? workingDirectory = null)
    {
        return FileSystem.RunCommand(_executablePath, $"{parameters} {url}", workingDirectory ?? _workingDirectory);
    }
    
    public async Task<string> RunWebCommandAsync(string url, string parameters, string? workingDirectory = null)
    {
        return await FileSystem.RunCommandAsync(_executablePath, $"{parameters} {url}", workingDirectory ?? _workingDirectory);
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
    
    public async Task<T?> ReceiveJsonResponse<T>(string url, string parameterString)
    {
        return await FileSystem.ReceiveJsonResponseAsync<T>(_executablePath, url, parameterString);
    }
    
    public async Task<T?> ReceiveJsonResponse<T>(string url, IProcessParameters parameters)
    {
        return await FileSystem.ReceiveJsonResponseAsync<T>(_executablePath, url, parameters.ToString());
    }
        
    public async Task<IEnumerable<T>> ReceiveMultiJsonResponse<T>(string url, string parameterString)
    {
        return await FileSystem.ReceiveMultiJsonResponseAsync<T>(_executablePath, url, parameterString);
    }
    
    public async Task<IEnumerable<T>> ReceiveMultiJsonResponse<T>(string url, IProcessParameters parameters)
    {
        return await FileSystem.ReceiveMultiJsonResponseAsync<T>(_executablePath, url, parameters.ToString());
    }
}