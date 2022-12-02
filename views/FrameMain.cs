using System.Diagnostics;
using JackTheVideoRipper.extensions;
using JackTheVideoRipper.models.enums;
using JackTheVideoRipper.modules;
using Timer = System.Threading.Timer;

namespace JackTheVideoRipper
{
    public partial class FrameMain : Form
    {
        #region Data Members

        private readonly MediaManager _mediaManager = new();
        
        private Timer? _listItemRowsUpdateTimer;

        #endregion

        #region Attributes

        private ListView.SelectedListViewItemCollection Selected => listItems.SelectedItems;

        private ListView.ListViewItemCollection ViewItems => listItems.Items;

        public ListViewItem FirstSelected => Selected[0];
        
        public bool NoneSelected => Selected.Count <= 0;

        public bool InItemBounds(MouseEventArgs e) => listItems.FocusedItem.Bounds.Contains(e.Location);
        
        private static bool IsRightClick(MouseEventArgs e) => e.Button == MouseButtons.Right;

        #endregion

        #region Form View Accessors

        private string ToolbarStatus
        {
            set => toolbarLabelStatus.Text = value;
        }

        #endregion
        
        #region Constructor

        public FrameMain()
        {
            InitializeComponent();

            SubscribeEvents();
        }

        #endregion

        #region Public Methods

        public void SetNotification(string notification)
        {
            notificationStatusLabel.Text = notification.TruncateEllipse(60);
        }
        
        public void ClearNotifications()
        {
            notificationStatusLabel.Text = string.Empty;
        }

        #endregion

        #region Private Methods

        // TODO: Should it just not wait and notify user of reconnection?
        private void ConnectionLossHandler()
        {
            Modals.Notification("Client has lost connection to internet!");
            _mediaManager.PauseAll();
            Tasks.WaitUntil(Core.IsConnectedToInternet).Wait(); //< Delay processes until connected
            _mediaManager.ResumeAll();
        }

        private void Update(object? state)
        {
            if (!Core.IsConnectedToInternet())
                ConnectionLossHandler();
            
            UpdateModule(_mediaManager.UpdateListItemRows);
        }

        private void UpdateModule(Action updateModuleAction)
        {
            BeginInvoke(updateModuleAction, null);
        }

        private void ClearAllViewItems()
        {
            ViewItems.Clear();
            _mediaManager.ClearAll();
        }

        private void OpenContextMenu()
        {
            contextMenuListItems.Show(Cursor.Position);
            SetContextVisibility("retryDownloadToolStripMenuItem", ProcessStatus.Error);
            SetContextVisibility("stopDownloadToolStripMenuItem", ProcessStatus.Running);
            SetContextVisibility("deleteFromDiskToolStripMenuItem", ProcessStatus.Succeeded);
        }

        private void SetContextVisibility(string name, ProcessStatus processStatus)
        {
            contextMenuListItems.Items[name].Visible = SelectedIsStatus(processStatus);
        }
        
        private bool SelectedIsStatus(ProcessStatus processStatus)
        {
            return _mediaManager.SelectedHasStatus(processStatus);
        }
        
        private void StartEventTimer(object sender, EventArgs e)
        {
            // Initiate the Update Loop
            _listItemRowsUpdateTimer = new Timer(Update, null, 0, 800);
            timerStatusBar.Enabled = true;
        }

        private void UpdateStatusBar()
        {
            ToolbarStatus = _mediaManager.ToolbarStatus;
            toolBarLabelCpu.Text = MediaManager.ToolbarCpu;
            toolBarLabelMemory.Text = MediaManager.ToolbarMemory;
            toolBarLabelNetwork.Text = MediaManager.ToolbarNetwork;
        }

        #endregion

        #region Form Events
        
        private void FrameMain_Load(object sender, EventArgs e)
        {
            Text = Core.ApplicationTitle;
            StartEventTimer(sender, e);
        }

        private async void FrameMain_Shown(object sender, EventArgs e)
        {
            await Core.Startup();
        }

        private void FrameMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Tells you if user cancelled
            if (_mediaManager.OnFormClosing())
                e.Cancel = true;
        }

         private void FolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
             if (FileSystem.SelectFile() is not { } filepath)
                 return;

