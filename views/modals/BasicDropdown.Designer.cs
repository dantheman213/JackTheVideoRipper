namespace JackTheVideoRipper.views.modals
{
   partial class BasicDropdown
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
         this.comboBox1 = new System.Windows.Forms.ComboBox();
         this.MainLabel = new System.Windows.Forms.Label();
         this.buttonCancel = new System.Windows.Forms.Button();
         this.buttonDownload = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // comboBox1
         // 
         this.comboBox1.FormattingEnabled = true;
         this.comboBox1.Location = new System.Drawing.Point(12, 48);
         this.comboBox1.Name = "comboBox1";
         this.comboBox1.Size = new System.Drawing.Size(185, 28);
         this.comboBox1.TabIndex = 0;
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
         this.MainLabel.Text = "Please select a value";
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
         // BasicDropdown
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(209, 133);
         this.ControlBox = false;
         this.Controls.Add(this.buttonDownload);
         this.Controls.Add(this.buttonCancel);
         this.Controls.Add(this.MainLabel);
         this.Controls.Add(this.comboBox1);
         this.Name = "BasicDropdown";
         this.ShowIcon = false;
         this.Text = "BasicDropdown";
         this.TopMost = true;
         this.ResumeLayout(false);

      }

      #endregion

      private ComboBox comboBox1;
      private Label MainLabel;
      private Button buttonCancel;
      private Button buttonDownload;
   }
}