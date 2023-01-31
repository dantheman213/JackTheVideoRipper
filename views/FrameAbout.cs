using JackTheVideoRipper.extensions;
using JackTheVideoRipper.Properties;

namespace JackTheVideoRipper
{
    public partial class FrameAbout : Form
    {
        public FrameAbout()
        {
            InitializeComponent();
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            linkLabel.LinkClicked += (_, _) => FileSystem.GetWebResourceHandle(AppInfo.ProgramUrl);
            Load += InitializeText;
        }

        private void InitializeText(object? sender, EventArgs args)
        {
            projectTitle.Text = AppInfo.ProgramName;
            labelVersion.Text = Common.GetAppVersion();

            try
            {
                textServices.Text = YouTubeDL.SupportedServices.MergeReturn();
            }
            catch (Exception ex)
            {
                FileSystem.LogException(ex);
                textServices.Text = @"ERROR: Can't get list of supported services.";
            }
            
            labelYouTubeDL.Text = $@"yt-dlp {YouTubeDL.CurrentVersion}";
        }
    }
}
