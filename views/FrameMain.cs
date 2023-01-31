using JackTheVideoRipper.extensions;
using JackTheVideoRipper.interfaces;
using JackTheVideoRipper.models;
using JackTheVideoRipper.models.enums;

namespace JackTheVideoRipper
{
    public partial class FrameMain : Form
    {
        #region Data Members

        private IAsyncResult? _rowUpdateTask;

        private readonly ContextMenuManager _contextMenuManager;

        private readonly Ripper _ripper;

        #endregion

        #region Attributes

        private ListView.SelectedListViewItemCollection Selected => listItems.SelectedItems;

        private ListView.ListViewItemCollection ViewItems => listItems.Items;

        public ListViewItem FirstSelected => Selected[0];

        public ListViewItem LastSelected => Selected[^1];
        
        public bool NoneSelected => Selected.Count <= 0;
        
        public ListViewItem FocusedItem => listItems.FocusedItem;

        public bool InItemBounds(MouseEventArgs e) => listItems.Visible && FocusedItem.InBounds(e.Location);

        public string CachedSelectedTag { get; private set; } = string.Empty;
        
        private bool IsUpdating => _rowUpdateTask is not null && !_rowUpdateTask.IsCompleted;

        #endregion

        #region Events

        private event Action ManagerUpdated = delegate {  };
        
        public event Action<object?, ContextActionEventArgs> ContextActionEvent = delegate {  };

        public event Action<object?, DependencyActionEventArgs> DependencyActionEvent = delegate {  };

        #endregion

        #region Form View Accessors

        private string NotificationStatus
        {
            set => notificationStatusLabel.Text = value;
        }
        
        private bool UpdateListViewRows
        {
            get => listItemRowsUpdateTimer.Enabled;
            set => listItemRowsUpdateTimer.Enabled = value;
        }
        
        private bool UpdateStatusBar
        {
            get => timerStatusBar.Enabled;
            set => timerStatusBar.Enabled = value;
        }

        private bool CheckForUpdates
        {
            get => timerCheckForUpdates.Enabled;
            set => timerCheckForUpdates.Enabled = value;
        }
        
        private bool UpdateProcessLimit
        {
            get => timerProcessLimit.Enabled;
            set => timerProcessLimit.Enabled = value;
        }

        #endregion
        
        #region Constructor

        public FrameMain(Ripper ripper)
        {
            // Needed for SubscribeEvents() calls (must come before)
            _ripper = ripper;
            
            InitializeComponent();

            SubscribeEvents();
            
            // Must come after InitializeComponents() call
            _contextMenuManager = new ContextMenuManager(contextMenuListItems);
        }

        #endregion

        #region Public Methods

        public void SetNotificationBrief(Notification notification)
        {
            string notificationMessage = notification.ShortenedMessage ?? notification.Message;
            NotificationStatus = $@"[{notification.DateQueued:T}]: {notificationMessage.TruncateEllipse(60)}";
        }

        #endregion

        #region Private Methods
        
        private void InitializeViews()
        {
            Text = Core.ApplicationTitle;
            OnSettingsUpdated(); //< Load initial values (for visibility bindings)
        }

        private async void Update(object? sender, EventArgs args)
        {
            if (!Core.IsConnectedToInternet())
                await _ripper.OnConnectionLost();

            if (IsUpdating)
                return;
            
            Application.DoEvents();
            
            _rowUpdateTask = UpdateModuleAsync(_ripper.Update);
        }

        private IAsyncResult? UpdateModuleAsync(Func<Task> updateModuleAction)
        {
            return Visible ? BeginInvoke(updateModuleAction, null) : default;
        }

        private void ClearAll()
        {
            ViewItems.Clear();
        }

        private void AddItem(IViewItem item)
        {
            if (item is not ListViewItem listViewItem)
                return;
            
            Threading.RunInMainContext(() => ViewItems.Add(listViewItem));
        }
        
        private void AddItems(IEnumerable<IViewItem> items)
        {
            if (items.Cast<ListViewItem>() is not { } listViewItems)
                return;
            
            Threading.RunInMainContext(() => ViewItems.AddRange(listViewItems));
        }
        
        private void RemoveItem(IViewItem item)
        {
            if (item is not ListViewItem listViewItem)
                return;
            
            Threading.RunInMainContext(() => ViewItems.Remove(listViewItem));
        }
        
        private void RemoveItems(IEnumerable<IViewItem> items)
        {
            if (items.Cast<ListViewItem>() is not { } listViewItems)
                return;
            
            Threading.RunInMainContext(() => ViewItems.RemoveRange(listViewItems));
        }
        
        private void StopUpdates()
        {
            // Cancel currently running update
            if (_rowUpdateTask is not null)
                EndInvoke(_rowUpdateTask);
            
            // Disable all timers
            CheckForUpdates = false;
            UpdateListViewRows = false;
            UpdateStatusBar = false;
            UpdateProcessLimit = false;
        }

        #endregion

        #region Timer Events

        private void InitializeTimers()
        {
            // Initiate the Update Loop
            UpdateListViewRows = true;
            UpdateStatusBar = true;
        }

