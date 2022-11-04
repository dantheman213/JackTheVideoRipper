namespace JackTheVideoRipper
{
    public partial class FrameSettings : Form
    {
        #region Form View Accessors

        public int MaxConcurrentDownloads
        {
            get => (int) numMaxConcurrent.Value;
            set => numMaxConcurrent.Value = value;
        }

        public string DefaultDownloadPath
        {
            get => textLocation.Text.Trim();
            set => textLocation.Text = value;
        }

        #endregion

        #region Constructor

        public FrameSettings()
        {
            InitializeComponent();
        }

        #endregion

        #region Private Methods

        private void GetSettings()
        {
            MaxConcurrentDownloads = Settings.Data.MaxConcurrentDownloads;
            DefaultDownloadPath = Settings.Data.DefaultDownloadPath;
        }

        private void SetSettings()
        {
            Settings.Data.MaxConcurrentDownloads = MaxConcurrentDownloads;
            Settings.Data.DefaultDownloadPath = DefaultDownloadPath;
        }
        
        private static void Save()
        {
            Settings.Save();
        }

        #endregion

        #region Form Events

        private void buttonLocationBrowse_Click(object sender, EventArgs e)
        {
            if (FileSystem.SelectFile(textLocation.Text.Trim()) is { } selectedPath)
                textLocation.Text = selectedPath;
        }

        private void FrameSettings_Load(object sender, EventArgs e)
        {
            GetSettings();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            SetSettings();
            Save();
            Close();
        }

        #endregion
    }
}
