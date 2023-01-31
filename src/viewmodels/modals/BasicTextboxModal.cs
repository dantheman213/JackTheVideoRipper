using JackTheVideoRipper.extensions;
using JackTheVideoRipper.views.modals;

namespace JackTheVideoRipper.viewmodels;

public class BasicTextboxModal
{
    private readonly FrameBasicTextbox _frameBasicTextbox;
    
    public BasicTextboxModal(string? title = null, string? defaultValue = null)
    {
        _frameBasicTextbox = new FrameBasicTextbox(title, defaultValue);
    }

    public string? Open()
    {
        return _frameBasicTextbox.Confirm() ? _frameBasicTextbox.Value : null;
    }
}