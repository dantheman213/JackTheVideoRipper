using JackTheVideoRipper.models;

namespace JackTheVideoRipper
{
    public partial class FrameYTDLDependencyInstall : TaskForm
    {
        public FrameYTDLDependencyInstall()
        {
            InitializeComponent();
        }

        public override async Task GetPrimaryTask()
        {
            await YouTubeDL.DownloadAndInstall();
        }
    }
}
