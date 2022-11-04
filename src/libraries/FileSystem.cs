using System.Diagnostics;
using System.Management;
using System.Reflection;
using System.Security.Principal;
using System.Text.RegularExpressions;
using JackTheVideoRipper.extensions;
using JackTheVideoRipper.Properties;
using Newtonsoft.Json;
using SpecialFolder = System.Environment.SpecialFolder;

namespace JackTheVideoRipper;

public static class FileSystem
{
    public const string PROGRAM_NAME = "JackTheVideoRipper";
    
    public const string ALL_FILES_FILTER = "All files (*.*)|*.*";
    
    private static readonly Regex _FilenamePattern = new("[^a-zA-Z0-9 -]", RegexOptions.Compiled);
    
    private static readonly Regex _UrlPattern = new(@"^(http|http(s)?://)?([\w-]+\.)+[\w-]+[.com|.in|.org|.net]+(\[\?%&=]*)?",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);
    
    public static string VersionInfo => FileVersionInfo.GetVersionInfo(ExecutingAssembly.Location).FileVersion ?? string.Empty;
    
    public static string TimeStampDate => $"{DateTime.Now:yyyyMMddhmmsstt}";

    public static readonly string TempPath = Path.GetTempPath();

    public static string TempFile => Path.GetTempFileName();

    private const string _PROGRAM_PREFIX = "jtvr";
    
    private const string _TASK_MANAGER_EXECUTABLE = "taskmgr.exe";
    
    private const string _EXPLORER_EXECUTABLE = "explorer.exe";

    private static string GetProcessQuery(int pid) => @$"Select * From Win32_Process Where ParentProcessID={pid}";

    public static Assembly ExecutingAssembly => Assembly.GetExecutingAssembly();
    
    public static WindowsIdentity CurrentUser => WindowsIdentity.GetCurrent();

    public static WindowsPrincipal CurrentUserPrincipal => new(CurrentUser);
    
    public static ProcessModule? MainModule => Process.GetCurrentProcess().MainModule;

    public static readonly string PathSeparator = Path.DirectorySeparatorChar.ToString();
    
    public static string GetSpecialFolderPath(SpecialFolder specialFolder)
    {
        return Environment.GetFolderPath(specialFolder);
    }

    public static string MergePaths(params string[] parts)
    {
        return Path.Combine(parts);
    }
    
    public static string MergePaths(string root, string child)
    {
        return Path.Combine(root, child);
    }
    
    public static class Paths
    {
        public static readonly string AppPath = Path.GetDirectoryName(MainModule?.FileName) ?? string.Empty;
        public static readonly string Local = GetSpecialFolderPath(SpecialFolder.LocalApplicationData);
        public static readonly string Common = GetSpecialFolderPath(SpecialFolder.CommonApplicationData);
        public static readonly string Root = MergePaths(Common, PROGRAM_NAME);
        public static readonly string Install = MergePaths(Root, "bin");
        public static readonly string Settings = MergePaths(Local, PROGRAM_NAME);
        public static readonly string UserProfile = Environment.ExpandEnvironmentVariables("%userprofile%");
        public static readonly string Download = Path.Combine(UserProfile, "Downloads");
    }
    
    public static void ValidateInstallDirectory()
    {
        CreateFolderIfNotExists(Paths.Install);
    }

    public static void CreateFolderIfNotExists(string directory)
    {
        if (!Directory.Exists(directory))
            CreateFolder(directory);
    }

    public static string ProgramPath(string executablePath)
    {
        return MergePaths(Paths.Install, executablePath);
    }
    
    public static Process? GetWebResourceHandle(string url, bool useShellExecute = true)
    {
        return Process.Start(new ProcessStartInfo(url) { UseShellExecute = useShellExecute });
    }

    public static void WriteJsonToFile(string filepath, object obj)
    {
        File.WriteAllText(filepath, JsonConvert.SerializeObject(obj));
    }

    public static T? Deserialize<T>(string obj)
    {
        return JsonConvert.DeserializeObject<T>(obj);
    }

    public static T? GetObjectFromJsonFile<T>(string url)
    {
        return JsonConvert.DeserializeObject<T>(File.ReadAllText(url));
    }
    
