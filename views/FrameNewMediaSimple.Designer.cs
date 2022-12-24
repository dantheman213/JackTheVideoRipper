namespace JackTheVideoRipper.views
{
    partial class FrameNewMediaSimple
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
            this.textUrl = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonLocationBrowse = new System.Windows.Forms.Button();
            this.textLocation = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkBoxExportAudio = new System.Windows.Forms.CheckBox();
            this.chkBoxExportVideo = new System.Windows.Forms.CheckBox();
            this.buttonGetCommand = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonDownload = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textUrl
            // 
            this.textUrl.Location = new System.Drawing.Point(67, 18);
            this.textUrl.Margin = new System.Windows.Forms.Padding(2);
            this.textUrl.Name = "textUrl";
            this.textUrl.Size = new System.Drawing.Size(280, 23);
            this.textUrl.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 21);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "URL:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.buttonLocationBrowse);
            this.groupBox2.Controls.Add(this.textLocation);
            this.groupBox2.Location = new System.Drawing.Point(12, 59);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(335, 66);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Location";
            // 
            // buttonLocationBrowse
            // 
            this.buttonLocationBrowse.Location = new System.Drawing.Point(253, 23);
            this.buttonLocationBrowse.Margin = new System.Windows.Forms.Padding(2);
            this.buttonLocationBrowse.Name = "buttonLocationBrowse";
            this.buttonLocationBrowse.Size = new System.Drawing.Size(69, 28);
            this.buttonLocationBrowse.TabIndex = 1;
            this.buttonLocationBrowse.Text = "Browse";
            this.buttonLocationBrowse.UseVisualStyleBackColor = true;
            // 
            // textLocation
            // 
            this.textLocation.Location = new System.Drawing.Point(11, 27);
            this.textLocation.Margin = new System.Windows.Forms.Padding(2);
            this.textLocation.Name = "textLocation";
            this.textLocation.ReadOnly = true;
            this.textLocation.Size = new System.Drawing.Size(238, 23);
            this.textLocation.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkBoxExportAudio);
            this.groupBox1.Controls.Add(this.chkBoxExportVideo);
            this.groupBox1.Location = new System.Drawing.Point(12, 139);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(335, 88);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Additional Options";
            // 
            // chkBoxExportAudio
            // 
            this.chkBoxExportAudio.AutoSize = true;
            this.chkBoxExportAudio.Checked = true;
            this.chkBoxExportAudio.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBoxExportAudio.Location = new System.Drawing.Point(11, 48);
            this.chkBoxExportAudio.Margin = new System.Windows.Forms.Padding(2);
            this.chkBoxExportAudio.Name = "chkBoxExportAudio";
            this.chkBoxExportAudio.Size = new System.Drawing.Size(95, 19);
            this.chkBoxExportAudio.TabIndex = 15;
            this.chkBoxExportAudio.Text = "Export Audio";
            this.chkBoxExportAudio.UseVisualStyleBackColor = true;
            // 
            // chkBoxExportVideo
            // 
            this.chkBoxExportVideo.AutoSize = true;
            this.chkBoxExportVideo.Checked = true;
            this.chkBoxExportVideo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBoxExportVideo.Location = new System.Drawing.Point(11, 26);
            this.chkBoxExportVideo.Margin = new System.Windows.Forms.Padding(2);
            this.chkBoxExportVideo.Name = "chkBoxExportVideo";
            this.chkBoxExportVideo.Size = new System.Drawing.Size(93, 19);
            this.chkBoxExportVideo.TabIndex = 14;
            this.chkBoxExportVideo.Text = "Export Video";
            this.chkBoxExportVideo.UseVisualStyleBackColor = true;
            // 
            // buttonGetCommand
            // 
            this.buttonGetCommand.Location = new System.Drawing.Point(11, 242);
            this.buttonGetCommand.Margin = new System.Windows.Forms.Padding(2);
            this.buttonGetCommand.Name = "buttonGetCommand";
            this.buttonGetCommand.Size = new System.Drawing.Size(104, 29);
            this.buttonGetCommand.TabIndex = 19;
            this.buttonGetCommand.Text = "Get Command";
            this.buttonGetCommand.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(145, 242);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(2);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(99, 29);
            this.buttonCancel.TabIndex = 18;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonDownload
            // 
            this.buttonDownload.Location = new System.Drawing.Point(248, 242);
            this.buttonDownload.Margin = new System.Windows.Forms.Padding(2);
            this.buttonDownload.Name = "buttonDownload";
            this.buttonDownload.Size = new System.Drawing.Size(99, 29);
            this.buttonDownload.TabIndex = 17;
            this.buttonDownload.Text = "Download";
            this.buttonDownload.UseVisualStyleBackColor = true;
            // 
            // FrameNewMediaSimple
            // 
            this.AcceptButton = this.buttonDownload;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(358, 285);
            this.Controls.Add(this.buttonGetCommand);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonDownload);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.textUrl);
            this.Controls.Add(this.label1);
            this.Name = "FrameNewMediaSimple";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New Media (Simple)";
            this.Load += new System.EventHandler(this.FrameNewMediaSimple_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox textUrl;
        private Label label1;
        private GroupBox groupBox2;
        private Button buttonLocationBrowse;
        private TextBox textLocation;
        private GroupBox groupBox1;
        private CheckBox chkBoxExportAudio;
        private CheckBox chkBoxExportVideo;
        private Button buttonGetCommand;
        private Button buttonCancel;
        private Button buttonDownload;
    }
}