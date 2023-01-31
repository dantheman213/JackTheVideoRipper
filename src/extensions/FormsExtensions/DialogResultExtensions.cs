namespace JackTheVideoRipper.extensions;

public static class DialogResultExtensions
{
    public static bool IsSuccess(this DialogResult dialogResult)
    {
        return dialogResult is DialogResult.OK or DialogResult.Yes or DialogResult.Continue;
    }
}