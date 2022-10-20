using JackTheVideoRipper.models;

namespace JackTheVideoRipper
{
    internal static class AppUpdate
    {
        private const string assemblyInfoFileUrl = "https://raw.githubusercontent.com/dantheman213/JackTheVideoRipper/master/version";

        public static AppVersionModel? CheckForNewAppVersion()
        {
            AppVersionModel model = new();

            try
            {
                HttpClient client = new();
                HttpResponseMessage response = client.GetAsync(new Uri(assemblyInfoFileUrl)).Result;

                string content;
                
                if (response.IsSuccessStatusCode)
                {
                    content = response.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    Console.WriteLine(@$"Failed to download {(int)response.StatusCode} ({response.ReasonPhrase})");
                    return null;
                }

                string serverVersion = content.Replace("\n", "");
                model.Version = serverVersion;
                    
                // substring(1) used to removed "v" from versions to compare
                Version dstVersion = new(model.Version[1..]);
                Version localVersion = new(Common.GetAppVersion()[1..]);

                int result = dstVersion.CompareTo(localVersion);
                if (result > 0)
                {
                    // Version on the Internet is higher...
                    model.IsNewerVersionAvailable = true;
                }

                return model;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }

            return null;
        }
    }
}
