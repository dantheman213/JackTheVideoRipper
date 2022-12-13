using JackTheVideoRipper.extensions;
using JackTheVideoRipper.modules;

namespace JackTheVideoRipper;

public struct MediaParameters
{
    public readonly string MediaSourceUrl;
    
    public string FilenameFormatted = string.Empty;
    
    public string Username = string.Empty;
    
    public string Password = string.Empty;
    
    public bool EncodeVideo = false;
    
    public bool AddMetaData = false;
    
    public bool IncludeAds = false;
    
    public bool EmbedThumbnail = false;
    
    public bool EmbedSubtitles = false;
    
    public bool ExportVideo = false;
    
    public bool ExportAudio = false;
    
    public string? AudioFormatId = null;
    
    public string? VideoFormatId = null;
    
    public string VideoFormat = string.Empty;
    
    public string AudioFormat = string.Empty;

    public bool RunMultiThreaded = false;

    private bool AudioOnly => ExportAudio && !ExportVideo;

    public MediaParameters(string url)
    {
        MediaSourceUrl = url;
    }

    private YouTubeDL.YouTubeParameters Build()
    {
        YouTubeDL.YouTubeParameters youTubeParameters = new();
        
        // Authentication
        if (Username.HasValue() && Password.HasValue())
        {
            youTubeParameters.Username(Username).Password(Password);
        }
        
        // Video Only Options
        if (!AudioOnly)
        {
            youTubeParameters.Format(VideoFormatId, AudioFormatId, true);

            if (EncodeVideo && VideoFormat.HasValue())
                youTubeParameters.RecodeVideo(VideoFormat);

            if (EmbedThumbnail)
                youTubeParameters.EmbedThumbnail();

            if (EmbedSubtitles)
                youTubeParameters.EmbedSubtitles();
        }
        // Audio Only Options
        else
        {
            youTubeParameters.Format(audioFormat: AudioFormatId).ExtractAudio().AudioFormat(AudioFormat).AudioQuality();
        }
        
        // General Options
        youTubeParameters.IgnoreErrors().NoCheckCertificate().PreferFfmpeg().NoWarnings().RestrictFilenames();
        
        if (AddMetaData)
            youTubeParameters.AddMetadata();

        if (IncludeAds)
            youTubeParameters.IncludeAds();

        if (RunMultiThreaded)
            youTubeParameters.ExternalDownloader(Aria2c.ExecutablePath, Aria2c.DEFAULT_ARGS);

        youTubeParameters.Output(FilenameFormatted);
        
        // Media Source
        youTubeParameters.Source(MediaSourceUrl);

        return youTubeParameters;
    }

    public override string ToString()
    {
        return Build().ToString();
    }
}