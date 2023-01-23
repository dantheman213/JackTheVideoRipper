using JackTheVideoRipper.extensions;
using Nager.PublicSuffix;

namespace JackTheVideoRipper.models;

public static class VideoProxy
{
    public static bool IsProxyLink(string url)
    {
        return IsProxyLink(FileSystem.ParseUrl(url));
    }
    
    public static bool IsProxyLink(DomainInfo? domainInfo)
    {
        if (domainInfo is null)
            return false;
        return false;
    }
    
    public static async Task<string> GetRedirectedLink(string parsedUrl, string originalUrl)
    {
        return GetProxyType(parsedUrl) switch
        {
            _ => originalUrl
        };
    }

    public static VideoProxyType GetProxyType(string? domain)
    {
        if (!Enum.TryParse(typeof(VideoProxyType), domain, true, out object? result)
            || result is not VideoProxyType proxyType)
            return VideoProxyType.None;

        return proxyType;
    }
}

public enum VideoProxyType
{
    None
}