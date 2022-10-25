using System.Text;
using JackTheVideoRipper.extensions;

namespace JackTheVideoRipper;

public struct Parameters
{
    public string MediaSourceUrl;
    public string FilenameFormatted;
    public string Username;
    public string Password;
    public bool EncodeVideo;
    public bool AddMetaData;
    public bool IncludeAds;
    public bool EmbedThumbnail;
    public bool EmbedSubs;
    public bool ExportVideo;
    public bool ExportAudio;
    public string AudioFormatId;
    public string VideoFormatId;
    public string VideoFormat;
    public string AudioFormat;

    private bool AudioOnly => ExportAudio && !ExportVideo;

    public override string ToString()
    {
        StringBuilder buffer = new();

        // Authentication
        if (Username.HasValue() && Password.HasValue())
        {
            buffer.Append($" --username {Username} --password {Password}");
        }

        // Video Only Options
        if (!AudioOnly)
        {
            buffer.Append($" -f {VideoFormatId}+{AudioFormatId}/best");
            
            if (EncodeVideo && VideoFormat.HasValue())
            {
                buffer.Append($" --recode-video {VideoFormat}");
            }
            
            if (EmbedThumbnail)
            {
                buffer.Append(" --embed-thumbnail");
            }
            
            if (EmbedSubs)
            {
                buffer.Append(" --embed-subs");
            }
        }
        // Audio Only Options
        else
        {
            buffer.Append($" -f {AudioFormatId} -x --audio-format {AudioFormat} --audio-quality 0");
        }
        
        // General Options
        buffer.Append(" -i --no-check-certificate --prefer-ffmpeg --no-warnings --restrict-filenames");
        
        if (AddMetaData)
        {
            buffer.Append(" --add-metadata");
        }
        
        if (IncludeAds)
        {
            buffer.Append(" --include-ads");
        }
        
        // youtube-dl doesn't like it when you provide --audio-format and extension in -o together
        buffer.Append(FilenameFormatted.Contains('.') ? 
            $" -o {FilenameFormatted.BeforeLast(".")}.%(ext)s" : 
            $" -o {FilenameFormatted}.%(ext)s");
        
        // Media Source
        buffer.Append($" {MediaSourceUrl}");

        return buffer.ToString();
    }
}