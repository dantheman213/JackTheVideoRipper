using System.Diagnostics;
using JackTheVideoRipper.extensions;
using JackTheVideoRipper.interfaces;
using JackTheVideoRipper.models;
using JackTheVideoRipper.models.containers;

namespace JackTheVideoRipper.modules;

internal static class FFMPEG
{
    #region Data Members

    private const string _EXECUTABLE_NAME = "ffmpeg.exe";
    
    public static readonly string ExecutablePath = FileSystem.ProgramPath(_EXECUTABLE_NAME);

    public static readonly string FfprobePath = FileSystem.ProgramPath("ffprobe.exe");
    
    private const string _DOWNLOAD_URL = "https://www.ffmpeg.org/download.html";

    private static readonly FfmpegParameters _DefaultParameters =
        new FfmpegParameters().NoStats().LogLevel(LogLevel.Error).HideBanner();

    private const string _IMAGE_FORMAT_STRING = "frame_%d.png";
    
    private static readonly Command _Command = new(ExecutablePath);

    private const string _NUMBER_OF_FRAMES_PARAMETERS =
        @"-v error -select_streams v:0 -count_packets -show_entries stream=nb_read_packets -of csv=p=0";

    #endregion

    #region Attributes

    public static bool IsInstalled => File.Exists(ExecutablePath);

    #endregion

    public static FfmpegParameters ConvertImageToJpg(string inputPath, string outputPath)
    {
        return _DefaultParameters
            .Input(inputPath)
            .Scale(1920)
            .Output(outputPath);
    }

    public static async Task<int> GetNumberOfFrames(string filepath)
    {
        var result = (await FileSystem.RunCommandAsync(FfprobePath, $"{_NUMBER_OF_FRAMES_PARAMETERS} {filepath}"))
            .Output;
        
        if (!int.TryParse(result, out int totalFrames))
            return -1;
        
        return totalFrames;
    }

    public static async Task DownloadLatest()
    {
        await FileSystem.GetWebResourceHandle(_DOWNLOAD_URL, FileSystem.Paths.Install).Run();
    }

    // TODO: Verify that input frame count == output frame count
    // TODO: Validate extensions (.mp4, .wav, .mp3, .avi)
    // TODO: Capture FFMPEG output

    // TODO: Graph for output sampling?? - Speed, frames, etc.

    public static FfmpegParameters SplitImages(VideoInformation videoInformation)
    {
        return new FfmpegParameters(videoInformation.InputFilepath)
            .FrameRate(videoInformation.FramesPerSecond)
            .Output(_IMAGE_FORMAT_STRING);
    }

    public static FfmpegParameters Recombine(VideoInformation videoInformation, bool includeAudio = false,
        string audioFilepath = "")
    {
        FfmpegParameters parameters = new FfmpegParameters()
            .Rate(videoInformation.FramesPerSecond)
            .FrameFormat("image2")
            .Resolution(videoInformation.Resolution)
            .Input(_IMAGE_FORMAT_STRING)
            .VideoCodec(VideoCodecs.H264);

        FfmpegParameters optionBasedParameters = includeAudio ?
            new FfmpegParameters(audioFilepath)
                .Bitrate("4M")
                .VideoPre()
                .AudioCodec() :
            new FfmpegParameters()
                .ConstantRateFactor(25)
                .PixelFormat(PixelFormats.YUV_420P);

        return parameters.Append(optionBasedParameters).Output(videoInformation.OutputFilepath);
    }

    // Used for super-resolution enhancements (external processing)
    public static FfmpegParameters AddAudio(string inputFilepath, string audioFilepath, string output)
    {
        return new FfmpegParameters(inputFilepath)
            .Input(audioFilepath)
            .Copy()
            .MapVideo(false)
            .MapAudio(true)
            .Output(output);
    }

    // TODO: Was used originally for the failure, actually occurred due to encoding error from Adobe Premiere Pro
    public static FfmpegParameters Convert(string inputFilepath)
    {
        return new FfmpegParameters()
            .HardwareAcceleration()
            .Input(inputFilepath)
            .CopyAudio()
            .CopyVideo(VideoCodecs.AYUV)
            .Output(inputFilepath, VideoFormats.AVI);
    }

    public static FfmpegParameters Recode(string inputFilepath)
    {
        return new FfmpegParameters()
            .HardwareAcceleration()
            .Input(inputFilepath);
    }

    // https://unix.stackexchange.com/questions/28803/how-can-i-reduce-a-videos-size-with-ffmpeg
    // Reasonable range for H.265 may be 24-30
    // Lower - 18-24
    public static FfmpegParameters Compress(string inputFilepath, int compressionRating = 30)
    {
        return new FfmpegParameters()
            .HardwareAcceleration()
            .Input(inputFilepath)
            .VideoCodec(VideoCodecs.H264)
            .ConstantRateFactor(compressionRating)
            .OutputFromInput(inputFilepath, "COMPRESSED");
    }

