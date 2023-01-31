using System.Net;

namespace JackTheVideoRipper.extensions;

public static class HttpResponseMessageExtensions
{
    public static string ResponseCode(this HttpResponseMessage response)
    {
        return $"{(int) response.StatusCode} ({response.ReasonPhrase})";
    }

    public static string DownloadResponse(this HttpResponseMessage response, string downloadPath)
    {
        try
        {
            using FileStream fileStream = new(downloadPath, FileMode.CreateNew);
            response.Content.CopyTo(fileStream, null, CancellationToken.None);
        }
        catch (Exception exception)
        {
            throw new WebException("Failed to write web response to disk", exception);
        }

        return downloadPath;
    }
    
    public static async Task<string> DownloadResponseAsync(this HttpResponseMessage response, string downloadPath)
    {
        try
        {
            await using FileStream fileStream = new(downloadPath, FileMode.CreateNew);
            await response.Content.CopyToAsync(fileStream);
        }
        catch (Exception exception)
        {
            throw new WebException("Failed to write web response to disk", exception);
        }

        return downloadPath;
    }

    public static string GetResponse(this HttpResponseMessage response)
    {
        return response.Content.ReadAsStringAsync().Result;
    }
    
    public static async Task<string> GetResponseAsync(this HttpResponseMessage response)
    {
        return await response.Content.ReadAsStringAsync();
    }
    
    public static string? GetFileNameFromRequest(this HttpResponseMessage response)
    {
        if (response.RequestMessage?.RequestUri is { } remotePath)
        {
            return remotePath.Segments.LastOrDefault();
        }

        return response.Content.Headers.ContentDisposition?.FileName;
    }
}