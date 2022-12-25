using System.Net;

namespace JackTheVideoRipper.extensions;

public static class HttpClientExtensions
{
    public static async Task<string?> DownloadResourceAsync(this HttpClient client, string resourceUrl)
    {
        return await client.GetResourceFileName(resourceUrl) is { } filename ?
            await client.DownloadResourceAsync(resourceUrl,
            FileSystem.CreateDownloadPath(filename)) : null;
    }
    
    public static async Task<string?> DownloadResourceAsync(this HttpClient client, string resourceUrl, string? downloadPath)
    {
        if (resourceUrl.Invalid(FileSystem.IsValidUrl) || downloadPath.Invalid(FileSystem.IsValidPath))
            return null;
        
        try
        {
            // Create Download Stream
            await using Stream stream = await client.GetStreamAsync(resourceUrl);

            // Create Local FileStream to Receive Download
            await using FileStream fileStream = new(downloadPath!, FileMode.Create);

            // Execute Copying from Remote Download Stream to Local
            await stream.CopyToAsync(fileStream);
        }
        catch (IOException exception)
        {
            throw new WebException($"Failed to write downloaded file to disk at {downloadPath!.WrapQuotes()}",
                exception);
        }
        catch (Exception exception)
        {
            throw new WebException($"Failed to download remote resource from {resourceUrl.WrapQuotes()}",
                exception);
        }

        return downloadPath;
    }
    
    public static async Task<string?> GetResourceFileName(this HttpClient client, string resourceUrl)
    {
        return (await client.GetAsync(resourceUrl)).GetFileNameFromRequest();
    }
}