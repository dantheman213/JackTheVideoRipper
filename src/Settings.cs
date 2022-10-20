using static JackTheVideoRipper.SettingsModel;

namespace JackTheVideoRipper
{
    internal class Settings
    {
        public static SettingsModel Data { get; set; }

        public static void Save()
        {
            if (!Exists())
                return;
            Common.WriteJsonToFile(Filepath, Data);
        }

        public static void Load()
        {
            if (!Exists())
            {
                System.IO.Directory.CreateDirectory(SettingsModel.Directory);
                Common.WriteJsonToFile(Filepath, GenerateDefaultSettings());
            }

            Data = Common.GetObjectFromJsonFile<SettingsModel>(Filepath) ?? GenerateDefaultSettings();
        }
    }
}
