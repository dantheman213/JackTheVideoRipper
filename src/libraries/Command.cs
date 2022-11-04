using System.Diagnostics;

namespace JackTheVideoRipper;

public readonly struct Command
{
    private readonly string _executablePath;

    public Command(string executablePath)
    {
        _executablePath = executablePath;
    }

    public string RunCommand(string parameters)
    {
        return FileSystem.RunCommand(_executablePath, parameters);
    }
    
    public string RunWebCommand(string parameters, string url)
    {
        return FileSystem.RunCommand(_executablePath, $"{parameters} {url}");
    }

    public Process CreateCommand(string parameters)
    {
        return FileSystem.CreateProcess(_executablePath, parameters);
    }
}