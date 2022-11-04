using System.Diagnostics;
using JackTheVideoRipper.extensions;
using JackTheVideoRipper.models;
using JackTheVideoRipper.models.enums;
using Timer = System.Threading.Timer;

namespace JackTheVideoRipper
{
    public partial class FrameMain : Form
    {
        private readonly ProcessPool ProcessPool;

        private static Timer? _listItemRowsUpdateTimer;
        
        private ListViewItem firstSelected => listItems.SelectedItems[0];
        
        private bool noneSelected => listItems.SelectedItems.Count <= 0;
        
        private ListViewItem lastViewItem => listItems.Items[^1];
        
        private string GetSubItem(int index) => firstSelected.SubItems[index].Text;

        private ProcessUpdateRow? SelectedRow => ProcessPool.Get(firstSelected);
        
        private string SelectedRowStatus => GetSubItem(1);
        
        public ProcessStatus SelectedProcessStatus => ProcessPool.Get(firstSelected)?.ProcessStatus ?? ProcessStatus.Created;
        
        private string _url => GetSubItem(7);
        private string _filepath => GetSubItem(8);

        private string ToolbarStatus
        {
            get => toolbarLabelStatus.Text;
            set => toolbarLabelStatus.Text = value;
        }

        public FrameMain()
        {
            ProcessPool = new ProcessPool(ProcessCompletionCallback, ProcessStartedCallback);
            InitializeComponent();

            // Allows us to read out the console values
            if (Debugger.IsAttached)
                Input.RunAsConsole();
        }

        private void Update(object? state)
        {
            UpdateListItemRows();
        }

        private string GetProgramStatus()
        {
            if (ProcessPool.AnyRunning)
                return "Downloading Media";
            if (ProcessPool.AnyQueued)
                return "Awaiting Download";
            if (ProcessPool.QueueEmpty)
                return "Idle";
            return "";
        }

        private void UpdateListItemRows()
        {
            ProcessPool.ActiveProcesses.ForEach(p => UpdateModule(p.UpdateRow));
        }

        private void UpdateModule(Action action)
        {
            BeginInvoke(action, null);
        }

        private void UpdateProcessQueue()
        {
            TimerProcessLimit_Tick();
        }

        private void ProcessCompletionCallback(ProcessUpdateRow processUpdateRow)
        {
            UpdateProcessQueue();
        }
        
        private void ProcessStartedCallback()
        {
            UpdateProcessQueue();
        }

        private void DownloadMediaDialog(MediaType type)
        {
            FrameNewMedia frameNewMedia = new(type);

            if (frameNewMedia.ShowDialog() != DialogResult.OK)
                return;

            AddMediaItemRow(frameNewMedia.MediaItemRow);
            UpdateProcessQueue();
        }

        private void AddListItem(ListViewItem item)
        {
            listItems.Items.Add(item);
        }

        private void RemoveListItem(ListViewItem item)
        {
            listItems.Items.Remove(item);
        }
        
        private void AddMediaItemRow(MediaItemRow row)
        {
            AddListItem(row);
            ProcessPool.QueueProcess(row, lastViewItem);
        }

        private void AddMediaItemRow(string title, MediaType type, string url, string filepath)
        {
            AddMediaItemRow(new MediaItemRow
            {
                Title = title,
                Type = type,
                Url = url,
                Parameters = new Parameters(),
                Filepath = filepath
            });
        }

        private void QueueBatchDownloads()
        {
            for (int i = 0; i < Settings.Data.MaxConcurrentDownloads; i++)
            {
                Application.DoEvents();
                Thread.Sleep(300);
                UpdateProcessQueue();
            }
        }

        private void ClearAllViewItems()
        {
            listItems.Items.Clear();
        }

        private void RemoveViewItems(IEnumerable<ProcessUpdateRow> processUpdateRows)
        {
            processUpdateRows.ForEach(p => RemoveListItem(p.ViewItem));
        }
        
