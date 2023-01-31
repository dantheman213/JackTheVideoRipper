using JackTheVideoRipper.extensions;

namespace JackTheVideoRipper.views.modals
{
   public partial class FrameBasicTextbox : Form
   {
      private readonly string _title;
      private readonly string _defaultValue;
      
      public string Value => textBox.Text;
      
      private const string _DEFAULT_TITLE = "Enter Text";
      
      public FrameBasicTextbox(string? title = _DEFAULT_TITLE, string? defaultValue = null)
      {
         _title = title ?? _DEFAULT_TITLE;
         _defaultValue = defaultValue ?? string.Empty;
         
         InitializeComponent();
         
         SubscribeEvents();
      }
      
      private void SubscribeEvents()
      {
         Load += OnFormLoad;
         buttonConfirm.Click += OnConfirmButtonClick;
      }
      
      private void OnFormLoad(object? sender, EventArgs args)
      {
         Text = _title;
         textBox.Text = _defaultValue;
         buttonConfirm.Select();
      }

      private void OnConfirmButtonClick(object? sender, EventArgs args)
      {
         this.Close(DialogResult.OK);
      }
   }
}
