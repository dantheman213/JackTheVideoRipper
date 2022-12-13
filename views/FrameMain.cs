using JackTheVideoRipper.extensions;
using JackTheVideoRipper.models;
using JackTheVideoRipper.models.enums;
using JackTheVideoRipper.modules;
using Timer = System.Threading.Timer;

namespace JackTheVideoRipper
{
    public partial class FrameMain : Form
    {
        #region Data Members

        private readonly MediaManager _mediaManager = new();
        private readonly NotificationsManager _notificationsManager = new();
        
        private Timer? _listItemRowsUpdateTimer;
        private Timer? _clearNotificationsTimer;
        
        private const int UPDATE_FREQUENCY = 800;
        
        private int _notificationClearTime = 5000; //< May want to be set by user later?
        
        private IAsyncResult? _rowUpdateTask;

        #endregion

        #region Attributes

        private ListView.SelectedListViewItemCollection Selected => listItems.SelectedItems;

        private ListView.ListViewItemCollection ViewItems => listItems.Items;

        public ListViewItem FirstSelected => Selected[0];
        
        public bool NoneSelected => Selected.Count <= 0;

        public bool InItemBounds(MouseEventArgs e) => listItems.FocusedItem.Bounds.Contains(e.Location);

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

        public void SetNotificationBrief(Notification notification)
        {
            string notificationMessage = notification.ShortenedMessage ?? notification.Message;
            notificationStatusLabel.Text = $"[{notification.DatePosted:T}]: {notificationMessage.TruncateEllipse(60)}";
            _clearNotificationsTimer?.Change(_notificationClearTime, _notificationClearTime);
        }
        
        public void ClearNotifications(object? sender = null)
        {
            notificationStatusLabel.Text = string.Empty;
        }

        private readonly List<IAsyncResult> _queuedActions = new();

        public void QueueAction(Action updateModuleAction)
        {
            UpdateModule(updateModuleAction);
        }
        
        public IAsyncResult QueueActionAsync(Func<Task> updateModuleAction)
        {
            IAsyncResult asyncResult = UpdateModuleAsync(updateModuleAction);
            _queuedActions.Add(asyncResult);
            return asyncResult;
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

            if (_rowUpdateTask is not null && !_rowUpdateTask.IsCompleted)
                return;
            
            _rowUpdateTask = UpdateModuleAsync(_mediaManager.UpdateListItemRows);
        }
        
        private void UpdateModule(Action updateModuleAction)
        {
            Invoke(updateModuleAction, null);
        }
        
        private IAsyncResult UpdateModuleAsync(Func<Task> updateModuleAction)
        {
            return BeginInvoke(updateModuleAction, null);
        }

        private void ClearAll()
        {
            ViewItems.Clear();
            _mediaManager.ClearAll();
        }

        private void OpenContextMenu()
        {
            contextMenuListItems.Show(Cursor.Position);
            SetContextVisibility("retryDownloadToolStripMenuItem",  ProcessStatus.Error);
            SetContextVisibility("stopDownloadToolStripMenuItem",   ProcessStatus.Running);
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
            _listItemRowsUpdateTimer = new Timer(Update, null, 0, UPDATE_FREQUENCY);
            timerStatusBar.Enabled = true;
        }

        private void UpdateStatusBar()
        {
            ToolbarStatus = _mediaManager.ToolbarStatus;
            toolBarLabelCpu.Text = MediaManager.ToolbarCpu;
            toolBarLabelMemory.Text = MediaManager.ToolbarMemory;
            toolBarLabelNetwork.Text = MediaManager.ToolbarNetwork;
        }

        private void OnSettingsUpdated()
        {
            openConsoleToolStripMenuItem.Visible = Settings.Data.EnableDeveloperMode;
            openHistoryToolStripMenuItem.Visible = Settings.Data.StoreHistory;
        }
        
        private async void KeyDownHandler(object? sender, KeyEventArgs args)
        {
            // Ctrl + V
            if (args is {KeyCode: Keys.V, Control: true})
            {
                await PasteContent();
                args.Handled = true;
                return;
            }
                
            switch (args.KeyCode)
            {
                case Keys.Oemtilde:
                    Output.OpenMainConsoleWindow();
                    args.Handled = true;
                    break;
            }
        }

        private async Task PasteContent()
        {
            string url = FileSystem.GetClipboardText();
            
            if (url.Invalid(FileSystem.IsValidUrl))
                return;
            
            _mediaManager.QueueProcess(new MediaItemRow(url));
        }

        #endregion

        #region Form Events

