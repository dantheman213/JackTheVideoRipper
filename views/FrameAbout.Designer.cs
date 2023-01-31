namespace JackTheVideoRipper
{
    partial class FrameAbout
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrameAbout));
         this.projectTitle = new System.Windows.Forms.Label();
         this.linkLabel = new System.Windows.Forms.LinkLabel();
         this.labelVersion = new System.Windows.Forms.Label();
         this.textServices = new System.Windows.Forms.TextBox();
         this.textDependencies = new System.Windows.Forms.TextBox();
         this.labelYouTubeDL = new System.Windows.Forms.Label();
         this.groupBox1 = new System.Windows.Forms.GroupBox();
         this.groupBox2 = new System.Windows.Forms.GroupBox();
         this.groupBox3 = new System.Windows.Forms.GroupBox();
         this.textContributors = new System.Windows.Forms.TextBox();
         this.groupBox4 = new System.Windows.Forms.GroupBox();
         this.textLicensing = new System.Windows.Forms.TextBox();
         this.lastUpdated = new System.Windows.Forms.Label();
         this.groupBox1.SuspendLayout();
         this.groupBox2.SuspendLayout();
         this.groupBox3.SuspendLayout();
         this.groupBox4.SuspendLayout();
         this.SuspendLayout();
         // 
         // projectTitle
         // 
         this.projectTitle.AutoSize = true;
         this.projectTitle.BackColor = System.Drawing.Color.Transparent;
         this.projectTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
         this.projectTitle.Location = new System.Drawing.Point(9, 7);
         this.projectTitle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
         this.projectTitle.Name = "projectTitle";
         this.projectTitle.Size = new System.Drawing.Size(108, 36);
         this.projectTitle.TabIndex = 0;
         this.projectTitle.Text = "Project";
         // 
         // linkLabel
         // 
         this.linkLabel.AutoSize = true;
         this.linkLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
         this.linkLabel.Location = new System.Drawing.Point(495, 13);
         this.linkLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
         this.linkLabel.Name = "linkLabel";
         this.linkLabel.Size = new System.Drawing.Size(80, 17);
         this.linkLabel.TabIndex = 1;
         this.linkLabel.TabStop = true;
         this.linkLabel.Text = "Visit Github";
         // 
         // labelVersion
         // 
         this.labelVersion.AutoSize = true;
         this.labelVersion.BackColor = System.Drawing.Color.Transparent;
         this.labelVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
         this.labelVersion.Location = new System.Drawing.Point(309, 7);
         this.labelVersion.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
         this.labelVersion.Name = "labelVersion";
         this.labelVersion.Size = new System.Drawing.Size(51, 20);
         this.labelVersion.TabIndex = 2;
         this.labelVersion.Text = "v0.0.0";
         // 
         // textServices
         // 
         this.textServices.Dock = System.Windows.Forms.DockStyle.Fill;
         this.textServices.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
         this.textServices.Location = new System.Drawing.Point(3, 19);
         this.textServices.Margin = new System.Windows.Forms.Padding(2);
         this.textServices.Multiline = true;
         this.textServices.Name = "textServices";
         this.textServices.ReadOnly = true;
         this.textServices.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
         this.textServices.Size = new System.Drawing.Size(556, 146);
         this.textServices.TabIndex = 3;
         // 
         // textDependencies
         // 
         this.textDependencies.Dock = System.Windows.Forms.DockStyle.Fill;
         this.textDependencies.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
         this.textDependencies.Location = new System.Drawing.Point(3, 19);
         this.textDependencies.Margin = new System.Windows.Forms.Padding(2);
         this.textDependencies.Multiline = true;
         this.textDependencies.Name = "textDependencies";
         this.textDependencies.ReadOnly = true;
         this.textDependencies.Size = new System.Drawing.Size(556, 102);
         this.textDependencies.TabIndex = 5;
         this.textDependencies.Text = "Microsoft Visual C++ 2010 Redistributables (x86)\r\nYT-DLP (& YouTubeDL)\r\nFFMPEG / " +
    "FFProbe\r\nAtomicParsley\r\nAria2C\r\nExifTool";
         // 
         // labelYouTubeDL
         // 
         this.labelYouTubeDL.AutoSize = true;
         this.labelYouTubeDL.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
         this.labelYouTubeDL.ForeColor = System.Drawing.Color.Red;
         this.labelYouTubeDL.Location = new System.Drawing.Point(13, 53);
         this.labelYouTubeDL.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
         this.labelYouTubeDL.Name = "labelYouTubeDL";
         this.labelYouTubeDL.Size = new System.Drawing.Size(79, 17);
         this.labelYouTubeDL.TabIndex = 7;
         this.labelYouTubeDL.Text = "yt-dlp 0.0.0";
         // 
         // groupBox1
         // 
         this.groupBox1.Controls.Add(this.textServices);
         this.groupBox1.Location = new System.Drawing.Point(9, 207);
         this.groupBox1.Name = "groupBox1";
         this.groupBox1.Size = new System.Drawing.Size(562, 168);
         this.groupBox1.TabIndex = 8;
         this.groupBox1.TabStop = false;
         this.groupBox1.Text = "Supported Services:";
         // 
         // groupBox2
         // 
         this.groupBox2.Controls.Add(this.textDependencies);
         this.groupBox2.Location = new System.Drawing.Point(9, 381);
         this.groupBox2.Name = "groupBox2";
         this.groupBox2.Size = new System.Drawing.Size(562, 124);
         this.groupBox2.TabIndex = 9;
         this.groupBox2.TabStop = false;
         this.groupBox2.Text = "Dependencies:";
         // 
         // groupBox3
         // 
         this.groupBox3.Controls.Add(this.textContributors);
         this.groupBox3.Location = new System.Drawing.Point(9, 86);
         this.groupBox3.Name = "groupBox3";
         this.groupBox3.Size = new System.Drawing.Size(562, 115);
         this.groupBox3.TabIndex = 9;
         this.groupBox3.TabStop = false;
         this.groupBox3.Text = "Contributors:";
         // 
         // textContributors
         // 
         this.textContributors.Dock = System.Windows.Forms.DockStyle.Fill;
         this.textContributors.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
         this.textContributors.Location = new System.Drawing.Point(3, 19);
         this.textContributors.Margin = new System.Windows.Forms.Padding(0);
         this.textContributors.Multiline = true;
         this.textContributors.Name = "textContributors";
         this.textContributors.ReadOnly = true;
         this.textContributors.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
         this.textContributors.Size = new System.Drawing.Size(556, 93);
         this.textContributors.TabIndex = 3;
         this.textContributors.Text = "Tony Froman (@fromanan)";
         // 
         // groupBox4
         // 
         this.groupBox4.Controls.Add(this.textLicensing);
         this.groupBox4.Location = new System.Drawing.Point(9, 511);
         this.groupBox4.Name = "groupBox4";
         this.groupBox4.Size = new System.Drawing.Size(562, 110);
         this.groupBox4.TabIndex = 10;
         this.groupBox4.TabStop = false;
         this.groupBox4.Text = "Licensing:";
         // 
         // textLicensing
         // 
         this.textLicensing.Dock = System.Windows.Forms.DockStyle.Fill;
         this.textLicensing.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
         this.textLicensing.Location = new System.Drawing.Point(3, 19);
         this.textLicensing.Margin = new System.Windows.Forms.Padding(2);
         this.textLicensing.Multiline = true;
         this.textLicensing.Name = "textLicensing";
         this.textLicensing.ReadOnly = true;
         this.textLicensing.Size = new System.Drawing.Size(556, 88);
         this.textLicensing.TabIndex = 5;
         this.textLicensing.Text = resources.GetString("textLicensing.Text");
         // 
         // lastUpdated
         // 
         this.lastUpdated.AutoSize = true;
         this.lastUpdated.BackColor = System.Drawing.Color.Transparent;
         this.lastUpdated.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
         this.lastUpdated.Location = new System.Drawing.Point(309, 27);
         this.lastUpdated.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
         this.lastUpdated.Name = "lastUpdated";
         this.lastUpdated.Size = new System.Drawing.Size(91, 16);
         this.lastUpdated.TabIndex = 11;
         this.lastUpdated.Text = "Last Updated:";
         // 
         // FrameAbout
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(590, 630);
         this.Controls.Add(this.lastUpdated);
         this.Controls.Add(this.groupBox4);
         this.Controls.Add(this.groupBox3);
         this.Controls.Add(this.groupBox2);
         this.Controls.Add(this.groupBox1);
         this.Controls.Add(this.labelYouTubeDL);
         this.Controls.Add(this.linkLabel);
         this.Controls.Add(this.projectTitle);
         this.Controls.Add(this.labelVersion);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
         this.Margin = new System.Windows.Forms.Padding(2);
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "FrameAbout";
         this.ShowIcon = false;
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "About";
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

        private System.Windows.Forms.Label projectTitle;
        private System.Windows.Forms.LinkLabel linkLabel;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.TextBox textServices;
        private System.Windows.Forms.TextBox textDependencies;
        private System.Windows.Forms.Label labelYouTubeDL;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private GroupBox groupBox3;
        private TextBox textContributors;
        private GroupBox groupBox4;
        private TextBox textLicensing;
        private Label lastUpdated;
    }
}