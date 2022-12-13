using JackTheVideoRipper.extensions;

namespace JackTheVideoRipper.views
{
   public partial class FrameConsole : Form
   {
      public bool InItemBounds(MouseEventArgs e) => ConsoleControl.Bounds.Contains(e.Location);

      private readonly string _instanceName;
      
      public FrameConsole(string instanceName, FormClosedEventHandler? consoleCloseHandler = null)
      {
         _instanceName = instanceName;
         InitializeComponent();
         SubscribeEvents();

         if (consoleCloseHandler is not null)
            FormClosed += consoleCloseHandler;
      }

      public void OpenConsole()
      {
         Task.Run(ShowDialog);
      }

      private void SubscribeEvents()
      {
         Load += FrameConsole_Load;
         
         KeyDown += OnKeyPress;

         //ConsoleControl.MouseDown += OnMouseDown;
         ConsoleControl.InternalRichTextBox.MouseDown += OnMouseDown;

         saveToFileToolStripMenuItem.Click += (_, _) =>
         {
            string? filename = FileSystem.SaveFileUsingDialog();
            if (filename.IsNullOrEmpty())
               return;
            FileSystem.SaveToFile(filename!, ConsoleControl.Text);
         };
      }

      private void FrameConsole_Load(object? sender, EventArgs e)
      {
         Text = _instanceName.HasValue() ? $"Console | {_instanceName}" : "Console";

         AllowTransparency = false;
         ConsoleControl.InternalRichTextBox.Font = new Font("Lucinda Console", 12);
      }

      private void OnMouseDown(object? sender, MouseEventArgs args)
      {
         if (args.IsRightClick() && InItemBounds(args))
            ShowContextMenu();
      }

      private void OnKeyPress(object? sender, KeyEventArgs args)
      {
         // Ctrl + A
         if (args is {KeyCode: Keys.A, Control: true})
         {
            ConsoleControl.InternalRichTextBox.SelectAll();
            args.Handled = true;
            return;
         }

         if (args is {KeyCode: Keys.Delete})
         {
            ConsoleControl.ClearOutput();
            args.Handled = true;
            return;
         }

         if (args.KeyCode == Keys.Oemtilde)
         {
            Close();
            args.Handled = true;
            return;
         }
      }

      private void ShowContextMenu()
      {
         consoleContextMenu.Show(Cursor.Position);
      }
   }
}
