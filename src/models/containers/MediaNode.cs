using Newtonsoft.Json;

namespace JackTheVideoRipper.models;

[Serializable]
public class MediaTable
{
    [JsonProperty("node_count")]
    public int NodeCount;
    
    [JsonProperty("media_nodes")]
    public List<MediaNode> MediaNodes
    {
        get => Table.Values.ToList();
        set
        {
            Table.Clear();
            Table = new Dictionary<int, MediaNode>(value.Select(v => new KeyValuePair<int, MediaNode>(v.TableIndex, v)));
        }
    }

    [JsonIgnore]
    public Dictionary<int, MediaNode> Table { get; private set; } = new();

    public void AddMedia(string title, string description, string link, string originalPath, string projectPath = "", string tags = "")
    {
        Table.Add(NodeCount, new MediaNode
        {
            Title = title,
            Description = description,
            Link = link,
            OriginalPath = originalPath,
            ProjectPath = projectPath,
            Tags = tags,
            TableIndex = NodeCount
        });

        NodeCount++;
    }

    public MediaIndex RetrieveReference(int index)
    {
        return !Table.ContainsKey(index) ? new MediaIndex() : Table[index].GenerateReference();
    }
    
    public MediaIndex RetrieveReference(string guid)
    {
        return FindByGuid(guid).GenerateReference();
    }

    public IEnumerable<MediaNode> FindByTitle(string title)
    {
        return Table.Values.Where(m => m.Title == title);
    }

    public MediaNode FindByGuid(string guid)
    {
        return Table.Values.FirstOrDefault(m => m.Guid == guid) ?? new MediaNode();
    }
}

[Serializable]
public class MediaNode
{
    [JsonProperty("title")]
    public string Title = string.Empty;

    [JsonProperty("description")] 
    public string Description = string.Empty;
    
    [JsonProperty("link")] 
    public string Link = string.Empty;
    
    [JsonProperty("original_path")] 
    public string OriginalPath = string.Empty;
    
    [JsonProperty("table_index")] 
    public int TableIndex = -1;
    
    [JsonProperty("guid")] 
    public string Guid = System.Guid.NewGuid().ToString();
    
    [JsonProperty("project_path")] 
    public string? ProjectPath;
    
    [JsonProperty("enhancements")] 
    public List<EnhancementNode> Enhancements = new();

    [JsonProperty("connected_media")] 
    public List<MediaIndex> ConnectedMedia = new();
    
    [JsonProperty("tags")] 
    public string Tags = string.Empty;

    public MediaIndex GenerateReference()
    {
        return new MediaIndex(this);
    }
}

public class MediaIndex
{
    [JsonIgnore]
    public const string NOT_FOUND = "%NOT_FOUND%";
    
    [JsonProperty("title")]
    public string Title = NOT_FOUND;

    [JsonProperty("table_index")] 
    public int TableIndex = -1;
    
    [JsonProperty("guid")] 
    public string Guid = string.Empty;

    public MediaIndex()
    {
    }

    public MediaIndex(MediaNode mediaNode)
    {
        Title = mediaNode.Title;
        TableIndex = mediaNode.TableIndex;
        Guid = mediaNode.Guid;
    }
}

public class EnhancementNode
{
    [JsonProperty("enhancement_types")]
    public List<string> EnhancementTypes = new();
    
    [JsonProperty("path")] 
    public string OriginalPath = string.Empty;
    
    [JsonProperty("table_index")] 
    public int TableIndex = -1;

    [JsonProperty("date_applied")]
    public DateTime? DateApplied;

    [JsonProperty("version_number")]
    public string VersionNumber = string.Empty;

    [JsonProperty("deprecated")]
    public bool Deprecated;

    [JsonProperty("primary")]
    public bool Primary;
    
    [JsonProperty("guid")] 
    public string Guid = System.Guid.NewGuid().ToString();
}