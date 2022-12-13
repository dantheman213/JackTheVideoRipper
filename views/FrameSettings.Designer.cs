namespace JackTheVideoRipper
{
    partial class FrameSettings
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
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonLocationBrowse = new System.Windows.Forms.Button();
            this.textLocation = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.enableDeveloperMode = new System.Windows.Forms.CheckBox();
            this.storeHistory = new System.Windows.Forms.CheckBox();
            this.skipMetadata = new System.Windows.Forms.CheckBox();
            this.numMaxConcurrent = new System.Windows.Forms.NumericUpDown();
            this.enableMultithreadedDownloads = new System.Windows.Forms.CheckBox();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxConcurrent)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(186, 261);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(100, 28);
            this.buttonCancel.TabIndex = 15;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(290, 261);
            this.buttonSave.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(100, 28);
            this.buttonSave.TabIndex = 14;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.buttonLocationBrowse);
            this.groupBox2.Controls.Add(this.textLocation);
            this.groupBox2.Location = new System.Drawing.Point(7, 183);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox2.Size = new System.Drawing.Size(382, 66);
            this.groupBox2.TabIndex = 16;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Default Save Location";
            // 
            // buttonLocationBrowse
            // 
            this.buttonLocationBrowse.Location = new System.Drawing.Point(310, 23);
            this.buttonLocationBrowse.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonLocationBrowse.Name = "buttonLocationBrowse";
            this.buttonLocationBrowse.Size = new System.Drawing.Size(68, 28);
            this.buttonLocationBrowse.TabIndex = 1;
            this.buttonLocationBrowse.Text = "Browse";
            this.buttonLocationBrowse.UseVisualStyleBackColor = true;
            // 
            // textLocation
            // 
            this.textLocation.Location = new System.Drawing.Point(11, 27);
            this.textLocation.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textLocation.Name = "textLocation";
            this.textLocation.ReadOnly = true;
            this.textLocation.Size = new System.Drawing.Size(295, 23);
            this.textLocation.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 22);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(158, 15);
            this.label1.TabIndex = 17;
            this.label1.Text = "Max Concurrent Downloads:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.enableMultithreadedDownloads);
            this.groupBox1.Controls.Add(this.enableDeveloperMode);
            this.groupBox1.Controls.Add(this.storeHistory);
            this.groupBox1.Controls.Add(this.skipMetadata);
            this.groupBox1.Controls.Add(this.numMaxConcurrent);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(7, 11);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(382, 156);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "General";
            // 
            // enableDeveloperMode
            // 
            this.enableDeveloperMode.AutoSize = true;
            this.enableDeveloperMode.Location = new System.Drawing.Point(14, 96);
            this.enableDeveloperMode.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.enableDeveloperMode.Name = "enableDeveloperMode";
            this.enableDeveloperMode.Size = new System.Drawing.Size(151, 19);
            this.enableDeveloperMode.TabIndex = 21;
            this.enableDeveloperMode.Text = "Enable Developer Mode";
            this.enableDeveloperMode.UseVisualStyleBackColor = true;
            // 
            // storeHistory
            // 
            this.storeHistory.AutoSize = true;
            this.storeHistory.Location = new System.Drawing.Point(14, 73);
            this.storeHistory.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.storeHistory.Name = "storeHistory";
            this.storeHistory.Size = new System.Drawing.Size(94, 19);
            this.storeHistory.TabIndex = 20;
            this.storeHistory.Text = "Store History";
            this.storeHistory.UseVisualStyleBackColor = true;
            // 
            // skipMetadata
            // 
            this.skipMetadata.AutoSize = true;
            this.skipMetadata.Location = new System.Drawing.Point(14, 50);
            this.skipMetadata.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.skipMetadata.Name = "skipMetadata";
            this.skipMetadata.Size = new System.Drawing.Size(217, 19);
            this.skipMetadata.TabIndex = 19;
            this.skipMetadata.Text = "Skip Metadata (When Downloading)";
            this.skipMetadata.UseVisualStyleBackColor = true;
            // 
            // numMaxConcurrent
            // 
            this.numMaxConcurrent.Location = new System.Drawing.Point(186, 21);
            this.numMaxConcurrent.Margin = new System.Windows.Forms.Padding(2);
            this.numMaxConcurrent.Name = "numMaxConcurrent";
            this.numMaxConcurrent.Size = new System.Drawing.Size(70, 23);
            this.numMaxConcurrent.TabIndex = 18;
            // 
            // enableMultithreadedDownloads
            // 
            this.enableMultithreadedDownloads.AutoSize = true;
            this.enableMultithreadedDownloads.Location = new System.Drawing.Point(14, 119);
            this.enableMultithreadedDownloads.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.enableMultithreadedDownloads.Name = "enableMultithreadedDownloads";
            this.enableMultithreadedDownloads.Size = new System.Drawing.Size(201, 19);
            this.enableMultithreadedDownloads.TabIndex = 22;
            this.enableMultithreadedDownloads.Text = "Enable Multithreaded Downloads";
            this.enableMultithreadedDownloads.UseVisualStyleBackColor = true;
            // 
            // FrameSettings
            // 
            this.AcceptButton = this.buttonSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(400, 304);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrameSettings";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.FrameSettings_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxConcurrent)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonLocationBrowse;
        private System.Windows.Forms.TextBox textLocation;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown numMaxConcurrent;
      private CheckBox skipMetadata;
        private CheckBox storeHistory;
        private CheckBox enableDeveloperMode;
        private CheckBox enableMultithreadedDownloads;
    }
}