    public static Process CreateProcess(string bin, string parameters, string workingDir = "", bool runAsAdmin = false)
    {
        if (bin.IsNullOrEmpty())
            return new Process();
            
        return new Process
        {
            StartInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = bin,
                Arguments = parameters,
                WorkingDirectory = workingDir.ValueOrDefault(Paths.AppPath),
                UseShellExecute = runAsAdmin,
                RedirectStandardError = !runAsAdmin,
                RedirectStandardOutput = !runAsAdmin,
                CreateNoWindow = true,
                Verb = runAsAdmin ? "runas" : string.Empty
            }
        };
    }
    
    public static string RunCommand(string binPath, string paramString, string workingDir = "", bool runAsAdmin = false)
    {
        return RunProcess(CreateProcess(binPath, paramString, workingDir, runAsAdmin));
    }

    public static string RunWebCommand(string binPath, string url, string parameterString)
    {
        return RunCommand(binPath, $"{parameterString} {url}");
    }
    
    public static T? ReceiveJsonResponse<T>(string binPath, string url, string parameterString)
    {
        return Deserialize<T>(RunWebCommand(binPath, url, parameterString));
    }
        
    // youtube-dl returns an individual json object per line
    public static IEnumerable<T> ReceiveMultiJsonResponse<T>(string binPath, string url, string parameterString)
    {
        return Deserialize<IEnumerable<T>>($"[{RunWebCommand(binPath, url, parameterString).Replace("\n", ",\n")}]") ?? Array.Empty<T>();
    }

    public static Process? OpenFileExplorer(string directory)
    {
        return Process.Start(new ProcessStartInfo
        {
            Arguments = directory,
            FileName = _EXPLORER_EXECUTABLE
        });
    }

    public static Process? OpenTaskManager()
    {
        return Process.Start(new ProcessStartInfo
        {
            CreateNoWindow = false,
            UseShellExecute = true,
            FileName = MergePaths(Environment.SystemDirectory, _TASK_MANAGER_EXECUTABLE),
            Arguments = string.Empty
        });
    }

    public static string DownloadTempFile(string url, string extension)
    {
        return DownloadWebFile(url, GetTempFilename(extension));
    }

    public static string DownloadWebFile(string url, string localPath)
    {
        DeleteIfExists(localPath);

        HttpResponseMessage response = SimpleWebQuery(url);

        if (response.IsSuccessStatusCode)
            return !LogExceptions(() => response.DownloadResponse(localPath)) ? string.Empty : localPath;
        
        Log(@$"Failed to download {response.ResponseCode()}");
        return string.Empty;

    }

    public static HttpResponseMessage SimpleWebQuery(string url)
    {
        using HttpClient client = new();
        return client.GetAsync(new Uri(url)).Result;
    }

    public static void DeleteIfExists(string filepath)
    {
        if (File.Exists(filepath))
            File.Delete(filepath);
    }

    public static void CreateFolder(string path)
    {
        Directory.CreateDirectory(path);
    }
    
    public static void OpenFolderWithFileSelect(string filePath)
    {
        Process.Start(_EXPLORER_EXECUTABLE, $"/select, \"{filePath}\"");
    }
    
    public static void OpenDownloads()
    {
        OpenFolder(Settings.Data.DefaultDownloadPath);
    }
    
    public static void OpenFolder(string? folderPath)
    {
        if (folderPath.HasValue() && Directory.Exists(folderPath))
        {
            OpenFileExplorer(folderPath);
        }
        else
        {
            Modals.Warning($@"{folderPath} Directory does not exist!");
        }
    }

    public static void OpenFile(string filepath)
    {
        if (!filepath.HasValue())
        {
            // couldn't find folder, rolling back to just the folder with no select
            Log($@"Couldn't find file to open at {filepath}");
            OpenDownloads();
            return;
        }
        
        if (File.Exists(filepath))
        {
            OpenFolderWithFileSelect(filepath);
        }
        else if (File.Exists($"{filepath}.part"))
        {
            OpenFolderWithFileSelect($"{filepath}.part");
        }
    }

    public static string GetFilename(string filepath)
    {
        return filepath.Contains('.') ? filepath.BeforeLast(".") : filepath;
    }
    
    public static string GetExtension(string path)
    {
        return path.Contains('.') ? path.AfterLast(".") : path;
    }

    public static string GetDirectory(string path)
    {
        return path.Contains(Path.DirectorySeparatorChar) ? path.BeforeLast(PathSeparator) : path;
    }

    public static string GetFileFilter(string extension)
    {
        return $@"{extension} file|*.{extension}";
    }

    public static string GetFileFilterWithAll(string extension)
    {
        return $@"{extension} file|*.{extension}|{ALL_FILES_FILTER}";
    }

    public static string? SaveCopy(string filepath)
    {
        SaveFileDialog saveFileDialog = new()
        {
            InitialDirectory = GetDirectory(filepath), // FrameMain.settings.defaultDownloadPath;      
            FileName = GetFilename(filepath),
            Filter = GetFileFilterWithAll(GetExtension(filepath))
        };

        return saveFileDialog.ShowDialog() == DialogResult.OK ? saveFileDialog.FileName : null;
    }

    public static string GetDownloadPath(string filename)
    {
        return MergePaths(Settings.Data.DefaultDownloadPath, ValidateFilename(filename));
    }
    
    public static string ValidateFilename(string filepath)
    {
        return $@"{SanitizeFilename(GetFilename(filepath))}.{GetExtension(filepath)}";
    }

    public static string SanitizeFilename(string filename)
    {
        return _FilenamePattern.Replace(filename, "_").Replace(' ', '_');
    }

    public static string GetClipboardText()
    {
        return Clipboard.GetText().Trim();
    }

    public static void SetClipboardText(string content)
    {
        Clipboard.SetText(content);
    }

    public static string? GetFileUsingDialog(string? initialDirectory = null, string filter = ALL_FILES_FILTER)
    {
        initialDirectory ??= Settings.Data.DefaultDownloadPath;
        
        OpenFileDialog openFileDialog = new()
        {
            InitialDirectory = initialDirectory,
            Filter = filter
        };

        if (openFileDialog.ShowDialog() != DialogResult.OK || !File.Exists(openFileDialog.FileName))
            return null;

        return File.ReadAllText(openFileDialog.FileName);
    }
    public static string TryRunProcess(Process process)
    {
        return TryStartProcess(process).StandardOutput.ReadToEnd().Trim();
    }
    
    public static string RunProcess(Process process)
    {
        process.Start();
        return process.StandardOutput.ReadToEnd().Trim();
    }

    public static Process TryStartProcess(Process process)
    {
        LogExceptions(() =>
        {
            if (!process.Start()) { Log(@"WARNING: Process already running!"); }
        });
       
        return process;
    }

    public static string GetTempFilename(string ext)
    {
        return MergePaths(TempPath, $"{_PROGRAM_PREFIX}_thumbnail_{TimeStampDate}.{ext}");
    }

    public static bool UserIsAdmin()
    {
        return CurrentUserPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    private static ManagementObjectSearcher? GetProcessSearcher(int pid)
    {
        ManagementObjectSearcher? result = null;
        LogExceptions(() => result = new ManagementObjectSearcher(GetProcessQuery(pid)));
        return result;
    }

    public static void TryKillProcessAndChildren(int pid)
    {
        if (GetProcessSearcher(pid) is not { } process)
            return;
            
        foreach (ManagementObject manager in process.Get().Cast<ManagementObject>())
        {
            LogExceptions(() => TryKillProcessAndChildren(GetProcessId(manager)));
        }

        TryKillProcess(pid);
    }

    public static int GetProcessId(ManagementObject manager)
    {
        return Convert.ToInt32(manager["ProcessID"]);
    }

    private static void TryKillProcess(int pid)
    {
        if (Process.GetProcessById(pid) is not { HasExited: true } process )
            return;

        LogExceptions(() => process.Kill());
    }

    private static bool LogExceptions(Action action)
    {
        try
        {
            action();
            return true;
        }
        catch (Exception ex)
        {
            LogException(ex);
            return false;
        }
    }

    private static void LogException(Exception exception)
    {
        Console.WriteLine(exception);
    }

    private static void Log(string message)
    {
        Console.WriteLine(message);
    }

    public static bool IsValidUrl(string url)
    {
        return _UrlPattern.IsMatch(url);
    }

    public class FileSystemException : Exception { }

    public static bool WarnIfFileExists(string filepath)
    {
        return !File.Exists(filepath) || Modals.Confirmation(Resources.OverwriteFile, "Warning: File Already Exists");
    }

    public static string? SelectFile(string initialPath = "")
    {
        FolderBrowserDialog folderBrowserDialog = new();
        
        if (initialPath.HasValue())
            folderBrowserDialog.SelectedPath = initialPath;
        
        return folderBrowserDialog.ShowDialog() == DialogResult.OK ? folderBrowserDialog.SelectedPath : null;
    }
}