namespace JackTheVideoRipper.views
{
   partial class FrameNotifications
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
            this.listItems = new System.Windows.Forms.ListView();
            this.DatePosted = new System.Windows.Forms.ColumnHeader();
            this.SenderName = new System.Windows.Forms.ColumnHeader();
            this.SenderGuid = new System.Windows.Forms.ColumnHeader();
            this.Message = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // listItems
            // 
            this.listItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.DatePosted,
            this.SenderName,
            this.SenderGuid,
            this.Message});
            this.listItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listItems.FullRowSelect = true;
            this.listItems.Location = new System.Drawing.Point(0, 0);
            this.listItems.Name = "listItems";
            this.listItems.Size = new System.Drawing.Size(700, 338);
            this.listItems.TabIndex = 0;
            this.listItems.UseCompatibleStateImageBehavior = false;
            this.listItems.View = System.Windows.Forms.View.Details;
            // 
            // DatePosted
            // 
            this.DatePosted.Text = "Date Posted";
            this.DatePosted.Width = 125;
            // 
            // SenderName
            // 
            this.SenderName.Text = "Sender Name";
            this.SenderName.Width = 125;
            // 
            // SenderGuid
            // 
            this.SenderGuid.Text = "Sender GUID";
            this.SenderGuid.Width = 125;
            // 
            // Message
            // 
            this.Message.Text = "Message";
            this.Message.Width = 350;
            // 
            // FrameNotifications
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 338);
            this.Controls.Add(this.listItems);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "FrameNotifications";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Notifications";
            this.ResumeLayout(false);

      }

        #endregion

        private ListView listItems;
        private ColumnHeader DatePosted;
        private ColumnHeader SenderName;
        private ColumnHeader SenderGuid;
        private ColumnHeader Message;
    }
}