using JackTheVideoRipper.Properties;

namespace JackTheVideoRipper
{
    public partial class FrameNewMedia : Form
    {
        public string Title;
        public string Type;
        public string Url;
        public string Parameters;
        public string Filepath;

        private string startType;
        private string? lastValidUrl;
        private Dictionary<string, string> videoIdLookupTable = new();
        private Dictionary<string, string> audioIdLookupTable = new();

        private const string Dashes = "---";

        public FrameNewMedia(string type)
        {
            startType = type;
            InitializeComponent();
        }

        private void FrameNewMedia_Load(object sender, EventArgs e)
        {
            if (startType == "audio")
            {
                chkBoxExportVideo.Checked = false;
            }
        }

        private readonly List<Task<bool>> _taskTypeQueue = new();
        private async void TextUrl_TextChanged(object sender, EventArgs e)
        {
            async Task<bool> IsStillTyping()
            {
                Application.DoEvents();

                int taskCount = _taskTypeQueue.Count;
                string oldStr = textUrl.Text;
                await Task.Delay(1800);

                return oldStr != textUrl.Text || taskCount != _taskTypeQueue.Count - 1;
            }

            _taskTypeQueue.Add(IsStillTyping());
            if (await _taskTypeQueue[^1])
                return;

            // typing appears to have stopped, continue
            _taskTypeQueue.Clear();
            IngestMediaUrl();
        }

