namespace JackTheVideoRipper
{
    public partial class FrameSettings : Form
    {
        public FrameSettings()
        {
            InitializeComponent();
        }

        private void buttonLocationBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new();
            folderBrowserDialog.SelectedPath = textLocation.Text.Trim();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                textLocation.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void FrameSettings_Load(object sender, EventArgs e)
        {
            numMaxConcurrent.Value = Settings.Data.MaxConcurrentDownloads;
            textLocation.Text = Settings.Data.DefaultDownloadPath;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            Settings.Data.MaxConcurrentDownloads = (int)numMaxConcurrent.Value;
            Settings.Data.DefaultDownloadPath = textLocation.Text.Trim();
            Save();

            Close();
        }

        private void Save()
        {
            Settings.Save();
        }
    }
}
