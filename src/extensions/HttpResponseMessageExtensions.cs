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
            response.Content.CopyToAsync(fileStream).Wait();
        }
        catch (Exception e)
        {
            throw new WebException("Failed to write web response to disk", e);
        }

        return downloadPath;
    }

    public static string GetResponse(this HttpResponseMessage response)
    {
        return response.Content.ReadAsStringAsync().Result;
    }
}