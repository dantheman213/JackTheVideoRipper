namespace JackTheVideoRipper.interfaces;

public interface IDynamicRow : IListViewItemRow
{
    Task<bool> Update();
}