namespace JackTheVideoRipper.models;

public class DependencyActionEventArgs : EventArgs
{
    public readonly Dependencies Dependency;
            
    public DependencyActionEventArgs(Dependencies dependency)
    {
        Dependency = dependency;
    }
}