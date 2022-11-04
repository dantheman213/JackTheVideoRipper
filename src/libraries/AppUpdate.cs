using JackTheVideoRipper.models;
using JackTheVideoRipper.Properties;

namespace JackTheVideoRipper
{
    internal static class AppUpdate
    {
        public static void CheckForNewAppVersion(bool isStartup = true)
        {
            // if sender obj is bool then version being checked on startup passively and dont show dialog that it's up to date
            AppVersionModel result = AppVersionModel.GetFromServer();
            switch (result is {IsNewerVersionAvailable: true})
            {
                case true when Modals.Update(result):
                    FileSystem.GetWebResourceHandle(URLs.UPDATE);
                    break;
                case false when !isStartup:
                    Core.SendNotification(Resources.UpToDate);
                    break;
            }
        }
    }
}
