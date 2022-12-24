using JackTheVideoRipper.extensions;

namespace JackTheVideoRipper.views
{
   public partial class FrameConsole : Form
   {
      #region Data Members

      private readonly string _instanceName;
      
      private static readonly Font _DefaultFont = new("Lucinda Console", 12);
      
      public bool Frozen { get; private set; }

      public event Action FreezeConsoleEvent = delegate {  };
      
      public event Action UnfreezeConsoleEvent = delegate {  };

      #endregion

      #region Attributes

      public bool InItemBounds(MouseEventArgs e) => ConsoleControl.Bounds.Contains(e.Location);

      private RichTextBox TextBox => ConsoleControl.InternalRichTextBox;

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
         await Core.RunTaskInMainThread(Show);
         await MoveToTop();
      }

      #endregion

      #region Private Methods
      
      public async Task MoveToTop()
      {
         await Core.RunTaskInMainThread(Activate);
      }

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
         TextBox.VScroll += UpdateConsoleFrozen;
         TextBox.MouseDown += OnMouseDown;
         TextBox.SelectionChanged += UpdateConsoleFrozen;
         saveToFileToolStripMenuItem.Click += OnSaveToFile;
      }

      private void FrameConsole_Load(object? sender, EventArgs e)
      {
         Text = _instanceName.HasValue() ? $"Console | {_instanceName}" : "Console";
         AllowTransparency = false;
         TextBox.Font = _DefaultFont;
      }

      private void OnSaveToFile(object? sender, EventArgs e)
      {
         if (FileSystem.SaveFileUsingDialog() is not { } filename || filename.IsNullOrEmpty())
            return;
         
         FileSystem.SaveToFile(filename, ConsoleControl.Text);
      }

      private void OnMouseDown(object? sender, MouseEventArgs args)
      {
         if (args.IsRightClick() && InItemBounds(args))
            ShowContextMenu();
      }

      private async void UpdateConsoleFrozen(object? sender, EventArgs args)
      {
         if (TextBox.IsAtMaxScroll() && !TextBox.HasSelected())
         {
            if (Frozen)
               await Tasks.StartAfter(UnfreezeConsole);
         }
         else if (!Frozen)
         {
            FreezeConsole();
         }
      }

      private void FreezeConsole()
      {
         Frozen = true;
         FreezeConsoleEvent();
      }

      private void UnfreezeConsole()
      {
         Frozen = false;
         UnfreezeConsoleEvent();
      }

      private void OnKeyPress(object? sender, KeyEventArgs args)
      {
         switch (args.KeyCode)
         {
            // Ctrl + A
            case Keys.A when args is { Control: true }:
               TextBox.SelectAll();
               args.Handled = true;
               return;
            // Ctrl + D
            case Keys.D when args is { Control: true }:
               TextBox.DeselectAll();
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
