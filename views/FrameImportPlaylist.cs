using JackTheVideoRipper.extensions;

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
            if (url.IsNullOrEmpty() || !Common.IsValidUrl(url)) return;
            Url = url;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
