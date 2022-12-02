using JackTheVideoRipper.extensions;

namespace JackTheVideoRipper;

public struct MediaParameters
{
    public string MediaSourceUrl;
    public string FilenameFormatted;
    public string Username;
    public string Password;
    public bool EncodeVideo;
    public bool AddMetaData;
    public bool IncludeAds;
    public bool EmbedThumbnail;
    public bool EmbedSubtitles;
    public bool ExportVideo;
    public bool ExportAudio;
    public string AudioFormatId;
    public string VideoFormatId;
    public string VideoFormat;
    public string AudioFormat;

    private bool AudioOnly => ExportAudio && !ExportVideo;

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