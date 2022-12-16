using JackTheVideoRipper.extensions;

namespace JackTheVideoRipper
{
    public partial class FrameImportPlaylist : Form
    {
        #region List Item Accessors

        private string TextUrl => textUrl.Text.Trim();

        private string _url = string.Empty;
        
        public string Url
        {
            get => _url;
            private set
            {
                if (value.Invalid(FileSystem.IsValidUrl))
                    return;
                _url = value;
            }
        }

        #endregion

        #region Constructor

        public FrameImportPlaylist()
        {
            InitializeComponent();
        }

        #endregion

        #region Form Events

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonImport_Click(object sender, EventArgs e)
        {
            Url = TextUrl;
            this.Close(DialogResult.OK);
        }

        #endregion

        #region Static Methods

        public static async Task<IEnumerable<string>?> GetMetadata()
        {
            FrameImportPlaylist frameImportPlaylist = new();
            
            if (!frameImportPlaylist.Confirm() || 
                await YouTubeDL.GetPlaylistMetadata(frameImportPlaylist.Url) is not { } items)
                return null;

            return items.Select(item => YouTubeDL.GetYouTubeLink(item.Id!));
        }

        #endregion
    }
}
