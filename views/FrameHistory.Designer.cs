namespace JackTheVideoRipper.views
{
    partial class FrameHistory
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
         this.ListView = new System.Windows.Forms.ListView();
         this.Title = new System.Windows.Forms.ColumnHeader();
         this.Url = new System.Windows.Forms.ColumnHeader();
         this.Parameters = new System.Windows.Forms.ColumnHeader();
         this.Filepath = new System.Windows.Forms.ColumnHeader();
         this.MediaType = new System.Windows.Forms.ColumnHeader();
         this.Tag = new System.Windows.Forms.ColumnHeader();
         this.DateStarted = new System.Windows.Forms.ColumnHeader();
         this.DateFinished = new System.Windows.Forms.ColumnHeader();
         this.Duration = new System.Windows.Forms.ColumnHeader();
         this.Filesize = new System.Windows.Forms.ColumnHeader();
         this.WebsiteName = new System.Windows.Forms.ColumnHeader();
         this.Result = new System.Windows.Forms.ColumnHeader();
         this.SuspendLayout();
         // 
         // ListView
         // 
         this.ListView.AllowColumnReorder = true;
         this.ListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Title,
            this.Url,
            this.Parameters,
            this.Filepath,
            this.MediaType,
            this.Tag,
            this.DateStarted,
            this.DateFinished,
            this.Duration,
            this.Filesize,
            this.WebsiteName,
            this.Result});
         this.ListView.Dock = System.Windows.Forms.DockStyle.Fill;
         this.ListView.Location = new System.Drawing.Point(0, 0);
         this.ListView.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
         this.ListView.Name = "ListView";
         this.ListView.Size = new System.Drawing.Size(1362, 709);
         this.ListView.TabIndex = 0;
         this.ListView.UseCompatibleStateImageBehavior = false;
         this.ListView.View = System.Windows.Forms.View.Details;
         // 
         // Title
         // 
         this.Title.Text = "Title";
         this.Title.Width = 120;
         // 
         // Url
         // 
         this.Url.Text = "Url";
         this.Url.Width = 120;
         // 
         // Parameters
         // 
         this.Parameters.Text = "Parameters";
         this.Parameters.Width = 120;
         // 
         // Filepath
         // 
         this.Filepath.Text = "Filepath";
         this.Filepath.Width = 120;
         // 
         // MediaType
         // 
         this.MediaType.Text = "Media Type";
         this.MediaType.Width = 120;
         // 
         // Tag
         // 
         this.Tag.Text = "Tag";
         this.Tag.Width = 120;
         // 
         // DateStarted
         // 
         this.DateStarted.Text = "Date Started";
         this.DateStarted.Width = 120;
         // 
         // DateFinished
         // 
         this.DateFinished.Text = "Date Finished";
         this.DateFinished.Width = 120;
         // 
         // Duration
         // 
         this.Duration.Text = "Duration";
         this.Duration.Width = 80;
         // 
         // Filesize
         // 
         this.Filesize.Text = "Filesize";
         this.Filesize.Width = 80;
         // 
         // WebsiteName
         // 
         this.WebsiteName.Text = "Website";
         this.WebsiteName.Width = 120;
         // 
         // Result
         // 
         this.Result.Text = "Result";
         this.Result.Width = 120;
         // 
         // FrameHistory
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(1362, 709);
         this.Controls.Add(this.ListView);
         this.DoubleBuffered = true;
         this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
         this.Name = "FrameHistory";
         this.Text = "History";
         this.ResumeLayout(false);

        }

        #endregion

        public ListView ListView;
        private ColumnHeader Title;
        private ColumnHeader Url;
        private ColumnHeader Parameters;
        private ColumnHeader Filepath;
        private ColumnHeader MediaType;
        private ColumnHeader Tag;
        private ColumnHeader DateStarted;
        private ColumnHeader DateFinished;
        private ColumnHeader Duration;
        private ColumnHeader Filesize;
        private ColumnHeader WebsiteName;
        private ColumnHeader Result;
    }
}