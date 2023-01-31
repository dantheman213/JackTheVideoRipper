using JackTheVideoRipper.extensions;
using JackTheVideoRipper.modules;
using static JackTheVideoRipper.FileSystem;

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
        Title =             info.Title.ValueOrDefault();
        Description =       info.Description.ValueOrDefault();
        MediaFilepath =     info.Filename.EvaluateOrDefault(GetDownloadPath);
    }
    
    public static string DownloadThumbnail(string thumbnailUrl)
    {
        if (thumbnailUrl.Invalid(IsValidUrl))
            return string.Empty;
            
        string urlExtension = GetExtension(thumbnailUrl);
            
        // allow jpg and png but don't allow webp since we'll convert that below
        if (urlExtension == Formats.Image.WEBP)
            urlExtension = Formats.Image.JPG;
            
        string tmpFilePath = GetTempFilename(urlExtension, "thumbnail");
        
        // popular format for saving thumbnails these days but PictureBox in WinForms can't handle it :(
        //  so we'll convert to jpg
        if (thumbnailUrl.EndsWith(Formats.Image.WEBP))
        {
            string tempFilepath = DownloadTempFile(thumbnailUrl, Formats.Image.WEBP);
            FFMPEG.ConvertImageToJpg(tempFilepath,tmpFilePath);
        }
        else
        {
            DownloadWebFile(thumbnailUrl, tmpFilePath);
        }

        return tmpFilePath;
    }
}