             foreach (string file in Directory.GetFiles(filepath, $"*.{FFMPEG.VideoFormats.MP4}"))
             {
                 FFMPEG.Compress(file);
             }
        }

        #endregion

        #region Timer Events

        private async void TimerCheckForUpdates_Tick(object sender, EventArgs e)
        {
            timerCheckForUpdates.Enabled = false;
            await AppUpdate.CheckForNewAppVersion(false);
        }

        private void TimerProcessLimit_Tick(object? sender = null, EventArgs? e = null)
        {
            timerProcessLimit.Enabled = true;
        }

        #endregion

        #region Event Handlers

        private void SubscribeEvents()
        {
            Core.NotificationEvent += SetNotification;

            // Subscribe to Media Manager Events
            _mediaManager.QueueUpdated += () => TimerProcessLimit_Tick();
            _mediaManager.ProcessAdded += item => ViewItems.Add(item);
            _mediaManager.ProcessRemoved += item => ViewItems.Remove(item);
            
            // Edit Menu
            copyFailedUrlsToClipboardToolStripMenuItem.Click += (_, _) => _mediaManager.CopyFailedUrls();
            retryAllToolStripMenuItem.Click += (_, _) => _mediaManager.RetryAll();
            stopAllToolStripMenuItem.Click += (_, _) => _mediaManager.StopAll();
            clearFailuresToolStripMenuItem.Click += (_, _) => _mediaManager.RemoveFailed();
            clearAllToolStripMenuItem.Click += (_, _) => ClearAllViewItems();
            clearSuccessesToolStripMenuItem.Click += (_, _) => _mediaManager.RemoveCompleted();
            pauseAllToolStripMenuItem.Click += (_, _) => _mediaManager.PauseAll();
            resumeAllToolStripMenuItem.Click += (_, _) => _mediaManager.ResumeAll();

            // Download Batch
            downloadBatchYouTubePlaylistlToolStripMenuItem.Click += (_, _) => _mediaManager.BatchPlaylist();
            downloadBatchDocumentToolStripMenuItem.Click += (_, _) => _mediaManager.BatchDocument();
            downloadBatchManualToolStripMenuItem.Click += (_, _) => _mediaManager.DownloadBatch();
            
            // Subpages
            aboutToolStripMenuItem.Click += (_, _) => Core.OpenAbout();
            convertMediaToolStripMenuItem.Click += (_, _) => Core.OpenConvert();
            
            // Core Buttons
            openDownloadFolderToolStripMenuItem.Click += (_, _) => FileSystem.OpenDownloads();
            openFolderToolStripMenuItem.Click += (_, _) => _mediaManager.PerformContextAction(ContextActions.Reveal);
            exitToolStripMenuItem.Click += (_, _) => Close();
            statusBar.DoubleClick += (_, _) => FileSystem.OpenTaskManager();
            openTaskManagerToolStripMenuItem.Click += (_, _) => FileSystem.OpenTaskManager();
            settingsToolStripMenuItem.Click += (_, _) => Core.OpenSettings();
            checkForUpdatesToolStripMenuItem.Click += (_, _) => Core.CheckForUpdates();
            
            // Dependencies
            openDependenciesFolderToolStripMenuItem.Click += (_, _) => Core.OpenInstallFolder();
            ytdlpToolStripMenuItem.Click += (_, _) => Core.DownloadDependency(Dependencies.YouTubeDL);
            vS2010RedistributableToolStripMenuItem.Click += (_, _) => Core.DownloadDependency(Dependencies.Redistributables);
            atomicParsleyToolStripMenuItem.Click += (_, _) => Core.DownloadDependency(Dependencies.AtomicParsley);
            vlcPlayerToolStripMenuItem.Click += (_, _) => Core.DownloadDependency(Dependencies.VLC);
            handbrakeToolStripMenuItem.Click += (_, _) => Core.DownloadDependency(Dependencies.Handbrake);
            fFmpegToolStripMenuItem.Click += (_, _) => Core.DownloadDependency(Dependencies.FFMPEG);
            
            // Media Downloads
            toolStripButtonDownloadVideo.Click += (_, _) => _mediaManager.DownloadMediaDialog(MediaType.Video);
            toolStripButtonDownloadAudio.Click += (_, _) => _mediaManager.DownloadMediaDialog(MediaType.Audio);
            downloadVideoToolStripMenuItem.Click += (_, _) => _mediaManager.DownloadMediaDialog(MediaType.Video);
            downloadAudioToolStripMenuItem.Click += (_, _) => _mediaManager.DownloadMediaDialog(MediaType.Audio);

            notificationStatusLabel.Click += (_, _) => ClearNotifications();

            // Context Menu
            listItems.MouseClick += (_, e) =>
            {
                if (IsRightClick(e) && InItemBounds(e))
                    OpenContextMenu();
            };
            
            // Timer Events            
            timerStatusBar.Tick += (_, _) => UpdateStatusBar();
            
            // Item Context Menu
            openMediaInPlayerToolStripMenuItem.Click += (_, _) =>
            {
                if (!NoneSelected) _mediaManager.PerformContextAction(ContextActions.OpenMedia);
            };
            
            openURLInBrowserToolStripMenuItem.Click += (_, _) =>
            {
                if (!NoneSelected) _mediaManager.PerformContextAction(ContextActions.OpenUrl);
            };
            
            copyUrlToolStripMenuItem.Click += (_, _) =>
            {
                if (!NoneSelected) _mediaManager.PerformContextAction(ContextActions.Copy);
            };
            
            deleteRowToolStripMenuItem.Click += (_, _) =>
            {
                if (!NoneSelected) _mediaManager.PerformContextAction(ContextActions.Delete);
            };
            
            stopDownloadToolStripMenuItem.Click += (_, _) =>
            {
                if (!NoneSelected) _mediaManager.PerformContextAction(ContextActions.Stop);
            };
            
            retryDownloadToolStripMenuItem.Click += (_, _) =>
            {
                if (!NoneSelected) _mediaManager.PerformContextAction(ContextActions.Retry);
            };
            
            deleteRowToolStripMenuItem.Click += (_, _) =>
            {
                if (!NoneSelected) _mediaManager.PerformContextAction(ContextActions.Remove);
            };
            
            listItems.DoubleClick += (_, _) =>
            {
                if (!NoneSelected) _mediaManager.PerformContextAction(ContextActions.OpenMedia);
            };
        }

      #endregion
   }
}
