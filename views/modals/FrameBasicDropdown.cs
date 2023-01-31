using JackTheVideoRipper.extensions;

namespace JackTheVideoRipper.views.modals
{
   public partial class FrameBasicDropdown : Form
   {
      private readonly string _title;
      private readonly object[] _options;
      private readonly string _defaultValue;
      
      public string Value => comboBox.SelectedItem.ToString() ?? string.Empty;

      private const string _DEFAULT_TITLE = "Select Option";
      private const string _DEFAULT_VALUE = "[ SELECT ]";
      
      public FrameBasicDropdown(IEnumerable<string> options, string? title = _DEFAULT_TITLE,
         string? defaultValue = _DEFAULT_VALUE)
      {
         _options = options.Cast<object>().ToArray();
         _title = title ?? _DEFAULT_TITLE;
         _defaultValue = defaultValue ?? _DEFAULT_VALUE;
         
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
         AddDefaultRow();
         comboBox.Items.AddRange(_options);
         buttonConfirm.Select();
      }
      
      private void OnConfirmButtonClick(object? sender, EventArgs args)
      {
         this.Close(DialogResult.OK);
      }

      private void AddDefaultRow()
      {
         comboBox.SelectedItem = null;
         comboBox.SelectedText = _defaultValue;
      }
   }
}
