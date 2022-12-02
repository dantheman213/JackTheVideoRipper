namespace JackTheVideoRipper
{
    public partial class FrameYTDLDependencyInstall : Form
    {
        public FrameYTDLDependencyInstall()
        {
            InitializeComponent();
        }

        private async void FrameYTDLDependencyInstall_Shown(object sender, EventArgs e)
        {
            Application.DoEvents();
            await YouTubeDL.DownloadAndInstall().ContinueWith(task => Close());
        }

        private void FrameYTDLDependencyInstall_Load(object sender, EventArgs e)
        {
        }
    }
}
