using System.Globalization;
using JackTheVideoRipper.extensions;

namespace JackTheVideoRipper.models.containers;

[Serializable]
public class ExifData
{
    public string ExifToolVersionNumber = string.Empty;

    public string FileName = string.Empty;

    public string Directory = string.Empty;

    public long FileSize;
    
    public DateTime FileModificationDateTime;
    
    public DateTime FileAccessDateTime;
    
    public DateTime FileCreationDateTime;
    
    public string FilePermissions = string.Empty;
    
    public string FileType = string.Empty;
    
    public string FileTypeExtension = string.Empty;
    
    public string Title = string.Empty;

    public string Subtitle = string.Empty;

    public string Rating = string.Empty;

    public string Tags = string.Empty;

    public string Comments = string.Empty;
    
    public string MimeType = string.Empty;
    
    public string MajorBrand = string.Empty;
    
    public string MinorVersion = string.Empty;
    
    public string CompatibleBrands = string.Empty;
    
    public string MovieHeaderVersion = string.Empty;

    public DateTime CreateDate;

    public DateTime ModifyDate;

    public int TimeScale;

    public TimeSpan Duration;

    public int PreferredRate;

    public float PreferredVolume;

    public TimeSpan PreviewTime;
    
    public TimeSpan PreviewDuration;
    
    public TimeSpan PosterTime;
    
    public TimeSpan SelectionTime;
    
    public TimeSpan SelectionDuration;
    
    public TimeSpan CurrentTime;
    
    public long NextTrackId;
    
    public string TrackHeaderVersion = string.Empty;
    
    public DateTime TrackCreateDate;
    
    public DateTime TrackModifyDate;
    
    public long TrackId;
    
    public TimeSpan TrackDuration;
    
    public string TrackLayer = string.Empty;
    
    public float TrackVolume;
    
    public int ImageWidth;
    
    public int ImageHeight;
    
    public string CompressorId = string.Empty;
    
    public int SourceImageWidth;
    
    public int SourceImageHeight;
    
    public int XResolution;
    
    public int YResolution;
    
    public int BitDepth;
    
    public float VideoFrameRate;
    
    public string GraphicsMode = string.Empty;

    public string OpColor = string.Empty;
    
    public string MatrixStructure = string.Empty;
    
    public string MediaHeaderVersion = string.Empty;
    
    public DateTime MediaCreateDate;
    
    public DateTime MediaModifyDate;
    
    public int MediaTimeScale;
    
    public TimeSpan MediaDuration;
    
    public string MediaLanguageCode = string.Empty;
    
    public string HandlerDescription = string.Empty;
    
    public string AudioFormat = string.Empty;
    
    public int AudioChannels;
    
    public int AudioBitsPerSample;
    
    public float AudioSampleRate;
    
    public string Balance = string.Empty;
    
    public string HandlerType = string.Empty;
    
    public string HandlerVendorId = string.Empty;
    
    public string GoogleStartTime = string.Empty;
    
    public TimeSpan GoogleTrackDuration;
    
    public long MediaDataSize;
    
    public long MediaDataOffset;
    
    public string ImageSize = string.Empty;
    
    public float Megapixels;
    
    public string AvgBitrate = string.Empty;
    
    public float Rotation;

    public int Frames;

    private Dictionary<string, string>? _valueDict;

    #region Constructor

    public ExifData()
    {
    }

    public ExifData(string data)
    {
        LoadData(data);
    }

    #endregion

    #region Public Methods

    public void LoadData(string data)
    {
        if (data.IsNullOrEmpty())
            return;
        _valueDict = BuildFromData(data);
        PopulateFields();
    }
    
    public void LoadData(IEnumerable<string> rows)
    {
        _valueDict = BuildFromData(rows);
        PopulateFields();
    }
    
    public static Dictionary<string, string> BuildFromData(string data)
    {
        return BuildFromData(data.SplitNewline());
    }
    