        private void FrameMain_Load(object sender, EventArgs e)
        {
            Text = Core.ApplicationTitle;
            _clearNotificationsTimer = new Timer(ClearNotifications, null, 0, _notificationClearTime);
            OnSettingsUpdated(); //< Load initial values (for visibility)
            StartEventTimer(sender, e);
        }

        private async void FrameMain_Shown(object sender, EventArgs e)
        {
            await Core.Startup();
        }

        private async void FrameMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Tells you if user cancelled
            if (_mediaManager.OnFormClosing())
                e.Cancel = true;

            if (_rowUpdateTask is not null)
                EndInvoke(_rowUpdateTask);
            
            _queuedActions.Where(a => !a.IsCompleted).ForEach(a => EndInvoke(a));

            await Core.Shutdown();
        }

         private void FolderToolStripMenuItem_Click(object? sender, EventArgs e)
        {
             if (FileSystem.SelectFile() is not { } filepath)
                 return;

             const string mp4SearchPattern = $"*.{FFMPEG.VideoFormats.MP4}";
             
             Directory.GetFiles(filepath, mp4SearchPattern).ForEach(FFMPEG.Compress);
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
            FrameSettings.SettingsUpdatedEvent += OnSettingsUpdated;

            KeyDown += KeyDownHandler;
            
            // Subscribe to Media Manager Events
            _mediaManager.QueueUpdated += () => TimerProcessLimit_Tick();
            _mediaManager.ProcessAdded += item => ViewItems.Add(item);
            _mediaManager.ProcessRemoved += item => ViewItems.Remove(item);
            
            // Edit Menu
            copyFailedUrlsToClipboardToolStripMenuItem.Click += (_, _) => _mediaManager.CopyFailedUrls();
            retryAllToolStripMenuItem.Click += (_, _) => _mediaManager.RetryAll();
            stopAllToolStripMenuItem.Click += (_, _) => _mediaManager.StopAll();
            clearFailuresToolStripMenuItem.Click += (_, _) => _mediaManager.RemoveFailed();
            clearAllToolStripMenuItem.Click += (_, _) => ClearAll();
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
            checkForUpdatesToolStripMenuItem.Click += async (_, _) => await Core.CheckForUpdates();
            
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

            // Tools
            validateVideoToolStripMenuItem.Click += (_, _) => MediaManager.VerifyIntegrity();
            compressVideoToolStripMenuItem.Click += FolderToolStripMenuItem_Click;
            openConsoleToolStripMenuItem.Click += (_, _) => Output.OpenMainConsoleWindow();
            openHistoryToolStripMenuItem.Click += (_, _) => History.Data.OpenHistory();

            // Notifications
            NotificationsManager.SendNotificationEvent += SetNotificationBrief;
            notificationStatusLabel.MouseDown += (_, e) =>
            {
                if (e.IsRightClick())
                {
                    ClearNotifications();
                }
                else
                {
                    _notificationsManager.OpenNotificationWindow();
                }
            };
            
            // Timer Events
            timerStatusBar.Tick += (_, _) => UpdateStatusBar();

            #region Item Context Menu
            
            // Context Menu
            listItems.MouseClick += (_, e) =>
            {
                if (e.IsRightClick() && InItemBounds(e))
                    OpenContextMenu();
            };
            
            openMediaInPlayerToolStripMenuItem.Click += (_, _) =>
            {
                ContextHandler(ContextActions.OpenMedia);
            };
            
            openURLInBrowserToolStripMenuItem.Click += (_, _) =>
            {
                ContextHandler(ContextActions.OpenUrl);
            };
            
            openProcessInConsoleToolStripMenuItem.Click += (_, _) =>
            {
                ContextHandler(ContextActions.OpenConsole);
            };
            
            copyUrlToolStripMenuItem.Click += (_, _) =>
            {
                ContextHandler(ContextActions.Copy);
            };
            
            deleteRowToolStripMenuItem.Click += (_, _) =>
            {
                ContextHandler(ContextActions.Delete);
            };
            
            stopDownloadToolStripMenuItem.Click += (_, _) =>
            {
                ContextHandler(ContextActions.Stop);
            };
            
            retryDownloadToolStripMenuItem.Click += (_, _) =>
            {
                ContextHandler(ContextActions.Retry);
            };
            
            deleteRowToolStripMenuItem.Click += (_, _) =>
            {
                ContextHandler(ContextActions.Remove);
            };
            
            listItems.DoubleClick += (_, _) =>
            {
                ContextHandler(ContextActions.OpenMedia);
            };
            
            #endregion
        }

        private void ContextHandler(ContextActions contextAction)
        {
            if (!NoneSelected) _mediaManager.PerformContextAction(contextAction);
        }

      #endregion
   }
}
