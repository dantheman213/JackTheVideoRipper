using JackTheVideoRipper.extensions;

namespace JackTheVideoRipper.views
{
    public partial class FrameHistory : Form
    {
        public FrameHistory()
        {
            InitializeComponent();
        }
        
        public static void PopulateListView(FrameHistory frameHistory)
        {
            var historyRows = History.Data.HistoryItemTable.GetRows();
            frameHistory.ListView.Items.AddRange(historyRows.Select(r => r.ViewItem).ToArray());
        }
    }
}
