using JackTheVideoRipper.extensions;
using JackTheVideoRipper.models;
using JackTheVideoRipper.models.enums;

namespace JackTheVideoRipper.views
{
    public partial class FrameNewMediaSimple : Form
    {
        #region Data Members

        public MediaItemRow<DownloadMediaParameters> MediaItemRow;

        private readonly MediaType _startType;
        
        private string? LastValidUrl { get; set; }

        #endregion

        #region Properties
        
        private string Filename => FileSystem.GetFilenameWithoutExtension(Filepath);

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
            DownloadMediaParameters mediaParameters = new(Url)
            {
                FilenameFormatted = Filepath,
                ExportAudio = ExportAudio,
                ExportVideo = ExportVideo,
                RunMultiThreaded = Settings.Data.EnableMultiThreadedDownloads,
                AddMetaData = true
            };

            MediaType mediaType = ExportVideo ? MediaType.Video : MediaType.Audio;

            MediaItemRow = new MediaItemRow<DownloadMediaParameters>(Url, string.Empty, Filepath, mediaType, mediaParameters);
        }
        
        private void Download()
        {
            if (Url.Valid(FileSystem.IsValidUrl))
            {
                if (!FileSystem.WarnIfFileExists(Filepath))
                    return;

                if (History.Data.ContainsUrl(Url) && !Modals.Confirmation("This item has already been downloaded, continue?"))
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
            if (Url.IsNullOrEmpty() || Filepath.IsNullOrEmpty()) 
                return;

            if (FileSystem.SaveCopy(Filepath) is { } result && result.HasValue())
                Filepath = result;
        }
        
        private void GetCommand()
        {
            if (Url.Valid(FileSystem.IsValidUrl))
            {
                GenerateDownloadCommand();
                
                FileSystem.SetClipboardText($"{YouTubeDL.ExecutablePath} {MediaItemRow.ProcessParameters}");

                Modals.Notification(@"Command copied to clipboard!", @"Generate Command");
            }
            else
            {
                Modals.Warning("No valid url provided to create command from");
            }
        }
        
        #region Static Methods

        public static MediaItemRow<DownloadMediaParameters>? GetMedia(MediaType type)
        {
            FrameNewMediaSimple frameNewMedia = new(type);
            return frameNewMedia.Confirm() ? frameNewMedia.MediaItemRow : null;
        }

        #endregion
        
        #region Event Handlers

        private void SubscribeEvents()
        {
            buttonDownload.Click += (_, _) => Download();
            buttonLocationBrowse.Click += (_, _) => Browse();
            
            buttonCancel.Click += (_, _) =>
            {
                this.Close(DialogResult.Cancel);
            };

            buttonGetCommand.Click += (_, _) => GetCommand();

            textUrl.TextChanged += async (_, _) =>
            {
                await Input.WaitForFinishTyping(() => Url);
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
