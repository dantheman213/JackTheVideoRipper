using JackTheVideoRipper.extensions;
using JackTheVideoRipper.interfaces;
using JackTheVideoRipper.views;

namespace JackTheVideoRipper.models;

public abstract class ProcessUpdateRow : ProcessRunner, IProcessUpdateRow, IDynamicRow
{
    #region Data Members

    public ListViewItem ViewItem { get; }

    public string Tag { get; } = Common.CreateTag();
    
    private FrameConsole? _frameConsole;
    
    private bool _consoleOpened;

    #endregion

    #region View Item Accessors

    public string Title
    {
        get => ViewItem.SubItems[0].Text;
        set => ViewItem.SubItems[0].Text = value;
    }
        
    public string Status
    {
        get => ViewItem.SubItems[1].Text;
        set => ViewItem.SubItems[1].Text = value;
    }
        
    public string MediaType
    {
        get => ViewItem.SubItems[2].Text;
        set => ViewItem.SubItems[2].Text = value;
    }
        
    public string FileSize
    {
        get => ViewItem.SubItems[3].Text;
        set => ViewItem.SubItems[3].Text = value;
    }
        
    public string Progress
    {
        get => ViewItem.SubItems[4].Text;
        set => ViewItem.SubItems[4].Text = value;
    }
        
    public string DownloadSpeed
    {
        get => ViewItem.SubItems[5].Text;
        set => ViewItem.SubItems[5].Text = value;
    }
        
    public string Eta
    {
        get => ViewItem.SubItems[6].Text;
        set => ViewItem.SubItems[6].Text = value;
    }
        
    public string Url
    {
        get => ViewItem.SubItems[7].Text;
        set => ViewItem.SubItems[7].Text = value;
    }
        
    public string Path
    {
        get => ViewItem.SubItems[8].Text;
        set => ViewItem.SubItems[8].Text = value;
    }
        
    private Color Color
    {
        get => ViewItem.BackColor;
        set => ViewItem.BackColor = value;
    }
        
    #endregion

    #region Constructor

    protected ProcessUpdateRow(IMediaItem mediaItem, Action<IProcessRunner> completionCallback) :
        base(mediaItem.MediaParameters.ToString(), completionCallback)
    {
        ViewItem = CreateListViewItem(mediaItem);
        History.Data.AddHistoryItem(Tag, mediaItem);
        Buffer.AddLog("Process initialized", ProcessLogType.Info);
    }
    
    #endregion

    #region Public Methods

    private string GetFileName(string filepath) => System.IO.Path.GetFileName(filepath);

    public void OpenInConsole()
    {
        if (_consoleOpened && (_frameConsole?.Visible ?? false))
        {
            _frameConsole.Activate();
            return;
        }
        
        string processName = GetFileName(FileName);
        string filename = GetFileName(Path);

        string instanceName = processName.HasValue() && filename.HasValue() ?
            $"{processName} | {filename}" : string.Empty;
        _frameConsole = Output.OpenConsoleWindow(instanceName, OnCloseConsole);
        Buffer.WriteLogsToConsole(_frameConsole.ConsoleControl);
        Buffer.LogAdded += OnLogAdded;

        _consoleOpened = true;
    }

    private void OnLogAdded(ProcessLogNode logNode)
    {
        Core.RunInMainThread(() => { _frameConsole?.ConsoleControl.WriteLog(logNode); });
    }
    
    public void OnCloseConsole(object? sender, FormClosedEventArgs args)
    {
        // Disconnect output handler?
        _frameConsole = null;
        _consoleOpened = false;
    }

    #endregion
    
    #region Overrides

    public override async Task<bool> Update()
    {
        if (!await base.Update())
            return false;

        if (GetStatus() is not { } status || status.IsNullOrEmpty())
            return false;
        
        Core.RunInMainThread(() =>
        {
            UpdateStatus(status);
        });

        return true;
    }

    public override async Task<bool> Start()
    {
        if (!await base.Start())
            return false;

        History.Data.MarkStarted(Tag);
        
        Buffer.AddLog("Process started execution", ProcessLogType.Info);

        return true;
    }

    protected override void Complete()
    {
        base.Complete();
        
        History.Data.MarkCompleted(Tag, result:ProcessStatus);
        Buffer.AddLog("Process completed", ProcessLogType.Info);
    }

    protected override bool SetProcessStatus(ProcessStatus processStatus)
    {
        if (!base.SetProcessStatus(processStatus))
            return false;

        Core.RunInMainThread(() =>
        {
            UpdateRowColors(processStatus);
            SetDefaultMessages(processStatus);
        });
        
        return true;
    }