    public static Dictionary<string, string> BuildFromData(IEnumerable<string> rows)
    {
        return new Dictionary<string, string>(rows.Select(row =>
        {
            string[] values = row.Split(":", 2, StringSplitOptions.TrimEntries);
            return new KeyValuePair<string, string>(values[0], values[1]);
        }));
    }

    #endregion

    #region Private Methods

    private string GetValue(string key)
    {
        return Exists(key) ? _valueDict![key] : string.Empty;
    }
    
    private bool Exists(string key)
    {
        return _valueDict!.ContainsKey(key);
    }


    // If we pass the program -s -s (no spaces), we want we need to put spaces between words...
    private void PopulateFields()
    {
        // Parse the values
        ExifToolVersionNumber       = GetValue("ExifTool Version Number".Remove(" "));
        FileName                    = GetValue("File Name".Remove(" "));
        Directory                   = GetValue("Directory");
        FileSize                    = ParseFileSize("File Size".Remove(" "));
        FileModificationDateTime    = ParseDate("File Modification Date/Time".Remove(" "));
        FileAccessDateTime          = ParseDate("File Access Date/Time".Remove(" "));
        FileCreationDateTime        = ParseDate("File Creation Date/Time".Remove(" "));
        FilePermissions             = GetValue("File Permissions".Remove(" "));
        FileType                    = GetValue("File Type".Remove(" "));
        FileTypeExtension           = GetValue("File Type Extension".Remove(" "));
        Title                       = GetValue("Title");
        Subtitle                    = GetValue("Subtitle");
        Rating                      = GetValue("Rating");
        Tags                        = GetValue("Tags");
        Comments                    = GetValue("Comments");
        MimeType                    = GetValue("MIME Type".Remove(" "));
        MajorBrand                  = GetValue("Major Brand".Remove(" "));
        MinorVersion                = GetValue("Minor Version".Remove(" "));
        CompatibleBrands            = GetValue("Compatible Brands".Remove(" "));
        MovieHeaderVersion          = GetValue("Movie Header Version".Remove(" "));
        CreateDate                  = ParseDate("Create Date".Remove(" "));
        ModifyDate                  = ParseDate("Modify Date".Remove(" "));
        TimeScale                   = ParseInt("Time Scale".Remove(" "));
        Duration                    = ParseTime("Duration");
        PreferredRate               = ParseInt("Preferred Rate".Remove(" "));
        PreferredVolume             = ParsePercent("Preferred Volume".Remove(" "));
        PreviewTime                 = ParseTime("Preview Time".Remove(" "));
        PreviewDuration             = ParseTime("Preview Duration".Remove(" "));
        PosterTime                  = ParseTime("Poster Time".Remove(" "));
        SelectionTime               = ParseTime("Selection Time".Remove(" "));
        SelectionDuration           = ParseTime("Selection Duration".Remove(" "));
        CurrentTime                 = ParseTime("Current Time".Remove(" "));
        NextTrackId                 = ParseLong("Next Track ID".Remove(" "));
        TrackHeaderVersion          = GetValue("Track Header Version".Remove(" "));
        TrackCreateDate             = ParseDate("Track Create Date".Remove(" "));
        TrackModifyDate             = ParseDate("Track Modify Date".Remove(" "));
        TrackId                     = ParseLong("Track ID".Remove(" "));
        TrackDuration               = ParseTime("Track Duration".Remove(" "));
        TrackLayer                  = GetValue("Track Layer".Remove(" "));
        TrackVolume                 = ParsePercent("Track Volume".Remove(" "));
        ImageWidth                  = ParseInt("Image Width".Remove(" "));
        ImageHeight                 = ParseInt("Image Height".Remove(" "));
        CompressorId                = GetValue("Compressor ID".Remove(" "));
        SourceImageWidth            = ParseInt("Source Image Width".Remove(" "));
        SourceImageHeight           = ParseInt("Source Image Height".Remove(" "));
        XResolution                 = ParseInt("X Resolution".Remove(" "));
        YResolution                 = ParseInt("Y Resolution".Remove(" "));
        BitDepth                    = ParseInt("Bit Depth".Remove(" "));
        VideoFrameRate              = ParseFloat("Video Frame Rate".Remove(" "));
        GraphicsMode                = GetValue("Graphics Mode".Remove(" "));
        OpColor                     = GetValue("Op Color".Remove(" "));
        MatrixStructure             = GetValue("Matrix Structure".Remove(" "));
        MediaHeaderVersion          = GetValue("Media Header Version".Remove(" "));
        MediaCreateDate             = ParseDate("Media Create Date".Remove(" "));
        MediaModifyDate             = ParseDate("Media Modify Date".Remove(" "));
        MediaTimeScale              = ParseInt("Media Time Scale".Remove(" "));
        MediaDuration               = ParseTime("Media Duration".Remove(" "));
        MediaLanguageCode           = GetValue("Media Language Code".Remove(" "));
        HandlerDescription          = GetValue("Handler Description".Remove(" "));
        AudioFormat                 = GetValue("Audio Format".Remove(" "));
        AudioChannels               = ParseInt("Audio Channels".Remove(" "));
        AudioBitsPerSample          = ParseInt("Audio Bits Per Sample".Remove(" "));
        AudioSampleRate             = ParseFloat("Audio Sample Rate".Remove(" "));
        Balance                     = GetValue("Balance");
        HandlerType                 = GetValue("Handler Type".Remove(" "));
        HandlerVendorId             = GetValue("Handler Vendor ID".Remove(" "));
        GoogleStartTime             = GetValue("Google Start Time".Remove(" "));
        GoogleTrackDuration         = ParseTime("Google Track Duration".Remove(" "));
        MediaDataSize               = ParseLong("Media Data Size".Remove(" "));
        MediaDataOffset             = ParseLong("Media Data Offset".Remove(" "));
        ImageSize                   = GetValue("Image Size".Remove(" "));
        Megapixels                  = ParseFloat("Megapixels".Remove(" "));
        AvgBitrate                  = GetValue("Avg Bitrate".Remove(" "));
        Rotation                    = ParseFloat("Rotation".Remove(" "));
        Frames                      = CalculateTotalFrames();
    }

