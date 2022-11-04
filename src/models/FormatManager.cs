using System.Text.RegularExpressions;
using JackTheVideoRipper.extensions;
using JackTheVideoRipper.models.enums;

namespace JackTheVideoRipper;

public class FormatManager
{
    private readonly Dictionary<string, string> _availableVideoFormats = new();
    private readonly Dictionary<string, string> _availableAudioFormats = new();
    private readonly List<string> _videoFormatList = new();
    private readonly List<string> _audioFormatList = new();
    
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
        switch (mediaType)
        {
            case MediaType.Video:
                return RecommendedVideoFormat.IsNullOrEmpty();
            case MediaType.Audio:
                return RecommendedAudioFormat.IsNullOrEmpty();
            default:
                throw new NotImplementedException();
        }
    }

    public void UpdateAvailableFormats(MediaInfoData mediaInfoData)
    {
        bool hasRequestedFormats = mediaInfoData.RequestedFormats.Count > 0;
        
        ResetAvailableFormats();

        foreach (MediaFormatItem format in mediaInfoData.AvailableFormats)
        {
            if (format.HasVideo && format.VideoString() is { } videoFormat && IsValidFormat(videoFormat))
            {
                if (NoRecommended(MediaType.Video) && hasRequestedFormats)
                {
                    videoFormat = $"{videoFormat} {Tags.RECOMMENDED}";
                    RecommendedVideoFormat = videoFormat;
                }
                else
                {
                    _videoFormatList.Add(videoFormat);
                }

                AddFormat(MediaType.Video, videoFormat, format.FormatId!);
            }

            if (format.HasAudio && format.AudioString() is { } audioFormat && IsValidFormat(audioFormat))
            {
                if (NoRecommended(MediaType.Audio) && hasRequestedFormats)
                {
                    audioFormat = $"{audioFormat} {Tags.RECOMMENDED}";
                    RecommendedAudioFormat = audioFormat;
                }
                else
                {
                    _audioFormatList.Add(audioFormat);
                }

                AddFormat(MediaType.Audio, audioFormat, format.FormatId!);
            }
        }
    }

    private static bool IsValidFormat(string str)
    {
        return !(str.Contains(Tags.UNRECOGNIZED_CODEC) || str.Contains(Tags.NONE));
    }
    
    public IEnumerable<string> GetAudioFormatRows()
    {
        var audioFormatRows = new List<string>
        {
            "Bitrate / Sample Rate / Format / Codec"
        };

        if (HasRecommendedAudioFormat)
        {
            audioFormatRows.Add(RecommendedAudioFormat);
        }

        return audioFormatRows.Concat(SortedAudioFormats);
    }

    public IEnumerable<string> GetVideoFormatRows()
    {
        var videoFormatRows = new List<string>
        {
            "Resolution / Bitrate / Format / Type / Additional Info"
        };

        if (HasRecommendedVideoFormat)
        {
            videoFormatRows.Add(RecommendedVideoFormat);
        }

        return videoFormatRows.Concat(GetReversedVideoFormats); // TODO: optimize this out
    }
}