    #endregion

    #region Protected Methods

    protected void UpdateStatus(string statusMessage)
    {
        Status = statusMessage;

        if (Buffer.TokenizedProcessLine is not { Length: >= 8 } tokens)
            return;
            
        // download messages stream fast, bump the cursor up to one of the latest messages, if it exists...
        // only start skipping cursor ahead once download messages have started otherwise important info could be skipped
        Buffer.SkipToEnd();

        SetProgressText(tokens);
    }

    #endregion

    #region Abstract Methods

    protected abstract void SetProgressText(IReadOnlyList<string> tokens);

    protected abstract string? GetStatus();

    #endregion

    #region Private Methods

    private void UpdateRowColors(ProcessStatus processStatus)
    {
        Color = processStatus switch
        {
            ProcessStatus.Queued    => Color.Bisque,
            ProcessStatus.Running   => Color.Turquoise,
            ProcessStatus.Cancelled => Color.LightYellow,
            ProcessStatus.Completed => Color.LightGreen,
            ProcessStatus.Error     => Color.LightCoral,
            ProcessStatus.Stopped   => Color.DarkSalmon,
            ProcessStatus.Created   => Color.LightGray,
            ProcessStatus.Paused    => Color.DarkGray,
            _                       => Color.White
        };
    }

    private void SetValues(string? status = null, string? size = null, string? progress = null,
        string? downloadSpeed = null, string? eta = null)
    {
        if (status.HasValue())
            Status = status!;
        if (size.HasValue())
            FileSize = size!;
        if (progress.HasValue())
            Progress = progress!;
        if (downloadSpeed.HasValue())
            DownloadSpeed = downloadSpeed!;
        if (eta.HasValue())
            Eta = eta!;
    }

    private void SetDefaultMessages(ProcessStatus processStatus)
    {
        switch (processStatus)
        {
            default:
            case ProcessStatus.Succeeded:
                break;
            case ProcessStatus.Running:
                SetValues(Statuses.STARTING);
                break;
            case ProcessStatus.Queued:
                SetValues(Statuses.QUEUED);
                break;
            case ProcessStatus.Created:
                SetValues(Statuses.WAITING,
                    size: Text.DEFAULT_SIZE, 
                    progress: Text.DEFAULT_PROGRESS,
                    downloadSpeed: Text.DEFAULT_SPEED, 
                    eta: Text.DEFAULT_TIME);
                break;
            case ProcessStatus.Completed:
                SetValues(Statuses.COMPLETE,
                    progress: Text.PROGRESS_COMPLETE,
                    downloadSpeed: Text.DEFAULT_SPEED,
                    eta: Text.DEFAULT_TIME);
                break;
            case ProcessStatus.Error:
                SetValues(Statuses.ERROR,
                    size: Text.DEFAULT_SIZE,
                    downloadSpeed: Text.DEFAULT_SPEED,
                    eta: Text.DEFAULT_TIME);
                break;
            case ProcessStatus.Stopped:
                SetValues(Statuses.STOPPED,
                    downloadSpeed: Text.DEFAULT_SPEED,
                    eta: Text.DEFAULT_TIME);
                break;
            case ProcessStatus.Cancelled:
                SetValues(Statuses.CANCELLED,
                    size: Text.DEFAULT_SIZE,
                    progress: Text.DEFAULT_PROGRESS,
                    downloadSpeed: Text.DEFAULT_SPEED,
                    eta: Text.DEFAULT_TIME);
                break;
            case ProcessStatus.Paused:
                SetValues(Statuses.PAUSED,
                    downloadSpeed: Text.DEFAULT_SPEED,
                    eta: Text.DEFAULT_TIME);
                break;
        }
    }
    
    private ListViewItem CreateListViewItem(IMediaItem mediaItem)
    {
        return new ListViewItem(DefaultRow(mediaItem))
        {
            Tag = Tag,
            BackColor = Color.LightGray,
            ImageIndex = (int) mediaItem.MediaType
        };
    }

    private static string[] DefaultRow(IMediaItem mediaItem)
    {
        return new[]
        {
            mediaItem.Title,
            Statuses.WAITING,
            mediaItem.MediaType.ToString(),
            Text.DEFAULT_SIZE,
            Text.DEFAULT_PROGRESS,
            Text.DEFAULT_SPEED,
            Text.DEFAULT_TIME,
            mediaItem.Url,
            mediaItem.Filepath
        };
    }

    #endregion
}