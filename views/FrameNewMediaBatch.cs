using JackTheVideoRipper.extensions;
using JackTheVideoRipper.models.enums;

namespace JackTheVideoRipper
{
    public partial class FrameNewMediaBatch : Form
    {
        private readonly string _startUrls;
        
        public string Urls;
        public MediaType Type;

        public readonly List<DownloadMediaItem> Items = new();

        public FrameNewMediaBatch(string? urls = "")
        {
            InitializeComponent();
            
            if (urls.IsNullOrEmpty())
                return;
            
            // make sure any copy pasting from other sources still has proper windows newlines
            _startUrls = urls.Replace("\r", string.Empty).Replace("\n", "\r\n");
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FrameNewMediaBatch_Load(object sender, EventArgs e)
        {
            if (_startUrls.HasValue())
            {
                textUrls.Text = _startUrls;
            }

            textLocation.Text = Settings.Data.DefaultDownloadPath;

            cbAudioQuality.SelectedIndex = 0;
            cbAudioEncoder.SelectedIndex = 0;

            cbVideoQuality.SelectedIndex = 0;
            cbVideoEncoder.SelectedIndex = 0;
        }

        private void ButtonDownload_Click(object sender, EventArgs e)
        {
            Urls = textUrls.Text.Trim();
            if (Urls.IsNullOrEmpty())
                return;
            
            switch (chkBoxExportVideo.Checked)
            {
                case true when chkBoxExportAudio.Checked:
                // video only
                case true when !chkBoxExportAudio.Checked:
                    // video and audio
                    // TODO: fix
                    Type = MediaType.Video; // TODO: +audio"; ?
                    break;
                case false when chkBoxExportAudio.Checked:
                    // audio only
                    Type = MediaType.Audio;
                    break;
            }

            string videoFormat = cbVideoEncoder.Text.Trim().ToLower() == "low" ? "worstvideo" : "bestvideo";

            string audioFormat = cbAudioEncoder.Text.Trim().ToLower() == "low" ? "worstaudio" : "bestaudio";

            string filePathTemplate = $"{textLocation.Text.Trim()}\\%(title)s.%(ext)s";

            foreach(string url in Urls.Replace("\r", string.Empty).Split('\n'))
            {
                if (!Common.IsValidUrl(url))
                {
                    MessageBox.Show($@"Invalid URL detected: {url}", "Invalid URL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Items.Clear();
                    return;
                }

                // TODO: improve
                // string opts = String.Format("-f bestvideo+bestaudio/best {2} -i --no-check-certificate --prefer-ffmpeg --no-warnings --restrict-filenames {2} {3} -o {4} {5}", (cbVideoEncoder.Enabled && cbVideoEncoder.SelectedIndex > 0 ? "--recode-video " + cbVideoEncoder.Text.Trim() : string.Empty), (chkBoxWriteMetadata.Checked ? "--add-metadata" : string.Empty), (chkBoxEmbedThumbnail.Checked ? "--embed-thumbnail" : string.Empty), (chkBoxIncludeAds.Checked ? "--include-ads" : string.Empty), FrameMain.settings.defaultDownloadPath + "\\%(title)s.%(ext)s", url);

                string opts = chkBoxExportVideo.Checked switch
                {
                    true when chkBoxExportAudio.Checked =>
                        $"-f {videoFormat}+{audioFormat}/best {(cbVideoEncoder.Enabled && cbVideoEncoder.SelectedIndex > 0 ? "--recode-video " + cbVideoEncoder.Text.Trim() : string.Empty)} -i --no-check-certificate --prefer-ffmpeg --no-warnings --restrict-filenames {(cbVideoEncoder.Enabled && cbVideoEncoder.SelectedIndex > 0 ? "--recode-video " + cbVideoEncoder.Text.Trim() : string.Empty)} {(chkBoxWriteMetadata.Checked ? "--add-metadata" : string.Empty)} {(chkBoxEmbedThumbnail.Checked ? "--embed-thumbnail" : string.Empty)} {(chkBoxIncludeAds.Checked ? "--include-ads" : string.Empty)} -o {filePathTemplate} {url}",
                    true when !chkBoxExportAudio.Checked =>
                        $"-f {videoFormat} {(cbVideoEncoder.Enabled && cbVideoEncoder.SelectedIndex > 0 ? "--recode-video " + cbVideoEncoder.Text.Trim() : string.Empty)} -i --no-check-certificate --prefer-ffmpeg --no-warnings --restrict-filenames {(chkBoxWriteMetadata.Checked ? "--add-metadata" : string.Empty)} {(chkBoxEmbedThumbnail.Checked ? "--embed-thumbnail" : string.Empty)} {(chkBoxIncludeAds.Checked ? "--include-ads" : string.Empty)} -o {filePathTemplate} {url}",
                    false when chkBoxExportAudio.Checked =>
                        $"-f {audioFormat} -x --audio-format {cbAudioEncoder.Text.Trim()} --audio-quality 0 -i --no-check-certificate --prefer-ffmpeg --no-warnings --restrict-filenames {(chkBoxWriteMetadata.Checked ? "--add-metadata" : string.Empty)} {(chkBoxIncludeAds.Checked ? "--include-ads" : string.Empty)} {(chkBoxEmbedThumbnail.Checked ? "--embed-thumbnail" : string.Empty)} -o {filePathTemplate} {url}",
                    _ => string.Empty
                };

                Items.Add(new DownloadMediaItem
                {
                    Title = string.Empty,
                    Filepath = string.Empty,
                    Parameters = opts,
                    Url = url
                });
            }
                
          
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonLocationBrowse_Click(object sender, EventArgs e)
        {
            string path = textLocation.Text.Trim();

            FolderBrowserDialog f = new();
            f.SelectedPath = path;
            if (f.ShowDialog() == DialogResult.OK)
            {
                textLocation.Text = f.SelectedPath.Trim();
            }
        }

        private void ChkBoxExportVideo_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkBoxExportVideo.Checked && !chkBoxExportAudio.Checked)
            {
                chkBoxExportVideo.Checked = true;
            }

            switch (chkBoxExportVideo.Checked)
            {
                case true:
                    cbVideoEncoder.Enabled = true;
                    cbVideoQuality.Enabled = true;
                    break;
                case false:
                    cbVideoEncoder.Enabled = false;
                    cbVideoQuality.Enabled = false;
                    break;
            }

            if (chkBoxExportAudio.Checked && !chkBoxExportVideo.Checked)
            {
                cbAudioEncoder.Enabled = true;
            }
            else
            {
                cbAudioEncoder.Enabled = false;
            }
        }

        private void ChkBoxExportAudio_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkBoxExportVideo.Checked && !chkBoxExportAudio.Checked)
            {
                chkBoxExportAudio.Checked = true;
            }

            cbAudioEncoder.Enabled = chkBoxExportAudio.Checked switch
            {
                true when chkBoxExportVideo.Checked => false,
                true when !chkBoxExportVideo.Checked => true,
                _ => cbAudioEncoder.Enabled
            };

            if (!chkBoxExportAudio.Checked)
            {
                cbAudioEncoder.Enabled = false;
                cbAudioQuality.Enabled = false;
            }
            else
            {
                cbAudioQuality.Enabled = true;
            }
        }
    }
}
