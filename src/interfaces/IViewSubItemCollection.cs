namespace JackTheVideoRipper.interfaces;

public interface IViewSubItemCollection
{
    public IViewSubItem this[int index] { get; }
    
    public IEnumerable<IViewSubItem> Items { get; }
}