using JackTheVideoRipper.extensions;
using JackTheVideoRipper.models.enums;

namespace JackTheVideoRipper.models;

public struct MediaItemRow
{
    public readonly string Tag;
    public readonly string Title;
    public readonly string Url;
    public Parameters? Parameters = null;
    public readonly string Filepath;
    public readonly MediaType Type;
    public readonly ListViewItem ListViewItem;

    public static implicit operator ListViewItem(MediaItemRow row)
    {
        return row.ListViewItem;
    }

    public MediaItemRow(string title, string url, string filepath, MediaType mediaType = MediaType.Video)
    {
        Title = title;
        Url = url;
        Filepath = filepath;
        Tag = $"{Common.RandomString(5)}{DateTime.UtcNow.Ticks}";
        ListViewItem = new ListViewItem(DefaultRow(Title, mediaType, Url, Filepath))
        {
            Tag = Tag,
            BackColor = Color.LightGray,
            ImageIndex = mediaType == MediaType.Video ? 0 : 1
        };
        Type = mediaType;
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

    public string ParameterString => Parameters?.As<Parameters>().ToString() ?? string.Empty;
}