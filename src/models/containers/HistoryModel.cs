﻿using JackTheVideoRipper.interfaces;
using JackTheVideoRipper.views;
using Newtonsoft.Json;

namespace JackTheVideoRipper.models.containers;

[Serializable]
public class HistoryModel : ConfigModel
{
    #region Static Data Members
        
    public static readonly string HistoryFilepath = Path.Combine(ConfigDirectory, "history.json");

    #endregion

    #region Attributes

    public override string Filepath => HistoryFilepath;

    #endregion

    #region Data Members

    private HistoryTable _historyItemTable = new();
    
    [JsonProperty("history_items")]
    public List<HistoryItem> HistoryItems
    {
        get => _historyItemTable.HistoryItems;
        set => _historyItemTable = new HistoryTable(value);
    }

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
        _historyItemTable.Add(tag, new HistoryItem(mediaItem, tag)
        {
            WebsiteName = string.Empty,     //< Change later...
            Result = ProcessStatus.Created  //< Change later...
        });
    }

    public bool ContainsUrl(string url)
    {
        return _historyItemTable.ContainsUrl(url);
    }

    public HistoryItem? GetByTag(string tag)
    {
        return _historyItemTable.GetByTag(tag);
    }

    public void Remove(string tag)
    {
        _historyItemTable.Remove(tag);
    }

    public void PopulateListView(ListView listView)
    {
        listView.Items.AddRange(_historyItemTable.GetOrderedItems.Select(item => new HistoryRow(item).ViewItem).ToArray());
    }

    public void OpenHistory()
    {
        FrameHistory frameHistory = new();

        PopulateListView(frameHistory.ListView);
        
        frameHistory.Show();
    }

    public void MarkStarted(string tag, DateTime? startDate = null)
    {
        _historyItemTable.MarkStarted(tag, startDate ?? DateTime.Now);
    }

    public void MarkCompleted(string tag, DateTime? completionDate = null, ProcessStatus result = ProcessStatus.Completed)
    {
        _historyItemTable.MarkCompleted(tag, completionDate ?? DateTime.Now, result);
    }

    public void UpdateFileInformation(string tag, string filepath, string filesize)
    {
        _historyItemTable.UpdateFileInformation(tag, filepath, filesize);
    }

    #endregion
}