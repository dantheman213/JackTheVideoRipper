namespace JackTheVideoRipper.views.modals
{
   partial class BasicTextbox
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
         this.MainLabel = new System.Windows.Forms.Label();
         this.buttonCancel = new System.Windows.Forms.Button();
         this.buttonDownload = new System.Windows.Forms.Button();
         this.textBox1 = new System.Windows.Forms.TextBox();
         this.SuspendLayout();
         // 
         // MainLabel
         // 
         this.MainLabel.BackColor = System.Drawing.SystemColors.ControlLight;
         this.MainLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
         this.MainLabel.Location = new System.Drawing.Point(12, 9);
         this.MainLabel.Name = "MainLabel";
         this.MainLabel.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
         this.MainLabel.Size = new System.Drawing.Size(185, 32);
         this.MainLabel.TabIndex = 14;
         this.MainLabel.Text = "Please enter a value";
         // 
         // buttonCancel
         // 
         this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.buttonCancel.Location = new System.Drawing.Point(11, 83);
         this.buttonCancel.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
         this.buttonCancel.Name = "buttonCancel";
         this.buttonCancel.Size = new System.Drawing.Size(91, 38);
         this.buttonCancel.TabIndex = 15;
         this.buttonCancel.Text = "Cancel";
         this.buttonCancel.UseVisualStyleBackColor = true;
         // 
         // buttonDownload
         // 
         this.buttonDownload.Location = new System.Drawing.Point(106, 83);
         this.buttonDownload.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
         this.buttonDownload.Name = "buttonDownload";
         this.buttonDownload.Size = new System.Drawing.Size(91, 38);
         this.buttonDownload.TabIndex = 16;
         this.buttonDownload.Text = "Confirm";
         this.buttonDownload.UseVisualStyleBackColor = true;
         // 
         // textBox1
         // 
         this.textBox1.Location = new System.Drawing.Point(12, 49);
         this.textBox1.Name = "textBox1";
         this.textBox1.Size = new System.Drawing.Size(185, 27);
         this.textBox1.TabIndex = 17;
         // 
         // BasicTextbox
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(210, 133);
         this.ControlBox = false;
         this.Controls.Add(this.textBox1);
         this.Controls.Add(this.buttonDownload);
         this.Controls.Add(this.buttonCancel);
         this.Controls.Add(this.MainLabel);
         this.Name = "BasicTextbox";
         this.ShowIcon = false;
         this.Text = "BasicTextbox";
         this.TopMost = true;
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion
      private Label MainLabel;
      private Button buttonCancel;
      private Button buttonDownload;
      private TextBox textBox1;
   }
}