using System.Diagnostics;
using JackTheVideoRipper.extensions;
using JackTheVideoRipper.interfaces;
using JackTheVideoRipper.models.processes;

namespace JackTheVideoRipper;

public readonly struct Command
{
    private readonly string _executablePath;

    private readonly string _defaultWorkingDirectory;

    private readonly Func<string, bool> _validationHandler;

    private readonly bool _throwOnValidationFailed;

    public Command(string executablePath, string defaultWorkingDirectory = "",
        Func<string, bool>? validationHandler = null,
        bool throwOnValidationFailed = false)
    {
        _executablePath = executablePath.WrapQuotes();
        _defaultWorkingDirectory = defaultWorkingDirectory;
        _validationHandler = validationHandler ?? DefaultValidationHandler;
        _throwOnValidationFailed = throwOnValidationFailed;
    }

    #region Validation

    private static bool DefaultValidationHandler(string output)
    {
        return true;
    }

    private bool Validate(ProcessResult result)
    {
        bool validationResult = _validationHandler(result.Output) && result.Succeeded;
        if (_throwOnValidationFailed && !validationResult)
            throw new ValidationFailedException(string.Format(Messages.CommandFailed, result.ExitCode));
        return validationResult;
    }
    
    private bool Validate(string result)
    {
        bool validationResult = _validationHandler(result);
        if (_throwOnValidationFailed && !validationResult)
            throw new ValidationFailedException(result);
        return validationResult;
    }

    #endregion

    #region Create Methods

    public Process CreateCommand(string parameters, string? workingDirectory = null)
    {
        return FileSystem.CreateProcess(_executablePath, parameters, 
            workingDirectory ?? _defaultWorkingDirectory);
    }
    
    public Process CreateCommand(IProcessParameters parameters, string? workingDirectory = null)
    {
        return CreateCommand(parameters.ToString(), workingDirectory);
    }

    #endregion

    #region Run Methods

    public string RunCommand(string parameters, string? workingDirectory = null)
    {
        string result = FileSystem.RunCommand(_executablePath, parameters,
            workingDirectory ?? _defaultWorkingDirectory);
        
        return Validate(result) ? result : string.Empty;
    }
    
    public string RunCommand(IProcessParameters parameters, string? workingDirectory = null)
    {
        return RunCommand(parameters.ToString(), workingDirectory);
    }
    
    public string RunWebCommand(string url, string parameters, string? workingDirectory = null)
    {
        return RunCommand($"{parameters} {url}", workingDirectory);
    }
    
    public string RunWebCommand(string url, IProcessParameters parameters, string? workingDirectory = null)
    {
        return RunWebCommand(url, parameters.ToString(), workingDirectory);
    }

    #endregion

    #region Async Methods

    public async Task<string> RunCommandAsync(string parameters, string? workingDirectory = null,
        bool runAsAdmin = false, bool redirectInput = true, int timeoutPeriod = -1)
    {
        ProcessResult result = await FileSystem.RunCommandAsync(_executablePath, parameters,
            workingDirectory ?? _defaultWorkingDirectory, runAsAdmin, redirectInput, timeoutPeriod);

        return Validate(result) ? result.Output : string.Empty;
    }
    
    public async Task<T?> RunCommandAsync<T>(string parameters, string? workingDirectory = null,
        bool runAsAdmin = false, bool redirectInput = true, int timeoutPeriod = -1)
    {
        ProcessResult result = await FileSystem.RunCommandAsync(_executablePath, parameters,
            workingDirectory ?? _defaultWorkingDirectory, runAsAdmin, redirectInput, timeoutPeriod);

        return Validate(result) ? result.Output.Convert<T>() : default;
    }

    public async Task<string> RunCommandAsync(IProcessParameters parameters, string? workingDirectory = null)
    {
        return await RunCommandAsync(parameters.ToString(), workingDirectory);
    }
    
    public async Task<T?> RunCommandAsync<T>(IProcessParameters parameters, string? workingDirectory = null)
    {
        return await RunCommandAsync<T>(parameters.ToString(), workingDirectory);
    }

    public async Task<string> RunWebCommandAsync(string url, string parameters, string? workingDirectory = null)
    {
        return await RunCommandAsync($"{parameters} {url}", workingDirectory);
    }

    #endregion

    #region Json Methods

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

    #endregion

    #region Embedded Types

    public class ValidationFailedException : Exception
    {
        public ValidationFailedException() { }
        
        public ValidationFailedException(string message) : base(message) { }
        
        public ValidationFailedException(string message, Exception innerException) : base(message, innerException) { }
    }

    #endregion
}