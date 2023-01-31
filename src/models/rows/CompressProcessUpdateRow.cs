using System.Diagnostics;
using JackTheVideoRipper.extensions;
using JackTheVideoRipper.interfaces;
using JackTheVideoRipper.models.containers;
using JackTheVideoRipper.modules;

namespace JackTheVideoRipper.models;

public class CompressProcessUpdateRow : ProcessUpdateRow
{
    private int _totalFrames;

    private readonly ExifData _exifData = new();
    
    public CompressProcessUpdateRow(IMediaItem mediaItem, Action<IProcessRunner> completionCallback) :
        base(mediaItem, completionCallback)
    {
        Task.Run(LoadMetadata);
    }

    protected override Process CreateProcess()
    {
        return FFMPEG.CreateCommand(ParameterString);
    }

    public override async Task<bool> Start()
    {
        SetViewField(() => Url = Text.NotApplicable);
        return await base.Start();
    }

    protected override void SetProgressText(IReadOnlyList<string> tokens)
    {
        if (tokens.Count < 8)
            return;

        if (!tokens[0].Contains("frame"))
            return;

        FfmpegFrame ffmpegFrame = new(tokens);
        Progress = CalculateProgress(ffmpegFrame.Frame);
        Eta = CalculateEta(ffmpegFrame.Frame, ffmpegFrame.Fps);
        FileSize = FileSystem.GetFileSizeFormatted(ffmpegFrame.Size);
        Speed = $"{ffmpegFrame.Fps} fps";
    }

    private string CalculateProgress(int frame)
    {
        return _totalFrames > 0 ? $"{frame * 100f / _totalFrames:F2}%" : Text.DefaultProgress;
    }

    private string CalculateEta(int frame, float fps)
    {
        return Common.TimeString(((float) _totalFrames - frame) / fps);
    }

    public static string GetField(IReadOnlyList<string> tokens, ref int count)
    {
        string value = tokens[count++];
        if (value.ContainsNumber())
            return value.Split('=')[1];
        if (count >= tokens.Count)
            return string.Empty;
        return tokens[count++];
    }

    protected override async Task<string> GetTitle()
    {
        return await ExifTool.GetTag(Filepath, "Title");
    }

    private static bool IsFileExistsLine(IEnumerable<string> tokens)
    {
        return IsFileExistsLine(tokens.Merge(" "));
    }
    
    private static bool IsFileExistsLine(string line)
    {
        return line.StartsWith("File") && line.EndsWith(Messages.FFMPEGFileExists);
    }

    protected override string GetStatus()
    {
        return Messages.Compressing;
    }
    
    private async Task LoadMetadata()
    {
        _exifData.LoadData(await ExifTool.GetMetadataString(Filepath));
        _totalFrames = _exifData.Frames > 0 ? _exifData.Frames : await FFMPEG.GetNumberOfFrames(Filepath);
    }

    private readonly struct FfmpegFrame
    {
        public readonly int Frame;
        public readonly float Fps;
        public readonly float Q;
        public readonly long Size;
        public readonly TimeOnly Time;
        public readonly float Bitrate;
        public readonly string BitrateUnit;
        public readonly float Speed;

        public FfmpegFrame(IReadOnlyList<string> tokens)
        {
            int count = 0;
            Frame = int.TryParse(GetField(tokens, ref count), out int frame) ? frame : -1;
            Fps = float.TryParse(GetField(tokens, ref count), out float fps) ? fps : -1;
            Q = float.TryParse(GetField(tokens, ref count), out float q) ? q : -1;
            Size = long.TryParse(GetField(tokens, ref count).BeforeFirstLetter(), out long size) ? size * 1000 : -1;
            Time = TimeOnly.TryParse(GetField(tokens, ref count), out TimeOnly time) ? time : default;

            string bitrateField = GetField(tokens, ref count);
            Bitrate = float.TryParse(bitrateField.BeforeFirstLetter(), out float bitrate) ? bitrate : -1;
            BitrateUnit = bitrateField.AfterFirstLetter();
            
            Speed = float.TryParse(GetField(tokens, ref count), out float speed) ? speed : -1;
        }
    }
}