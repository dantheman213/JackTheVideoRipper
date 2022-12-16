using JackTheVideoRipper.extensions;

namespace JackTheVideoRipper.views
{
   public partial class FrameConsole : Form
   {
      #region Data Members

      private readonly string _instanceName;

      #endregion

      #region Attributes

      public bool InItemBounds(MouseEventArgs e) => ConsoleControl.Bounds.Contains(e.Location);

      #endregion

      #region Constructor

      public FrameConsole(string instanceName, FormClosedEventHandler? consoleCloseHandler = null)
      {
         _instanceName = instanceName;
         InitializeComponent();
         SubscribeEvents();

         if (consoleCloseHandler is not null)
            FormClosed += consoleCloseHandler;
      }

      #endregion

      #region Public Methods

      public async Task OpenConsole()
      {
         await Task.Run(ShowDialog);
      }

      #endregion

      #region Private Methods

      private void ShowContextMenu()
      {
         consoleContextMenu.Show(Cursor.Position);
      }

      #endregion

      #region Event Handlers

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
         switch (args.KeyCode)
         {
            // Ctrl + A
            case Keys.A when args is { Control: true }:
               ConsoleControl.InternalRichTextBox.SelectAll();
               args.Handled = true;
               return;
            case Keys.Delete:
               ConsoleControl.ClearOutput();
               args.Handled = true;
               return;
            case Keys.Oemtilde:
               Close();
               args.Handled = true;
               return;
         }
      }

      #endregion
   }
}
