using JackTheVideoRipper.interfaces;
using static System.Windows.Forms.ListView;

namespace JackTheVideoRipper.extensions;

public static class ListViewItemCollectionExtensions
{
    public static void AddRange(this ListViewItemCollection itemCollection, IEnumerable<ListViewItem> items)
    {
        Parallel.ForEach(items, item =>
        {
            lock (itemCollection)
            {
                itemCollection.Add(item);
            }
        });
    }
    
    public static void RemoveRange(this ListViewItemCollection itemCollection, IEnumerable<ListViewItem> items)
    {
        Parallel.ForEach(items, item =>
        {
            lock (itemCollection)
            {
                itemCollection.Remove(item);
            }
        });
    }
    
    public static void AddRange(this ListViewItemCollection itemCollection, IEnumerable<IViewItem> items)
    {
        Parallel.ForEach(items, item =>
        {
            lock (itemCollection)
            {
                itemCollection.Add(item.As<ListViewItem>());
            }
        });
    }
    
    public static void RemoveRange(this ListViewItemCollection itemCollection, IEnumerable<IViewItem> items)
    {
        Parallel.ForEach(items, item =>
        {
            lock (itemCollection)
            {
                itemCollection.Remove(item.As<ListViewItem>());
            }
        });
    }
}