using JackTheVideoRipper.extensions;
using JackTheVideoRipper.interfaces;
using JackTheVideoRipper.models.processes;
using JackTheVideoRipper.models.rows;

namespace JackTheVideoRipper.models;

public abstract class ProcessUpdateRow : ProcessRunner, IProcessUpdateRow, IDynamicRow
{
    #region Data Members

    public IViewItem ViewItem { get; }

    public string Tag { get; } = Common.CreateTag();
    
    private readonly Console _console = new();

    public readonly string Filepath;

    public readonly string Filename;

    #endregion

    #region Attributes
    
    private string DefaultTitle => Filename.SplitCamelCase().ReplaceUnderscore().RemoveMultiSpace().Trim();

    #endregion

    #region View Item Accessors
    
    public readonly Dictionary<ViewField, IViewSubItem> ViewCollection;

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
        
    public string Speed
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
        base(mediaItem.ParametersString, completionCallback)
    {
        Filepath = mediaItem.Filepath;
        Filename = FileSystem.GetFilenameWithoutExtension(Filepath);
        ViewItem = Ripper.CreateMediaViewItem(mediaItem);
        ViewCollection = new Dictionary<ViewField, IViewSubItem>
        {
            { ViewField.Title,      ViewItem.SubItems[0] },
            { ViewField.Status,     ViewItem.SubItems[1] },
            { ViewField.MediaType,  ViewItem.SubItems[2] },
            { ViewField.Size,       ViewItem.SubItems[3] },
            { ViewField.Progress,   ViewItem.SubItems[4] },
            { ViewField.Speed,      ViewItem.SubItems[5] },
            { ViewField.Eta,        ViewItem.SubItems[6] },
            { ViewField.Url,        ViewItem.SubItems[7] },
            { ViewField.Path,       ViewItem.SubItems[8] }
        };

        AddToHistory(mediaItem);
        
        InitializeBuffer();
    }
    
    #endregion

    #region Public Methods

    public async Task OpenInConsole()
    {
        await _console.Open(GetInstanceName());
    }

    #endregion
    
    #region Overrides

    private string? GetProcessStatus()
    {
        if (ProcessStatus is ProcessStatus.Running && !Started)
            return $"Process | {Messages.StartupTasks}";

        return GetStatus();
    }

    public override async Task<bool> Update()
    {
        if (!await base.Update())
            return false;

        if (GetProcessStatus() is not { } status || status.IsNullOrEmpty())
            return false;
        
        SetViewField(() => UpdateViewItemFields(status));
        
        return true;
    }

    public override async Task<bool> Start()
    {
        if (!await base.Start())
            return false;
        
        StartMessage();
        await RetrieveTitle();
        return true;
    }

    protected override void Complete()
    {
        base.Complete();
        FinishMessage();
    }

    protected override bool SetProcessStatus(ProcessStatus processStatus)
    {
        if (!base.SetProcessStatus(processStatus))
            return false;

        SetViewField(() =>
        {
            UpdateRowColors(processStatus);
            SetDefaultMessages(processStatus);
        });
        
        return true;
    }

    #endregion

    #region Protected Methods

    protected void UpdateViewItemFields(string status)
    {
        ViewItem.Suspend();
        UpdateStatus(status);
        ViewItem.Resume();
    }

    protected void UpdateStatus(string statusMessage)
    {
        Status = statusMessage;

        if (Buffer.TokenizedProcessLine is not {Length: > 0} tokens)
            return;
            
        // download messages stream fast, bump the cursor up to one of the latest messages, if it exists...
        // only start skipping cursor ahead once download messages have started otherwise important info could be skipped
        Buffer.SkipToEnd();

        SetProgressText(tokens);
    }
    
    protected async Task RetrieveTitle()
    {
        if (Title.HasValue())
            return;
        
        SetTitle((await GetTitle()).ValueOrDefault(DefaultTitle));
    }

    #endregion

    #region Abstract Methods
    
    protected abstract Task<string> GetTitle();

    protected abstract void SetProgressText(IReadOnlyList<string> tokens);

    protected abstract string? GetStatus();

    #endregion

    #region Private Methods

    private void SetTitle(string title)
    {
        SetViewField(() => Title = title);
    }

    private void AddToHistory(IMediaItem mediaItem)
    {
        History.Data.AddHistoryItem(Tag, mediaItem);
    }
    
    private void StartMessage()
    {
        History.Data.MarkStarted(Tag);
        Buffer.AddLog(Messages.ProcessStarted, ProcessLogType.Info);
    }

    private void FinishMessage()
    {
        History.Data.MarkCompleted(Tag, result:ProcessStatus);
        Buffer.AddLog(Messages.ProcessCompleted, ProcessLogType.Info);
    }

    private void InitializeBuffer()
    {
        Buffer.LogAdded += OnLogAdded;
        Buffer.AddLog(Messages.ProcessInitialized, ProcessLogType.Info);
    }

    private string GetInstanceName()
    {
        string processName = FileSystem.GetFilename(ProcessFileName);
        string filename = FileSystem.GetFilename(Path);
        return processName.HasValue() && filename.HasValue() ? $"{processName} | {filename}" : string.Empty;
    }

    private void OnLogAdded(ProcessLogNode logNode)
    {
        _console.WriteOutput(logNode);
    }

