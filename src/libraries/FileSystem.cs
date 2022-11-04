using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using JackTheVideoRipper.extensions;
using Newtonsoft.Json;

namespace JackTheVideoRipper;

public static class FileSystem
{
    public static void ValidateInstallDirectory()
    {
        if (!Directory.Exists(Common.Paths.Install))
            Directory.CreateDirectory(Common.Paths.Install);
    }

    public static string ProgramPath(string executablePath)
    {
        return Path.Combine(Common.Paths.Install, executablePath);
    }
    
    public static Process? GetWebResourceHandle(string url, bool useShellExecute = true)
    {
        return Process.Start(new ProcessStartInfo(url) {UseShellExecute = useShellExecute});
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
                WorkingDirectory = workingDir.HasValue() ? workingDir : Common.Paths.AppPath,
                UseShellExecute = runAsAdmin,
                RedirectStandardError = !runAsAdmin,
                RedirectStandardOutput = !runAsAdmin,
                CreateNoWindow = true,
                Verb = runAsAdmin ? "runas" : ""
            }
        };
    }
    
    public static string RunCommand(string binPath, string paramString, string workingDir = "", bool runAsAdmin = false)
    {
        return RunProcess(CreateProcess(binPath, paramString, workingDir, runAsAdmin));
    }
    
    public static T? ReceiveJsonResponse<T>(string binPath, string url, string parameterString)
    {
        return Deserialize<T>(RunCommand(binPath, $"{parameterString} {url}"));
    }
        
    public static T? ReceiveMultiJsonResponse<T>(string binPath, string url, string parameterString)
    {
        return Deserialize<T>($"[{RunCommand(binPath, $"{parameterString} {url}").Split("\n").Merge("\n")}]");
    }

    public static Process? OpenFileExplorer(string directory)
    {
        ProcessStartInfo startInfo = new()
        {
            Arguments = directory,
            FileName = "explorer.exe"
        };

        return Process.Start(startInfo);
    }

    public static Process? OpenTaskManager()
    {
        ProcessStartInfo startInfo = new()
        {
            CreateNoWindow = false, //just hides the window if set to true
            UseShellExecute = true, //use shell (current programs privillage)
            FileName = Path.Combine(Environment.SystemDirectory, "taskmgr.exe"), //The file path and file name
            Arguments = string.Empty //Add your arguments here
        }; //a processstartinfo object

        return Process.Start(startInfo);
    }

    public static void DownloadFile(string url, string localPath)
    {
        if (File.Exists(localPath))
        {
            File.Delete(localPath);
        }

        HttpClient client = new();
        HttpResponseMessage response = client.GetAsync(new Uri(url)).Result;

        if (response.IsSuccessStatusCode)
        {
            using FileStream fileStream = new(localPath, FileMode.CreateNew);
            response.Content.CopyToAsync(fileStream).Wait();
        }
        else
        {
            Console.WriteLine(@$"Failed to download {(int) response.StatusCode} ({response.ReasonPhrase})");
        }
    }

    public static void CreateFolder(string path)
    {
        Directory.CreateDirectory(path);
    }
    
    public static void OpenFolderWithFileSelect(string filePath)
    {
        Process.Start("explorer.exe", $"/select, \"{filePath}\"");
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
        if (filepath.HasValue())
        {
            // TODO: fix file pathing issue
            if (File.Exists(filepath))
            {
                OpenFolderWithFileSelect(filepath);
            }
            else if (File.Exists($"{filepath}.part"))
            {
                OpenFolderWithFileSelect($"{filepath}.part");
            }
            return;
        }

        // couldn't find folder, rolling back to just the folder with no select
        Console.WriteLine($@"Couldn't find file to open at {filepath}");
        OpenDownloads();
    }
    
    public static readonly string PathSeparator = Path.DirectorySeparatorChar.ToString();
    
    public static string GetFilename(string filepath)
    {
        return filepath.Contains('.') ? filepath.BeforeLast(".") : filepath;
    }

    public static string GetDirectory(string path)
    {
        return path.Contains(Path.DirectorySeparatorChar) ? path.BeforeLast(PathSeparator) : path;
    }
    
    public static string GetExtension(string path)
    {
        return path.Contains('.') ? path.AfterLast(".") : path;
    }
    
    public static string GetFileFilter(string extension)
    {
        return $@"{extension} file|*.{extension}";
    }

    public static string GetFileFilterWithAll(string extension)
    {
        return $@"{extension} file|*.{extension}|{AllFilesFilter}";
    }

    public const string AllFilesFilter = "All files (*.*)|*.*";

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

    public static string? GetDownloadPath(string filename)
    {
        return Settings.Data.DefaultDownloadPath is not null ? 
            Path.Combine(Settings.Data.DefaultDownloadPath, filename) :
            null;
    }
    
    public static string ValidateFilename(string filepath)
    {
        return $@"{SanitizeFilename(GetFilename(filepath))}.{GetExtension(filepath)}";
    }
    
    private static readonly Regex FilenamePattern = new("[^a-zA-Z0-9 -]", RegexOptions.Compiled);

    public static string SanitizeFilename(string filename)
    {
        return FilenamePattern.Replace(filename, "_").Replace(' ', '_');

        // return str.Split(Path.GetInvalidFileNameChars()).Merge("_");
    }

    public static string GetClipboardData()
    {
        return Clipboard.GetText().Trim();
    }

    public static void SetClipboardData(string content)
    {
        Clipboard.SetText(content);
    }

    public static string VersionInfo =>
        FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion ?? string.Empty;

    public static string GetFileUsingDialog(string? initialDirectory = null, string filter = AllFilesFilter)
    {
        initialDirectory ??= Settings.Data.DefaultDownloadPath;
        
        OpenFileDialog openFileDialog = new()
        {
            InitialDirectory = initialDirectory,
            Filter = filter
        };

        if (openFileDialog.ShowDialog() != DialogResult.OK || !File.Exists(openFileDialog.FileName))
            return string.Empty;

        return File.ReadAllText(openFileDialog.FileName);
    }
    public static string TryRunProcess(Process process)
    {
        try
        {
            process.Start();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        return process.StandardOutput.ReadToEnd().Trim();
    }
    

    public static string RunProcess(Process process)
    {
        process.Start();
        return process.StandardOutput.ReadToEnd().Trim();
    }

    public static Process TryStartProcess(Process process)
    {
        try
        {
            process.Start();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
       
        return process;
    }
}