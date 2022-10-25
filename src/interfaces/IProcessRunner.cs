using System.Diagnostics;

namespace JackTheVideoRipper.interfaces;

public interface IProcessRunner
{
    public Process Process { get; set; }
    
    List<string> Results { get; set; }

    void AppendStatusLine();

    void TrackStandardOut();

    void AppendErrorLine();

    void TrackStandardError();
}