        private async void TimerCheckForUpdates_Tick(object sender, EventArgs e)
        {
            CheckForUpdates = false;
            await Ripper.OnCheckForApplicationUpdates();
            CheckForUpdates = true;
        }

        private void TimerProcessLimit_Tick(object? sender = null, EventArgs? e = null)
        {
            UpdateProcessLimit = true;
        }

        #endregion

        #region Form Events

        private void OnFormLoad(object? sender, EventArgs e)
        {
            InitializeViews();
            Threading.InitializeScheduler();
            InitializeTimers();
        }

        private void OnFormShown(object? sender, EventArgs e)
        {
            
        }

        private void OnFormClosing(object? sender, FormClosingEventArgs e)
        {
            // Tells you if user cancelled
            _ripper.OnApplicationClosing(sender, e);
            if (e.Cancel)
                return;

            // Make sure our updates don't continue while we close, signal completion
            StopUpdates();
        }

        #endregion

        #region Event Handlers
        
        private async void KeyDownHandler(object? sender, KeyEventArgs args)
        {
            switch (args.KeyCode)
            {
                // Ctrl + V
                case Keys.V when args is {Control: true}:
                    await _ripper.OnPasteContent();
                    args.Handled = true;
                    return;
                case Keys.Oemtilde:
                    await Output.OpenMainConsoleWindow();
                    args.Handled = true;
                    break;
            }
        }
        
        private void OnUpdateStatusBar(object? sender, EventArgs args)
        {
            toolbarLabelStatus.Text = Statistics.Toolbar.ToolbarStatus;
            toolBarLabelCpu.Text = Statistics.Toolbar.ToolbarCpu;
            toolBarLabelMemory.Text = Statistics.Toolbar.ToolbarMemory;
            toolBarLabelNetwork.Text = Statistics.Toolbar.ToolbarNetwork;
        }

        private void OnSettingsUpdated()
        {
            openConsoleToolStripMenuItem.Visible = Settings.Data.EnableDeveloperMode;
            openHistoryToolStripMenuItem.Visible = Settings.Data.StoreHistory;
        }
        
        private void OnClearNotifications()
        {
            NotificationStatus = string.Empty;
        }

        private void OnFormClick(object? sender, EventArgs args)
        {
            CachedSelectedTag = FirstSelected.Tag.Cast<string>();
        }

        private void SubscribeEvents()
        {
            // Bind to Settings Being Updated
            FrameSettings.SettingsUpdatedEvent += OnSettingsUpdated;

            // User Events
            KeyDown += KeyDownHandler;
            Click += OnFormClick;
            listItems.MouseClick += OnListItemsMouseClick;
            
            // Core Handlers
            Load += OnFormLoad;
            Shown += OnFormShown;
            Shown += Ripper.OnEndStartup;
            FormClosing += OnFormClosing;
            
            ManagerUpdated = delegate { TimerProcessLimit_Tick(); };
            _ripper.SubscribeMediaManagerEvents(ManagerUpdated, AddItem, AddItems,
                RemoveItem, RemoveItems);
            
            // Edit Menu
            SubscribeEditMenu();

            // Subpages
            SubscribeSubpageActions();
            
            // Core Buttons
            openDownloadFolderToolStripMenuItem.Click += Ripper.OnOpenDownloads;
            exitToolStripMenuItem.Click += (_, _) => Close();
            statusBar.DoubleClick += Ripper.OnOpenTaskManager;
            openTaskManagerToolStripMenuItem.Click += Ripper.OnOpenTaskManager;
            settingsToolStripMenuItem.Click += Ripper.OnOpenSettings;
            checkForUpdatesToolStripMenuItem.Click += Ripper.OnCheckForUpdates;
            openDependenciesFolderToolStripMenuItem.Click += Ripper.OnOpenInstallFolder;
            
            // Dependencies
            SubscribeDependencies();
            
            // Media Downloads
            SubscribeMediaTasks();            

            // Tools
            SubscribeToolMenu();

            // Notifications
            SubscribeNotificationsBar();

            // Item Context Menu
            SubscribeContextEvents();
        }

        private void SubscribeSubpageActions()
        {
            aboutToolStripMenuItem.Click += Ripper.OnOpenAbout;
            convertMediaToolStripMenuItem.Click += Ripper.OnOpenConvert;
        }

        private void SubscribeMediaTasks()
        {
            toolStripButtonDownloadVideo.Click += _ripper.OnDownloadVideo;
            toolStripButtonDownloadAudio.Click += _ripper.OnDownloadAudio;
            downloadVideoToolStripMenuItem.Click += _ripper.OnDownloadVideo;
            downloadAudioToolStripMenuItem.Click += _ripper.OnDownloadAudio;
            
            // Download Batch
            downloadBatchYouTubePlaylistlToolStripMenuItem.Click += _ripper.OnBatchPlaylist;
            downloadBatchDocumentToolStripMenuItem.Click += _ripper.OnBatchDocument;
            downloadBatchManualToolStripMenuItem.Click += _ripper.OnDownloadBatch;
        }

