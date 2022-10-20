namespace JackTheVideoRipper
{
    public partial class FrameConvert : Form
    {
        private static string handBrakeUrl = "https://handbrake.fr/";

        public FrameConvert()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Common.GetWebResourceHandle(handBrakeUrl, false);
        }
    }
}