        private void IngestMediaUrl()
        {
            FrameCheckMetadata frameCheckMetadata = new();
            try
            {
                string url = textUrl.Text.Trim();
                if (url != lastValidUrl && Common.IsValidUrl(url))
                {
                    lastValidUrl = url;

                    Enabled = false;
                    frameCheckMetadata.Show();
                    Application.DoEvents();

                    MediaInfoData? info = YouTubeDl.GetMediaData(url);

                    // Meta data lookup failed (happens on initial lookup)
                    if (info is null)
                        return;

                    string? thumbnailFilePath = YouTubeDl.DownloadThumbnail(info.thumbnail);
                    pbPreview.ImageLocation = thumbnailFilePath;
                  
                    labelTitle.Text = info.title;
                    labelDescription.Text = info.description;
                    // TODO: may need to be revised now that using --restrict-filenames flag in youtube-dl

                    string filename =
                        $@"{Common.StripIllegalFileNameChars(info.filename[..info.filename.LastIndexOf('.')])}{info.filename[info.filename.LastIndexOf('.')..]}";

                    textLocation.Text = Path.Combine(Settings.Data.DefaultDownloadPath, filename);

                    if (info.formats is { Count: > 0 })
                    {
                        cbVideoFormat.Items.Clear();
                        cbAudioFormat.Items.Clear();

                        if (info.requestedFormats is { Count: > 0 })
                        {
                            info.formats.Insert(0, info.requestedFormats[0]);

                            if (info.requestedFormats.Count > 1)
                            {
                                info.formats.Insert(0, info.requestedFormats[1]);
                            }
                        }

                        string recommendedVideoFormat = "";
                        string recommendedAudioFormat = "";
                        List<string> videoFormatList = new();
                        List<string> audioFormatList = new();
                        videoIdLookupTable = new Dictionary<string, string>();
                        audioIdLookupTable = new Dictionary<string, string>();

                        foreach (MediaFormatItem format in info.formats)
                        {
                            if (!string.IsNullOrEmpty(format.width) && !string.IsNullOrEmpty(format.height))
                            {
                                string codec = !string.IsNullOrEmpty(format.vcodec) && format.vcodec != "none" ? 
                                    format.vcodec : "unrecognized codec";
                                string tbr = !string.IsNullOrEmpty(format.tbr) ? 
                                    Math.Floor(Convert.ToDecimal(format.tbr)) + "k" : Dashes; // rounds down
                                string fps = !string.IsNullOrEmpty(format.fps) ? 
                                    format.fps + "fps" : Dashes;
                                string note = !string.IsNullOrEmpty(format.formateNote) ? 
                                    format.formateNote : Dashes;
                                string str =
                                    $"{format.width.PadRight(4)} x {format.height.PadLeft(4)} / {tbr.PadRight(7)} / {format.ext.PadRight(5)} / {note.PadRight(6)} / {fps.PadLeft(6)} {codec}";

                                if (info.requestedFormats != null && string.IsNullOrEmpty(recommendedVideoFormat))
                                {
                                    str = $"{str} [Recommended]";
                                    recommendedVideoFormat = str;
                                }
                                else
                                {
                                    videoFormatList.Add(str);
                                }

                                if (!videoIdLookupTable.ContainsKey(str))
                                {
                                    videoIdLookupTable.Add(str, format.formatId);
                                }
                            }

                            if (!string.IsNullOrEmpty(format.acodec) && format.acodec != "none")
                            {
                                string bitrate = string.IsNullOrEmpty(format.abr) ? Dashes : format.abr + " kbps";
                                string sampleRate = string.IsNullOrEmpty(format.asr) ? Dashes : format.asr + "Hz";
                                string str =
                                    $"{bitrate.PadRight(9)} / {sampleRate.PadLeft(8)} / {format.ext.PadRight(5)} / {format.acodec}";

                                if (info.requestedFormats != null && string.IsNullOrEmpty(recommendedAudioFormat))
                                {
                                    str = $"{str} [Recommended]";
                                    recommendedAudioFormat = str;
                                }
                                else
                                {
                                    audioFormatList.Add(str);
                                }

                                if (!audioIdLookupTable.ContainsKey(str))
                                {
                                    audioIdLookupTable.Add(str, format.formatId);
                                }
                            }
                        }

                        cbVideoFormat.Items.Add("Resolution / Bitrate / Format / Type / Additional Info");
                        if (!string.IsNullOrEmpty(recommendedVideoFormat))
                        {
                            cbVideoFormat.Items.Add(recommendedVideoFormat);
                        }

                        //try
                        //{
                            // attempt to sort by bitrate but not sure it's necessary
                            //videoFormatList.Sort((x, y) => Int32.Parse(Common.RemoveAllNonNumericValuesFromString(x.Trim().Split('/')[0].Trim()))
                            //                    .CompareTo(Int32.Parse(Common.RemoveAllNonNumericValuesFromString(y.Trim().Split('/')[1].Trim()))
                            //                ));
                        //}
                        //catch (FormatException ex)
                        //{
                        //    Console.WriteLine(ex);
                        //}
                        videoFormatList.Reverse(); // TODO: optimize this out
                        foreach (string item in videoFormatList)
                        {
                            cbVideoFormat.Items.Add(item);
                        }

                        // audio
                        cbAudioFormat.Items.Add("Bitrate / Sample Rate / Format / Codec");

                        if (!string.IsNullOrEmpty(recommendedAudioFormat))
                        {
                            cbAudioFormat.Items.Add(recommendedAudioFormat);
                        }

                        try
                        {
                            audioFormatList.Sort((x, y) => double.Parse(x.Trim().Split(' ')[0])
                                .CompareTo(double.Parse(y.Trim().Split(' ')[0])));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }

                        audioFormatList.Reverse(); // TODO: optimize this out

                        foreach (string item in audioFormatList)
                        {
                            cbAudioFormat.Items.Add(item);
                        }

                        if (cbVideoFormat.Items.Count < 2)
                        {
                            cbVideoFormat.Items.Add("(no video metadata could be extracted)");
                        }
                        cbVideoFormat.SelectedIndex = 1;
                        if (cbAudioFormat.Items.Count < 2)
                        {
                            cbAudioFormat.Items.Add("(no audio metadata could be extracted)");
                        }
                        cbAudioFormat.SelectedIndex = 1;

                        // -- 
                        if (cbVideoEncoder.Items.Count > 0)
                        {
                            cbVideoEncoder.SelectedIndex = 0;
                        }
                        if (cbAudioEncoder.Items.Count > 0)
                        {
                            cbAudioEncoder.SelectedIndex = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                MessageBox.Show(@"Unable to detect metadata!", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            frameCheckMetadata.Close();
            Enabled = true;
        }

        private void TimerPostLoad_Tick(object sender, EventArgs e)
        {
            timerPostLoad.Enabled = false;

            string clipboard = Clipboard.GetText().Trim();
            if (!Common.IsValidUrl(clipboard)) 
                return;
            textUrl.Text = clipboard;
            TextUrl_TextChanged(sender, e);
        }

        private void ButtonDownload_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textUrl.Text.Trim()))
            {
                GenerateDownloadCommand();
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                // TODO?
            }
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void ButtonLocationBrowse_Click(object sender, EventArgs e)
        {
            if (textUrl.Text.Trim() == "" || textLocation.Text.Trim() == "") 
                return;

            string? dir = textLocation.Text[..textLocation.Text.LastIndexOf("\\", StringComparison.Ordinal)];
            string? fileName = textLocation.Text[(textLocation.Text.LastIndexOf("\\", StringComparison.Ordinal) + 1)..];
            string? ext = fileName[(fileName.LastIndexOf(".", StringComparison.Ordinal) + 1)..];

            SaveFileDialog saveFileDialog = new()
            {
                InitialDirectory = dir, // FrameMain.settings.defaultDownloadPath;      
                FileName = fileName,
                Filter = $@"{ext} file|*.{ext}|All files (*.*)|*.*"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                textLocation.Text = saveFileDialog.FileName;
            }
        }

        private void ChkBoxExportAudio_CheckedChanged(object sender, EventArgs e)
        {
            switch (chkBoxExportAudio.Checked)
            {
                case false when !chkBoxExportVideo.Checked:
                    chkBoxExportAudio.Checked = true;
                    break;
                case true when !chkBoxExportVideo.Checked:
                    cbAudioEncoder.Enabled = true;
                    break;
                case true:
                    cbAudioFormat.Enabled = true;
                    CbAudioEncoder_TextChanged(sender, e); // TODO: fix
                    break;
                default:
                    cbAudioFormat.Enabled = false;
                    cbAudioEncoder.Enabled = false;
                    break;
            }
        }

        private void ChkBoxExportVideo_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkBoxExportVideo.Checked && !chkBoxExportAudio.Checked)
            {
                chkBoxExportVideo.Checked = true;
            }

            if (chkBoxExportVideo.Checked)
            {
                cbVideoFormat.Enabled = true;
                cbVideoEncoder.Enabled = true;
                cbAudioEncoder.Enabled = false;
                CbVideoEncoder_TextChanged(sender, e); // TODO: fix
            }
            else
            {
                cbVideoFormat.Enabled = false;
                cbVideoEncoder.Enabled = false;
                cbAudioEncoder.Enabled = true;

                tabImportType.SelectedTab = tabPageAudio;
            }
        }

        private string GetFilename(string filepath)
        {
            return filepath[..(filepath.LastIndexOf('.') + 1)];
        }

        private void CbVideoEncoder_TextChanged(object sender, EventArgs e)
        {
            if (IgnoreTextChanged) 
                return;
            string filePath = textLocation.Text.Trim();
            filePath = $"{GetFilename(filePath)}{cbVideoEncoder.Text.Trim()}";
            textLocation.Text = filePath;
        }
        
        private bool IgnoreTextChanged => !cbVideoFormat.Enabled || cbVideoEncoder.SelectedIndex <= 0;

        private void SetText(string text)
        {
            string filePath = textLocation.Text.Trim();
            filePath = $"{GetFilename(filePath)}{text.Split('/')[2].Trim()}";
            textLocation.Text = filePath;
        }
        
        private void CbVideoFormat_TextChanged(object sender, EventArgs e)
        {
            if (IgnoreTextChanged) 
                return;
            
            try
            {
                SetText(cbVideoFormat.Text);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void CbAudioFormat_TextChanged(object sender, EventArgs e)
        {
            if (chkBoxExportVideo.Checked || IgnoreTextChanged) 
                return;
            
            try
            {
                SetText(cbAudioFormat.Text);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void CbAudioEncoder_TextChanged(object sender, EventArgs e)
        {
            if (chkBoxExportVideo.Checked || IgnoreTextChanged)
                return;
            string filePath = textLocation.Text.Trim();
            filePath = $"{GetFilename(filePath)}{cbAudioEncoder.Text.Trim()}";
            textLocation.Text = filePath;
        }

        private void ButtonGetCommand_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textUrl.Text.Trim()))
            {
                GenerateDownloadCommand();
                
                Clipboard.SetText($"{YouTubeDl.binPath} {Parameters}");

                MessageBox.Show(@"Command copied to clipboard!", @"Generate Command", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // TODO?
            }
        }

        private bool IsValidVideo => !IgnoreTextChanged && cbVideoEncoder.Text.Trim() != "mp4";

        private bool IsValidAudio => cbAudioEncoder.Enabled && cbAudioEncoder.SelectedIndex > 0 &&
                                     cbAudioEncoder.Text.Trim() != "mp3" && cbAudioEncoder.Text.Trim() != "m4a";

        private void GenerateDownloadCommand()
        {
            Url = textUrl.Text.Trim();
            Filepath = textLocation.Text.Trim();

            if (chkBoxEmbedThumbnail.Checked)
            {
                if (IsValidVideo)
                {
                    MessageBox.Show(Resources.InvalidVideo, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (IsValidAudio)
                {
                    MessageBox.Show(Resources.InvalidAudio, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (!videoIdLookupTable.TryGetValue(cbVideoFormat.Text, out string? videoFormatId))
                return;
            
            if (!audioIdLookupTable.TryGetValue(cbAudioFormat.Text, out string? audioFormatId))
                return;

            GenerateCommand(videoFormatId, audioFormatId);

            Title = labelTitle.Text.Trim();
        }

        private void GenerateCommand(string videoFormatId, string audioFormatId)
        {
            int k = Filepath.LastIndexOf('.');
            string fileNameFormatted = Filepath;
            if (k > -1)
            {
                fileNameFormatted = Filepath[..k];
            }
            string filePathTemplate = $"{fileNameFormatted}.%(ext)s"; // youtube-dl doesn't like it when you provide --audio-format and extension in -o together
            string optAuth = "";
            if (!string.IsNullOrEmpty(textUsername.Text) && !string.IsNullOrEmpty(textPassword.Text))
            {
                optAuth = $"--username {textUsername.Text} --password {textPassword.Text}";
            }
            string optEncode = !IgnoreTextChanged ? "--recode-video " + cbVideoEncoder.Text.Trim() : "";
            string optMetadata = chkBoxWriteMetadata.Checked ? "--add-metadata" : "";
            string optAds = chkBoxIncludeAds.Checked ? "--include-ads" : "";
            string optEmbedThumbnail = chkBoxEmbedThumbnail.Checked ? "--embed-thumbnail" : "";
            string optEmbedSubs = chkEmbedSubs.Checked ? "--embed-subs" : "";
            const string optGeneral = "-i --no-check-certificate --prefer-ffmpeg --no-warnings --restrict-filenames";
            switch (chkBoxExportVideo.Checked)
            {
                case true when chkBoxExportAudio.Checked:
                case true when !chkBoxExportAudio.Checked:
                    // video and audio
                    // TODO: split video/audio and video only
                    Parameters =
                        $"-f {videoFormatId}+{audioFormatId}/best {optEncode} {optGeneral} {optMetadata} {optEmbedThumbnail} {optEmbedSubs} {optAds} {optAuth} -o {filePathTemplate} {Url}";
                    Type = "video"; // TODO: +audio"; ?
                    break;
                case false when chkBoxExportAudio.Checked:
                    // audio only
                    Parameters =
                        $"-f {audioFormatId} -x --audio-format {cbAudioEncoder.Text.Trim()} --audio-quality 0 {optGeneral} {optMetadata} {optAds} -o {optAds} {filePathTemplate}";
                    Type = "audio";
                    break;
            }
        }

        private void CbVideoFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbVideoFormat.SelectedIndex == 0)
            {
                cbVideoFormat.SelectedIndex = 1;
            }
        }

        private void CbAudioFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbAudioFormat.SelectedIndex == 0)
            {
                cbAudioFormat.SelectedIndex = 1;
            }
        }
    }
}
