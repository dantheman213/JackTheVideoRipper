using System.Text.RegularExpressions;
using JackTheVideoRipper.models;
using JackTheVideoRipper.Properties;
using Timer = System.Threading.Timer;

namespace JackTheVideoRipper
{
    public partial class FrameMain : Form
    {
        private static readonly Dictionary<string, ProcessUpdateRow> _ProcessUpdates = new();

        private static readonly Queue<ProcessUpdateRow> ProcessQueue = new();
        private static readonly HashSet<ProcessUpdateRow> FinishedProcesses = new();

        private static Timer? _listItemRowsUpdateTimer;
        
        private ListViewItem firstSelected => listItems.SelectedItems[0];
        
        private bool noneSelected => listItems.SelectedItems.Count <= 0;
        
        private ListViewItem lastViewItem => listItems.Items[^1];
        
        private string GetSubItem(int index) => firstSelected.SubItems[index].Text;

        public ProcessStatus GetSelectedProcessStatus()
        {
            ListViewItem selected = firstSelected;
            
            if (FinishedProcesses.FirstOrDefault(p => p.ViewItem == selected) is { } processUpdateRow)
            {
                return processUpdateRow.Failed ? ProcessStatus.Error : ProcessStatus.Complete;
            }
            
            if (_ProcessUpdates.Values.FirstOrDefault(p => p.ViewItem == selected) is not null)
            {
                return ProcessStatus.Running;
            }
            
            if (ProcessQueue.FirstOrDefault(p => p.ViewItem == selected) is not null)
            {
                return ProcessStatus.Queued;
            }

            return ProcessStatus.Error;
        }
        
        private static bool locked;

        public FrameMain()
        {
            InitializeComponent();
        }

        private static DialogResult OpenYesNoModal(string text, string caption, MessageBoxIcon icon = MessageBoxIcon.Warning)
        {
            return MessageBox.Show(text, caption, MessageBoxButtons.YesNo, icon);
        }
        
        private static DialogResult OpenConfirmModal(string text, string caption, MessageBoxIcon icon = MessageBoxIcon.Warning)
        {
            return MessageBox.Show(text, caption, MessageBoxButtons.OK, icon);
        }
        
        private static void CheckDependencies()
        {
            if (!YouTubeDl.IsInstalled())
            {
                DialogResult result = OpenYesNoModal(Resources.InstallationSuccess, "Required Components Installed");
                if (result == DialogResult.Yes)
                {
                    FrameYTDLDependencyInstall frameDependencyInstall = new();
                    frameDependencyInstall.ShowDialog();
                    // TODO ?
                    OpenConfirmModal(Resources.InstallationSuccess, "Required Components Installed",
                        MessageBoxIcon.Information);
                    frameDependencyInstall.Close();
                }
                else
                {
                    OpenConfirmModal(Resources.InstallationError, "Application Error", MessageBoxIcon.Error);
                    return;
                }
            }

            if (!FFmpeg.IsInstalled())
            {
                OpenConfirmModal(Resources.FfmpegMissing, "Required components not installed");
            }

            if (!AtomicParsley.IsInstalled())
            {
                OpenConfirmModal(Resources.AtomicParsleyMissing, "Required components not installed");
            }
        }
        
        private static void LoadConfig()
        {
            Settings.Load();
        }

        private void FrameMain_Load(object sender, EventArgs e)
        {
            LoadConfig();
            
            Text = $@"JackTheVideoRipper {Common.GetAppVersion()}";
            _listItemRowsUpdateTimer = new Timer(UpdateListItemRows, null, 0, 800);
            TimerStatusBar_Tick(sender, e);
            timerStatusBar.Enabled = true;
        }

        private bool Contains(string line, string word)
        {
            return line.IndexOf(word, StringComparison.Ordinal) > -1;
        }