    private void UpdateRowColors(ProcessStatus processStatus)
    {
        Color = processStatus switch
        {
            // Type Specific
            ProcessStatus.Running when this is DownloadProcessUpdateRow => Color.Turquoise,
            ProcessStatus.Running when this is CompressProcessUpdateRow => Color.MediumPurple,
            ProcessStatus.Running when this is ConversionProcessUpdateRow => Color.SaddleBrown,
            
            // General
            ProcessStatus.Queued    => Color.Bisque,
            ProcessStatus.Cancelled => Color.LightYellow,
            ProcessStatus.Completed => Color.LightGreen,
            ProcessStatus.Error     => Color.LightCoral,
            ProcessStatus.Stopped   => Color.DarkSalmon,
            ProcessStatus.Created   => Color.LightGray,
            ProcessStatus.Paused    => Color.DarkGray,
            _                       => Color.White
        };
    }

    private void SetValues(ViewField fields, params string[] values)
    {
        Queue<string> valueQueue = new(values);
        if ((fields & ViewField.Status) > 0)
            Status = valueQueue.Dequeue();
        if ((fields & ViewField.Size) > 0)
            FileSize = valueQueue.Dequeue();
        if ((fields & ViewField.Progress) > 0)
            Progress = valueQueue.Dequeue();
        if ((fields & ViewField.Speed) > 0)
            Speed = valueQueue.Dequeue();
        if ((fields & ViewField.Eta) > 0)
            Eta = valueQueue.Dequeue();
    }

    [Flags]
    public enum ViewField
    {
        None        = 0,
        Status      = 1<<0,
        Size        = 1<<1,
        Progress    = 1<<2,
        Speed       = 1<<3,
        Eta         = 1<<4,
        Title       = 1<<5,
        MediaType   = 1<<6,
        Url         = 1<<7,
        Path        = 1<<8,
        Static      = (1<<5) | (1<<6) | (1<<7) | (1<<8),
        Dynamic     = (1<<0) | (1<<1) | (1<<2) | (1<<3) | (1<<4),
        All         = (1<<0) | (1<<1) | (1<<2) | (1<<3) | (1<<4) | (1<<5) | (1<<6) | (1<<7) | (1<<8),
    }

    private static readonly Dictionary<ProcessStatus, ViewField> _StatusToViewFieldsDict = new()
    {
        { ProcessStatus.Succeeded,  ViewField.None },
        { ProcessStatus.Running,    ViewField.Status },
        { ProcessStatus.Queued,     ViewField.Status },
        { ProcessStatus.Created,    ViewField.Dynamic },
        { ProcessStatus.Completed,  ViewField.Status | ViewField.Progress | ViewField.Speed | ViewField.Eta },
        { ProcessStatus.Error,      ViewField.Status | ViewField.Size | ViewField.Speed | ViewField.Eta },
        { ProcessStatus.Stopped,    ViewField.Status | ViewField.Speed | ViewField.Eta },
        { ProcessStatus.Cancelled,  ViewField.Dynamic },
        { ProcessStatus.Paused,     ViewField.Status | ViewField.Speed | ViewField.Eta }
    };
    
    private static readonly Dictionary<ProcessStatus, string> _StatusToMessageDict = new()
    {
        { ProcessStatus.Succeeded,  Statuses.Succeeded },
        { ProcessStatus.Running,    Statuses.Starting },
        { ProcessStatus.Queued,     Statuses.Queued },
        { ProcessStatus.Created,    Statuses.Waiting },
        { ProcessStatus.Completed,  Statuses.Complete },
        { ProcessStatus.Error,      Statuses.Error },
        { ProcessStatus.Stopped,    Statuses.Stopped },
        { ProcessStatus.Cancelled,  Statuses.Cancelled },
        { ProcessStatus.Paused,     Statuses.Paused }
    };

    private void SetDefaultMessages(ProcessStatus processStatus)
    {
        ViewField flags = _StatusToViewFieldsDict[processStatus];
        string[] values;
        
        switch (processStatus)
        {
            default:
            case ProcessStatus.Succeeded:
                return;
            case ProcessStatus.Running:
                values = new[]
                {
                    _StatusToMessageDict[processStatus]
                };
                break;
            case ProcessStatus.Queued:
                values = new[]
                {
                    _StatusToMessageDict[processStatus]
                };
                break;
            case ProcessStatus.Created:
                values = new[]
                {
                    _StatusToMessageDict[processStatus],
                    Text.DefaultSize,
                    Text.DefaultProgress,
                    Text.DefaultSpeed,
                    Text.DefaultTime
                };
                break;
            case ProcessStatus.Completed:
                values = new[]
                {
                    _StatusToMessageDict[processStatus],
                    Text.ProgressComplete,
                    Text.DefaultSpeed,
                    Text.DefaultTime
                };
                break;
            case ProcessStatus.Error:
                values = new[]
                {
                    _StatusToMessageDict[processStatus],
                    Text.DefaultSize,
                    Text.DefaultSpeed,
                    Text.DefaultTime
                };
                break;
            case ProcessStatus.Stopped:
                values = new[]
                {
                    _StatusToMessageDict[processStatus],
                    Text.DefaultSpeed,
                    Text.DefaultTime
                };
                break;
            case ProcessStatus.Cancelled:
                values = new[]
                {
                    _StatusToMessageDict[processStatus],
                    Text.DefaultSize,
                    Text.DefaultProgress,
                    Text.DefaultSpeed,
                    Text.DefaultTime
                };
                break;
            case ProcessStatus.Paused:
                values = new[]
                {
                    _StatusToMessageDict[processStatus],
                    Text.DefaultSpeed,
                    Text.DefaultTime
                };
                break;
        }

        SetValues(flags, values);
    }

    #endregion

    #region Static Methods

    // TODO: Currently this is causing deadlock on the main thread, unless we run in the background as so...
    protected static void SetViewField(Action setValueAction)
    {
        Task.Run(() => Threading.RunInMainContext(setValueAction));
    }

    #endregion
}