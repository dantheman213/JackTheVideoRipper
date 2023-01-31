namespace JackTheVideoRipper.interfaces;

public interface IGeneratorForm<out T>
{
    T? Output { get; }
}