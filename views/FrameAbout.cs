namespace JackTheVideoRipper
{
    public partial class FrameAbout : Form
    {
        private static string projectUrl = "https://github.com/dantheman213/JackTheVideoRipper";

        public FrameAbout()
        {
            InitializeComponent();
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FileSystem.GetWebResourceHandle(projectUrl);
        }

        private void FrameAbout_Load(object sender, EventArgs e)
        {
            labelVersion.Text = Common.GetAppVersion();

            try
            {
                string lines = $"* {YouTubeDL.GetExtractors().Replace("\n", "\r\n* ")}";
                textExtractors.Text = lines;
                labelYouTubeDL.Text = $@"yt-dlp {YouTubeDL.GetVersion()}";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                textExtractors.Text = @"ERROR: Can't get list of supported services.";
            }
        }
    }
}
