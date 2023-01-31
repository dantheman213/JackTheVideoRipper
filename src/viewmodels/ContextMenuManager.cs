namespace JackTheVideoRipper.models;

public class ContextMenuManager
{
    private readonly ContextMenuStrip _contextMenuListItems;
    
    public ContextMenuManager(ContextMenuStrip contextMenuListItems)
    {
        _contextMenuListItems = contextMenuListItems;
    }
    
    private ToolStripItemCollection ContextItems => _contextMenuListItems.Items;
    
    private static readonly Dictionary<string, ProcessStatus> _ContextItemsDict = new()
    {
        { "retryDownloadToolStripMenuItem",     ProcessStatus.Error },
        { "stopDownloadToolStripMenuItem",      ProcessStatus.Running },
        { "deleteFromDiskToolStripMenuItem",    ProcessStatus.Succeeded },
        { "resumeDownloadToolStripMenuItem",    ProcessStatus.Paused },
        { "pauseDownloadToolStripMenuItem",     ProcessStatus.Running },
        { "redownloadMediaToolStripMenuItem",   ProcessStatus.Completed }
    };

    public async Task OpenContextMenu()
    {
        await Parallel.ForEachAsync(_ContextItemsDict, SetContextVisibility);
        ShowContextItem("deleteRowToolStripMenuItem");
        ShowContextMenu();
    }

    private void ShowContextMenu()
    {
        _contextMenuListItems.Show(Cursor.Position);
    }
    
    private void ShowContextItem(string name)
    {
        SetContextVisibility(name);
    }

    private void HideContextItem(string name)
    {
        SetContextVisibility(name, value:false);
    }

    private async ValueTask SetContextVisibility(KeyValuePair<string, ProcessStatus> keyValuePair,
        CancellationToken token)
    {
        await Threading.RunInMainContext(() => SetContextVisibility(keyValuePair.Key, keyValuePair.Value));
    }

    private void SetContextVisibility(string name, ProcessStatus? processStatus = null, bool value = true)
    {
        ContextItems[name].Visible = ValueIfStatus(processStatus, value);
    }
    
    private static bool ValueIfStatus(ProcessStatus? processStatus = null, bool value = true)
    {
        return Ripper.Instance.GetSelectedStatus() == processStatus ? value : !value;
    }
}