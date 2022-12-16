using JackTheVideoRipper.models;

namespace JackTheVideoRipper.extensions;

public static class ExceptionExtensions
{
    public static void SaveToFile(this Exception exception)
    {
        FileSystem.WriteJsonToFile(FileSystem.GetDownloadPath($"stacktrace_{DateTime.Now:yyyyMMddHHmmss}"), 
            new ExceptionModel(exception));
    }
}