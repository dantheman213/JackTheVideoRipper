using JackTheVideoRipper.extensions;
using JackTheVideoRipper.modules;

namespace JackTheVideoRipper.models.parameters;

public class YouTubeParameters : ProcessParameters<YouTubeParameters>
{
    public YouTubeParameters AddMetadata()
    {
        return AddNoValue("add-metadata");
    }
    
    public YouTubeParameters IncludeAds()
    {
        return AddNoValue("include-ads");
    }
    
    public YouTubeParameters RestrictFilenames()
    {
        return AddNoValue("restrict-filenames");
    }
    
    public YouTubeParameters PreferFfmpeg()
    {
        return AddNoValue("prefer-ffmpeg");
    }
    
    public YouTubeParameters NoCheckCertificate()
    {
        return AddNoValue("no-check-certificate");
    }
    
    public YouTubeParameters Source(string url)
    {
        return Append(url.WrapQuotes());
    }
    
    public YouTubeParameters Output(string? filename = null)
    {
        // youtube-dl doesn't like it when you provide --audio-format and extension in -o together
        string outputFilename = filename.HasValue() ? ReplaceExtension(filename!) : YouTubeDL.DefaultFilename;
        return Add('o', outputFilename.WrapQuotes());
    }

    private static string ReplaceExtension(string filename)
    {
        return $"{(filename.Contains('.') ? filename.BeforeLast(".") : filename)}.%(ext)s";
    }
    
    public YouTubeParameters AudioQuality(string qualitySpecifier = "0")
    {
        return Add("audio-quality", qualitySpecifier);
    }
    
    public YouTubeParameters AudioFormat(string format)
    {
        return Add("audio-format", format);
    }
    
    public YouTubeParameters ExtractAudio()
    {
        return AddNoValue('x');
    }
    
    public YouTubeParameters EmbedSubtitles()
    {
        return AddNoValue("embed-subs");
    }
    
    public YouTubeParameters EmbedThumbnail()
    {
        return AddNoValue("embed-thumbnail");
    }
    
    public YouTubeParameters RecodeVideo(string videoFormat)
    {
        return Add("recode-video", videoFormat);
    }

    public YouTubeParameters Format(string? videoFormat = null, string? audioFormat = null, bool useBest = false)
    {
        string formatSpecifier = videoFormat switch
        {
            not null when audioFormat is not null   => $"{videoFormat}+{audioFormat}",
            not null                                => videoFormat,
            null when audioFormat is not null       => audioFormat,
            _                                       => "best"
        };

        string bestSpecifier = useBest && formatSpecifier is not "best" ? "/best" : string.Empty;
        
        return Add('f', $"{formatSpecifier}{bestSpecifier}");
    }
    
    public YouTubeParameters Username(string username)
    {
        return Add("username", username);
    }
    
    public YouTubeParameters Password(string password)
    {
        return Add("password", password);
    }
    
    public YouTubeParameters YesPlaylist()
    {
        return AddNoValue("yes-playlist");
    }
    
    public YouTubeParameters FlatPlaylist()
    {
        return AddNoValue("flat-playlist");
    }
    
    public YouTubeParameters DumpJson()
    {
        return AddNoValue("dump-json");
    }
    
    public YouTubeParameters PrintJson()
    {
        return AddNoValue("print-json");
    }
    
    public YouTubeParameters SkipDownload()
    {
        return AddNoValue("skip-download");
    }
    
    // Do not download or write to disk
    public YouTubeParameters Simulate()
    {
        return AddNoValue('s');
    }
    
    public YouTubeParameters IgnoreErrors()
    {
        return AddNoValue('i');
    }
    
    public YouTubeParameters NoWarnings()
    {
        return AddNoValue("no-warnings");
    }

    public YouTubeParameters NoCache()
    {
        return AddNoValue("no-cache-dir");
    }
    
    public YouTubeParameters ListExtractors()
    {
        return Append("list-extractors");
    }
    
    public YouTubeParameters GetTitle()
    {
        return Append("get-title");
    }
    
    public YouTubeParameters Version()
    {
        return Append("version");
    }

    public YouTubeParameters FfmpegLocation()
    {
        return Add("ffmpeg-location", FFMPEG.ExecutablePath);
    }
    
    public YouTubeParameters AllSubtitles()
    {
        return AddNoValue("all-subs");
    }

    public YouTubeParameters MergeOutputFormat(string format)
    {
        return Add("merge-output-format", format);
    }

    public YouTubeParameters ExternalDownloader(string downloaderFilepath)
    {
        return Add("external-downloader", downloaderFilepath.WrapQuotes());
    }
    
    public YouTubeParameters ExternalDownloader(string downloaderFilepath, string downloaderArgs)
    {
        return ExternalDownloader(downloaderFilepath).ExternalDownloaderArgs(downloaderArgs);
    }
    
    public YouTubeParameters ExternalDownloaderArgs(string downloaderArgs)
    {
        return Add("external-downloader-args", downloaderArgs.WrapQuotes());
    }

    public YouTubeParameters DownloadArchive(string archivePath)
    {
        return Add("download-archive", archivePath.WrapQuotes());
    }

    public YouTubeParameters Links(string linksPath)
    {
        return Add('a', linksPath.WrapQuotes());
    }
}