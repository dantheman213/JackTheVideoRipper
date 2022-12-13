using System.Text.RegularExpressions;
using JackTheVideoRipper.extensions;
using JackTheVideoRipper.models.enums;
using JackTheVideoRipper.Properties;

namespace JackTheVideoRipper.models;

public class FormatManager
{
    private readonly Dictionary<string, string> _availableVideoFormats = new();
    private readonly Dictionary<string, string> _availableAudioFormats = new();
    private readonly List<string> _videoFormatList = new();
    private readonly List<string> _audioFormatList = new();
    
    private readonly string[] AudioFormatRows = {"Bitrate / Sample Rate / Format / Codec"};
    private readonly string[] VideoFormatRows = {"Resolution / Bitrate / Format / Type / Additional Info"};
    
    public void ResetAvailableFormats()
    {
        _availableVideoFormats.Clear();
        _availableAudioFormats.Clear();
    }
    
    public void AddFormat(MediaType mediaType, string key, string formatId)
    {
        switch (mediaType)
        {
            case MediaType.Video when !_availableVideoFormats.ContainsKey(key):
                _availableVideoFormats.Add(key, formatId);
                break;
            case MediaType.Audio when !_availableAudioFormats.ContainsKey(key):
                _availableAudioFormats.Add(key, formatId);
                break;
            case MediaType.Image:
            default:
                throw new ArgumentOutOfRangeException(nameof(mediaType), mediaType, null);
        }
    }

    private readonly Regex _extractDecimalPattern = new(@"([^\s]+)", RegexOptions.Compiled);

    private IEnumerable<string> SortedAudioFormats =>
        _audioFormatList.OrderByDescending(x => double.Parse(_extractDecimalPattern.Match(x).Groups[1].Value));

    public string? GetVideoFormatId(string videoFormat)
    {
        return _availableVideoFormats.TryGetValue(videoFormat, out string? videoFormatId) ? videoFormatId : null;
    }
    
    public string? GetAudioFormatId(string audioFormat)
    {
        return _availableAudioFormats.TryGetValue(audioFormat, out string? audioFormatId) ? audioFormatId : null;
    }

    public bool HasRecommendedVideoFormat => RecommendedVideoFormat.HasValue();
    
    public bool HasRecommendedAudioFormat => RecommendedAudioFormat.HasValue();

    public IEnumerable<string> GetReversedVideoFormats => _videoFormatList.Reversed();
    
    public IEnumerable<string> GetReversedAudioFormats => _audioFormatList.Reversed();

    public string RecommendedVideoFormat { get; private set; } = string.Empty;

    public string RecommendedAudioFormat { get; private set; } = string.Empty;

    private bool NoRecommended(MediaType mediaType)
    {
        return mediaType switch
        {
            MediaType.Video => RecommendedVideoFormat.IsNullOrEmpty(),
            MediaType.Audio => RecommendedAudioFormat.IsNullOrEmpty(),
            MediaType.Image => throw new NotImplementedException(),
            _ => throw new NotImplementedException()
        };
    }

    public void UpdateAvailableFormats(MediaInfoData mediaInfoData)
    {
        bool hasRequestedFormats = mediaInfoData.RequestedFormats.Count > 0;
        
        ResetAvailableFormats();

        foreach (MediaFormatItem format in mediaInfoData.AvailableFormats)
        {
            if (format.HasVideo && format.VideoString() is { } videoFormat)
                UpdateFormats(format, MediaType.Video, videoFormat, hasRequestedFormats);

            if (format.HasAudio && format.AudioString() is { } audioFormat)
                UpdateFormats(format, MediaType.Audio, audioFormat, hasRequestedFormats);
        }
    }

    private void UpdateFormats(MediaFormatItem format, MediaType mediaType, string mediaFormat, bool hasRequestedFormats)
    {
        if (mediaFormat.Invalid(IsValidFormat))
            return;
        
        if (NoRecommended(mediaType) && hasRequestedFormats)
        {
            mediaFormat = $"{mediaFormat} {Tags.RECOMMENDED}";
            RecommendedVideoFormat = mediaFormat;
        }
        else switch (mediaType)
        {
            case MediaType.Video:
                _videoFormatList.Add(mediaFormat);
                break;
            case MediaType.Audio:
                _audioFormatList.Add(mediaFormat);
                break;
        }

        AddFormat(mediaType, mediaFormat, format.FormatId!);
    }

    private static bool IsValidFormat(string str)
    {
        return !(str.Contains(Text.UNRECOGNIZED_CODEC) || str.Contains(Text.NONE));
    }

    public IEnumerable<string> GetAudioFormatRows()
    {
        var audioFormatRows = AudioFormatRows.ToList();

        if (HasRecommendedAudioFormat)
            audioFormatRows.Add(RecommendedAudioFormat);

        return audioFormatRows.Concat(SortedAudioFormats);
    }

    public IEnumerable<string> GetVideoFormatRows()
    {
        var videoFormatRows = VideoFormatRows.ToList();

        if (HasRecommendedVideoFormat)
            videoFormatRows.Add(RecommendedVideoFormat);

        return videoFormatRows.Concat(GetReversedVideoFormats); // TODO: optimize this out
    }
}