        private void UpdateListItemRows(object? state)
        {
            List<ProcessUpdateRow> finishedProcesses = new();
            
            foreach (ProcessUpdateRow processUpdateRow in _ProcessUpdates.Values)
            {
                if (processUpdateRow.Completed)
                {
                    finishedProcesses.Add(processUpdateRow);
                    Update(() => processUpdateRow.Complete());
                    TimerProcessLimit_Tick();
                    continue;
                }

                if (processUpdateRow.Results.Count - 1 < processUpdateRow.Cursor)
                    continue;

                string line = Regex.Replace(processUpdateRow.Results[processUpdateRow.Cursor], @"\s+", " ");
                if (!string.IsNullOrEmpty(line))
                {
                    if (Contains(line, "[youtube]"))
                    {
                        Update(() => processUpdateRow.UpdateStatus("Reading Metadata"));
                    }
                    else if (Contains(line, "[ffmpeg]"))
                    {
                        Update(() => processUpdateRow.UpdateStatus("Transcoding") );
                    }
                    else if (Contains(line, "[download]"))
                    {
                        string[] parts = line.Split(' ');
                        
                        if (Contains(line, "%") && parts.Length >= 8)
                        {
                            // download messages stream fast, bump the cursor up to one of the latest messages, if it exists...
                            // only start skipping cursor ahead once download messages have started otherwise important info could be skipped
                            if (processUpdateRow.Cursor + 10 < processUpdateRow.Results.Count)
                            {
                                processUpdateRow.Cursor = processUpdateRow.Results.Count;
                            }

                            Update(() => processUpdateRow.DownloadUpdate(parts));
                        }
                    }
                    else if (line.ToLower().IndexOf("error", StringComparison.Ordinal) > -1 || line[..21] == "Usage: youtube-dl.exe")
                    {
                        Console.WriteLine($@"error {line}");
                        finishedProcesses.Add(processUpdateRow);
                        Update(() => { processUpdateRow.SetErrorState(); });
                        Console.Write(processUpdateRow.Results);
                        TimerProcessLimit_Tick();
                    }
                }

                processUpdateRow.Cursor += 1;
            }

            FinishedProcesses.UnionWith(finishedProcesses);
            foreach (ProcessUpdateRow processUpdateRow in finishedProcesses)
            {
                _ProcessUpdates.Remove(processUpdateRow.Tag);
            }
        }

        private void Update(Action action)
        {
            BeginInvoke(action, null);
        }

        private void DownloadVideoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DownloadMediaDialog("video");
        }