    #endregion

    #region Parsing Methods

    private int CalculateTotalFrames()
    {
        return (int) Math.Floor(Duration.TotalSeconds * VideoFrameRate);
    }

    private DateTime ParseDate(string key)
    {
        DateTime result = default;
        string dateString = GetValue(key);
        return dateString.IsNullOrEmpty() || dateString.Before("-") == "0000:00:00 00:00:00" && 
            !DateTime.TryParseExact(dateString.Before("-"), "yyyy:MM:dd HH:mm:sszzzz", null, DateTimeStyles.None, out result) ? 
            default : result;
    }

    private TimeSpan ParseTime(string key)
    {
        string timeString = GetValue(key);
        if (timeString.IsNullOrEmpty() || !TimeSpan.TryParse(timeString, null, out TimeSpan result))
            return default;
        return result;
    }

    private float ParsePercent(string key)
    {
        return Exists(key) ? float.Parse(GetValue(key).Remove("%")) : 0;
    }

    private long ParseLong(string key)
    {
        return Exists(key) ? long.Parse(GetValue(key)) : 0;
    }
    
    private float ParseFloat(string key)
    {
        return Exists(key) ? float.Parse(GetValue(key)) : 0;
    }

    private int ParseInt(string key)
    {
        return Exists(key) ? int.Parse(GetValue(key)) : 0;
    }

    private long ParseFileSize(string key)
    {
        return Exists(key) ? FileSystem.ParseFileSize(GetValue(key)) : 0;
    }

    #endregion
}