namespace JackTheVideoRipper.models.enums;

public enum DownloadStage
{
    None,
    Metadata,
    Transcoding,
    Downloading,
    Waiting,
    Error
}