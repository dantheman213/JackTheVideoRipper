namespace JackTheVideoRipper
{
    public partial class FrameConvert : Form
    {
        private const string _HAND_BRAKE_URL = "https://handbrake.fr/";

        public FrameConvert()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FileSystem.GetWebResourceHandle(_HAND_BRAKE_URL, useShellExecute:false);
        }
    }
}
