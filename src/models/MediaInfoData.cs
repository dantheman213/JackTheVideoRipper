﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JackTheVideoRipper
{
    class MediaInfoData
    {
        [JsonProperty("format_id")]
        public string formatId { get; set; }
        public string description { get; set; }
        public string thumbnail { get; set; }
        public List<MediaFormatItem> formats { get; set; }
        [JsonProperty("requested_formats")]
        public List<MediaFormatItem> requestedFormats { get; set; }
        [JsonProperty("like_count")]
        public string likeCount { get; set; }
        [JsonProperty("upload_date")]
        public string uploadDate { get; set; }
        [JsonProperty("view_count")]
        public string viewCount { get; set; }
        public string duration { get; set; }
        public string fps { get; set; }
        public string uploader { get; set; }
        public string title { get; set; }
        public string format { get; set; }
        [JsonProperty("_filename")]
        public string filename { get; set; }
    }

    class MediaFormatItem
    {
        [JsonProperty("format_id")]
        public string formatId { get; set; }
        public string format { get; set; }
        [JsonProperty("format_note")]
        public string formateNote { get; set; }
        public string ext { get; set; }
        public string vcodec { get; set; }
        public string acodec { get; set; }
        public string filesize { get; set; }
        public string height { get; set; }
        public string width { get; set; }
        public string abr { get; set; } // audio bitrate
        public string vbr { get; set; } // video bitrate
        public string asr { get; set; } // sampling rate
        public string tbr { get; set; } // average bitrate of audio and video in KBit/s
        public string fps { get; set; }
    }
}
