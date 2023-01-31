using JackTheVideoRipper.extensions;
using JackTheVideoRipper.interfaces;
using JackTheVideoRipper.views;
using Newtonsoft.Json;

namespace JackTheVideoRipper.models.containers;

[Serializable]
public class HistoryModel : ConfigModel
{
    #region Static Data Members
        
    public static readonly string HistoryFilepath = FileSystem.MergePaths(ConfigDirectory, "history.json");

    #endregion

    #region Attributes

    public override string Filepath => HistoryFilepath;

    #endregion

    #region Data Members

    public HistoryTable HistoryItemTable { get; private set; }= new();
    
    [JsonProperty("history_items")]
    public List<HistoryItem> HistoryItems
    {
        get => HistoryItemTable.HistoryItems;
        set => HistoryItemTable = new HistoryTable(value);
    }

    [JsonProperty("downloaded_urls")]
    public string[] DownloadedUrls = Array.Empty<string>();

    #endregion

    #region Constructor

    [JsonConstructor]
    public HistoryModel(List<HistoryItem> historyItems)
    {
        HistoryItems = historyItems;
    }
    
    public HistoryModel()
    {
    }

    #endregion

    #region Public Methods

    public void AddHistoryItem(string tag, IMediaItem mediaItem)
    {
        HistoryItemTable.Add(tag, new HistoryItem(mediaItem, tag)
        {
            WebsiteName = string.Empty,     //< Change later...
            Result = ProcessStatus.Created  //< Change later...
        });
    }

    public bool ContainsUrl(string url)
    {
        return HistoryItemTable.ContainsUrl(url);
    }

    public HistoryItem? GetByTag(string tag)
    {
        return HistoryItemTable.GetByTag(tag);
    }

    public void Remove(string tag)
    {
        HistoryItemTable.Remove(tag);
    }

    public void MarkStarted(string tag, DateTime? startDate = null)
    {
        HistoryItemTable.MarkStarted(tag, startDate ?? DateTime.Now);
    }

    public void MarkCompleted(string tag, DateTime? completionDate = null, ProcessStatus result = ProcessStatus.Completed)
    {
        HistoryItemTable.MarkCompleted(tag, completionDate ?? DateTime.Now, result);
    }

    public void UpdateFileInformation(string tag, string filepath, string filesize)
    {
        HistoryItemTable.UpdateFileInformation(tag, filepath, filesize);
    }

    #endregion
}