using System.Diagnostics;
using JackTheVideoRipper.models;
using JackTheVideoRipper.views;
using Console = JackTheVideoRipper.models.Console;

namespace JackTheVideoRipper;

public static class Output
{
    #region Data Members

    private static readonly Console _Console = new("Main");
    
    public static bool ConsoleAttached { get; private set; }

    #endregion

    #region Attributes

    private static bool OutputAvailable => _Console.Opened && ConsoleAttached;

    private static Type CallerType => new StackTrace().GetFrame(2)?.GetMethod()?.DeclaringType!;

    #endregion

    #region Writing Methods

    public static void Write(string message, Color? color = null, bool sendAsNotification = false)
    {
        if (Debugger.IsAttached)
            System.Console.Write(message);
        
        if (sendAsNotification)
            NotificationsManager.SendNotification(new Notification(message, CallerType));

        _Console.WriteOutput(CreateLog(message, color));
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

    #endregion

    #region Public Methods

    public static void OpenMainConsoleWindow()
    {
        _Console.Open();

        if (!ConsoleAttached)
            ConsoleAttached = Input.OpenConsole();
    }
    
    public static async Task<FrameConsole> OpenConsoleWindow(string instanceName,
        FormClosedEventHandler? consoleCloseHandler = null)
    {
        FrameConsole? frameConsole = null;

        void SetConsole()
        {
            frameConsole = new FrameConsole(instanceName, consoleCloseHandler);
        }

        await Core.RunTaskInMainThread(SetConsole).ContinueWith(_ => frameConsole!.OpenConsole());
        
        return frameConsole!;
    }
    
    #endregion

    #region Private Methods

    private static LogNode CreateLog(string message, Color? color = null)
    {
        LogNode logNode = new(DateTime.Now, message, color ?? Color.White);
        _Console.QueueLog(logNode);
        return logNode;
    }

    #endregion
}