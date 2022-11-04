namespace JackTheVideoRipper;

public class FileNode
{
    public readonly string Uri;
    public readonly Type Type;
    public readonly string Extension;

    public bool IsValidUrl => FileSystem.IsValidUrl(Uri);

    public FileNode(string uri, Type type)
    {
        Uri = uri;
        Type = type;
        Extension = FileSystem.GetExtension(uri);
    }

    public string Convert(ConversionSource conversionSource, string outputFilepath)
    {
        switch (conversionSource)
        {
            case ConversionSource.Image:
                FFMPEG.ConvertImageToJpg(Uri, outputFilepath);
                return outputFilepath;
            default:
                throw new NotImplementedException();
        }
    }

    public string Download(string filepath)
    {
        return FileSystem.DownloadTempFile(Uri, filepath);
    }

    public string DownloadWeb(string filepath)
    {
        return FileSystem.DownloadWebFile(Uri, filepath);
    }

    public enum ConversionSource
    {
        Image, Video, Audio
    }
}