namespace JackTheVideoRipper.views
{
    partial class FrameErrorHandler
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
            this.tbSource = new System.Windows.Forms.TextBox();
            this.rtbStackTrace = new System.Windows.Forms.RichTextBox();
            this.SourceLabel = new System.Windows.Forms.Label();
            this.CallerLabel = new System.Windows.Forms.Label();
            this.tbCaller = new System.Windows.Forms.TextBox();
            this.LabelType = new System.Windows.Forms.Label();
            this.tbType = new System.Windows.Forms.TextBox();
            this.StackTraceLabel = new System.Windows.Forms.Label();
            this.gbDetails = new System.Windows.Forms.GroupBox();
            this.tbMessage = new System.Windows.Forms.TextBox();
            this.gbMessage = new System.Windows.Forms.GroupBox();
            this.MainLabel = new System.Windows.Forms.Label();
            this.bQuit = new System.Windows.Forms.Button();
            this.bContinue = new System.Windows.Forms.Button();
            this.bSaveDetails = new System.Windows.Forms.Button();
            this.gbDetails.SuspendLayout();
            this.gbMessage.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbSource
            // 
            this.tbSource.Location = new System.Drawing.Point(73, 30);
            this.tbSource.Name = "tbSource";
            this.tbSource.Size = new System.Drawing.Size(193, 25);
            this.tbSource.TabIndex = 0;
            // 
            // rtbStackTrace
            // 
            this.rtbStackTrace.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.rtbStackTrace.Location = new System.Drawing.Point(12, 231);
            this.rtbStackTrace.Name = "rtbStackTrace";
            this.rtbStackTrace.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtbStackTrace.Size = new System.Drawing.Size(569, 331);
            this.rtbStackTrace.TabIndex = 2;
            this.rtbStackTrace.Text = "";
            // 
            // SourceLabel
            // 
            this.SourceLabel.AutoSize = true;
            this.SourceLabel.Location = new System.Drawing.Point(12, 33);
            this.SourceLabel.Name = "SourceLabel";
            this.SourceLabel.Size = new System.Drawing.Size(49, 17);
            this.SourceLabel.TabIndex = 3;
            this.SourceLabel.Text = "Source";
            this.SourceLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CallerLabel
            // 
            this.CallerLabel.AutoSize = true;
            this.CallerLabel.Location = new System.Drawing.Point(12, 68);
            this.CallerLabel.Name = "CallerLabel";
            this.CallerLabel.Size = new System.Drawing.Size(41, 17);
            this.CallerLabel.TabIndex = 5;
            this.CallerLabel.Text = "Caller";
            this.CallerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tbCaller
            // 
            this.tbCaller.Location = new System.Drawing.Point(73, 65);
            this.tbCaller.Name = "tbCaller";
            this.tbCaller.Size = new System.Drawing.Size(193, 25);
            this.tbCaller.TabIndex = 4;
            // 
            // LabelType
            // 
            this.LabelType.AutoSize = true;
            this.LabelType.Location = new System.Drawing.Point(12, 104);
            this.LabelType.Name = "LabelType";
            this.LabelType.Size = new System.Drawing.Size(36, 17);
            this.LabelType.TabIndex = 7;
            this.LabelType.Text = "Type";
            this.LabelType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tbType
            // 
            this.tbType.Location = new System.Drawing.Point(73, 101);
            this.tbType.Name = "tbType";
            this.tbType.Size = new System.Drawing.Size(193, 25);
            this.tbType.TabIndex = 6;
            // 
            // StackTraceLabel
            // 
            this.StackTraceLabel.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.StackTraceLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.StackTraceLabel.ForeColor = System.Drawing.SystemColors.Control;
            this.StackTraceLabel.Location = new System.Drawing.Point(12, 201);
            this.StackTraceLabel.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.StackTraceLabel.Name = "StackTraceLabel";
            this.StackTraceLabel.Padding = new System.Windows.Forms.Padding(5, 5, 40, 5);
            this.StackTraceLabel.Size = new System.Drawing.Size(569, 27);
            this.StackTraceLabel.TabIndex = 9;
            this.StackTraceLabel.Text = "StackTrace";
            this.StackTraceLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // gbDetails
            // 
            this.gbDetails.BackColor = System.Drawing.SystemColors.ControlLight;
            this.gbDetails.Controls.Add(this.SourceLabel);
            this.gbDetails.Controls.Add(this.tbSource);
            this.gbDetails.Controls.Add(this.tbCaller);
            this.gbDetails.Controls.Add(this.LabelType);
            this.gbDetails.Controls.Add(this.CallerLabel);
            this.gbDetails.Controls.Add(this.tbType);
            this.gbDetails.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.gbDetails.Location = new System.Drawing.Point(297, 45);
            this.gbDetails.Name = "gbDetails";
            this.gbDetails.Size = new System.Drawing.Size(284, 143);
            this.gbDetails.TabIndex = 10;
            this.gbDetails.TabStop = false;
            this.gbDetails.Text = "Details";
            // 
            // tbMessage
            // 
            this.tbMessage.Location = new System.Drawing.Point(17, 30);
            this.tbMessage.Multiline = true;
            this.tbMessage.Name = "tbMessage";
            this.tbMessage.Size = new System.Drawing.Size(244, 96);
            this.tbMessage.TabIndex = 11;
            // 
            // gbMessage
            // 
            this.gbMessage.BackColor = System.Drawing.SystemColors.ControlLight;
            this.gbMessage.Controls.Add(this.tbMessage);
            this.gbMessage.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.gbMessage.Location = new System.Drawing.Point(12, 45);
            this.gbMessage.Name = "gbMessage";
            this.gbMessage.Size = new System.Drawing.Size(279, 143);
            this.gbMessage.TabIndex = 12;
            this.gbMessage.TabStop = false;
            this.gbMessage.Text = "Message";
            // 
            // MainLabel
            // 
            this.MainLabel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.MainLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.MainLabel.Location = new System.Drawing.Point(12, 9);
            this.MainLabel.Name = "MainLabel";
            this.MainLabel.Padding = new System.Windows.Forms.Padding(3);
            this.MainLabel.Size = new System.Drawing.Size(569, 24);
            this.MainLabel.TabIndex = 13;
            this.MainLabel.Text = "The thread encountered an unhandled exception while executing.";
            // 
            // bQuit
            // 
            this.bQuit.Location = new System.Drawing.Point(466, 578);
            this.bQuit.Name = "bQuit";
            this.bQuit.Size = new System.Drawing.Size(115, 28);
            this.bQuit.TabIndex = 14;
            this.bQuit.Text = "&Quit";
            this.bQuit.UseVisualStyleBackColor = true;
            // 
            // bContinue
            // 
            this.bContinue.Location = new System.Drawing.Point(345, 578);
            this.bContinue.Name = "bContinue";
            this.bContinue.Size = new System.Drawing.Size(115, 28);
            this.bContinue.TabIndex = 15;
            this.bContinue.Text = "&Continue";
            this.bContinue.UseVisualStyleBackColor = true;
            // 
            // bSaveDetails
            // 
            this.bSaveDetails.Location = new System.Drawing.Point(12, 578);
            this.bSaveDetails.Name = "bSaveDetails";
            this.bSaveDetails.Size = new System.Drawing.Size(115, 28);
            this.bSaveDetails.TabIndex = 16;
            this.bSaveDetails.Text = "&Save Details";
            this.bSaveDetails.UseVisualStyleBackColor = true;
            // 
            // FrameErrorHandler
            // 
            this.AcceptButton = this.bContinue;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.bQuit;
            this.ClientSize = new System.Drawing.Size(593, 621);
            this.Controls.Add(this.bSaveDetails);
            this.Controls.Add(this.bContinue);
            this.Controls.Add(this.bQuit);
            this.Controls.Add(this.MainLabel);
            this.Controls.Add(this.gbMessage);
            this.Controls.Add(this.gbDetails);
            this.Controls.Add(this.StackTraceLabel);
            this.Controls.Add(this.rtbStackTrace);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "FrameErrorHandler";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FrameErrorHandler";
            this.TopMost = true;
            this.gbDetails.ResumeLayout(false);
            this.gbDetails.PerformLayout();
            this.gbMessage.ResumeLayout(false);
            this.gbMessage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private TextBox tbSource;
        private RichTextBox rtbStackTrace;
        private Label SourceLabel;
        private Label CallerLabel;
        private TextBox tbCaller;
        private Label LabelType;
        private TextBox tbType;
        private Label StackTraceLabel;
        private GroupBox gbDetails;
        private TextBox tbMessage;
        private GroupBox gbMessage;
        private Label MainLabel;
        private Button bQuit;
        private Button bContinue;
        private Button bSaveDetails;
    }
}