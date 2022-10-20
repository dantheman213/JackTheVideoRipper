using System.Diagnostics;

namespace JackTheVideoRipper
{
    internal class ProcessUpdateRow
    {
        public Process Process { get; set; }
        public ListViewItem ViewItem { get; set; }
        public List<string> Results { get; set; } = new() { "" }; // Placeholder result
        public int Cursor { get; set; } // where in message buffer are we
        public bool Started = false;
        public bool Finished = false;

        public bool Completed => Started && Process.HasExited;

        public bool Failed => Process.ExitCode > 0;

        public string Tag;

        private string _parameterString;

        public ProcessUpdateRow(string parameterString)
        {
            _parameterString = parameterString;
            CreateProcess();
        }

        public void CreateProcess()
        {
            Process = YouTubeDl.Run(_parameterString);
        }

        public void Start()
        {
            Process.Start();
            Started = true;
            TrackStandardOut();
            TrackStandardError();
            ViewItem.BackColor = Color.Turquoise;
        }
        
        public bool SubViewHasText(int index, string text)
        {
            return ViewItem.SubItems[index].Text.Contains(text);
        }
        
        public bool SubViewHasText(int index, params string[] texts)
        {
            return texts.Any(text => ViewItem.SubItems[index].Text.Contains(text));
        }
        
        public void SetSubViewItems(string? zeroth = null, string? first = null, 
            string? second = null, string? third = null, string? fourth = null, string? fifth = null, 
            string? sixth = null, string? seventh = null, string? eighth = null)
        {
            if (zeroth is not null)
                ViewItem.SubItems[0].Text = zeroth;
            if (first is not null)
                ViewItem.SubItems[1].Text = first;
            if (second is not null)
                ViewItem.SubItems[2].Text = second;
            if (third is not null)
                ViewItem.SubItems[3].Text = third;
            if (fourth is not null)
                ViewItem.SubItems[4].Text = fourth;
            if (fifth is not null)
                ViewItem.SubItems[5].Text = fifth;
            if (sixth is not null)
                ViewItem.SubItems[6].Text = sixth;
            if (seventh is not null)
                ViewItem.SubItems[7].Text = seventh;
            if (eighth is not null)
                ViewItem.SubItems[8].Text = eighth;
        }

        public void AppendStatusLine()
        {
            string? line = Process.StandardOutput.ReadLine();
            if (line is not null)
                Results.Add(line);
        }

        public void TrackStandardOut()
        {
            Task.Run(() =>
            {
                // spawns a new thread to read standard out data
                while (Process is { HasExited: false })
                {
                    AppendStatusLine();
                }
            });
        }
        
        public void AppendErrorLine()
        {
            string? line = Process.StandardError.ReadLine();
            if (line is not null)
                Results.Add(line);
        }

        public void TrackStandardError()
        {
            Task.Run(() =>
            {
                // spawns a new thread to read error stream data
                while (Process is { HasExited: false })
                {
                    AppendErrorLine();
                }
            });
        }

        private void SetColor(Color color)
        {
            ViewItem.BackColor = color;
        }

        public void Complete()
        {
            string status = Failed ? "Error" : "Complete";
            SetColor(Failed ? Color.LightCoral : Color.LightGreen);
            SetSubViewItems(first:status, fourth:"100%", fifth:"", sixth:"00:00");
            Finished = true;
        }

        public void Stop()
        {
            SetColor(Color.DarkSalmon);
            SetSubViewItems(first:"Stopped", fifth:"", sixth:"00:00");
            Process.Kill();
            Finished = true;
        }

        public void UpdateStatus(string status)
        {
            if (!SubViewHasText(1, status))
                SetSubViewItems(first: status);
        }

        public void DownloadUpdate(string[] parts)
        {
            SetSubViewItems( 
                first: ViewItem.SubItems[1].Text != @"Downloading" ? "Downloading" : null, 
                third: ViewItem.SubItems[3].Text is "" or "-" ? parts[3] : null,
                fourth: parts[1].Trim() != "100%" ? parts[1] : null,
                fifth: ViewItem.SubItems[5].Text = parts[5],
                sixth: parts[7].Trim() != "00:00" ? parts[7] : null);
        }

        public void SetErrorState()
        {
            if (SubViewHasText(1, "Error"))
                return;
            SetSubViewItems(first: "Error", fifth:"", sixth:"00:00");
            Process.Kill();
        }

        public void Retry()
        {
            Started = false;
            Finished = false;
            CreateProcess();
            SetSubViewItems(first: "Waiting", third:"-", fourth:"", fifth:"0%", sixth:"0.0 KB/s");
        }
    }
}
