namespace JackTheVideoRipper
{
    public partial class FrameYTDLDependencyInstall : Form
    {
        public FrameYTDLDependencyInstall()
        {
            InitializeComponent();
        }

        private void FrameYTDLDependencyInstall_Shown(object sender, EventArgs e)
        {
            Application.DoEvents();

            YouTubeDl.DownloadAndInstall();
            Close();
        }

        private void FrameYTDLDependencyInstall_Load(object sender, EventArgs e)
        {

        }
    }
}
