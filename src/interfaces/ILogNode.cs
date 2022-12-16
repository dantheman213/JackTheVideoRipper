using JackTheVideoRipper.models;

namespace JackTheVideoRipper.interfaces;

public interface ILogNode
{
    IReadOnlyList<ConsoleLine> Serialize();
}