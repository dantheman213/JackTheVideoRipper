using JackTheVideoRipper.extensions;

namespace JackTheVideoRipper;

public struct MetaData
{
    public readonly string ThumbnailFilepath;
    public readonly string Title;
    public readonly string Description;
    public readonly string MediaFilepath;

    public MetaData(MediaInfoData info)
    {
        ThumbnailFilepath = info.Thumbnail.EvaluateOrDefault(DownloadThumbnail);
        Title = info.Title.ValueOrDefault();
        Description = info.Description.ValueOrDefault();
        MediaFilepath = info.Filename.EvaluateOrDefault(FileSystem.GetDownloadPath);
    }

    private const string _WEBP_EXTENSION = "webp";
    
    public static string DownloadThumbnail(string thumbnailUrl)
    {
        if (!FileSystem.IsValidUrl(thumbnailUrl))
            return string.Empty;
            
        string urlExt = FileSystem.GetExtension(thumbnailUrl);
            
        // allow jpg and png but don't allow webp since we'll convert that below
        if (urlExt == _WEBP_EXTENSION)
            urlExt = "jpg";
            
        string tmpFilePath = FileSystem.GetTempFilename(urlExt);
                
        // popular format for saving thumbnails these days but PictureBox in WinForms can't handle it :( so we'll convert to jpg
        if (thumbnailUrl.EndsWith(_WEBP_EXTENSION))
        {
            FFMPEG.ConvertImageToJpg(FileSystem.DownloadTempFile(thumbnailUrl, _WEBP_EXTENSION), tmpFilePath);
        }
        else
        {
            FileSystem.DownloadWebFile(thumbnailUrl, tmpFilePath);
        }

        return tmpFilePath;
    }
}