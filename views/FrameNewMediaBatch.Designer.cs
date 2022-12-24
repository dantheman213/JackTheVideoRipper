namespace JackTheVideoRipper
{
    partial class FrameNewMediaBatch
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
            this.textUrls = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkBoxExportVideo = new System.Windows.Forms.CheckBox();
            this.cbVideoEncoder = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbVideoQuality = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbAudioEncoder = new System.Windows.Forms.ComboBox();
            this.chkBoxExportAudio = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbAudioQuality = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.buttonLocationBrowse = new System.Windows.Forms.Button();
            this.textLocation = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.chkBoxIncludeAds = new System.Windows.Forms.CheckBox();
            this.chkEmbedSubs = new System.Windows.Forms.CheckBox();
            this.chkBoxEmbedThumbnail = new System.Windows.Forms.CheckBox();
            this.chkBoxWriteMetadata = new System.Windows.Forms.CheckBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonDownload = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // textUrls
            // 
            this.textUrls.Location = new System.Drawing.Point(9, 33);
            this.textUrls.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textUrls.Multiline = true;
            this.textUrls.Name = "textUrls";
            this.textUrls.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textUrls.Size = new System.Drawing.Size(759, 124);
            this.textUrls.TabIndex = 0;
            this.textUrls.WordWrap = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "URLs (one per line):";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkBoxExportVideo);
            this.groupBox1.Controls.Add(this.cbVideoEncoder);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cbVideoQuality);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Location = new System.Drawing.Point(9, 173);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Size = new System.Drawing.Size(373, 163);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Video";
            // 
            // chkBoxExportVideo
            // 
            this.chkBoxExportVideo.AutoSize = true;
            this.chkBoxExportVideo.Checked = true;
            this.chkBoxExportVideo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBoxExportVideo.Location = new System.Drawing.Point(16, 28);
            this.chkBoxExportVideo.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.chkBoxExportVideo.Name = "chkBoxExportVideo";
            this.chkBoxExportVideo.Size = new System.Drawing.Size(93, 19);
            this.chkBoxExportVideo.TabIndex = 19;
            this.chkBoxExportVideo.Text = "Export Video";
            this.chkBoxExportVideo.UseVisualStyleBackColor = true;
            this.chkBoxExportVideo.CheckedChanged += new System.EventHandler(this.ChkBoxExportVideo_CheckedChanged);
            // 
            // cbVideoEncoder
            // 
            this.cbVideoEncoder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbVideoEncoder.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.cbVideoEncoder.FormattingEnabled = true;
            this.cbVideoEncoder.Items.AddRange(new object[] {
            "(do not transcode)",
            "mkv",
            "mp4",
            "avi",
            "webm",
            "ogg",
            "flv"});
            this.cbVideoEncoder.Location = new System.Drawing.Point(82, 98);
            this.cbVideoEncoder.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cbVideoEncoder.Name = "cbVideoEncoder";
            this.cbVideoEncoder.Size = new System.Drawing.Size(282, 23);
            this.cbVideoEncoder.TabIndex = 18;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 98);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 15);
            this.label2.TabIndex = 17;
            this.label2.Text = "Encoder:";
            // 
            // cbVideoQuality
            // 
            this.cbVideoQuality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbVideoQuality.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.cbVideoQuality.FormattingEnabled = true;
            this.cbVideoQuality.Items.AddRange(new object[] {
            "High",
            "Low"});
            this.cbVideoQuality.Location = new System.Drawing.Point(82, 63);
            this.cbVideoQuality.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cbVideoQuality.Name = "cbVideoQuality";
            this.cbVideoQuality.Size = new System.Drawing.Size(282, 23);
            this.cbVideoQuality.TabIndex = 16;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 63);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(48, 15);
            this.label7.TabIndex = 15;
            this.label7.Text = "Quality:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbAudioEncoder);
            this.groupBox2.Controls.Add(this.chkBoxExportAudio);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.cbAudioQuality);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(395, 173);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox2.Size = new System.Drawing.Size(373, 163);
            this.groupBox2.TabIndex = 20;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Audio";
            // 
            // cbAudioEncoder
            // 
            this.cbAudioEncoder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAudioEncoder.Enabled = false;
            this.cbAudioEncoder.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.cbAudioEncoder.FormattingEnabled = true;
            this.cbAudioEncoder.Items.AddRange(new object[] {
            "(do not transcode)",
            "mp3",
            "aac",
            "flac",
            "m4a",
            "opus",
            "vorbis",
            "wav"});
            this.cbAudioEncoder.Location = new System.Drawing.Point(82, 97);
            this.cbAudioEncoder.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cbAudioEncoder.Name = "cbAudioEncoder";
            this.cbAudioEncoder.Size = new System.Drawing.Size(282, 23);
            this.cbAudioEncoder.TabIndex = 20;
            // 
            // chkBoxExportAudio
            // 
            this.chkBoxExportAudio.AutoSize = true;
            this.chkBoxExportAudio.Checked = true;
            this.chkBoxExportAudio.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBoxExportAudio.Location = new System.Drawing.Point(16, 28);
            this.chkBoxExportAudio.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.chkBoxExportAudio.Name = "chkBoxExportAudio";
            this.chkBoxExportAudio.Size = new System.Drawing.Size(95, 19);
            this.chkBoxExportAudio.TabIndex = 19;
            this.chkBoxExportAudio.Text = "Export Audio";
            this.chkBoxExportAudio.UseVisualStyleBackColor = true;
            this.chkBoxExportAudio.CheckedChanged += new System.EventHandler(this.ChkBoxExportAudio_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 98);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 15);
            this.label3.TabIndex = 17;
            this.label3.Text = "Encoder:";
            // 
            // cbAudioQuality
            // 
            this.cbAudioQuality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAudioQuality.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.cbAudioQuality.FormattingEnabled = true;
            this.cbAudioQuality.Items.AddRange(new object[] {
            "High",
            "Low"});
            this.cbAudioQuality.Location = new System.Drawing.Point(82, 63);
            this.cbAudioQuality.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cbAudioQuality.Name = "cbAudioQuality";
            this.cbAudioQuality.Size = new System.Drawing.Size(282, 23);
            this.cbAudioQuality.TabIndex = 16;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 63);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 15);
            this.label4.TabIndex = 15;
            this.label4.Text = "Quality:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.buttonLocationBrowse);
            this.groupBox3.Controls.Add(this.textLocation);
            this.groupBox3.Location = new System.Drawing.Point(9, 347);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox3.Size = new System.Drawing.Size(373, 66);
            this.groupBox3.TabIndex = 22;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Location";
            // 
            // buttonLocationBrowse
            // 
            this.buttonLocationBrowse.Location = new System.Drawing.Point(296, 23);
            this.buttonLocationBrowse.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonLocationBrowse.Name = "buttonLocationBrowse";
            this.buttonLocationBrowse.Size = new System.Drawing.Size(68, 28);
            this.buttonLocationBrowse.TabIndex = 1;
            this.buttonLocationBrowse.Text = "Browse";
            this.buttonLocationBrowse.UseVisualStyleBackColor = true;
            this.buttonLocationBrowse.Click += new System.EventHandler(this.ButtonLocationBrowse_Click);
            // 
            // textLocation
            // 
            this.textLocation.Location = new System.Drawing.Point(11, 27);
            this.textLocation.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textLocation.Name = "textLocation";
            this.textLocation.ReadOnly = true;
            this.textLocation.Size = new System.Drawing.Size(276, 23);
            this.textLocation.TabIndex = 0;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.chkBoxIncludeAds);
            this.groupBox4.Controls.Add(this.chkEmbedSubs);
            this.groupBox4.Controls.Add(this.chkBoxEmbedThumbnail);
            this.groupBox4.Controls.Add(this.chkBoxWriteMetadata);
            this.groupBox4.Location = new System.Drawing.Point(395, 347);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox4.Size = new System.Drawing.Size(369, 93);
            this.groupBox4.TabIndex = 21;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Additional Options";
            // 
            // chkBoxIncludeAds
            // 
            this.chkBoxIncludeAds.AutoSize = true;
            this.chkBoxIncludeAds.Location = new System.Drawing.Point(200, 33);
            this.chkBoxIncludeAds.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.chkBoxIncludeAds.Name = "chkBoxIncludeAds";
            this.chkBoxIncludeAds.Size = new System.Drawing.Size(88, 19);
            this.chkBoxIncludeAds.TabIndex = 16;
            this.chkBoxIncludeAds.Text = "Include Ads";
            this.chkBoxIncludeAds.UseVisualStyleBackColor = true;
            // 
            // chkEmbedSubs
            // 
            this.chkEmbedSubs.AutoSize = true;
            this.chkEmbedSubs.Location = new System.Drawing.Point(200, 56);
            this.chkEmbedSubs.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.chkEmbedSubs.Name = "chkEmbedSubs";
            this.chkEmbedSubs.Size = new System.Drawing.Size(91, 19);
            this.chkEmbedSubs.TabIndex = 13;
            this.chkEmbedSubs.Text = "Embed Subs";
            this.chkEmbedSubs.UseVisualStyleBackColor = true;
            // 
            // chkBoxEmbedThumbnail
            // 
            this.chkBoxEmbedThumbnail.AutoSize = true;
            this.chkBoxEmbedThumbnail.Location = new System.Drawing.Point(13, 56);
            this.chkBoxEmbedThumbnail.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.chkBoxEmbedThumbnail.Name = "chkBoxEmbedThumbnail";
            this.chkBoxEmbedThumbnail.Size = new System.Drawing.Size(123, 19);
            this.chkBoxEmbedThumbnail.TabIndex = 12;
            this.chkBoxEmbedThumbnail.Text = "Embed Thumbnail";
            this.chkBoxEmbedThumbnail.UseVisualStyleBackColor = true;
            // 
            // chkBoxWriteMetadata
            // 
            this.chkBoxWriteMetadata.AutoSize = true;
            this.chkBoxWriteMetadata.Checked = true;
            this.chkBoxWriteMetadata.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBoxWriteMetadata.Location = new System.Drawing.Point(13, 33);
            this.chkBoxWriteMetadata.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.chkBoxWriteMetadata.Name = "chkBoxWriteMetadata";
            this.chkBoxWriteMetadata.Size = new System.Drawing.Size(107, 19);
            this.chkBoxWriteMetadata.TabIndex = 11;
            this.chkBoxWriteMetadata.Text = "Write Metadata";
            this.chkBoxWriteMetadata.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(561, 460);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(100, 28);
            this.buttonCancel.TabIndex = 24;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
            // 
            // buttonDownload
            // 
            this.buttonDownload.Location = new System.Drawing.Point(665, 460);
            this.buttonDownload.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonDownload.Name = "buttonDownload";
            this.buttonDownload.Size = new System.Drawing.Size(100, 28);
            this.buttonDownload.TabIndex = 23;
            this.buttonDownload.Text = "Download";
            this.buttonDownload.UseVisualStyleBackColor = true;
            this.buttonDownload.Click += new System.EventHandler(this.ButtonDownload_Click);
            // 
            // FrameNewMediaBatch
            // 
            this.AcceptButton = this.buttonDownload;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(777, 499);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonDownload);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textUrls);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrameNewMediaBatch";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New Media Batch";
            this.Load += new System.EventHandler(this.FrameNewMediaBatch_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textUrls;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cbVideoEncoder;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbVideoQuality;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox chkBoxExportVideo;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chkBoxExportAudio;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbAudioQuality;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbAudioEncoder;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button buttonLocationBrowse;
        private System.Windows.Forms.TextBox textLocation;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox chkBoxIncludeAds;
        private System.Windows.Forms.CheckBox chkEmbedSubs;
        private System.Windows.Forms.CheckBox chkBoxEmbedThumbnail;
        private System.Windows.Forms.CheckBox chkBoxWriteMetadata;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonDownload;
    }
}