        private void SubscribeDependencies()
        {
            ytdlpToolStripMenuItem.Click += (sender, _) =>
                DependencyActionEvent(sender, new DependencyActionEventArgs(Dependencies.YouTubeDL));
            vS2010RedistributableToolStripMenuItem.Click += (sender, _) =>
                DependencyActionEvent(sender, new DependencyActionEventArgs(Dependencies.Redistributables));
            atomicParsleyToolStripMenuItem.Click += (sender, _) =>
                DependencyActionEvent(sender, new DependencyActionEventArgs(Dependencies.AtomicParsley));
            vlcPlayerToolStripMenuItem.Click += (sender, _) =>
                DependencyActionEvent(sender, new DependencyActionEventArgs(Dependencies.VLC));
            handbrakeToolStripMenuItem.Click += (sender, _) =>
                DependencyActionEvent(sender, new DependencyActionEventArgs(Dependencies.Handbrake));
            fFmpegToolStripMenuItem.Click += (sender, _) => 
                DependencyActionEvent(sender, new DependencyActionEventArgs(Dependencies.FFMPEG));
        }

        private void SubscribeToolMenu()
        {
            validateVideoToolStripMenuItem.Click += Ripper.OnVerifyIntegrity;
            compressVideoToolStripMenuItem.Click += _ripper.OnCompressVideo;
            repairVideoToolStripMenuItem.Click += _ripper.OnRepairVideo;
            recodeVideoToolStripMenuItem.Click += _ripper.OnRecodeVideo;
            openConsoleToolStripMenuItem.Click += Ripper.OnOpenConsole;
            openHistoryToolStripMenuItem.Click += Ripper.OnOpenHistory;
        }
        
        private void SubscribeEditMenu()
        {
            copyFailedUrlsToClipboardToolStripMenuItem.Click += _ripper.OnCopyFailedUrls;
            retryAllToolStripMenuItem.Click += _ripper.OnRetryAll;
            stopAllToolStripMenuItem.Click += _ripper.OnStopAll;
            clearFailuresToolStripMenuItem.Click += _ripper.OnRemoveFailed;
            clearAllToolStripMenuItem.Click += (_, _) => ClearAll();
            clearAllToolStripMenuItem.Click += _ripper.OnClearAllViewItems;
            clearSuccessesToolStripMenuItem.Click += _ripper.OnRemoveCompleted;
            pauseAllToolStripMenuItem.Click += _ripper.OnPauseAll;
            resumeAllToolStripMenuItem.Click += _ripper.OnResumeAll;
        }

        private void SubscribeNotificationsBar()
        {
            NotificationsManager.SendNotificationEvent += SetNotificationBrief;
            NotificationsManager.ClearPushNotificationsEvent += OnClearNotifications;
            notificationStatusLabel.MouseDown += _ripper.OnNotificationBarClicked;
        }

        private async void OnListItemsMouseClick(object? sender, MouseEventArgs e)
        {
            if (e.IsRightClick() && InItemBounds(e))
                await _contextMenuManager.OpenContextMenu();
        }

        private void SubscribeContextEvents()
        {
            openFolderToolStripMenuItem.Click += (sender, _) =>
            {
                ContextActionEvent(sender, new ContextActionEventArgs(ContextActions.Reveal));
            };
            
            openMediaInPlayerToolStripMenuItem.Click += (sender, _) =>
            {
                ContextActionEvent(sender, new ContextActionEventArgs(ContextActions.OpenMedia));
            };
            
            openURLInBrowserToolStripMenuItem.Click += (sender, _) =>
            {
                ContextActionEvent(sender, new ContextActionEventArgs(ContextActions.OpenUrl));
            };
            
            openProcessInConsoleToolStripMenuItem.Click += (sender, _) =>
            {
                ContextActionEvent(sender, new ContextActionEventArgs(ContextActions.OpenConsole));
            };
            
            copyUrlToolStripMenuItem.Click += (sender, _) =>
            {
                ContextActionEvent(sender, new ContextActionEventArgs(ContextActions.Copy));
            };
            
            deleteFromDiskToolStripMenuItem.Click += (sender, _) =>
            {
                ContextActionEvent(sender, new ContextActionEventArgs(ContextActions.Delete));
            };
            
            stopDownloadToolStripMenuItem.Click += (sender, _) =>
            {
                ContextActionEvent(sender, new ContextActionEventArgs(ContextActions.Stop));
            };
            
            retryDownloadToolStripMenuItem.Click += (sender, _) =>
            {
                ContextActionEvent(sender, new ContextActionEventArgs(ContextActions.Retry));
            };
            
            deleteRowToolStripMenuItem.Click += (sender, _) =>
            {
                ContextActionEvent(sender, new ContextActionEventArgs(ContextActions.Remove));
            };
            
            listItems.DoubleClick += (sender, _) =>
            {
                ContextActionEvent(sender, new ContextActionEventArgs(ContextActions.OpenMedia));
            };
        }

        #endregion
   }
}
