using JackTheVideoRipper.extensions;
using JackTheVideoRipper.models.enums;

namespace JackTheVideoRipper
{
    public partial class FrameNewMediaBatch : Form
    {
        #region Data Members

        public readonly List<DownloadMediaItem> Items = new();

        #endregion

        #region Properties
        
        public MediaType Type => ExportAudio && !ExportVideo ? MediaType.Audio : MediaType.Video;

        private string FilePathTemplate => Path.Combine(Filepath, "%(title)s.%(ext)s");

        private bool EncodeVideo => cbVideoEncoder.Enabled && cbVideoEncoder.SelectedIndex > 0;

        private bool EncodeAudio => cbAudioEncoder.Enabled && cbAudioEncoder.SelectedIndex > 0;
        
        private bool IsValidVideo => VideoExtension.HasValueAndNot("mp4");

        private bool IsValidAudio => AudioExtension.HasValueAndNot("mp3", "m4a");
        
        private bool ShouldUpdateAudio => ExportAudio && !EncodeVideo;
        
        private string Filename => FileSystem.GetFilename(Filepath);

        #endregion

        #region Form Element Accessors

        private string Filepath
        {
            get => textLocation.Text.Trim();
            set => textLocation.Text = value;
        }

        private string Urls
        {
            get => textUrls.Text.Trim();
            set => textUrls.Text = value;
        }
        
        private bool ExportAudio
        {
            get => chkBoxExportAudio.Checked;
            set => chkBoxExportAudio.Checked = value;
        }
        
        private bool ExportVideo
        {
            get => chkBoxExportVideo.Checked;
            set => chkBoxExportVideo.Checked = value;
        }

        private string AudioEncoder => cbAudioEncoder.Text.Trim();

        private string VideoEncoder => cbVideoEncoder.Text.Trim();

        private string AudioExtension => cbAudioEncoder.Text.Trim();
        
        private string VideoExtension => cbVideoEncoder.Text.Trim();
        
        private bool WriteMetaData => chkBoxWriteMetadata.Checked;

        private bool IncludeAds => chkBoxIncludeAds.Checked;
        
        private bool EmbedThumbnail => chkBoxEmbedThumbnail.Checked;

        private bool EmbedSubtitles => chkEmbedSubs.Checked;
        
        private string VideoFormat => VideoEncoder.ToLower() == Tags.LOW ? Tags.WORST_VIDEO : Tags.BEST_VIDEO;

        private string AudioFormat => AudioEncoder.ToLower() == Tags.LOW ? Tags.WORST_AUDIO : Tags.BEST_AUDIO;
        
        #endregion

        #region Constructor

        public FrameNewMediaBatch(string urls = "")
        {
            InitializeComponent();
            
            if (urls.IsNullOrEmpty())
                return;
            
            // make sure any copy pasting from other sources still has proper windows newlines
            Urls = urls.Remove("\r").Replace("\n", "\r\n");
        }

        #endregion

        #region Event Handlers

        private void FrameNewMediaBatch_Load(object sender, EventArgs e)
        {
            Filepath = Settings.Data.DefaultDownloadPath;
            ResetSelectorIndices();
        }
        
        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ButtonDownload_Click(object sender, EventArgs e)
        {
            if (Urls.IsNullOrEmpty())
                return;
            
            Urls.Remove("\r").Split('\n').ForEach(ProcessUrl);

            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonLocationBrowse_Click(object sender, EventArgs e)
        {
            if (FileSystem.SelectFile() is { } filepath)
                Filepath = filepath;
        }
        
        private void ChkBoxExportAudio_CheckedChanged(object sender, EventArgs e)
        {
            if (!ExportAudio && !ExportVideo)
                ExportAudio = true;
            
            if (ExportAudio)
            {
                ToggleAudio(true);
                CbAudioEncoder_TextChanged();
            }
            else
            {
                ToggleAudio(false);
            }
        }

        private void ChkBoxExportVideo_CheckedChanged(object sender, EventArgs e)
        {
            // We must export one of the two
            if (!ExportVideo && !ExportAudio)
                ExportVideo = true;

            if (ExportVideo)
            {
                ToggleVideo(true);
                CbVideoEncoder_TextChanged();
            }
            else
            {
                ToggleVideo(false);
                //tabImportType.SelectedTab = tabPageAudio;
            }
        }

        private void CbVideoEncoder_TextChanged()
        {
            if (!EncodeVideo) 
                return;
            Filepath = $"{Filename}.{VideoExtension}";
        }
        
        private void CbAudioEncoder_TextChanged()
        {
            if (ShouldUpdateAudio)
                return;
            Filepath = $"{Filename}.{AudioExtension}";
        }
        
        #endregion

        #region Private Methods

        private void ResetSelectorIndices()
        {
            cbAudioQuality.SelectedIndex = 0;
            cbAudioEncoder.SelectedIndex = 0;

            cbVideoQuality.SelectedIndex = 0;
            cbVideoEncoder.SelectedIndex = 0;
        }
        
        private void ToggleVideo(bool enabled)
        {
            cbVideoEncoder.Enabled = enabled;
            cbAudioEncoder.Enabled = !enabled;
        }
        
        private void ToggleAudio(bool enabled)
        {
            cbAudioEncoder.Enabled = enabled && !ExportVideo;
            cbVideoEncoder.Enabled = !enabled || ExportVideo;
        }
        
        private void AddItem(string parameters, string url)
        {
            Items.Add(new DownloadMediaItem
            {
                Title = string.Empty,
                Filepath = string.Empty,
                Parameters = parameters,
                Url = url,
                MediaType = Type
            });
        }
        
        private void ProcessUrl(string url)
        {
            if (!FileSystem.IsValidUrl(url))
            {
                Core.SendNotification($"Invalid URL detected, skipping: {url}");
                //Modals.Warning($@"Invalid URL detected, skipping: {url}", @"Invalid URL");
                return;
            }

            Parameters parameters = new()
            {
                ExportAudio = ExportAudio,
                ExportVideo = ExportVideo,
                EncodeVideo = EncodeVideo,
                VideoFormat = VideoFormat,
                AudioFormat = AudioFormat,
                AddMetaData = WriteMetaData,
                IncludeAds = IncludeAds,
                EmbedThumbnail = EmbedThumbnail,
                EmbedSubtitles = EmbedSubtitles,
                FilenameFormatted = FilePathTemplate
            };

            AddItem(parameters.ToString(), url);
        }

        #endregion
        
        #region Static Methods

        public static List<DownloadMediaItem>? GetMedia(string urls)
        {
            FrameNewMediaBatch frameNewMediaBatch = new(urls);

            if (frameNewMediaBatch.ShowDialog() != DialogResult.OK ||
                frameNewMediaBatch.Items is not {Count: > 0} items)
                return null;

            return items;
        }

        #endregion
    }
}
