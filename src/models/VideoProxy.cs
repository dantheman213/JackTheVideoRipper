using Nager.PublicSuffix;

namespace JackTheVideoRipper.models;

public static class VideoProxy
{
    public const string PORNKAI = "pornkai";

    public static bool IsProxyLink(string url)
    {
        return IsProxyLink(FileSystem.ParseUrl(url));
    }
    
    public static bool IsProxyLink(DomainInfo? domainInfo)
    {
        if (domainInfo is null)
            return false;
        if (domainInfo.Domain is PORNKAI)
            return true;
        return false;
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
    None,
    Pornkai
}