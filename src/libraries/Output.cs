using System.Diagnostics;
using JackTheVideoRipper.models;
using JackTheVideoRipper.views;

namespace JackTheVideoRipper;

public static class Output
{
    #region Data Members

    private static FrameConsole? _frameConsole;
    
    private static ConsoleControl.ConsoleControl? _console;
    
    private static bool _usingConsole;
    
    public static bool ConsoleAttached { get; private set; }
    
    private static readonly List<LogNode> _LogHistory = new();

    #endregion

    #region Attributes

    private static bool OutputAvailable => _usingConsole && ConsoleAttached;

    private static Type CallerType => new StackTrace().GetFrame(2)?.GetMethod()?.DeclaringType!;

    #endregion

    #region Public Methods

    public static void Write(string message, Color? color = null, bool sendAsNotification = false)
    {
        if (Debugger.IsAttached)
            Console.Write(message);
        
        if (sendAsNotification)
            NotificationsManager.SendNotification(new Notification(message, CallerType));

        WriteConsole(CreateLog(message, color));
    }
    
    public static void Write(object? message, Color? color = null, bool sendAsNotification = false)
    {
        if (message is null)
            return;

        Write(message.ToString()!, color, sendAsNotification);
    }
    
    public static void WriteLine(string message, Color? color = null, bool sendAsNotification = false)
    {
        Write($"{message}\n", color, sendAsNotification);
    }
    
    public static void WriteLine(object? message, Color? color = null, bool sendAsNotification = false)
    {
        if (message is null)
            return;
        
        Write($"{message}\n", color, sendAsNotification);
    }

    public static void OpenMainConsoleWindow()
    {
        if (_frameConsole is not null)
        {
            Core.RunInMainThread(() => _frameConsole.Activate());
            return;
        }

        if (!ConsoleAttached)
            ConsoleAttached = Input.OpenConsole();

        _frameConsole = new FrameConsole("Main", OnCloseConsole);
        _console = _frameConsole.ConsoleControl;
        _usingConsole = true;
        
        _frameConsole.OpenConsole();
        _LogHistory.ForEach(WriteConsole);
        _frameConsole.Refresh();
    }
    
    public static FrameConsole OpenConsoleWindow(string instanceName, FormClosedEventHandler? consoleCloseHandler = null)
    {
        FrameConsole? frameConsole = null;
        
        Core.RunInMainThread(() =>
        {
            frameConsole = new FrameConsole(instanceName, consoleCloseHandler);
        });

        frameConsole!.OpenConsole();

        return frameConsole;
    }
    
    #endregion

    #region Private Methods

    private static LogNode CreateLog(string message, Color? color = null)
    {
        LogNode logNode = new(DateTime.Now, message, color ?? Color.White);
        _LogHistory.Add(logNode);
        return logNode;
    }
    
    private static void WriteConsole(LogNode logNode)
    {
        if (!OutputAvailable)
            return;
        _console!.WriteOutput(logNode.Message, logNode.Color);
    }
    
    private static void OnCloseConsole(object? sender, FormClosedEventArgs args)
    {
        _frameConsole = null;
        _console = null;
        _usingConsole = false;
    }

    #endregion
}