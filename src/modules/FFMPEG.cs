using System.Diagnostics;
using JackTheVideoRipper.extensions;
using JackTheVideoRipper.models.containers;
using JackTheVideoRipper.models.parameters;

namespace JackTheVideoRipper.modules;

public static class FFMPEG
{
    #region Data Members
    
    public static readonly string ExecutablePath = FileSystem.ProgramPath(Executables.FFMPEG);
    
    private static readonly Command _Command = new(ExecutablePath);

    public static readonly string FFProbePath = FileSystem.ProgramPath(Executables.FFProbe);
    
    private static readonly Command _FFProbeCommand = new(FFProbePath);

    private static readonly FfmpegParameters _DefaultParameters =
        new FfmpegParameters().NoStats().LogLevel(LogLevel.Error).HideBanner();

    private const string _IMAGE_FORMAT_STRING = "frame_%d.png";

    private const string _NUMBER_OF_FRAMES_PARAMETERS =
        @"-v error -select_streams v:0 -count_packets -show_entries stream=nb_read_packets -of csv=p=0";
    
    public const string DEFAULT_BITRATE = "4M";
    
    public const string DEFAULT_FRAME_FORMAT = "image2";

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
        return await _FFProbeCommand.RunCommandAsync<int>($"{_NUMBER_OF_FRAMES_PARAMETERS} {filepath}");
    }

    public static async Task DownloadLatest()
    {
        await FileSystem.GetWebResourceHandle(Urls.FFMPEG, FileSystem.Paths.Install).Run();
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

    public static FfmpegParameters Recombine(VideoInformation videoInformation, string? audioFilepath = null)
    {
        FfmpegParameters parameters = new FfmpegParameters()
            .Rate(videoInformation.FramesPerSecond)
            .FrameFormat(DEFAULT_FRAME_FORMAT)
            .Resolution(videoInformation.Resolution)
            .Input(_IMAGE_FORMAT_STRING)
            .VideoCodec(VideoCodecs.H264);

        FfmpegParameters optionBasedParameters = audioFilepath.HasValue() ?
            new FfmpegParameters(audioFilepath!)
                .Bitrate(DEFAULT_BITRATE)
                .VideoPre()
                .AudioCodec() :
            new FfmpegParameters()
                .ConstantRateFactor()
                .PixelFormat(Formats.Pixel.YUV_420P);

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
        return new FfmpegParameters(inputFilepath, hardwareAcceleration:true)
            .CopyAudio()
            .CopyVideo(VideoCodecs.AYUV)
            .Output(inputFilepath, Formats.Video.AVI);
    }

    public static FfmpegParameters Recode(string inputFilepath)
    {
        return new FfmpegParameters(inputFilepath, hardwareAcceleration:true);
    }

    // https://unix.stackexchange.com/questions/28803/how-can-i-reduce-a-videos-size-with-ffmpeg
    // Reasonable range for H.265 may be 24-30
    // Lower - 18-24
    public static FfmpegParameters Compress(string inputFilepath, int compressionRating = 30)
    {
        return new FfmpegParameters(inputFilepath, hardwareAcceleration:true)
            .VideoCodec(VideoCodecs.H264)
            .ConstantRateFactor(compressionRating)
            .OutputFromInput(Operation.Compress);
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
    // Returns a list of task parameters in order they should be executed (dependency list)
    public static async Task<FfmpegParameters[]> RepairVideo(string videoFilepath)
    {
        VideoInformation videoInformation = await ExtractVideoInformation(videoFilepath,
            GetOutputFilename(videoFilepath, Operation.Repair));
        string audioFilepath = GetOutputFilename(videoFilepath, Operation.Audio, Formats.Audio.AAC);
        return new[]
        {
            SplitImages(videoInformation),
            ExtractAudio(videoFilepath),
            Recombine(videoInformation, audioFilepath)
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

    public static FfmpegParameters RemoveAudio(string filepath)
    {
        return new FfmpegParameters(filepath)
            .NoAudio()
            .VideoCodec()
            .OutputFromInput(Operation.Audio, Formats.Audio.AAC);
    }
    
    public static FfmpegParameters ExtractAudio(string filepath)
    {
        return new FfmpegParameters(filepath)
            .NoVideo()
            .AudioCodec()
            .OutputFromInput(Operation.NoAudio);
    }
    
    public static Process CreateCommand(string parameters)
    {
        return _Command.CreateCommand(parameters);
    }

    public static async Task<VideoInformation> ExtractVideoInformation(string filepath, string outputFilepath)
    {
        if (!File.Exists(filepath))
            throw new CouldNotExtractInfoException("Requested filepath does not exist!", new FileSystem.InvalidPathException());

        ExifData exifData = new(await ExifTool.GetTags(filepath, "Video Frame Rate", "Image Size"));

        // These fields should be filled by retrieving the file information of the videoFilepath parameter
        return new VideoInformation
        {
            InputFilepath = filepath,
            TotalFrames = await GetNumberOfFrames(filepath),
            //Duration = metadata.Duration.Duration(),
            FramesPerSecond = exifData.VideoFrameRate,
            Resolution = exifData.ImageSize,
            OutputFilepath = outputFilepath
        };
    }

    public static string GetOutputFilename(string inputFilename, Operation operation, string? outputFormat = null)
    {
        string suffix = operation switch
        {
            Operation.Repair    => "FIXED",
            Operation.Compress  => "COMPRESSED",
            Operation.Recode    => "RECODED",
            Operation.Audio     => "AUDIO",
            Operation.NoAudio   => "NO_AUDIO",
            _                   => "OUTPUT"
        };

        string outputFilename = FileSystem.AppendSuffix(inputFilename, suffix, "_");
        return outputFormat is null
            ? outputFilename
            : FileSystem.ChangeExtension(outputFilename, outputFormat);
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

    public enum Operation
    {
        Repair,
        Compress,
        Recode,
        Audio,
        NoAudio
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

    public record VideoInformation
    {
        public string InputFilepath = string.Empty;

        public int TotalFrames;

        public float FramesPerSecond;

        public int Duration;

        public string Resolution = string.Empty;

        public string OutputFilepath = string.Empty;
    }

    public class CouldNotExtractInfoException : Exception
    {
        public CouldNotExtractInfoException() { }
        
        public CouldNotExtractInfoException(string message) : base(message) { }
        
        public CouldNotExtractInfoException(string message, Exception innerException) : base(message, innerException) { }
    }

    #endregion
}