        private void OpenContextMenu()
        {
            contextMenuListItems.Show(Cursor.Position);
            contextMenuListItems.Items["retryDownloadToolStripMenuItem"].Visible =
                SelectedProcessStatus == ProcessStatus.Error;
            contextMenuListItems.Items["stopDownloadToolStripMenuItem"].Visible =
                SelectedProcessStatus == ProcessStatus.Running;
        }
        
        private void StartEventTimer(object sender, EventArgs e)
        {
            _listItemRowsUpdateTimer = new Timer(Update, null, 0, 800);
            TimerStatusBar_Tick(sender, e);
            timerStatusBar.Enabled = true;
        }

        #region Form Events
        
        private void FrameMain_Load(object sender, EventArgs e)
        {
            Settings.Load();
            Text = $@"JackTheVideoRipper {Common.GetAppVersion()}";
            StartEventTimer(sender, e);
        }

        private void FrameMain_Shown(object sender, EventArgs e)
        {
            Core.Startup();
        }

        private void FrameMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!ProcessPool.AnyActive)
                return;
            
            if (Core.ConfirmExit())
            {
                ProcessPool.KillAllActive();
            }
            else
            {
                e.Cancel = true;
            }
        }

        #endregion

        #region Timer Events
        
        private void TimerStatusBar_Tick(object sender, EventArgs e)
        {
            ToolbarStatus = $"{GetProgramStatus(),-20} | ";
            toolBarLabelCpu.Text = $@"CPU: {Statistics.GetCpuUsagePercentage(),7} | ";
            //toolBarLabelMemory.Text = $@"Available Memory: {Statistics.GetAvailableMemory(),9} | ";
            toolBarLabelNetwork.Text = $@"Network Usage: {Statistics.GetNetworkTransfer(),10}";
        }
        
        private void TimerCheckForUpdates_Tick(object sender, EventArgs e)
        {
            timerCheckForUpdates.Enabled = false;
            Core.CheckForUpdates(false);
        }

        private void TimerProcessLimit_Tick(object? sender = null, EventArgs? e = null)
        {
            timerProcessLimit.Enabled = true;
        }

        #endregion

        #region Event Handlers


        private bool IsStatus(params string[] statuses)
        {
            return statuses.Any(s => SelectedRowStatus == s);
        }

        private bool IsStatus(string status)
        {
            return SelectedRowStatus == status;
        }
        
        private void RetryDownloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (noneSelected || !IsStatus(Statuses.ERROR))
                return;

            ProcessPool.RetryProcess(firstSelected);
            UpdateProcessQueue();
        }

        private void StopDownloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (noneSelected || IsStatus(Statuses.COMPLETE, Statuses.ERROR))
                return;

            BeginInvoke(() => { SelectedRow?.Stop(); }, null);
        }

        private void DeleteRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (noneSelected || !IsStatus(Statuses.COMPLETE, Statuses.ERROR))
                return;

            ProcessPool.Remove(firstSelected);
            RemoveListItem(firstSelected);
        }
        
        private void CopyUrlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (noneSelected)
                return;

            FileSystem.SetClipboardText(_url);
        }

        private void ListItems_DoubleClick(object sender, EventArgs e)
        {
            if (noneSelected)
                return;
            
            OpenMediaInPlayerToolStripMenuItem_Click(sender, e);
        }
        
        private void DownloadVideoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DownloadMediaDialog(MediaType.Video);
        }

        private void DownloadAudioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DownloadMediaDialog(MediaType.Audio);
        }

        private void OpenFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileSystem.OpenFile(_filepath);
        }

        private bool InItemBounds(MouseEventArgs e)
        {
            return listItems.FocusedItem.Bounds.Contains(e.Location);
        }

        private void ListItems_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right || !InItemBounds(e))
                return;

            OpenContextMenu();
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Pages.OpenPage<FrameAbout>();
        }

        private void ConvertMediaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Pages.OpenPage<FrameConvert>();
        }

        private void OpenURLInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (noneSelected)
                return;
            
            string url = _url;
            if (url.HasValue())
            {
                FileSystem.GetWebResourceHandle(url);
            }
        }

        private void OpenMediaInPlayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (noneSelected || IsStatus(Statuses.COMPLETE)) 
                return;
            
            string filePath = _filepath;
            if (filePath.HasValue() && File.Exists(filePath))
            {
                FileSystem.GetWebResourceHandle(filePath);
            }
        }

        private void ToolStripButtonDownloadVideo_Click(object sender, EventArgs e)
        {
            DownloadVideoToolStripMenuItem_Click(sender, e);
        }

        private void ToolStripButtonDownloadAudio_Click(object sender, EventArgs e)
        {
            DownloadAudioToolStripMenuItem_Click(sender, e);
        }
        
        private void DownloadFfmpegToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Core.DownloadDependency(Dependencies.FFMPEG);
        }

        private void DownloadHandbrakeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Core.DownloadDependency(Dependencies.Handbrake);
        }

        private void DownloadVLCPlayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Core.DownloadDependency(Dependencies.VLC);
        }
        
        private void DownloadAtomicParsleyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Core.DownloadDependency(Dependencies.AtomicParsley);
        }
        
        private void DownloadVS2010RedistributableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Core.DownloadDependency(Dependencies.Redistributables);
        }

        private void OpenDownloadFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileSystem.OpenDownloads();
        }
        
        private void DownloadBatchManualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string? urls = null;
            if (sender is string s)
                urls = s;

            FrameNewMediaBatch frameNewMediaBatch = new(urls!);
            
            if (frameNewMediaBatch.ShowDialog() != DialogResult.OK || 
                frameNewMediaBatch.Items is not { Count: > 0 } items)
                return;
            
            items.ForEach(item => AddMediaItemRow(item.Title!, frameNewMediaBatch.Type, item.Url!, item.Filepath!));

            QueueBatchDownloads();
        }

        private void DownloadBatchDocumentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string? fileContent = FileSystem.GetFileUsingDialog();

            if (fileContent.IsNullOrEmpty())
                return;

            string urls = Import.GetAllUrlsFromPayload(fileContent!).MergeReturn();

            DownloadBatchManualToolStripMenuItem_Click(urls, e);
        }
        
        private void DownloadBatchYouTubePlaylistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrameImportPlaylist frameImportPlaylist = new();
            
            if (frameImportPlaylist.ShowDialog() != DialogResult.OK || 
                YouTubeDL.GetPlaylistMetadata(frameImportPlaylist.Url) is not { } items)
                return;

            string result = items.Select(item => YouTubeDL.GetYouTubeLink(item.Id!)).Merge("\r\n");

            DownloadBatchManualToolStripMenuItem_Click(result, e);
        }
        
        private void SettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Pages.OpenPage<FrameSettings>() == DialogResult.OK)
            {
                QueueBatchDownloads();
            }
        }

        private void OpenTaskManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileSystem.OpenTaskManager();
        }

        private void StatusBar_DoubleClick(object sender, EventArgs e)
        {
            OpenTaskManagerToolStripMenuItem_Click(sender, e);
        }

        private void DownloadYtdlpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Core.DownloadDependency(Dependencies.YouTubeDL);;
        }

        private void OpenDependenciesFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileSystem.OpenFileExplorer(FileSystem.Paths.Install);
        }

        private void ClearSuccessesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveViewItems(ProcessPool.GetAll(ProcessStatus.Completed));
            ProcessPool.RemoveAll(ProcessStatus.Completed);
        }

        private void ClearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearAllViewItems();
            ProcessPool.ClearAll();
        }

        private void ClearFailuresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveViewItems(ProcessPool.GetAll(ProcessStatus.Error));
            ProcessPool.RemoveAll(ProcessStatus.Error);
        }

        private void StopAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessPool.StopAll();
        }

        private void RetryAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessPool.RetryAllProcesses();
        }
        
        private void CopyFailedUrlsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileSystem.SetClipboardText(ProcessPool.GetAllFailedUrls().Merge("\n"));
        }

        #endregion
    }
}
