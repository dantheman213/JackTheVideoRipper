using static JackTheVideoRipper.SettingsModel;
using static JackTheVideoRipper.FileSystem;

namespace JackTheVideoRipper
{
    internal static class Settings
    {
        public static SettingsModel Data { get; private set; }

        public static void Save()
        {
            if (!Exists())
                return;
            WriteJsonToFile(Filepath, Data);
        }

        public static void Load()
        {
            if (!Exists())
            {
                CreateFolder(SettingsModel.Directory);
                WriteJsonToFile(Filepath, GenerateDefaultSettings());
            }

            Data = GetObjectFromJsonFile<SettingsModel>(Filepath) ?? GenerateDefaultSettings();
        }
    }
}
