using JackTheVideoRipper.extensions;
using JackTheVideoRipper.models;
using JackTheVideoRipper.models.enums;

namespace JackTheVideoRipper.views
{
    public partial class FrameNewMediaSimple : Form
    {
        #region Data Members

        public MediaItemRow MediaItemRow;

        private readonly MediaType _startType;
        
        private string? LastValidUrl { get; set; }

        #endregion

        #region Properties
        
        private string Filename => FileSystem.GetFilename(Filepath);

        #endregion

        #region Form Element Accessors

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
        
        #endregion
        
        public FrameNewMediaSimple(MediaType type)
        {
            _startType = type;
            InitializeComponent();
            SubscribeEvents();
        }
        
        private bool ValidateUrl(string url)
        {
            if (url == LastValidUrl || url.Invalid(FileSystem.IsValidUrl))
                return false;
            
            LastValidUrl = url;

            return true;
        }
        
        private void CopyUrlToClipboard()
        {
            string clipboard = FileSystem.GetClipboardText();
            
            if (clipboard.Invalid(FileSystem.IsValidUrl))
                return;
            
            Url = clipboard;
        }
        
        private void GenerateDownloadCommand()
        {
            GenerateMediaItemRow();
        }

        private void GenerateMediaItemRow()
        {
            MediaParameters mediaParameters = new(Url)
            {
                FilenameFormatted = Filepath,
                ExportAudio = ExportAudio,
                ExportVideo = ExportVideo,
                RunMultiThreaded = Settings.Data.EnableMultiThreadedDownloads
            };

            MediaType mediaType = ExportVideo ? MediaType.Video : MediaType.Audio;

            MediaItemRow = new MediaItemRow(Url, string.Empty, Filepath, mediaType, mediaParameters);
        }
        
        private void Download()
        {
            if (!Url.Invalid(FileSystem.IsValidUrl))
            {
                if (!FileSystem.WarnIfFileExists(Filepath))
                    return;

                GenerateDownloadCommand();
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                Modals.Warning("Failed to parse provided url!");
            }
        }
        
        private void Browse()
        {
            if (Url.IsNullOrEmpty() || Filepath.IsNullOrEmpty()) 
                return;

            if (FileSystem.SaveCopy(Filepath) is { } result && result.HasValue())
                Filepath = result;
        }
        
        private void GetCommand()
        {
            if (Url.HasValue())
            {
                GenerateDownloadCommand();
                
                FileSystem.SetClipboardText($"{YouTubeDL.ExecutablePath} {MediaItemRow.MediaParameters}");

                Modals.Notification(@"Command copied to clipboard!", @"Generate Command");
            }
            else
            {
                // TODO?
            }
        }
        
        #region Static Methods

        public static MediaItemRow? GetMedia(MediaType type)
        {
            FrameNewMediaSimple frameNewMedia = new(type);

            if (frameNewMedia.ShowDialog() != DialogResult.OK)
                return null;

            return frameNewMedia.MediaItemRow;
        }

        #endregion
        
        #region Event Handlers

        private void SubscribeEvents()
        {
            buttonDownload.Click += (_, _) => Download();
            buttonLocationBrowse.Click += (_, _) => Browse();
            
            buttonCancel.Click += (_, _) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };

            buttonGetCommand.Click += (_, _) => GetCommand();

            textUrl.TextChanged += (_, _) =>
            {
                Input.WaitForFinishTyping(() => Url);
                ValidateUrl(Url);
            };
        }

        private void FrameNewMediaSimple_Load(object sender, EventArgs e)
        {
            if (_startType == MediaType.Audio)
                ExportVideo = false;
        }

        #endregion
    }
}
