namespace JackTheVideoRipper
{
    public partial class FrameCheckMetadata : Form
    {
        public FrameCheckMetadata()
        {
            InitializeComponent();
        }

        private void timerPostLoad_Tick(object sender, EventArgs e)
        {
            // Timeout
            Close();
        }
    }
}
