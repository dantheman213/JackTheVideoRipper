using System.Diagnostics;
using JackTheVideoRipper.extensions;
using JackTheVideoRipper.interfaces;
using JackTheVideoRipper.models.enums;

namespace JackTheVideoRipper;

public class ProcessUpdateRow : ProcessRunner
{
    #region Data Members

    public ListViewItem ViewItem { get; init; } = null!;

    public string Tag { get; init; } = null!;

    #endregion

    #region Properties

    public DownloadStage DownloadStage => GetDownloadStage(ProcessLine);

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
        
    public ProcessUpdateRow(string parameterString, Action<IProcessRunner> completionCallback) :
        base(parameterString, completionCallback)
    {
    }
        
    #endregion
    
    #region Overrides

    protected override Process CreateProcess()
    {
        return YouTubeDL.CreateCommand(ParameterString);
    }

    public override void Update()
    {
        base.Update();

        switch (DownloadStage)
        {
            default:
            case DownloadStage.None:
            case DownloadStage.Waiting:
                break;
            case DownloadStage.Metadata:
                UpdateDownloadStatus(Messages.READING_METADATA);
                break;
            case DownloadStage.Transcoding:
                UpdateDownloadStatus(Messages.TRANSCODING);
                break;
            case DownloadStage.Downloading:
                UpdateDownloadStatus(Messages.DOWNLOADING);
                break;
            case DownloadStage.Error:
                SetErrorState();
                break;
        }
    }

    protected override void SetProcessStatus(ProcessStatus processStatus)
    {
        base.SetProcessStatus(processStatus);
        Color = processStatus switch
        {
            ProcessStatus.Running   => Color.Turquoise,
            ProcessStatus.Cancelled => Color.LightYellow,
            ProcessStatus.Completed => Color.LightGreen,
            ProcessStatus.Error     => Color.LightCoral,
            ProcessStatus.Stopped   => Color.DarkSalmon,
            ProcessStatus.Created   => Color.LightGray,
            ProcessStatus.Paused    => Color.DarkGray,
            _ => Color.White
        };
        SetDefaultMessages(processStatus);
    }

    #endregion

    #region Private Methods

    private void UpdateDownloadStatus(string statusMessage)
    {
        if (TokenizedProcessLine is not { Length: >= 8 } tokens )
            return;
            
        // download messages stream fast, bump the cursor up to one of the latest messages, if it exists...
        // only start skipping cursor ahead once download messages have started otherwise important info could be skipped
        SkipToEnd();

        Status = statusMessage;

        SetDownloadProgressText(tokens);
    }
    
    // Extract elements of CLI output from YouTube-DL
    private void SetDownloadProgressText(IReadOnlyList<string> tokens)
    {
        Progress = tokens[1];
        FileSize = tokens[3];
        DownloadSpeed = tokens[5];
        Eta = tokens[7];
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
        
    private static DownloadStage GetDownloadStage(string line)
    {
        if (line.IsNullOrEmpty())
            return DownloadStage.None;
        if (line.Contains(Tags.YOUTUBE) || line.Contains(Tags.INFO))
            return DownloadStage.Metadata;
        if (line.Contains(Tags.FFMPEG))
            return DownloadStage.Transcoding;
        if (line.Contains(Tags.DOWNLOAD))
            return DownloadStage.Downloading;
        if (line.Contains(Tags.ERROR, StringComparison.OrdinalIgnoreCase) || line[..21].Contains("Usage"))
            return DownloadStage.Error;
        if (line.Contains('%'))
            return DownloadStage.Waiting;
        return DownloadStage.None;
    }

    private void SetDefaultMessages(ProcessStatus processStatus)
    {
        switch (processStatus)
        {
            default:
            case ProcessStatus.Succeeded:
            case ProcessStatus.Queued:
            case ProcessStatus.Running:
                break;
            case ProcessStatus.Created:
                SetValues(Statuses.WAITING, Tags.DEFAULT_SIZE, Tags.DEFAULT_PROGRESS, Tags.DEFAULT_SPEED, 
                    eta:Tags.DEFAULT_TIME);
                break;
            case ProcessStatus.Completed:
                SetValues(Statuses.COMPLETE, progress:Tags.PROGRESS_COMPLETE, downloadSpeed:Tags.DEFAULT_SPEED,
                    eta:Tags.DEFAULT_TIME);
                break;
            case ProcessStatus.Error:
                SetValues(Statuses.ERROR, Tags.DEFAULT_SIZE, downloadSpeed:Tags.DEFAULT_SPEED,
                    eta:Tags.DEFAULT_TIME);
                break;
            case ProcessStatus.Stopped:
                SetValues(Statuses.STOPPED, downloadSpeed:Tags.DEFAULT_SPEED, eta:Tags.DEFAULT_TIME);
                break;
            case ProcessStatus.Cancelled:
                SetValues(Status = Statuses.CANCELLED, Tags.DEFAULT_SIZE, Tags.DEFAULT_PROGRESS, Tags.DEFAULT_SPEED,
                    eta:Tags.DEFAULT_TIME);
                break;
            case ProcessStatus.Paused:
                SetValues(Status = Statuses.PAUSED, downloadSpeed:Tags.DEFAULT_SPEED, eta:Tags.DEFAULT_TIME);
                break;
        }
    }

    #endregion
}