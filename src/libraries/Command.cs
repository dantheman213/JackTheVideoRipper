using System.Diagnostics;

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

    public string RunCommand(string parameters)
    {
        return FileSystem.RunCommand(_executablePath, parameters, _workingDirectory);
    }
    
    public string RunWebCommand(string parameters, string url)
    {
        return FileSystem.RunCommand(_executablePath, $"{parameters} {url}", _workingDirectory);
    }

    public Process CreateCommand(string parameters)
    {
        return FileSystem.CreateProcess(_executablePath, parameters, _workingDirectory);
    }
}