using System.Diagnostics;
using JackTheVideoRipper.interfaces;

namespace JackTheVideoRipper;

public readonly struct Command
{
    private readonly string _executablePath;

    private readonly string _workingDirectory;

    private readonly Func<string, bool> _validationHandler;

    public Command(string executablePath, string workingDirectory = "", Func<string, bool>? validationHandler = null)
    {
        _executablePath = executablePath;
        _workingDirectory = workingDirectory;
        _validationHandler = validationHandler ?? DefaultValidationHandler;
    }

    private static bool DefaultValidationHandler(string output)
    {
        return true;
    }

    private bool Validate((int ExitCode, string Output) result)
    {
        return _validationHandler(result.Output) && result.ExitCode == 0;
    }
    
    private bool Validate(string result)
    {
        return _validationHandler(result);
    }
    
    public Process CreateCommand(string parameters, string? workingDirectory = null)
    {
        return FileSystem.CreateProcess(_executablePath, parameters, workingDirectory ?? _workingDirectory);
    }
    
    public Process CreateCommand(IProcessParameters parameters, string? workingDirectory = null)
    {
        return CreateCommand(parameters.ToString(), workingDirectory);
    }

    public string RunCommand(string parameters, string? workingDirectory = null)
    {
        var result = FileSystem.RunCommand(_executablePath, parameters,
            workingDirectory ?? _workingDirectory);
        
        return Validate(result) ? result : string.Empty;
    }
    
    public string RunCommand(IProcessParameters parameters, string? workingDirectory = null)
    {
        return RunCommand(parameters.ToString(), workingDirectory);
    }
    
    public async Task<string> RunCommandAsync(string parameters, string? workingDirectory = null)
    {
        var result = await FileSystem.RunCommandAsync(_executablePath, parameters,
            workingDirectory ?? _workingDirectory);

        return Validate(result) ? result.Output : string.Empty;
    }

    public async Task<string> RunCommandAsync(IProcessParameters parameters, string? workingDirectory = null)
    {
        return await RunCommandAsync(parameters.ToString(), workingDirectory);
    }
    
    public string RunWebCommand(string url, string parameters, string? workingDirectory = null)
    {
        return RunCommand($"{parameters} {url}", workingDirectory);
    }
    
    public string RunWebCommand(string url, IProcessParameters parameters, string? workingDirectory = null)
    {
        return RunWebCommand(url, parameters.ToString(), workingDirectory);
    }
    
    public async Task<string> RunWebCommandAsync(string url, string parameters, string? workingDirectory = null)
    {
        return await RunCommandAsync($"{parameters} {url}", workingDirectory);
    }

    public async Task<T?> ReceiveJsonResponse<T>(string url, string parameterString)
    {
        return await FileSystem.ReceiveJsonResponseAsync<T>(_executablePath, url, parameterString);
    }
    
    public async Task<T?> ReceiveJsonResponse<T>(string url, IProcessParameters parameters)
    {
        return await ReceiveJsonResponse<T>(url, parameters.ToString());
    }
        
    public async Task<IEnumerable<T>> ReceiveMultiJsonResponse<T>(string url, string parameterString)
    {
        return await FileSystem.ReceiveMultiJsonResponseAsync<T>(_executablePath, url, parameterString);
    }
    
    public async Task<IEnumerable<T>> ReceiveMultiJsonResponse<T>(string url, IProcessParameters parameters)
    {
        return await ReceiveMultiJsonResponse<T>(url, parameters.ToString());
    }
}