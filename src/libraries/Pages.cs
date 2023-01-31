using System.Runtime.ExceptionServices;
using JackTheVideoRipper.extensions;
using JackTheVideoRipper.interfaces;
using JackTheVideoRipper.views;

namespace JackTheVideoRipper;

public static class Pages
{
    public static DialogResult OpenPage<T>() where T : Form, new()
    {
        using Form form = new T();
        return form.ShowDialog();
    }

    public static T OpenPageBackground<T>() where T : Form, new()
    {
        T form = new();
        if (form is FrameHistory frameHistory)
            FrameHistory.PopulateListView(frameHistory);
        form.Show();
        return form;
    }
    
    public static bool OpenPageConfirm<T>() where T : Form, new()
    {
        using Form form = new T();
        return form.ShowDialog().IsSuccess();
    }

    public static T? OpenValueForm<TF, T>() where TF : Form, IGeneratorForm<T>, new()
    {
        using TF form = new();
        DialogResult result = form.ShowDialog();
        return result.IsSuccess() ? form.Output : default;
    }
    
    #region Exception Handling

    public static void OpenExceptionHandler(object? sender, ThreadExceptionEventArgs args)
    {
        OpenExceptionHandler(args.Exception);
    }
    
    public static void OpenExceptionHandler(object? sender, FirstChanceExceptionEventArgs args)
    {
        OpenExceptionHandler(args.Exception);
    }
    
    public static void OpenExceptionHandler(object? sender, UnhandledExceptionEventArgs args)
    {
        if (args.ExceptionObject is not Exception exception)
            return;
        OpenExceptionHandler(exception);
    }

    public static void OpenExceptionHandler(Exception exception)
    {
        if (new FrameErrorHandler(exception).ShowDialog() == DialogResult.Abort)
            Core.Crash(Messages.UnhandledException, exception);
    }

    #endregion
}