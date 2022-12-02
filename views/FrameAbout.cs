using JackTheVideoRipper.extensions;

namespace JackTheVideoRipper
{
    public partial class FrameAbout : Form
    {
        private const string _PROJECT_URL = "https://github.com/dantheman213/JackTheVideoRipper";

        public FrameAbout()
        {
            InitializeComponent();
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FileSystem.GetWebResourceHandle(_PROJECT_URL);
        }

        private void FrameAbout_Load(object sender, EventArgs e)
        {
            labelVersion.Text = Common.GetAppVersion();

            try
            {
                textExtractors.Text = YouTubeDL.GetSupportedServices().Select(s => $"* {s}").MergeReturn();
                labelYouTubeDL.Text = $@"yt-dlp {YouTubeDL.CurrentVersion}";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                textExtractors.Text = @"ERROR: Can't get list of supported services.";
            }
        }
    }
}
