namespace JackTheVideoRipper.extensions;

public static class StreamExtensions
{
    public static async Task WriteToFileAsync(this Stream stream, string outputPath, int? size)
    {
        /*byte[] fileStreamBuffer = new byte[size];
        int result3 = await stream.ReadAsync(fileStreamBuffer);
        await fileStream.WriteAsync(fileStreamBuffer);*/
        await using FileStream fileStream = File.Open(outputPath, FileMode.OpenOrCreate, FileAccess.Write);
        if (size is null)
        {
            await stream.CopyToAsync(fileStream);
        }
        else
        {
            await stream.CopyToAsync(fileStream, (int) size);
        }
    }
    
    public static void WriteToFile(this Stream stream, string outputPath, int? size)
    {
        using FileStream fileStream = File.Open(outputPath, FileMode.OpenOrCreate, FileAccess.Write);
        if (size is null)
        {
            stream.CopyTo(fileStream);
        }
        else
        {
            stream.CopyTo(fileStream, (int) size);
        }
    }
}