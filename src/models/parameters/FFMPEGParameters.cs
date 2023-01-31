using JackTheVideoRipper.extensions;
using JackTheVideoRipper.interfaces;
using JackTheVideoRipper.modules;

namespace JackTheVideoRipper.models.parameters;

public class FfmpegParameters : ProcessParameters<FfmpegParameters>, IRequiresFileParameters
{
    public string Filepath => InputFilepath;
    
    public string InputFilepath = string.Empty;

    public string OutputFilepath = string.Empty;
    
    #region Constructor

    public FfmpegParameters()
    {
    }

    public FfmpegParameters(string inputFilepath, bool hardwareAcceleration = false, string encoder = "cuda")
    {
        Input(inputFilepath, hardwareAcceleration, encoder);
    }

    #endregion

    #region Public

    public FfmpegParameters Rate(float rate)
    {
        return Add('r', $"{rate:F2}");
    }

    public FfmpegParameters Input(string filepath)
    {
        InputFilepath = filepath;
        return Add('i', filepath.WrapQuotes());
    }
    
    public FfmpegParameters Input(string filepath, bool hardwareAcceleration, string encoder)
    {
        InputFilepath = filepath;
        return hardwareAcceleration ? HardwareAcceleration(encoder).Input(filepath) : Input(filepath);
    }

    public FfmpegParameters Output(string filepath, string? outputFormat = null)
    {
        OutputFilepath = outputFormat is null ?
            filepath.WrapQuotes() : 
            FileSystem.ChangeExtension(filepath, outputFormat);
        return Append(OutputFilepath.WrapQuotes());
    }
    
    public FfmpegParameters OutputFromInput(FFMPEG.Operation operation, string? outputFormat = null)
    {
        OutputFilepath = FFMPEG.GetOutputFilename(InputFilepath, operation, outputFormat);
        return Append(OutputFilepath.WrapQuotes());
    }

    public FfmpegParameters AudioCodec(string codec = "copy")
    {
        return Add("acodec", codec);
    }

    public FfmpegParameters VideoCodec(string codec = "copy")
    {
        return Add("vcodec", codec);
    }

    public FfmpegParameters HardwareAcceleration(string encoder = "cuda")
    {
        return Add("hwaccel", encoder);
    }

    public FfmpegParameters Copy()
    {
        return Add('c', "copy");
    }

    public FfmpegParameters CopyAudio(string codec = "copy")
    {
        return Add("c:a", codec);
    }

    public FfmpegParameters CopyVideo(string codec = "copy")
    {
        return Add("c:v", codec);
    }

    public FfmpegParameters MapAudio(bool shouldMap)
    {
        return Add("map", $"{(shouldMap ? 1 : 0)}:a");
    }

    public FfmpegParameters MapVideo(bool shouldMap)
    {
        return Add("map", $"{(shouldMap ? 1 : 0)}:v");
    }

    public FfmpegParameters FrameRate(float framesPerSecond)
    {
        return Add("vf", $"fps={framesPerSecond:F2}");
    }

    public FfmpegParameters Scale(int width = -1, int height = -1)
    {
        return Add("vf",$"scale={width}:{height}".WrapQuotes());
    }

    public FfmpegParameters FrameFormat(string frameFormat = "null")
    {
        return Add('f', frameFormat);
    }

    public FfmpegParameters Resolution(string resolution)
    {
        return Add('s', resolution);
    }

    public FfmpegParameters ConstantRateFactor(int rateFactor = 30)
    {
        return Add("crf", rateFactor);
    }

    public FfmpegParameters PixelFormat(string format)
    {
        return Add("pix_fmt", format);
    }

    public FfmpegParameters Bitrate(string bitrate)
    {
        return Add('b', bitrate);
    }

    public FfmpegParameters VideoPre(string pre = "normal")
    {
        return Add("vpre", pre);
    }

    public FfmpegParameters NoStats()
    {
        return AddNoValue("nostats");
    }

    public FfmpegParameters LogLevel(FFMPEG.LogLevel logLevel = FFMPEG.LogLevel.Quiet)
    {
        return Add("loglevel", logLevel.ToString().ToLower());
    }

    public FfmpegParameters HideBanner()
    {
        return AddNoValue("hide_banner");
    }

    public FfmpegParameters Miscellaneous(string parameters)
    {
        return Append(parameters);
    }

    public FfmpegParameters NoVideo()
    {
        return AddNoValue("vn");
    }
    
    public FfmpegParameters NoAudio()
    {
        return AddNoValue("an");
    }

    #endregion

    #region Overrides

    public override string ToString()
    {
        return base.ToString().Replace("--", "-");
    }

    #endregion
}