        private void DownloadAudioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DownloadMediaDialog("audio");
        }

        private void DownloadMediaDialog(string type)
        {
            FrameNewMedia frameNewMedia = new(type);

            if (frameNewMedia.ShowDialog() != DialogResult.OK)
                return;
            
            AddMediaItemRow(frameNewMedia.Title, frameNewMedia.Type, frameNewMedia.Url, frameNewMedia.Parameters, frameNewMedia.Filepath);
            TimerProcessLimit_Tick();
        }

        private void AddMediaItemRow(string title, string type, string url, string opts, string filePath)
        {
            string tag = $"{Common.RandomString(5)}{DateTime.UtcNow.Ticks}";
            string[] elements = { title, "Waiting", type, "-", "", "0%", "0.0 KB/s", url, filePath };
            
            listItems.Items.Add(new ListViewItem(elements)
            {
                Tag = tag,
                BackColor = Color.LightGray
            });

            AddProcess(tag, opts, type == "audio" ? 1 : 0);
        }

        private void AddProcess(string tag, string parameterString, int index)
        {
            ProcessUpdateRow processUpdateRow = new(parameterString)
            {
                ViewItem = lastViewItem,
                Tag = tag
            };

            ProcessQueue.Enqueue(processUpdateRow);
            processUpdateRow.ViewItem.ImageIndex = index;
        }

        private void OpenFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filePath = firstSelected.SubItems[8].Text;
            if (!string.IsNullOrEmpty(filePath))
            {
                // TODO: fix file pathing issue
                if (File.Exists(filePath))
                {
                    Common.OpenFolderWithFileSelect(filePath);
                }
                else if (File.Exists($"{filePath}.part"))
                {
                    Common.OpenFolderWithFileSelect($"{filePath}.part");
                }
                return;
            }

            // couldn't find folder, rolling back to just the folder with no select
            Console.WriteLine($@"Couldn't find file to open at {filePath}");
            Common.OpenFolder(Settings.Data.DefaultDownloadPath);
        }

        private void ListItems_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right || !listItems.FocusedItem.Bounds.Contains(e.Location))
                return;

            contextMenuListItems.Show(Cursor.Position);
            contextMenuListItems.Items["retryDownloadToolStripMenuItem"].Visible =
                GetSelectedProcessStatus() == ProcessStatus.Error;
            contextMenuListItems.Items["stopDownloadToolStripMenuItem"].Visible =
                GetSelectedProcessStatus() == ProcessStatus.Running;
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using Form form = new FrameAbout();
            form.ShowDialog();
        }

        private void ConvertMediaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using Form form = new FrameConvert();
            form.ShowDialog();
        }

        private void OpenURLInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (noneSelected)
                return;
            
            string url = firstSelected.SubItems[7].Text;
            if (!string.IsNullOrEmpty(url))
            {
                Common.GetWebResourceHandle(url);
            }
        }

        private void OpenMediaInPlayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (noneSelected || GetSubItem(1) != "Complete") 
                return;
            
            string filePath = GetSubItem(8);
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                Common.GetWebResourceHandle(filePath);
            }
        }
        
        private ProcessUpdateRow GetSelectedRow()
        {
            return _ProcessUpdates.FirstOrDefault(p => p.Value.ViewItem == firstSelected).Value;
        }
        
        private void RetryDownloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (noneSelected || GetSubItem(1) != "Error")
                return;

            ProcessUpdateRow? processUpdateRow = FinishedProcesses.FirstOrDefault(p => p.ViewItem == firstSelected);
            if (processUpdateRow is not null)
            {
                FinishedProcesses.Remove(processUpdateRow);
                ProcessQueue.Enqueue(processUpdateRow);
                processUpdateRow.Retry();
                TimerProcessLimit_Tick();
            }
        }

        private void StopDownloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (noneSelected || (GetSubItem(1) == "Complete" && GetSubItem(1) == "Error"))
                return;

            ProcessUpdateRow processUpdateRow = GetSelectedRow();
            Update(() => { processUpdateRow.Stop(); });
        }

        private void DeleteRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (noneSelected || (GetSubItem(1) != "Complete" && GetSubItem(1) != "Error"))
                return;

            FinishedProcesses.RemoveWhere(p => p.ViewItem == firstSelected);
            listItems.Items.Remove(firstSelected);
        }
        
        private void CopyUrlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (noneSelected)
                return;

            Clipboard.SetText(firstSelected.SubItems[7].Text);
        }

        private void ListItems_DoubleClick(object sender, EventArgs e)
        {
            if (!noneSelected)
            {
                OpenMediaInPlayerToolStripMenuItem_Click(sender, e);
            }
        }

        private void FrameMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!listItems.Items.Cast<ListViewItem>().Any(item =>
                {
                    string? text = item.SubItems[1].Text;
                    return text != "Complete" && text != "Error";
                })) 
                return;
            
            if (MessageBox.Show(Resources.ExitWarning, "Verify Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                e.Cancel = true;
                return;
            }

            KillActiveDownloads();

            e.Cancel = false;
        }

        private void KillActiveDownloads()
        {
            // kill all processes
            foreach (ProcessUpdateRow pur in _ProcessUpdates.Values)
            {
                try
                {
                    Common.KillProcessAndChildren(pur.Process.Id);
                }
                catch
                {
                    // do nothing
                }
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
            Common.GetWebResourceHandle("https://www.ffmpeg.org/download.html");
        }

        private void DownloadHandbrakeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Common.GetWebResourceHandle("https://handbrake.fr/downloads.php");
        }

        private void DownloadVLCPlayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Common.GetWebResourceHandle("https://www.videolan.org/vlc");
        }
        
        private void DownloadAtomicParsleyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Common.GetWebResourceHandle("http://atomicparsley.sourceforge.net");
        }

        private void TimerStatusBar_Tick(object sender, EventArgs e)
        {
            toolBarLabelCpu.Text = $@"CPU: {Common.GetCpuUsagePercentage()}";
            toolBarLabelMemory.Text = $@"Available Memory: {Common.GetAvailableMemory()}";
            toolBarLabelNetwork.Text = $@"Network Ingress: {Common.GetNetworkTransfer()}";
        }

        private void CheckForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // if sender obj is bool then version being checked on startup passively and dont show dialog that it's up to date
            AppVersionModel? result = AppUpdate.CheckForNewAppVersion();
            switch (result is { IsNewerVersionAvailable: true })
            {
                case true:
                {
                    if (OpenYesNoModal(string.Format(Resources.NewUpdate, result?.Version), "New Version Available",
                            MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        Common.GetWebResourceHandle("https://github.com/dantheman213/JackTheVideoRipper/releases");
                    }

                    break;
                }
                case false when sender is not bool:
                    OpenConfirmModal(Resources.UpToDate, "Version Current", MessageBoxIcon.Information);
                    break;
            }
        }

        private void FrameMain_Shown(object sender, EventArgs e)
        {
            CheckDependencies();

            Task.Run(YouTubeDl.CheckForUpdates);
        }

        private void DownloadVS2010RedistributableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Common.GetWebResourceHandle("https://www.microsoft.com/en-us/download/details.aspx?id=26999");
        }

        private void OpenDownloadFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Common.OpenFolder(Settings.Data.DefaultDownloadPath);
        }
        
        private void DownloadBatchManualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string? urls = null;
            if (sender is string s)
            {
                urls = s;
            }

            FrameNewMediaBatch frameNewMediaBatch = new(urls);
            DialogResult result = frameNewMediaBatch.ShowDialog();

            if (result != DialogResult.OK || frameNewMediaBatch.items is not { Count: > 0 })
                return;
            
            foreach(DownloadMediaItem item in frameNewMediaBatch.items)
            {
                AddMediaItemRow(item.Title, frameNewMediaBatch.type, item.Url, item.Parameters, item.FilePath);
            }

            QueueBatchDownloads();
        }

        private void QueueBatchDownloads()
        {
            for (int i = 0; i < Settings.Data.MaxConcurrentDownloads; i++)
            {
                Application.DoEvents();
                Thread.Sleep(300);
                TimerProcessLimit_Tick();
            }
        }

        private void DownloadBatchDocumentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                InitialDirectory = Settings.Data.DefaultDownloadPath,
                Filter = @"All Files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK || !File.Exists(openFileDialog.FileName))
                return;

            string payload = File.ReadAllText(openFileDialog.FileName);
            if (string.IsNullOrEmpty(payload)) 
                return;

            DownloadBatchManualToolStripMenuItem_Click(string.Join("\r\n", Import.GetAllUrlsFromPayload(payload)), e);
        }

        private string GetYouTubeLink(string id) => $"https://www.youtube.com/watch?v={id}";

        private void DownloadBatchYouTubePlaylistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrameImportPlaylist frameImportPlaylist = new();
            
            if (frameImportPlaylist.ShowDialog() != DialogResult.OK)
                return;

            if (YouTubeDl.GetPlaylistMetadata(frameImportPlaylist.Url) is not { } items)
                return;
            
            string result = string.Join("\r\n", items.Select(item => GetYouTubeLink(item.Id)));

            DownloadBatchManualToolStripMenuItem_Click(result, e);
        }

        private void TimerCheckForUpdates_Tick(object sender, EventArgs e)
        {
            timerCheckForUpdates.Enabled = false;
            CheckForUpdatesToolStripMenuItem_Click(false, e);
        }

        private void TimerProcessLimit_Tick(object? sender = null, EventArgs? e = null)
        {
            if (ProcessQueue.Count < 1)
                return;

            for (int i = _ProcessUpdates.Count; i < Settings.Data.MaxConcurrentDownloads; i++)
            {
                if (ProcessQueue.Count < 1)
                    break;
                ProcessUpdateRow nextProcess = ProcessQueue.Dequeue();
                _ProcessUpdates.Add(nextProcess.Tag, nextProcess);
                nextProcess.Start();
                timerProcessLimit.Enabled = false;
            }
        }

        private void SettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (new FrameSettings().ShowDialog() == DialogResult.OK)
            {
                QueueBatchDownloads();
            }
        }

        private void OpenTaskManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Common.OpenTaskManager();
        }

        private void StatusBar_DoubleClick(object sender, EventArgs e)
        {
            OpenTaskManagerToolStripMenuItem_Click(sender, e);
        }

        private void DownloadYtdlpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Common.GetWebResourceHandle("https://github.com/yt-dlp/yt-dlp");
        }

        private void OpenDependenciesFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Common.OpenFileExplorer(Common.InstallDirectory);
        }

        private void FileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void EditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void ClearSuccessesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ProcessUpdateRow processUpdateRow in FinishedProcesses.Where(p => !p.Failed))
            {
                listItems.Items.Remove(processUpdateRow.ViewItem);
            }
            
            FinishedProcesses.RemoveWhere(p => !p.Failed);
        }

        private void ClearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listItems.Items.Clear();
            FinishedProcesses.Clear();
            foreach (ProcessUpdateRow processUpdateRow in _ProcessUpdates.Values)
            {
                processUpdateRow.Stop();
            }
            _ProcessUpdates.Clear();
            ProcessQueue.Clear();
        }

        private void ClearFailuresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ProcessUpdateRow processUpdateRow in FinishedProcesses.Where(p => p.Failed))
            {
                listItems.Items.Remove(processUpdateRow.ViewItem);
            }
            
            FinishedProcesses.RemoveWhere(p => p.Failed);
        }

        private void StopAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ProcessUpdateRow processUpdateRow in _ProcessUpdates.Values)
            {
                processUpdateRow.Stop();
            }
        }

        private void RetryAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ProcessUpdateRow processUpdateRow in FinishedProcesses.Where(p => p.Failed))
            {
                ProcessQueue.Enqueue(processUpdateRow);
                processUpdateRow.Retry();
                TimerProcessLimit_Tick();
            }

            FinishedProcesses.RemoveWhere(p => p.Failed);
        }

        private void CopyFailedUrlsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IEnumerable<string> failedUrls = FinishedProcesses
                .Where(p => p.Failed)
                .Select(p => p.ViewItem.SubItems[7].Text);
            Clipboard.SetText(string.Join("\n", failedUrls));
        }
    }
}
