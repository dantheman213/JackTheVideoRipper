namespace JackTheVideoRipper
{
    public partial class FrameImportPlaylist : Form
    {
        public string Url;

        public FrameImportPlaylist()
        {
            InitializeComponent();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonImport_Click(object sender, EventArgs e)
        {
            string url = textUrl.Text.Trim();
            if (string.IsNullOrEmpty(url) || !Common.IsValidUrl(url)) return;
            Url = url;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
