using System.Net;
using System.Text.RegularExpressions;
using JackTheVideoRipper.extensions;

namespace JackTheVideoRipper;

public static class Web
{
    public static async Task<HttpStatusCode> GetResourceStatus(string url)
    {
        return (await FileSystem.SimpleWebQueryAsync(url, completionOption:HttpCompletionOption.ResponseHeadersRead)).StatusCode;
    }
    
    private static async Task<string> GetRedirect(string newUrl)
    {
        HttpResponseMessage response = await FileSystem.SimpleWebQueryAsync(newUrl,
            completionOption:HttpCompletionOption.ResponseHeadersRead);
        
        switch (response.StatusCode)
        {
            default:
            case HttpStatusCode.OK:
                return newUrl;
            case HttpStatusCode.Gone:
            case HttpStatusCode.Redirect:
            case HttpStatusCode.MovedPermanently:
            case HttpStatusCode.RedirectKeepVerb:
            case HttpStatusCode.RedirectMethod:
            {
                if (response.Headers.Location is not { } location)
                    return newUrl;

                // Doesn't have a URL Schema, meaning it's a relative or absolute URL
                if (!location.IsAbsoluteUri)
                    location = new Uri(new Uri(newUrl), location);

                return location.AbsoluteUri;
            }
            case HttpStatusCode.Forbidden:
            case HttpStatusCode.NotFound:
                throw new RedirectFailedException($"Url {newUrl.WrapQuotes()} resulted in status code {response.ResponseCode()}");
        }
    }

    // https://stackoverflow.com/questions/704956/getting-the-redirected-url-from-the-original-url
    public static async Task<string> GetRedirectedUrl(string url, int maxRedirectCount = 8)
    {
        if(string.IsNullOrWhiteSpace(url))
            return url;

        string newUrl = url;
        do
        {
            try
            {
                newUrl = await GetRedirect(newUrl);
            }
            catch (WebException webException)
            {
                // Return the last known good URL
                FileSystem.LogException(webException);
                return newUrl;
            }
            catch (Exception exception)
            {
                throw new RedirectFailedException(innerException: exception);
            }
        } while (maxRedirectCount-- > 0);

        return newUrl;
    }

    public class RedirectFailedException : WebException
    {
        public RedirectFailedException(string message = "", Exception? innerException = null) : 
            base(message, innerException)
        {
        }
    }
    
    private static readonly Regex _GetTitlePattern =
        new(@"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase);

    public static string GetTitle(string html)
    {
        return _GetTitlePattern.Match(html).Groups["Title"].Value;
    }
}