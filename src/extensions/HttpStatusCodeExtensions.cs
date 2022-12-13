using System.Net;

namespace JackTheVideoRipper.extensions;

public static class HttpStatusCodeExtensions
{
    public static bool IsRedirect(this HttpStatusCode statusCode)
    {
        return statusCode is HttpStatusCode.Redirect
            or HttpStatusCode.MovedPermanently
            or HttpStatusCode.RedirectKeepVerb
            or HttpStatusCode.RedirectMethod;
    }
}