using JackTheVideoRipper.models.enums;

namespace JackTheVideoRipper;

public class ContextActionEventArgs : EventArgs
{
    public readonly ContextActions ContextAction;
            
    public ContextActionEventArgs(ContextActions contextAction)
    {
        ContextAction = contextAction;
    }
}