namespace JackTheVideoRipper
{
    partial class FrameMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
         this.components = new System.ComponentModel.Container();
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrameMain));
         this.listItems = new System.Windows.Forms.ListView();
         this.cTitle = new System.Windows.Forms.ColumnHeader();
         this.cStatus = new System.Windows.Forms.ColumnHeader();
         this.cType = new System.Windows.Forms.ColumnHeader();
         this.cSize = new System.Windows.Forms.ColumnHeader();
         this.cProgress = new System.Windows.Forms.ColumnHeader();
         this.cDownloadSpeed = new System.Windows.Forms.ColumnHeader();
         this.cETA = new System.Windows.Forms.ColumnHeader();
         this.cURL = new System.Windows.Forms.ColumnHeader();
         this.cPath = new System.Windows.Forms.ColumnHeader();
         this.imgList = new System.Windows.Forms.ImageList(this.components);
         this.menuStrip = new System.Windows.Forms.MenuStrip();
         this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
         this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.clearAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.retryAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.stopAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
         this.clearSuccessesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.clearFailuresToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
         this.copyFailedUrlsToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.youTubeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.downloadVideoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.downloadAudioToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.batchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.downloadBatchManualToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.downloadBatchDocumentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.downloadBatchYouTubePlaylistlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
         this.openDownloadFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
         this.checkForUpdatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
         this.openDependenciesFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
         this.recommendedUtilitiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.vlcPlayerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.handbrakeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.bundledUtilitiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.vS2010RedistributableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.fFmpegToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.atomicParsleyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.ytdlpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
         this.openTaskManagerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.contextMenuListItems = new System.Windows.Forms.ContextMenuStrip(this.components);
         this.convertMediaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
         this.openFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.openURLInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.openMediaInPlayerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
         this.retryDownloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.copyUrlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.stopDownloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.toolBar = new System.Windows.Forms.ToolStrip();
         this.toolStripButtonDownloadVideo = new System.Windows.Forms.ToolStripButton();
         this.toolStripButtonDownloadAudio = new System.Windows.Forms.ToolStripButton();
         this.statusBar = new System.Windows.Forms.StatusStrip();
         this.toolbarLabelStatus = new System.Windows.Forms.ToolStripStatusLabel();
         this.toolBarLabelCpu = new System.Windows.Forms.ToolStripStatusLabel();
         this.toolBarLabelMemory = new System.Windows.Forms.ToolStripStatusLabel();
         this.toolBarLabelNetwork = new System.Windows.Forms.ToolStripStatusLabel();
         this.timerStatusBar = new System.Windows.Forms.Timer(this.components);
         this.timerCheckForUpdates = new System.Windows.Forms.Timer(this.components);
         this.timerProcessLimit = new System.Windows.Forms.Timer(this.components);
         this.splitContainer1 = new System.Windows.Forms.SplitContainer();
         this.menuStrip.SuspendLayout();
         this.contextMenuListItems.SuspendLayout();
         this.toolBar.SuspendLayout();
         this.statusBar.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
         this.splitContainer1.Panel1.SuspendLayout();
         this.splitContainer1.Panel2.SuspendLayout();
         this.splitContainer1.SuspendLayout();
         this.SuspendLayout();
         // 
         // listItems
         // 
         this.listItems.BackColor = System.Drawing.SystemColors.Window;
         this.listItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.cTitle,
            this.cStatus,
            this.cType,
            this.cSize,
            this.cProgress,
            this.cDownloadSpeed,
            this.cETA,
            this.cURL,
            this.cPath});
         this.listItems.Dock = System.Windows.Forms.DockStyle.Fill;
         this.listItems.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
         this.listItems.FullRowSelect = true;
         this.listItems.Location = new System.Drawing.Point(0, 0);
         this.listItems.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
         this.listItems.MultiSelect = false;
         this.listItems.Name = "listItems";
         this.listItems.Size = new System.Drawing.Size(1070, 439);
         this.listItems.SmallImageList = this.imgList;
         this.listItems.TabIndex = 0;
         this.listItems.UseCompatibleStateImageBehavior = false;
         this.listItems.View = System.Windows.Forms.View.Details;
         this.listItems.DoubleClick += new System.EventHandler(this.ListItems_DoubleClick);
         this.listItems.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ListItems_MouseClick);
         // 
         // cTitle
         // 
         this.cTitle.Text = "Title";
         this.cTitle.Width = 400;
         // 
         // cStatus
         // 
         this.cStatus.Text = "Status";
         this.cStatus.Width = 140;
         // 
         // cType
         // 
         this.cType.Text = "Type";
         // 
         // cSize
         // 
         this.cSize.Text = "Size";
         this.cSize.Width = 130;
         // 
         // cProgress
         // 
         this.cProgress.Text = "Progress";
         this.cProgress.Width = 120;
         // 
         // cDownloadSpeed
         // 
         this.cDownloadSpeed.Text = "Download Speed";
         this.cDownloadSpeed.Width = 157;
         // 
         // cETA
         // 
         this.cETA.Text = "ETA";
         this.cETA.Width = 147;
         // 
         // cURL
         // 
         this.cURL.Text = "URL";
         this.cURL.Width = 300;
         // 
         // cPath
         // 
         this.cPath.Text = "Path";
         this.cPath.Width = 300;
         // 
         // imgList
         // 
         this.imgList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
         this.imgList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgList.ImageStream")));
         this.imgList.TransparentColor = System.Drawing.Color.Transparent;
         this.imgList.Images.SetKeyName(0, "movie-medium.ico");
         this.imgList.Images.SetKeyName(1, "song-medium.ico");
         // 
         // menuStrip
         // 
         this.menuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
         this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.youTubeToolStripMenuItem,
            this.helpToolStripMenuItem});
         this.menuStrip.Location = new System.Drawing.Point(0, 0);
         this.menuStrip.Name = "menuStrip";
         this.menuStrip.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
         this.menuStrip.Size = new System.Drawing.Size(1070, 24);
         this.menuStrip.TabIndex = 1;
         this.menuStrip.Text = "menuStrip1";
         // 
         // fileToolStripMenuItem
         // 
         this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem,
            this.toolStripSeparator5,
            this.exitToolStripMenuItem});
         this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
         this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
         this.fileToolStripMenuItem.Text = "&File";
         // 
         // settingsToolStripMenuItem
         // 
         this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
         this.settingsToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
         this.settingsToolStripMenuItem.Text = "&Settings";
         this.settingsToolStripMenuItem.Click += new System.EventHandler(this.SettingsToolStripMenuItem_Click);
         // 
         // toolStripSeparator5
         // 
         this.toolStripSeparator5.Name = "toolStripSeparator5";
         this.toolStripSeparator5.Size = new System.Drawing.Size(113, 6);
         // 
         // exitToolStripMenuItem
         // 
         this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
         this.exitToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
         this.exitToolStripMenuItem.Text = "E&xit";
         this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
         // 
         // editToolStripMenuItem
         // 
         this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearAllToolStripMenuItem,
            this.retryAllToolStripMenuItem,
            this.stopAllToolStripMenuItem,
            this.toolStripSeparator7,
            this.clearSuccessesToolStripMenuItem,
            this.clearFailuresToolStripMenuItem,
            this.toolStripSeparator8,
            this.copyFailedUrlsToClipboardToolStripMenuItem});
         this.editToolStripMenuItem.Name = "editToolStripMenuItem";
         this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
         this.editToolStripMenuItem.Text = "&Edit";
         // 
         // clearAllToolStripMenuItem
         // 
         this.clearAllToolStripMenuItem.Name = "clearAllToolStripMenuItem";
         this.clearAllToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
         this.clearAllToolStripMenuItem.Text = "&Clear All";
         this.clearAllToolStripMenuItem.Click += new System.EventHandler(this.ClearAllToolStripMenuItem_Click);
         // 
         // retryAllToolStripMenuItem
         // 
         this.retryAllToolStripMenuItem.Name = "retryAllToolStripMenuItem";
         this.retryAllToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
         this.retryAllToolStripMenuItem.Text = "&Retry All";
         this.retryAllToolStripMenuItem.Click += new System.EventHandler(this.RetryAllToolStripMenuItem_Click);
         // 
         // stopAllToolStripMenuItem
         // 
         this.stopAllToolStripMenuItem.Name = "stopAllToolStripMenuItem";
         this.stopAllToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
         this.stopAllToolStripMenuItem.Text = "&Stop All";
         this.stopAllToolStripMenuItem.Click += new System.EventHandler(this.StopAllToolStripMenuItem_Click);
         // 
         // toolStripSeparator7
         // 
         this.toolStripSeparator7.Name = "toolStripSeparator7";
         this.toolStripSeparator7.Size = new System.Drawing.Size(156, 6);
         // 
         // clearSuccessesToolStripMenuItem
         // 
         this.clearSuccessesToolStripMenuItem.Name = "clearSuccessesToolStripMenuItem";
         this.clearSuccessesToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
         this.clearSuccessesToolStripMenuItem.Text = "Clear &Successes";
         this.clearSuccessesToolStripMenuItem.Click += new System.EventHandler(this.ClearSuccessesToolStripMenuItem_Click);
         // 
         // clearFailuresToolStripMenuItem
         // 
         this.clearFailuresToolStripMenuItem.Name = "clearFailuresToolStripMenuItem";
         this.clearFailuresToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
         this.clearFailuresToolStripMenuItem.Text = "Clear &Failures";
         this.clearFailuresToolStripMenuItem.Click += new System.EventHandler(this.ClearFailuresToolStripMenuItem_Click);
         // 
         // toolStripSeparator8
         // 
         this.toolStripSeparator8.Name = "toolStripSeparator8";
         this.toolStripSeparator8.Size = new System.Drawing.Size(156, 6);
         // 
         // copyFailedUrlsToClipboardToolStripMenuItem
         // 
         this.copyFailedUrlsToClipboardToolStripMenuItem.Name = "copyFailedUrlsToClipboardToolStripMenuItem";
         this.copyFailedUrlsToClipboardToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
         this.copyFailedUrlsToClipboardToolStripMenuItem.Text = "Copy Failed Urls";
         this.copyFailedUrlsToClipboardToolStripMenuItem.Click += new System.EventHandler(this.CopyFailedUrlsToolStripMenuItem_Click);
         // 
         // youTubeToolStripMenuItem
         // 
         this.youTubeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.downloadVideoToolStripMenuItem,
            this.downloadAudioToolStripMenuItem,
            this.batchToolStripMenuItem,
            this.toolStripSeparator3,
            this.openDownloadFolderToolStripMenuItem});
         this.youTubeToolStripMenuItem.Name = "youTubeToolStripMenuItem";
         this.youTubeToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
         this.youTubeToolStripMenuItem.Text = "&Media";
         // 
         // downloadVideoToolStripMenuItem
         // 
         this.downloadVideoToolStripMenuItem.Name = "downloadVideoToolStripMenuItem";
         this.downloadVideoToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
         this.downloadVideoToolStripMenuItem.Text = "Download &Video";
         this.downloadVideoToolStripMenuItem.Click += new System.EventHandler(this.DownloadVideoToolStripMenuItem_Click);
         // 
         // downloadAudioToolStripMenuItem
         // 
         this.downloadAudioToolStripMenuItem.Name = "downloadAudioToolStripMenuItem";
         this.downloadAudioToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
         this.downloadAudioToolStripMenuItem.Text = "Download &Audio";
         this.downloadAudioToolStripMenuItem.Click += new System.EventHandler(this.DownloadAudioToolStripMenuItem_Click);
         // 
         // batchToolStripMenuItem
         // 
         this.batchToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.downloadBatchManualToolStripMenuItem,
            this.downloadBatchDocumentToolStripMenuItem,
            this.downloadBatchYouTubePlaylistlToolStripMenuItem});
         this.batchToolStripMenuItem.Name = "batchToolStripMenuItem";
         this.batchToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
         this.batchToolStripMenuItem.Text = "Download &Batch";
         // 
         // downloadBatchManualToolStripMenuItem
         // 
         this.downloadBatchManualToolStripMenuItem.Name = "downloadBatchManualToolStripMenuItem";
         this.downloadBatchManualToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
         this.downloadBatchManualToolStripMenuItem.Text = "&Manual";
         this.downloadBatchManualToolStripMenuItem.Click += new System.EventHandler(this.DownloadBatchManualToolStripMenuItem_Click);
         // 
         // downloadBatchDocumentToolStripMenuItem
         // 
         this.downloadBatchDocumentToolStripMenuItem.Name = "downloadBatchDocumentToolStripMenuItem";
         this.downloadBatchDocumentToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
         this.downloadBatchDocumentToolStripMenuItem.Text = "Document";
         this.downloadBatchDocumentToolStripMenuItem.Click += new System.EventHandler(this.DownloadBatchDocumentToolStripMenuItem_Click);
         // 
         // downloadBatchYouTubePlaylistlToolStripMenuItem
         // 
         this.downloadBatchYouTubePlaylistlToolStripMenuItem.Name = "downloadBatchYouTubePlaylistlToolStripMenuItem";
         this.downloadBatchYouTubePlaylistlToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
         this.downloadBatchYouTubePlaylistlToolStripMenuItem.Text = "YouTube Playlist";
         this.downloadBatchYouTubePlaylistlToolStripMenuItem.Click += new System.EventHandler(this.DownloadBatchYouTubePlaylistToolStripMenuItem_Click);
         // 
         // toolStripSeparator3
         // 
         this.toolStripSeparator3.Name = "toolStripSeparator3";
         this.toolStripSeparator3.Size = new System.Drawing.Size(193, 6);
         // 
         // openDownloadFolderToolStripMenuItem
         // 
         this.openDownloadFolderToolStripMenuItem.Name = "openDownloadFolderToolStripMenuItem";
         this.openDownloadFolderToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
         this.openDownloadFolderToolStripMenuItem.Text = "&Open Download Folder";
         this.openDownloadFolderToolStripMenuItem.Click += new System.EventHandler(this.OpenDownloadFolderToolStripMenuItem_Click);
         // 
         // helpToolStripMenuItem
         // 
         this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.toolStripSeparator1,
            this.checkForUpdatesToolStripMenuItem,
            this.toolStripSeparator6,
            this.openDependenciesFolderToolStripMenuItem,
            this.toolStripSeparator2,
            this.recommendedUtilitiesToolStripMenuItem,
            this.bundledUtilitiesToolStripMenuItem,
            this.toolStripSeparator4,
            this.openTaskManagerToolStripMenuItem});
         this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
         this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
         this.helpToolStripMenuItem.Text = "Help";
         // 
         // aboutToolStripMenuItem
         // 
         this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
         this.aboutToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
         this.aboutToolStripMenuItem.Text = "About";
         this.aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItem_Click);
         // 
         // toolStripSeparator1
         // 
         this.toolStripSeparator1.Name = "toolStripSeparator1";
         this.toolStripSeparator1.Size = new System.Drawing.Size(213, 6);
         // 
         // checkForUpdatesToolStripMenuItem
         // 
         this.checkForUpdatesToolStripMenuItem.Name = "checkForUpdatesToolStripMenuItem";
         this.checkForUpdatesToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
         this.checkForUpdatesToolStripMenuItem.Text = "Check For Updates";
         this.checkForUpdatesToolStripMenuItem.Click += new System.EventHandler(this.CheckForUpdatesToolStripMenuItem_Click);
         // 
         // toolStripSeparator6
         // 
         this.toolStripSeparator6.Name = "toolStripSeparator6";
         this.toolStripSeparator6.Size = new System.Drawing.Size(213, 6);
         // 
         // openDependenciesFolderToolStripMenuItem
         // 
         this.openDependenciesFolderToolStripMenuItem.Name = "openDependenciesFolderToolStripMenuItem";
         this.openDependenciesFolderToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
         this.openDependenciesFolderToolStripMenuItem.Text = "Open Dependencies Folder";
         this.openDependenciesFolderToolStripMenuItem.Click += new System.EventHandler(this.OpenDependenciesFolderToolStripMenuItem_Click);
         // 
         // toolStripSeparator2
         // 
         this.toolStripSeparator2.Name = "toolStripSeparator2";
         this.toolStripSeparator2.Size = new System.Drawing.Size(213, 6);
         // 
         // recommendedUtilitiesToolStripMenuItem
         // 
         this.recommendedUtilitiesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.vlcPlayerToolStripMenuItem,
            this.handbrakeToolStripMenuItem});
         this.recommendedUtilitiesToolStripMenuItem.Name = "recommendedUtilitiesToolStripMenuItem";
         this.recommendedUtilitiesToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
         this.recommendedUtilitiesToolStripMenuItem.Text = "Recommended Utilities";
         // 
         // vlcPlayerToolStripMenuItem
         // 
         this.vlcPlayerToolStripMenuItem.Name = "vlcPlayerToolStripMenuItem";
         this.vlcPlayerToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
         this.vlcPlayerToolStripMenuItem.Text = "VLC Player";
         this.vlcPlayerToolStripMenuItem.Click += new System.EventHandler(this.DownloadVLCPlayerToolStripMenuItem_Click);
         // 
         // handbrakeToolStripMenuItem
         // 
         this.handbrakeToolStripMenuItem.Name = "handbrakeToolStripMenuItem";
         this.handbrakeToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
         this.handbrakeToolStripMenuItem.Text = "Handbrake";
         this.handbrakeToolStripMenuItem.Click += new System.EventHandler(this.DownloadHandbrakeToolStripMenuItem_Click);
         // 
         // bundledUtilitiesToolStripMenuItem
         // 
         this.bundledUtilitiesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.vS2010RedistributableToolStripMenuItem,
            this.fFmpegToolStripMenuItem,
            this.atomicParsleyToolStripMenuItem,
            this.ytdlpToolStripMenuItem});
         this.bundledUtilitiesToolStripMenuItem.Name = "bundledUtilitiesToolStripMenuItem";
         this.bundledUtilitiesToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
         this.bundledUtilitiesToolStripMenuItem.Text = "Bundled Utilities";
         // 
         // vS2010RedistributableToolStripMenuItem
         // 
         this.vS2010RedistributableToolStripMenuItem.Name = "vS2010RedistributableToolStripMenuItem";
         this.vS2010RedistributableToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
         this.vS2010RedistributableToolStripMenuItem.Text = "VS 2010 Redistributable";
         this.vS2010RedistributableToolStripMenuItem.Click += new System.EventHandler(this.DownloadVS2010RedistributableToolStripMenuItem_Click);
         // 
         // fFmpegToolStripMenuItem
         // 
         this.fFmpegToolStripMenuItem.Name = "fFmpegToolStripMenuItem";
         this.fFmpegToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
         this.fFmpegToolStripMenuItem.Text = "FFmpeg";
         this.fFmpegToolStripMenuItem.Click += new System.EventHandler(this.DownloadFfmpegToolStripMenuItem_Click);
         // 
         // atomicParsleyToolStripMenuItem
         // 
         this.atomicParsleyToolStripMenuItem.Name = "atomicParsleyToolStripMenuItem";
         this.atomicParsleyToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
         this.atomicParsleyToolStripMenuItem.Text = "AtomicParsley";
         this.atomicParsleyToolStripMenuItem.Click += new System.EventHandler(this.DownloadAtomicParsleyToolStripMenuItem_Click);
         // 
         // ytdlpToolStripMenuItem
         // 
         this.ytdlpToolStripMenuItem.Name = "ytdlpToolStripMenuItem";
         this.ytdlpToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
         this.ytdlpToolStripMenuItem.Text = "yt-dlp";
         this.ytdlpToolStripMenuItem.Click += new System.EventHandler(this.DownloadYtdlpToolStripMenuItem_Click);
         // 
         // toolStripSeparator4
         // 
         this.toolStripSeparator4.Name = "toolStripSeparator4";
         this.toolStripSeparator4.Size = new System.Drawing.Size(213, 6);
         // 
         // openTaskManagerToolStripMenuItem
         // 
         this.openTaskManagerToolStripMenuItem.Name = "openTaskManagerToolStripMenuItem";
         this.openTaskManagerToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
         this.openTaskManagerToolStripMenuItem.Text = "Open Task Manager";
         this.openTaskManagerToolStripMenuItem.Click += new System.EventHandler(this.OpenTaskManagerToolStripMenuItem_Click);
         // 
         // contextMenuListItems
         // 
         this.contextMenuListItems.ImageScalingSize = new System.Drawing.Size(24, 24);
         this.contextMenuListItems.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.convertMediaToolStripMenuItem,
            this.toolStripMenuItem1,
            this.openFolderToolStripMenuItem,
            this.openURLInBrowserToolStripMenuItem,
            this.openMediaInPlayerToolStripMenuItem,
            this.toolStripMenuItem2,
            this.retryDownloadToolStripMenuItem,
            this.copyUrlToolStripMenuItem,
            this.stopDownloadToolStripMenuItem,
            this.deleteRowToolStripMenuItem});
         this.contextMenuListItems.Name = "contextMenuListItems";
         this.contextMenuListItems.Size = new System.Drawing.Size(190, 192);
         // 
         // convertMediaToolStripMenuItem
         // 
         this.convertMediaToolStripMenuItem.Name = "convertMediaToolStripMenuItem";
         this.convertMediaToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
         this.convertMediaToolStripMenuItem.Text = "Convert Media";
         this.convertMediaToolStripMenuItem.Click += new System.EventHandler(this.ConvertMediaToolStripMenuItem_Click);
         // 
         // toolStripMenuItem1
         // 
         this.toolStripMenuItem1.Name = "toolStripMenuItem1";
         this.toolStripMenuItem1.Size = new System.Drawing.Size(186, 6);
         // 
         // openFolderToolStripMenuItem
         // 
         this.openFolderToolStripMenuItem.Name = "openFolderToolStripMenuItem";
         this.openFolderToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
         this.openFolderToolStripMenuItem.Text = "Open Folder";
         this.openFolderToolStripMenuItem.Click += new System.EventHandler(this.OpenFolderToolStripMenuItem_Click);
         // 
         // openURLInBrowserToolStripMenuItem
         // 
         this.openURLInBrowserToolStripMenuItem.Name = "openURLInBrowserToolStripMenuItem";
         this.openURLInBrowserToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
         this.openURLInBrowserToolStripMenuItem.Text = "Open URL In Browser";
         this.openURLInBrowserToolStripMenuItem.Click += new System.EventHandler(this.OpenURLInBrowserToolStripMenuItem_Click);
         // 
         // openMediaInPlayerToolStripMenuItem
         // 
         this.openMediaInPlayerToolStripMenuItem.Name = "openMediaInPlayerToolStripMenuItem";
         this.openMediaInPlayerToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
         this.openMediaInPlayerToolStripMenuItem.Text = "Open Media In Player";
         this.openMediaInPlayerToolStripMenuItem.Click += new System.EventHandler(this.OpenMediaInPlayerToolStripMenuItem_Click);
         // 
         // toolStripMenuItem2
         // 
         this.toolStripMenuItem2.Name = "toolStripMenuItem2";
         this.toolStripMenuItem2.Size = new System.Drawing.Size(186, 6);
         // 
         // retryDownloadToolStripMenuItem
         // 
         this.retryDownloadToolStripMenuItem.Name = "retryDownloadToolStripMenuItem";
         this.retryDownloadToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
         this.retryDownloadToolStripMenuItem.Text = "Retry Download";
         this.retryDownloadToolStripMenuItem.Click += new System.EventHandler(this.RetryDownloadToolStripMenuItem_Click);
         // 
         // copyUrlToolStripMenuItem
         // 
         this.copyUrlToolStripMenuItem.Name = "copyUrlToolStripMenuItem";
         this.copyUrlToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
         this.copyUrlToolStripMenuItem.Text = "Copy Url to Clipboard";
         this.copyUrlToolStripMenuItem.Click += new System.EventHandler(this.CopyUrlToolStripMenuItem_Click);
         // 
         // stopDownloadToolStripMenuItem
         // 
         this.stopDownloadToolStripMenuItem.Name = "stopDownloadToolStripMenuItem";
         this.stopDownloadToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
         this.stopDownloadToolStripMenuItem.Text = "Stop Download";
         this.stopDownloadToolStripMenuItem.Click += new System.EventHandler(this.StopDownloadToolStripMenuItem_Click);
         // 
         // deleteRowToolStripMenuItem
         // 
         this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
         this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
         this.deleteRowToolStripMenuItem.Text = "Delete Row";
         this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.DeleteRowToolStripMenuItem_Click);
         // 
         // toolBar
         // 
         this.toolBar.ImageScalingSize = new System.Drawing.Size(34, 39);
         this.toolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonDownloadVideo,
            this.toolStripButtonDownloadAudio});
         this.toolBar.Location = new System.Drawing.Point(0, 24);
         this.toolBar.Name = "toolBar";
         this.toolBar.Size = new System.Drawing.Size(1070, 46);
         this.toolBar.TabIndex = 2;
         this.toolBar.Text = "toolStrip1";
         // 
         // toolStripButtonDownloadVideo
         // 
         this.toolStripButtonDownloadVideo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
         this.toolStripButtonDownloadVideo.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonDownloadVideo.Image")));
         this.toolStripButtonDownloadVideo.ImageTransparentColor = System.Drawing.Color.Black;
         this.toolStripButtonDownloadVideo.Name = "toolStripButtonDownloadVideo";
         this.toolStripButtonDownloadVideo.Size = new System.Drawing.Size(38, 43);
         this.toolStripButtonDownloadVideo.ToolTipText = "Download Media as Video";
         this.toolStripButtonDownloadVideo.Click += new System.EventHandler(this.ToolStripButtonDownloadVideo_Click);
         // 
         // toolStripButtonDownloadAudio
         // 
         this.toolStripButtonDownloadAudio.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
         this.toolStripButtonDownloadAudio.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonDownloadAudio.Image")));
         this.toolStripButtonDownloadAudio.ImageTransparentColor = System.Drawing.Color.Black;
         this.toolStripButtonDownloadAudio.Name = "toolStripButtonDownloadAudio";
         this.toolStripButtonDownloadAudio.Size = new System.Drawing.Size(38, 43);
         this.toolStripButtonDownloadAudio.ToolTipText = "Download Media as Audio";
         this.toolStripButtonDownloadAudio.Click += new System.EventHandler(this.ToolStripButtonDownloadAudio_Click);
         // 
         // statusBar
         // 
         this.statusBar.Dock = System.Windows.Forms.DockStyle.Fill;
         this.statusBar.ImageScalingSize = new System.Drawing.Size(24, 24);
         this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolbarLabelStatus,
            this.toolBarLabelCpu,
            this.toolBarLabelMemory,
            this.toolBarLabelNetwork});
         this.statusBar.Location = new System.Drawing.Point(0, 0);
         this.statusBar.Name = "statusBar";
         this.statusBar.Padding = new System.Windows.Forms.Padding(1, 0, 11, 0);
         this.statusBar.Size = new System.Drawing.Size(1070, 25);
         this.statusBar.TabIndex = 3;
         this.statusBar.Text = "statusStrip1";
         this.statusBar.DoubleClick += new System.EventHandler(this.StatusBar_DoubleClick);
         // 
         // toolbarLabelStatus
         // 
         this.toolbarLabelStatus.Name = "toolbarLabelStatus";
         this.toolbarLabelStatus.Size = new System.Drawing.Size(35, 20);
         this.toolbarLabelStatus.Text = "Idle | ";
         // 
         // toolBarLabelCpu
         // 
         this.toolBarLabelCpu.Name = "toolBarLabelCpu";
         this.toolBarLabelCpu.Size = new System.Drawing.Size(67, 20);
         this.toolBarLabelCpu.Text = "CPU: N/A | ";
         // 
         // toolBarLabelMemory
         // 
         this.toolBarLabelMemory.Name = "toolBarLabelMemory";
         this.toolBarLabelMemory.Size = new System.Drawing.Size(140, 20);
         this.toolBarLabelMemory.Text = "Available Memory: N/A | ";
         // 
         // toolBarLabelNetwork
         // 
         this.toolBarLabelNetwork.Name = "toolBarLabelNetwork";
         this.toolBarLabelNetwork.Size = new System.Drawing.Size(115, 20);
         this.toolBarLabelNetwork.Text = "Network Usage: N/A";
         // 
         // timerStatusBar
         // 
         this.timerStatusBar.Interval = 2500;
         this.timerStatusBar.Tick += new System.EventHandler(this.TimerStatusBar_Tick);
         // 
         // timerCheckForUpdates
         // 
         this.timerCheckForUpdates.Enabled = true;
         this.timerCheckForUpdates.Interval = 1000;
         this.timerCheckForUpdates.Tick += new System.EventHandler(this.TimerCheckForUpdates_Tick);
         // 
         // timerProcessLimit
         // 
         this.timerProcessLimit.Interval = 3500;
         this.timerProcessLimit.Tick += new System.EventHandler(this.TimerProcessLimit_Tick);
         // 
         // splitContainer1
         // 
         this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.splitContainer1.Location = new System.Drawing.Point(0, 70);
         this.splitContainer1.Margin = new System.Windows.Forms.Padding(2);
         this.splitContainer1.Name = "splitContainer1";
         this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
         // 
         // splitContainer1.Panel1
         // 
         this.splitContainer1.Panel1.Controls.Add(this.listItems);
         // 
         // splitContainer1.Panel2
         // 
         this.splitContainer1.Panel2.Controls.Add(this.statusBar);
         this.splitContainer1.Size = new System.Drawing.Size(1070, 466);
         this.splitContainer1.SplitterDistance = 439;
         this.splitContainer1.SplitterWidth = 2;
         this.splitContainer1.TabIndex = 4;
         // 
         // FrameMain
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(1070, 536);
         this.Controls.Add(this.splitContainer1);
         this.Controls.Add(this.toolBar);
         this.Controls.Add(this.menuStrip);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.MainMenuStrip = this.menuStrip;
         this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
         this.Name = "FrameMain";
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
         this.Text = "JackTheVideoRipper";
         this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrameMain_FormClosing);
         this.Load += new System.EventHandler(this.FrameMain_Load);
         this.Shown += new System.EventHandler(this.FrameMain_Shown);
         this.menuStrip.ResumeLayout(false);
         this.menuStrip.PerformLayout();
         this.contextMenuListItems.ResumeLayout(false);
         this.toolBar.ResumeLayout(false);
         this.toolBar.PerformLayout();
         this.statusBar.ResumeLayout(false);
         this.statusBar.PerformLayout();
         this.splitContainer1.Panel1.ResumeLayout(false);
         this.splitContainer1.Panel2.ResumeLayout(false);
         this.splitContainer1.Panel2.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
         this.splitContainer1.ResumeLayout(false);
         this.ResumeLayout(false);
         this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listItems;
        private System.Windows.Forms.ColumnHeader cTitle;
        private System.Windows.Forms.ColumnHeader cSize;
        private System.Windows.Forms.ColumnHeader cProgress;
        private System.Windows.Forms.ColumnHeader cDownloadSpeed;
        private System.Windows.Forms.ColumnHeader cETA;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem youTubeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem downloadVideoToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader cURL;
        private System.Windows.Forms.ToolStripMenuItem downloadAudioToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader cType;
        private System.Windows.Forms.ColumnHeader cPath;
        private System.Windows.Forms.ContextMenuStrip contextMenuListItems;
        private System.Windows.Forms.ToolStripMenuItem openFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convertMediaToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ColumnHeader cStatus;
        private System.Windows.Forms.ToolStripMenuItem openURLInBrowserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openMediaInPlayerToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem retryDownloadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopDownloadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyUrlToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteRowToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolBar;
        private System.Windows.Forms.ToolStripButton toolStripButtonDownloadVideo;
        private System.Windows.Forms.ToolStripButton toolStripButtonDownloadAudio;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel toolbarLabelStatus;
        private System.Windows.Forms.ToolStripStatusLabel toolBarLabelCpu;
        private System.Windows.Forms.ToolStripStatusLabel toolBarLabelMemory;
        private System.Windows.Forms.Timer timerStatusBar;
        private System.Windows.Forms.ToolStripMenuItem checkForUpdatesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripStatusLabel toolBarLabelNetwork;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem openDownloadFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem batchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem downloadBatchManualToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem downloadBatchDocumentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem downloadBatchYouTubePlaylistlToolStripMenuItem;
        private System.Windows.Forms.Timer timerCheckForUpdates;
        private System.Windows.Forms.Timer timerProcessLimit;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private SplitContainer splitContainer1;
        private ImageList imgList;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripMenuItem openTaskManagerToolStripMenuItem;
        private ToolStripMenuItem openDependenciesFolderToolStripMenuItem;
        private ToolStripMenuItem recommendedUtilitiesToolStripMenuItem;
        private ToolStripMenuItem vlcPlayerToolStripMenuItem;
        private ToolStripMenuItem handbrakeToolStripMenuItem;
        private ToolStripMenuItem bundledUtilitiesToolStripMenuItem;
        private ToolStripMenuItem vS2010RedistributableToolStripMenuItem;
        private ToolStripMenuItem fFmpegToolStripMenuItem;
        private ToolStripMenuItem atomicParsleyToolStripMenuItem;
        private ToolStripMenuItem ytdlpToolStripMenuItem;
      private ToolStripMenuItem editToolStripMenuItem;
      private ToolStripMenuItem clearAllToolStripMenuItem;
      private ToolStripMenuItem retryAllToolStripMenuItem;
      private ToolStripMenuItem clearSuccessesToolStripMenuItem;
      private ToolStripSeparator toolStripSeparator7;
      private ToolStripMenuItem clearFailuresToolStripMenuItem;
      private ToolStripMenuItem stopAllToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator8;
        private ToolStripMenuItem copyFailedUrlsToClipboardToolStripMenuItem;
    }
}

