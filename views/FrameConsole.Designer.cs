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
         this.ConsoleControl.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
         this.ConsoleControl.Name = "ConsoleControl";
         this.ConsoleControl.Padding = new System.Windows.Forms.Padding(10);
         this.ConsoleControl.SendKeyboardCommandsToProcess = false;
         this.ConsoleControl.ShowDiagnostics = false;
         this.ConsoleControl.Size = new System.Drawing.Size(914, 600);
         this.ConsoleControl.TabIndex = 0;
         // 
         // consoleContextMenu
         // 
         this.consoleContextMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
         this.consoleContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToFileToolStripMenuItem});
         this.consoleContextMenu.Name = "Console";
         this.consoleContextMenu.Size = new System.Drawing.Size(157, 28);
         // 
         // saveToFileToolStripMenuItem
         // 
         this.saveToFileToolStripMenuItem.Name = "saveToFileToolStripMenuItem";
         this.saveToFileToolStripMenuItem.Size = new System.Drawing.Size(156, 24);
         this.saveToFileToolStripMenuItem.Text = "Save To File";
         // 
         // FrameConsole
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
         this.ClientSize = new System.Drawing.Size(914, 600);
         this.Controls.Add(this.ConsoleControl);
         this.DoubleBuffered = true;
         this.KeyPreview = true;
         this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
         this.Name = "FrameConsole";
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Console";
         this.TopMost = true;
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