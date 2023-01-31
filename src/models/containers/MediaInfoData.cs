using JackTheVideoRipper.extensions;
using Newtonsoft.Json;

namespace JackTheVideoRipper
{
    public class MediaInfoData
    {
        [JsonProperty("format_id")]
        public string? FormatId { get; set; }
        
        [JsonProperty("description")]
        public string? Description { get; set; }
        
        [JsonProperty("thumbnail")]
        public string? Thumbnail { get; set; }
        
        [JsonProperty("formats")]
        public List<MediaFormatItem> Formats { get; set; } = new();
        
        [JsonProperty("requested_formats")]
        public List<MediaFormatItem> RequestedFormats { get; set; } = new();
        
        [JsonProperty("like_count")]
        public string? LikeCount { get; set; }
        
        [JsonProperty("upload_date")]
        public string? UploadDate { get; set; }
        
        [JsonProperty("view_count")]
        public string? ViewCount { get; set; }
        
        [JsonProperty("duration")]
        public string? Duration { get; set; }
        
        [JsonProperty("fps")]
        public string? Fps { get; set; }
        
        [JsonProperty("uploader")]
        public string? Uploader { get; set; }
        
        [JsonProperty("title")]
        public string? Title { get; set; }
        
        [JsonProperty("format")]
        public string? Format { get; set; }
        
        [JsonProperty("_filename")]
        public string? Filename { get; set; }

        #region Attributes

        [JsonIgnore]
        public IEnumerable<MediaFormatItem> AvailableFormats => RequestedFormats.Take(2).Concat(Formats);

        [JsonIgnore]
        public MetaData MetaData => new(this);

        #endregion
    }

    public class MediaFormatItem
    {
        [JsonProperty("format_id")]
        public string? FormatId { get; set; }
        
        [JsonProperty("format")]
        public string? Format { get; set; }
        
        [JsonProperty("format_note")]
        public string? FormatNote { get; set; }
        
        [JsonProperty("ext")]
        public string? Ext { get; set; }
        
        [JsonProperty("vcodec")]
        public string? VCodec { get; set; }
        
        [JsonProperty("acodec")]
        public string? ACodec { get; set; }
        
        [JsonProperty("filesize")]
        public string? Filesize { get; set; }
        
        [JsonProperty("height")]
        public string? Height { get; set; }
        
        [JsonProperty("width")]
        public string? Width { get; set; }
        
        [JsonProperty("abr")]
        public string? Abr { get; set; } // audio bitrate
        
        [JsonProperty("vbr")]
        public string? Vbr { get; set; } // video bitrate
        
        [JsonProperty("asr")]
        public string? Asr { get; set; } // sampling rate
        
        [JsonProperty("tbr")]
        public string? Tbr { get; set; } // average bitrate of audio and video in KBit/s
        
        [JsonProperty("fps")]
        public string? Fps { get; set; }

        #region Attributes

        [JsonIgnore]
        public bool HasVideo => Width.HasValue() && Height.HasValue();

        [JsonIgnore]
        public bool HasAudio => ACodec.HasValueAndNot(Text.None);

        [JsonIgnore]
        public string Bitrate => Abr.FormattedOrDefault("{0} kbps", Text.Dashes);
        
        [JsonIgnore]
        public string SampleRate => Asr.FormattedOrDefault("{0} Hz", Text.Dashes);

        [JsonIgnore]
        public string Codec => VCodec.HasValueAndNot(Text.None) ? VCodec! : Text.UnrecognizedCodec;

        [JsonIgnore]
        public string AvgBitrate => Tbr.HasValue() ? $"{Math.Floor(Convert.ToDecimal(Tbr))} k" : Text.Dashes;
        
        [JsonIgnore]
        public string DisplayFps => Fps.FormattedOrDefault("{0} fps", Text.Dashes);

        [JsonIgnore]
        public string DisplayNote => FormatNote.ValueOrDefault(Text.Dashes);
        
        public string VideoString()
        {
            return $"{Width,-4} x {Height,4} / {AvgBitrate,-7} / {Ext,-5} / {DisplayNote,-6} / {DisplayFps,6} {Codec}";
        }

        public string AudioString()
        {
            return $"{Bitrate,-9} / {SampleRate,8} / {Ext,-5} / {ACodec}";
        }
        
        #endregion
    }
}
