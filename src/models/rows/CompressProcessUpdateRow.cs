using System.Diagnostics;
using JackTheVideoRipper.extensions;
using JackTheVideoRipper.interfaces;
using JackTheVideoRipper.models.containers;
using JackTheVideoRipper.modules;

namespace JackTheVideoRipper.models;

public class CompressProcessUpdateRow : ProcessUpdateRow
{
    private readonly string _fileName;
    
    private int _totalFrames;

    private readonly ExifData _exifData = new();
    
    public CompressProcessUpdateRow(IMediaItem mediaItem, Action<IProcessRunner> completionCallback) :
        base(mediaItem, completionCallback)
    {
        _fileName = mediaItem.Filepath;
        Task.Run(LoadMetadata);
    }

    protected override Process CreateProcess()
    {
        return FFMPEG.CreateCommand(ParameterString);
    }

    public override async Task<bool> Start()
    {
        await Core.RunTaskInMainThread(() => Url = "N/A");
        return await base.Start();
    }

    protected override void SetProgressText(IReadOnlyList<string> tokens)
    {
        if (tokens.Count < 8)
            return;

        if (!tokens[0].Contains("frame"))
            return;

        var queue = new Queue<string>(tokens);
        int frame = -1;
        int fps = -1;

        while (!queue.Empty())
        {
            string value = GetField(queue);
            switch (GetTypeOfField(value))
            {
                case FieldType.Time when DateTime.TryParse(value, out DateTime dateTime):
                    break;
                case FieldType.Decimal when float.TryParse(value, out float floatValue):
                    break;
                case FieldType.Integer when int.TryParse(value, out int intValue):
                    if (frame == -1)
                    {
                        frame = intValue;
                        Progress = _totalFrames > 0 ? $"{frame * 100f / _totalFrames:F2}%" : Text.DEFAULT_PROGRESS;
                    }
                    else if (fps == -1)
                    {
                        fps = intValue;
                        DownloadSpeed = $"{intValue} fps";
                    }
                    break;
                case FieldType.Bitrate when float.TryParse(value.BeforeFirstLetter(), out float bitrate):
                    string rateUnit = value.AfterFirstLetter();
                    break;
                case FieldType.Speed when float.TryParse(value.BeforeFirstLetter(), out float speed):
                    string speedUnit = value.AfterFirstLetter();
                    break;
                case FieldType.Size when long.TryParse(value.BeforeFirstLetter(), out long longValue):
                    FileSize = FileSystem.GetSizeWithSuffix(longValue * 1000);
                    break;
            }
        }

        Eta = Common.TimeString(((float) _totalFrames - frame) / fps);
    }

    public static string GetField(Queue<string> queue)
    {
        string value = queue.Dequeue();
        if (value.ContainsNumber())
            return value.Split('=')[1];
        if (queue.Empty())
            return string.Empty;
        return queue.Dequeue();
    }

    public enum FieldType
    {
        Integer,
        Time,
        Decimal,
        Bitrate,
        Speed,
        Size
    }

    public static FieldType GetTypeOfField(string field)
    {
        return field switch
        {
            _ when field.Contains(':')      => FieldType.Time,
            _ when field.Contains('x')      => FieldType.Speed,
            _ when field.Contains('/')      => FieldType.Bitrate,
            _ when field.Contains('.')      => FieldType.Decimal,
            _ when field.ContainsLetter()   => FieldType.Size,
            _                               => FieldType.Integer,
        };
    }

    protected override async Task<string> GetTitle()
    {
        return await ExifTool.GetTag(_fileName, "Title");
    }

    private static bool IsFileExistsLine(IEnumerable<string> tokens)
    {
        return IsFileExistsLine(tokens.Merge(" "));
    }
    
    private static bool IsFileExistsLine(string line)
    {
        return line.StartsWith("File") && line.EndsWith("already exists. Overwrite? [y/N]");
    }

    protected override string GetStatus()
    {
        return Messages.COMPRESSING;
    }
    
    private async Task LoadMetadata()
    {
        _exifData.LoadData(await ExifTool.GetMetadataString(_fileName));
        _totalFrames = _exifData.Frames > 0 ? _exifData.Frames : await FFMPEG.GetNumberOfFrames(_fileName);
    }
}