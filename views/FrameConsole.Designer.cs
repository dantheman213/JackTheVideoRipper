namespace JackTheVideoRipper.views
{
   partial class FrameConsole
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
            this.ConsoleControl = new ConsoleControl.ConsoleControl();
            this.consoleContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.saveToFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.consoleContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // ConsoleControl
            // 
            this.ConsoleControl.AutoSize = true;
            this.ConsoleControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ConsoleControl.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.ConsoleControl.ContextMenuStrip = this.consoleContextMenu;
            this.ConsoleControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ConsoleControl.IsInputEnabled = true;
            this.ConsoleControl.Location = new System.Drawing.Point(0, 0);
            this.ConsoleControl.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ConsoleControl.Name = "ConsoleControl";
            this.ConsoleControl.Padding = new System.Windows.Forms.Padding(9, 8, 9, 8);
            this.ConsoleControl.SendKeyboardCommandsToProcess = false;
            this.ConsoleControl.ShowDiagnostics = false;
            this.ConsoleControl.Size = new System.Drawing.Size(800, 450);
            this.ConsoleControl.TabIndex = 0;
            // 
            // consoleContextMenu
            // 
            this.consoleContextMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.consoleContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToFileToolStripMenuItem});
            this.consoleContextMenu.Name = "Console";
            this.consoleContextMenu.Size = new System.Drawing.Size(135, 26);
            // 
            // saveToFileToolStripMenuItem
            // 
            this.saveToFileToolStripMenuItem.Name = "saveToFileToolStripMenuItem";
            this.saveToFileToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.saveToFileToolStripMenuItem.Text = "Save To File";
            // 
            // FrameConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ConsoleControl);
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.Name = "FrameConsole";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Console";
            this.TransparencyKey = System.Drawing.Color.Transparent;
            this.consoleContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

      }

        #endregion

        public ConsoleControl.ConsoleControl ConsoleControl;
        private ContextMenuStrip consoleContextMenu;
        private ToolStripMenuItem saveToFileToolStripMenuItem;
    }
}