    /** TODO:
     * 1. Batch into segments of no more than n frames
     * 2. Cache the frames
     * 3. Recombine into small videos
     * 4. Combine smaller videos every block
     * 5. Delete cached frames after every block to not bloat system
     * 6. Recombine all at the end
     *  Look up how YouTube-DL does this (.part files)
     */
    public static async Task<FfmpegParameters[]> RepairVideo(string videoFilepath)
    {
        VideoInformation videoInformation = await ExtractVideoInformation(videoFilepath);
        videoInformation.OutputFilepath = FileSystem.AppendSuffix(videoFilepath, "FIXED", "_");
        return new[]
        {
            SplitImages(videoInformation),
            Recombine(videoInformation)
        };
    }

    public static FfmpegParameters VerifyIntegrity(string videoFilepath, string? logFilepath = null)
    {
        string outputFilepath = logFilepath ?? $"{FileSystem.TempFile}.log";
        return new FfmpegParameters(videoFilepath)
            .LogLevel(LogLevel.Error)
            .FrameFormat() // Don't process frames
            .Miscellaneous($"- >{outputFilepath.WrapQuotes()} 2>&1");
    }

    public static string RunFFMPEG(string parameters)
    {
        return _Command.RunCommand(parameters);
    }

    public static string RunFFMPEG(IProcessParameters parameters)
    {
        return _Command.RunCommand(parameters);
    }
    
    public static Process CreateCommand(string parameters)
    {
        return _Command.CreateCommand(parameters);
    }

    public static Process CreateCommand(IProcessParameters parameters)
    {
        return _Command.CreateCommand(parameters);
    }
    
    public static async Task<VideoInformation> ExtractVideoInformation(string filepath)
    {
        if (!File.Exists(filepath))
            return new VideoInformation();

        ExifData exifData = new(await ExifTool.GetTags(filepath, "Video Frame Rate", "Image Size"));

        // These fields should be filled by retrieving the file information of the videoFilepath parameter
        return new VideoInformation
        {
            InputFilepath = filepath,
            TotalFrames = await GetNumberOfFrames(filepath),
            //Duration = metadata.Duration.Duration(),
            FramesPerSecond = exifData.VideoFrameRate,
            Resolution = exifData.ImageSize,
            OutputFilepath = FileSystem.AppendSuffix(filepath, "FIXED", "_")
        };
    }

    #region Enums

    public enum LogLevel
    {
        Quiet,
        Panic,
        Fatal,
        Error,
        Warning,
        Info,
        Verbose,
        Debug
    }

    #endregion

    #region Embedded Types

    public static class Resolutions
    {
        public const string R_240P = "352x240";

        public const string R_360P = "480x360";

        public const string R_480P = "858x480";     //< SD

        public const string R_720P = "1280x720";    //< HD

        public const string R_1080P = "1920x1080";  //< FHD

        public const string R_1440P = "2560x1440";  //< UHD

        public const string R_2160P = "3840x2160";  //< 4K
    }
    
    public static class VideoCodecs
    {
        public const string H264 = "libx264";
        
        public const string H265 = "libx265";

        public const string AYUV = "ayuv";

        public const string HEVC = "hevc"; // H.265
    }

    public static class VideoFormats
    {
        public const string AVI = "avi";
        
        public const string MP4 = "mp4";
        
        public const string MOV = "mov";
        
        public const string M4V = "m4v";

        public const string MPEG = "mpeg";

        public const string MKV = "mkv";
    }

    public static class AudioFormats
    {
        public const string MP3 = "mp3";

        public const string WAV = "wav";

        public const string OGG = "ogg";
        
        public const string M4A = "m4a";

        public const string AAC = "aac";
    }

    public static class PixelFormats
    {
        public const string YUV_420P = "yuv420p";
    }
    
    public record VideoInformation
    {
        public string InputFilepath = string.Empty;

        public int TotalFrames;

        public float FramesPerSecond;

        public int Duration;

        public string Resolution = string.Empty;

        public string OutputFilepath = string.Empty;
    }

    public class FfmpegParameters : ProcessParameters<FfmpegParameters>
    {
        #region Constructor

        public FfmpegParameters()
        {
        }

        public FfmpegParameters(string inputFilepath)
        {
            Input(inputFilepath);
        }

        #endregion

        #region Public

        public FfmpegParameters Rate(float rate)
        {
            return Add('r', $"{rate:F2}");
        }

        public FfmpegParameters Input(string filepath)
        {
            return Add('i', filepath.WrapQuotes());
        }

        public FfmpegParameters Output(string filepath, string? outputFormat = null)
        {
            return Append(outputFormat is null ?
                filepath.WrapQuotes() : 
                FileSystem.ChangeExtension(filepath, outputFormat).WrapQuotes());
        }
        
        public FfmpegParameters OutputFromInput(string inputFilepath, string suffix, string? outputFormat = null)
        {
            string outputFilepath = FileSystem.AppendSuffix(inputFilepath, suffix, "_");
            
            return Append(outputFormat is null ?
                outputFilepath.WrapQuotes() : 
                FileSystem.ChangeExtension(outputFilepath, outputFormat).WrapQuotes());
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

        public FfmpegParameters ConstantRateFactor(int framerate)
        {
            return Add("crf", framerate);
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

        public FfmpegParameters LogLevel(LogLevel logLevel = FFMPEG.LogLevel.Quiet)
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

        #endregion

        #region Overrides

        public override string ToString()
        {
            return base.ToString().Replace("--", "-");
        }

        #endregion
    }
    
    #endregion
}