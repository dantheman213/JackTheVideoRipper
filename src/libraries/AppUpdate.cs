using JackTheVideoRipper.models;
using JackTheVideoRipper.extensions;

namespace JackTheVideoRipper
{
    internal static class AppUpdate
    {
        private const string assemblyInfoFileUrl =
            "https://raw.githubusercontent.com/dantheman213/JackTheVideoRipper/master/version";

        public static AppVersionModel CheckForNewAppVersion()
        {
            AppVersionModel model = new()
            {
                Version = GetVersionFromServer()?.Remove("\n") ?? string.Empty
            };

            // substring(1) used to removed "v" from versions to compare
            Version latestVersion = new(model.Version[1..]);

            // Version on the Internet is higher...
            model.IsNewerVersionAvailable = OutOfDate(latestVersion);

            return model;
        }

        private static bool OutOfDate(Version serverVersion)
        {
            Version localVersion = new(Common.GetAppVersion()[1..]);
            return serverVersion.CompareTo(localVersion) > 0;
        }

        private static string? GetVersionFromServer()
        {
            HttpClient client = new();

            HttpResponseMessage response;
            try
            {
                response = client.GetAsync(new Uri(assemblyInfoFileUrl)).Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }

            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsStringAsync().Result;
            }
            
            Console.WriteLine(@$"Failed to download {(int)response.StatusCode} ({response.ReasonPhrase})");
            return null;
        }
    }
}
