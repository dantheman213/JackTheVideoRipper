using JackTheVideoRipper.extensions;
using JackTheVideoRipper.models.enums;

namespace JackTheVideoRipper.models;

public struct MediaItemRow
{
    public readonly string Tag;
    public string Title = string.Empty;
    public MediaType Type = MediaType.Video;
    public string Url = string.Empty;
    public Parameters? Parameters = null;
    public string Filepath = string.Empty;

    private ListViewItem? _listViewItem = null;

    public ListViewItem ListViewItem
    {
        get
        {
            if (_listViewItem is null)
                CreateListViewItem();
            return _listViewItem!;
        }
    }

    public static implicit operator ListViewItem(MediaItemRow row)
    {
        return row.ListViewItem;
    }

    public MediaItemRow()
    {
        Tag = $"{Common.RandomString(5)}{DateTime.UtcNow.Ticks}";
    }

    public static string[] DefaultRow(string title, MediaType type, string url, string filepath)
    {
        return new[]
        {
            title,
            Statuses.WAITING,
            type.ToString(),
            Tags.DEFAULT_SIZE,
            Tags.DEFAULT_PROGRESS,
            Tags.DEFAULT_SPEED,
            Tags.DEFAULT_TIME,
            url,
            filepath
        };
    }

    private void CreateListViewItem()
    {
        _listViewItem = new ListViewItem(DefaultRow(Title, Type, Url, Filepath))
        {
            Tag = Tag,
            BackColor = Color.LightGray,
            ImageIndex = Type == MediaType.Video ? 0 : 1
        };
    }

    public string ParameterString => Parameters is not null ? Parameters.As<Parameters>().ToString() : string.Empty;
}