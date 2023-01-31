using JackTheVideoRipper.extensions;
using JackTheVideoRipper.views.modals;

namespace JackTheVideoRipper.viewmodels;

public class BasicDropdownModal
{
    private readonly FrameBasicDropdown _frameBasicDropdown;
    
    public BasicDropdownModal(IEnumerable<string> options, string? title = null, string? defaultValue = null)
    {
        _frameBasicDropdown = new FrameBasicDropdown(options, title, defaultValue);
    }
    
    public string? Open()
    {
        return _frameBasicDropdown.Confirm() ? _frameBasicDropdown.Value : null;
    }
}