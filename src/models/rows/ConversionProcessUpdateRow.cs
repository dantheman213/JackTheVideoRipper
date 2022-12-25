using System.Diagnostics;
using JackTheVideoRipper.interfaces;

namespace JackTheVideoRipper.models;

public class ConversionProcessUpdateRow : ProcessUpdateRow
{
    public ConversionProcessUpdateRow(IMediaItem mediaItem, Action<IProcessRunner> completionCallback) : 
        base(mediaItem, completionCallback)
    {
    }

    protected override Process CreateProcess()
    {
        throw new NotImplementedException();
    }

    protected override Task<string> GetTitle()
    {
        throw new NotImplementedException();
    }

    protected override void SetProgressText(IReadOnlyList<string> tokens)
    {
        throw new NotImplementedException();
    }

    protected override string? GetStatus()
    {
        throw new NotImplementedException();
    }
}