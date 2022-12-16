using JackTheVideoRipper.extensions;
using JackTheVideoRipper.models;
using JackTheVideoRipper.models.enums;
using JackTheVideoRipper.modules;
using JackTheVideoRipper.Properties;

namespace JackTheVideoRipper
{
    public partial class FrameNewMedia : Form
    {
        #region Data Members

        public MediaItemRow MediaItemRow;

        private readonly MediaType _startType;

        private string LastValidUrl { get; set; } = string.Empty;

        private readonly FormatManager _formatManager = new();

        #endregion

        #region Properties

        private bool FormatVideo => cbVideoFormat.Enabled && cbVideoEncoder.SelectedIndex > 0;

        private bool FormatAudio => cbAudioEncoder.Enabled && cbAudioEncoder.SelectedIndex > 0;
        
        private bool IsValidVideo => VideoExtension.HasValueAndNotIgnoreCase(FFMPEG.VideoFormats.MP4);

        private bool IsValidAudio => AudioExtension.HasValueAndNotIgnoreCase(FFMPEG.AudioFormats.MP3, FFMPEG.AudioFormats.M4A);
        
        private bool ShouldUpdateAudio => ExportAudio && !FormatVideo;
        
        private string Filename => FileSystem.GetFilename(Filepath);

        #endregion

        #region Form Element Accessors

        private string Title
        {
            get => labelTitle.Text.Trim();
            set => labelTitle.Text = value;
        }

        private string Url
        {
            get => textUrl.Text.Trim();
            set => textUrl.Text = value;
        }

        private string Filepath
        {
            get => textLocation.Text.Trim();
            set => textLocation.Text = value;
        }

