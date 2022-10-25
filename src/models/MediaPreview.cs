namespace JackTheVideoRipper;

public struct MediaPreview
{
    public readonly string ThumbnailFilepath;
    public readonly string Title;
    public readonly string Description;
    public readonly string MediaFilepath;

    public MediaPreview(MediaInfoData info)
    {
        ThumbnailFilepath = info.Thumbnail is not null ? YouTubeDL.DownloadThumbnail(info.Thumbnail) ?? "" : "";
        Title = info.Title ?? "";
        Description = info.Description ?? "";
        MediaFilepath = info.Filename is not null ? FileSystem.GetDownloadPath(FileSystem.ValidateFilename(info.Filename)) ?? "" : "";
    }
}