        private string Description
        {
            get => labelDescription.Text.Trim();
            set => labelDescription.Text = value;
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

        private int VideoFormatIndex
        {
            get => cbVideoFormat.SelectedIndex;
            set => cbVideoFormat.SelectedIndex = value;
        }
        
        private int AudioFormatIndex
        {
            get => cbAudioFormat.SelectedIndex;
            set => cbAudioFormat.SelectedIndex = value;
        }

        private string AudioEncoder => cbAudioEncoder.Text.Trim();

        private string VideoEncoder => cbVideoEncoder.Text.Trim();

        private string AudioFormat => cbAudioFormat.Text.Trim();

        private string VideoFormat => cbVideoFormat.Text.Trim();

        private string AudioExtension => cbAudioEncoder.Text.Trim();
        
        private string VideoExtension => cbVideoEncoder.Text.Trim();
        
        private ComboBox.ObjectCollection VideoFormatItems => cbVideoFormat.Items;
        private ComboBox.ObjectCollection AudioFormatItems => cbAudioFormat.Items;
        
        private string Username => textUsername.Text;

        private string Password => textPassword.Text;
        
        private bool WriteMetaData => chkBoxWriteMetadata.Checked;

        private bool IncludeAds => chkBoxIncludeAds.Checked;
        
        private bool EmbedThumbnail => chkBoxEmbedThumbnail.Checked;

        private bool EmbedSubtitles => chkEmbedSubs.Checked;

        private string VideoFormatId => _formatManager.GetVideoFormatId(VideoFormat)!;
        
        private string AudioFormatId => _formatManager.GetAudioFormatId(AudioFormat)!;
        
        private object[] VideoFormatRows => _formatManager.GetVideoFormatRows().ToArray<object>();
        
        private object[] AudioFormatRows => _formatManager.GetAudioFormatRows().ToArray<object>();
        
        #endregion

        #region Constructor

        public FrameNewMedia(MediaType type)
        {
            _startType = type;
            InitializeComponent();
            SubscribeEvents();
        }

        #endregion

        #region Private Methods

        private void AddVideoFormats()
        {
            VideoFormatItems.AddRange(VideoFormatRows);
            
            if (VideoFormatItems.Count < 2)
                VideoFormatItems.Add("(no video metadata could be extracted)");
            
            VideoFormatIndex = cbVideoEncoder.Items.Count > 0 ? 0 : 1;
        }

        private void AddAudioFormats()
        {
            AudioFormatItems.AddRange(AudioFormatRows);
            
            if (AudioFormatItems.Count < 2)
                AudioFormatItems.Add("(no audio metadata could be extracted)");
            
            AudioFormatIndex = cbAudioEncoder.Items.Count > 0 ? 0 : 1;
        }

        private void UpdateFormatViewItems()
        {
            AddVideoFormats();
            AddAudioFormats();
        }
        
        private void ClearFormatViewItems()
        {
            VideoFormatItems.Clear();
            AudioFormatItems.Clear();
        }

        private bool ValidateUrl(string url)
        {
            if (url == LastValidUrl || url.Invalid(FileSystem.IsValidUrl))
                return false;
            
            LastValidUrl = url;

            return true;
        }

        private async Task RetrieveMetadata()
        {
            FrameCheckMetadata frameCheckMetadata = new();
            
            Enabled = false;
            
            frameCheckMetadata.Show();
            Application.DoEvents();

            try { await ExtractMediaInfo(); }
            catch (Exception ex)
            {
                FileSystem.LogException(ex);
                Modals.Error(@"Unable to retrieve metadata!");
            }
            
            frameCheckMetadata.Close();
            Enabled = true;
        }

        public async Task ExtractMediaInfo()
        {
            // Meta data lookup failed (happens on initial lookup)
            if (await YouTubeDL.GetMediaData(LastValidUrl) is not { } info)
                return;

            if (info.Formats is {Count: > 0})
                RefreshFormatViews(info);

            UpdateMediaPreview(info.MetaData);
        }

        private void RefreshFormatViews(MediaInfoData mediaInfoData)
        {
            ClearFormatViewItems();
            _formatManager.UpdateAvailableFormats(mediaInfoData);
            UpdateFormatViewItems();
        }

        private void UpdateMediaPreview(MetaData metaData)
        {
            pbPreview.ImageLocation = metaData.ThumbnailFilepath;
            Title = metaData.Title;
            Description = metaData.Description;
            Filepath = metaData.MediaFilepath;
        }

        private void CopyUrlFromClipboard()
        {
            if (FileSystem.GetClipboardAsUrl() is not { } url)
                return;
            
            Url = url;
        }

        private void UpdateFormat(string formatExtension)
        {
            Filepath = $"{Filename}{formatExtension.Split('/')[2].Trim()}";
        }

        private bool ValidateMedia()
        {
            if (!EmbedThumbnail)
                return true;
            
            if (FormatVideo && IsValidVideo)
            {
                Modals.Error(Resources.InvalidVideo);
                return false;
            }
            
            if (FormatAudio && IsValidAudio)
            {
                Modals.Error(Resources.InvalidAudio);
                return false;
            }

            return true;
        }

        private void GenerateDownloadCommand()
        {
            if (!ValidateMedia())
                return;

            GenerateMediaItemRow();
        }

        private void GenerateMediaItemRow()
        {
            MediaParameters mediaParameters = new(LastValidUrl)
            {
                FilenameFormatted = Filepath,
                Username = Username,
                Password = Password,
                EncodeVideo = FormatVideo,
                AddMetaData = WriteMetaData,
                IncludeAds = IncludeAds,
                EmbedThumbnail = EmbedThumbnail,
                EmbedSubtitles = EmbedSubtitles,
                ExportAudio = ExportAudio,
                ExportVideo = ExportVideo,
                AudioFormat = AudioExtension,
                VideoFormat = VideoExtension,
                VideoFormatId = VideoFormatId,
                AudioFormatId = AudioFormatId,
                RunMultiThreaded = Settings.Data.EnableMultiThreadedDownloads
            };

            MediaType mediaType = ExportVideo ? MediaType.Video : MediaType.Audio;

            MediaItemRow = new MediaItemRow(LastValidUrl, Title, Filepath, mediaType, mediaParameters);
        }

        private void ToggleVideo(bool enabled)
        {
            cbVideoFormat.Enabled = enabled;
            cbVideoEncoder.Enabled = enabled;
            cbAudioEncoder.Enabled = !enabled;
        }
        
        private void ToggleAudio(bool enabled)
        {
            cbAudioFormat.Enabled = enabled;
            cbAudioEncoder.Enabled = enabled && !ExportVideo;
            cbVideoEncoder.Enabled = !enabled || ExportVideo;
        }
        
        private void Download()
        {
            if (!LastValidUrl.Invalid(FileSystem.IsValidUrl))
            {
                if (!FileSystem.WarnIfFileExists(Filepath))
                    return;

                GenerateDownloadCommand();
                this.Close(DialogResult.OK);
            }
            else
            {
                Modals.Warning("Failed to parse provided url!");
            }
        }
        
        private void Browse()
        {
            if (LastValidUrl.IsNullOrEmpty() || Filepath.IsNullOrEmpty()) 
                return;

            if (FileSystem.SaveCopy(Filepath) is { } result && result.HasValue())
                Filepath = result;
        }
        
        private void UpdateAudioEncoder()
        {
            if (ShouldUpdateAudio)
                Filepath = $"{Filename}.{AudioExtension}";
        }

        private void UpdateAudioFormat()
        {
            if (ShouldUpdateAudio)
                UpdateFormat(AudioFormat);
        }
        
        private void UpdateVideoEncoder()
        {
            if (FormatVideo) 
                Filepath = $"{Filename}.{VideoExtension}";
        }

        private void UpdateVideoFormat()
        {
            if (FormatVideo)
                UpdateFormat(VideoFormat);
        }
        
        private void GetCommand()
        {
            if (!Url.HasValue())
                return;
            
            GenerateDownloadCommand();
                
            FileSystem.SetClipboardText($"{YouTubeDL.ExecutablePath} {MediaItemRow.MediaParameters}");

            Modals.Notification(@"Command copied to clipboard!", @"Generate Command");
        }
        
        private void OnCheckExportAudioChanged()
        {
            if (!ExportAudio && !ExportVideo)
                ExportAudio = true;
            
            if (ExportAudio)
            {
                ToggleAudio(true);
                UpdateAudioEncoder();
            }
            else
            {
                ToggleAudio(false);
            }
        }

        private void OnCheckExportVideoChanged()
        {
            // We must export one of the two
            if (!ExportVideo && !ExportAudio)
                ExportVideo = true;

            if (ExportVideo)
            {
                ToggleVideo(true);
                UpdateVideoEncoder();
            }
            else
            {
                ToggleVideo(false);
                tabImportType.SelectedTab = tabPageAudio;
            }
        }
        
        private void Cancel()
        {
            this.Close(DialogResult.Cancel);
        }
        
        #endregion

        #region Static Methods

        public static MediaItemRow? GetMedia(MediaType type)
        {
            FrameNewMedia frameNewMedia = new(type);
            return frameNewMedia.Confirm() ? frameNewMedia.MediaItemRow : null;
        }

        #endregion

        #region Timer Events

        private void TimerPostLoad_Tick(object sender, EventArgs e)
        {
            timerPostLoad.Enabled = false;
            CopyUrlFromClipboard();
            TextUrl_TextChanged(sender, e);
        }

        #endregion

        #region Event Handlers

        private void SubscribeEvents()
        {
            buttonDownload.Click += (_, _) => Download();
            buttonLocationBrowse.Click += (_, _) => Browse();
            
            buttonCancel.Click += (_, _) => Cancel();

            cbAudioEncoder.TextUpdate += (_, _) => UpdateAudioEncoder();
            cbAudioFormat.TextUpdate += (_, _) => UpdateAudioFormat();

            cbVideoEncoder.TextUpdate += (_, _) => UpdateVideoEncoder();
            cbVideoFormat.TextUpdate += (_, _) => UpdateVideoFormat();

            cbVideoFormat.SelectedIndexChanged += (_, _) =>
            {
                if (VideoFormatIndex == 0) VideoFormatIndex = 1;
            };

            cbAudioFormat.SelectedIndexChanged += (_, _) =>
            {
                if (AudioFormatIndex == 0) AudioFormatIndex = 1;
            };

            buttonGetCommand.Click += (_, _) => GetCommand();
            
            chkBoxExportAudio.CheckedChanged += (_, _) => OnCheckExportAudioChanged();
            chkBoxExportVideo.CheckedChanged += (_, _) => OnCheckExportVideoChanged();
        }

        private void FrameNewMedia_Load(object sender, EventArgs e)
        {
            if (_startType == MediaType.Audio)
                ExportVideo = false;
        }

        private async void TextUrl_TextChanged(object sender, EventArgs e)
        {
            await Input.WaitForFinishTyping(() => Url).ContinueWith(async _ =>
            {
                if (!ValidateUrl(Url))
                    return;

                if (!Settings.Data.SkipMetadata)
                    await RetrieveMetadata();
            });
        }

        #endregion
    }
}
