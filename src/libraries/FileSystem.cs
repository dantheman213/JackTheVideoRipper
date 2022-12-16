using System.Diagnostics;
using System.Management;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using System.Text.RegularExpressions;
using JackTheVideoRipper.extensions;
using JackTheVideoRipper.interfaces;
using JackTheVideoRipper.Properties;
using Nager.PublicSuffix;
using Newtonsoft.Json;
using SpecialFolder = System.Environment.SpecialFolder;

namespace JackTheVideoRipper;

public static class FileSystem
{
    #region Data Members
    
    public const string PROGRAM_NAME = "JackTheVideoRipper";
    
    private const string _PROGRAM_PREFIX = "jtvr";
    
    private const string _TASK_MANAGER_EXECUTABLE = "taskmgr.exe";
    
    private const string _EXPLORER_EXECUTABLE = "explorer.exe";

    public static class Filters
    {
        public static readonly string AllFiles = FileSystemResources.Filter_AllFiles;
        public static readonly string AllMedia = FileSystemResources.Filter_AllMediaFiles;
        public static readonly string VideoFiles = FileSystemResources.Filter_VideoFiles;
        public static readonly string AudioFiles = FileSystemResources.Filter_AudioFiles;
        public static readonly string ImageFiles = FileSystemResources.Filter_ImageFiles;
    }
    
    private static readonly Regex _FilenamePattern = new("[^a-zA-Z0-9 -]", RegexOptions.Compiled);
    
    private static readonly Regex _UrlPattern = new(@"^(http|http(s)?://)?([\w-]+\.)+[\w-]+[.com|.in|.org|.net]+(\[\?%&=]*)?",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static readonly string TempPath = Path.GetTempPath();

    public static readonly string PathSeparator = Path.DirectorySeparatorChar.ToString();

    public static class Paths
    {
        public static readonly string AppPath = Path.GetDirectoryName(MainModule?.FileName).ValueOrDefault();
        public static readonly string Local = GetSpecialFolderPath(SpecialFolder.LocalApplicationData);
        public static readonly string Common = GetSpecialFolderPath(SpecialFolder.CommonApplicationData);
        public static readonly string Root = MergePaths(Common, PROGRAM_NAME);
        public static readonly string Install = MergePaths(Root, "bin");
        public static readonly string Settings = MergePaths(Local, PROGRAM_NAME);
        public static readonly string UserProfile = Environment.ExpandEnvironmentVariables("%userprofile%");
        public static readonly string Download = Path.Combine(UserProfile, "Downloads");
    }
    
    #endregion

    #region Attributes

    public static string TempFile => Path.GetTempFileName();
    
    public static Version OSVersion => Environment.OSVersion.Version;

    public static FileVersionInfo FileVersionInfo => FileVersionInfo.GetVersionInfo(ExecutingAssembly.Location);

    public static string VersionInfo => FileVersionInfo.FileVersion.ValueOrDefault();
    
    public static string TimeStampDate => $"{DateTime.Now:yyyyMMddhmmsstt}";

    private static string GetProcessQuery(int pid) => string.Format(FileSystemResources.SelectProcessById, pid);

    public static Assembly ExecutingAssembly => Assembly.GetExecutingAssembly();
    
    public static WindowsIdentity CurrentUser => WindowsIdentity.GetCurrent();

    public static WindowsPrincipal CurrentUserPrincipal => new(CurrentUser);
    
    public static ProcessModule? MainModule => Process.GetCurrentProcess().MainModule;

    #endregion

    #region Directory Methods

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

    public static void ValidateInstallDirectory()
    {
        CreateFolderIfNotExists(Paths.Install);
    }
    
    public static void CreateFolder(string path)
    {
        Directory.CreateDirectory(path);
    }
    
    public static void CreateFolder(params string[] pathHierarchy)
    {
        CreateFolder(MergePaths(pathHierarchy));
    }

    public static async Task OpenDownloads()
    {
        await OpenFolder(Settings.Data.DefaultDownloadPath);
    }
    
    public static async Task OpenFolder(string? folderPath)
    {
        if (folderPath.HasValue() && Directory.Exists(folderPath))
        {
            await OpenFileExplorer(folderPath);
        }
        else
        {
            Modals.Warning(string.Format(Resources.DirectoryDoesNotExist, folderPath?.WrapQuotes()));
        }
    }

    public static void CreateFolderIfNotExists(string directory)
    {
        if (!Directory.Exists(directory))
            CreateFolder(directory);
    }

    public static void CreateFolderIfNotExists(params string[] directories)
    {
        while (true)
        {
            if (directories.Length == 0)
                return;

            CreateFolder(directories);

            directories = directories.Skip(1).ToArray();
        }
    }

    public static void CreatePathIfNoneExists(string filepath)
    {
        CreateFolderIfNotExists(Directory.GetDirectories(filepath));
    }

    public static string ProgramPath(string executablePath)
    {
        return MergePaths(Paths.Install, executablePath);
    }
    
    public static string GetDownloadPath(string filename)
    {
        return MergePaths(Settings.Data.DefaultDownloadPath, ValidateFilename(filename));
    }
    
    private static void ValidatePath(string filepath, bool deleteFileIfExists = false)
    {
        CreatePathIfNoneExists(filepath);
        if (deleteFileIfExists)
            DeleteFileIfExists(filepath);
    }

    #endregion

    #region Process Methods

    public static Process? GetWebResourceHandle(string url, bool useShellExecute = true)
    {
        return Process.Start(new ProcessStartInfo(url) { UseShellExecute = useShellExecute });
    }

    public static Process CreateProcess(ProcessStartInfo processStartInfo, bool enableRaisingEvents = false)
    {
        return new Process
        {
            StartInfo = processStartInfo,
            EnableRaisingEvents = enableRaisingEvents
        };
    }

    public static Process CreateProcess(string bin, string parameters, string workingDir = "", bool runAsAdmin = false, 
        bool executeShell = false, bool redirect = true, bool enableRaisingEvents = false)
    {
        if (bin.IsNullOrEmpty())
            throw new FileSystemException("Could not create process with empty bin path!");

        return CreateProcess(new ProcessStartInfo
        {
            WindowStyle = ProcessWindowStyle.Hidden,
            FileName = bin,
            Arguments = parameters,
            WorkingDirectory = workingDir.ValueOrDefault(Paths.AppPath),
            UseShellExecute = executeShell,
            RedirectStandardError = redirect,
            RedirectStandardOutput = redirect,
            CreateNoWindow = true,
            Verb = runAsAdmin ? "runas" : string.Empty
        }, enableRaisingEvents);
    }
    
    public static string TryRunProcess(Process process)
    {
        return TryStartProcess(process).StandardOutput.ReadToEnd().Trim();
    }
    
    public static string RunProcess(Process process)
    {
        process.Start();
        return process.GetOutput();
    }
    
    public static string RunProcess(string path, string arguments)
    {
        return RunProcess(CreateProcess(path, arguments));
    }
    
    public static string RunProcess(string path, IProcessParameters parameters)
    {
        return RunProcess(path, parameters.ToString());
    }
    
    public static async Task<int> RunProcessAsync(string filepath, string args)
    {
        return await RunProcessAsync(CreateProcess(filepath, args, enableRaisingEvents: true));
    }
    
    public static async Task<int> RunProcessAsync(ProcessStartInfo processStartInfo)
    {
        return await RunProcessAsync(CreateProcess(processStartInfo, enableRaisingEvents: true));
    }
    
    public static async Task<int> RunProcessAsync(Process process)
    {
        return await CreateProcessAsync(process, true).ConfigureAwait(false);
    }
    
    private static Task<int> CreateProcessAsync(Process process, bool startProcess = false, bool logOutput = false, 
        bool throwExceptions = true)
    {
        TaskCompletionSource<int> taskCompletionSource = new();

        process.Exited += (_, _) => taskCompletionSource.SetResult(process.ExitCode);

        if (logOutput)
        {
            process.OutputDataReceived += (_, args) => Log(args.Data);
            process.ErrorDataReceived += (_, args) => LogException(args.Data);
        }

        if (startProcess && !process.Start())
        {
            //you may allow for the process to be re-used (started = false) 
            //but I'm not sure about the guarantees of the Exited event in such a case
            InvalidOperationException exception = new(string.Format(Resources.CouldNotStartProcess, process));
            if (throwExceptions)
                throw exception;
            if (logOutput)
                LogException(exception);
        }

        if (logOutput)
        {
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }

        return taskCompletionSource.Task;
    }

    public static Process TryStartProcess(ProcessStartInfo processStartInfo)
    {
        return TryStartProcess(CreateProcess(processStartInfo));
    }

    public static Process TryStartProcess(Process process)
    {
        LogExceptions(() => { if (!process.Start()) { Log(Resources.ProcessAlreadyRunning); } });
        return process;
    }
    
    public static async Task<Process> TryStartProcessAsync(ProcessStartInfo processStartInfo)
    {
        return await TryStartProcessAsync(CreateProcess(processStartInfo));
    }

    public static async Task<Process> TryStartProcessAsync(Process process)
    {
        await LogExceptionsAsync(async () =>
        {
            if (await CreateProcessAsync(process, true) == 0)
            {
                Log(Resources.ProcessAlreadyRunning);
            }
        });
        return process;
    }

    #endregion

    #region Command Methods

    public static string RunCommand(string binPath, string paramString, string workingDir = "", bool runAsAdmin = false)
    {
        return RunProcess(CreateProcess(binPath, paramString, workingDir, runAsAdmin));
    }
    
    public static async Task<string> RunCommandAsync(string binPath, string paramString, string workingDir = "", 
        bool runAsAdmin = false)
    {
        Process process = CreateProcess(binPath, paramString, workingDir, runAsAdmin);
        await CreateProcessAsync(process, true).ConfigureAwait(false);
        return process.GetOutput();
    }

    public static string RunWebCommand(string binPath, string url, string parameterString)
    {
        return url.Valid(IsValidUrl) ? RunCommand(binPath, $"{parameterString} {url}") : string.Empty;
    }
    
    public static async Task<string> RunWebCommandAsync(string binPath, string url, string parameterString)
    {
        return url.Valid(IsValidUrl) ? await RunCommandAsync(binPath, $"{parameterString} {url}") : string.Empty;
    }

    #endregion

    #region JSON Methods

    public static void WriteJsonToFile(string filepath, object? obj)
    {
        File.WriteAllText(filepath, JsonConvert.SerializeObject(obj));
    }
    
    public static void SerializeAndDownload(object? obj)
    {
        WriteJsonToFile(Settings.Data.DefaultDownloadPath, obj);
    }

    public static T? Deserialize<T>(string obj)
    {
        return JsonConvert.DeserializeObject<T>(obj);
    }

    public static T? GetObjectFromJsonFile<T>(string filepath)
    {
        return JsonConvert.DeserializeObject<T>(File.ReadAllText(filepath));
    }
    
    public static T? ReceiveJsonResponse<T>(string binPath, string url, string parameterString)
    {
        return url.Valid(IsValidUrl) ? Deserialize<T>(RunWebCommand(binPath, url, parameterString)) : default;
    }
    
    public static async Task<T?> ReceiveJsonResponseAsync<T>(string binPath, string url, string parameterString)
    {
        return url.Valid(IsValidUrl) ? Deserialize<T>(await RunWebCommandAsync(binPath, url, parameterString)) : default;
    }
        
    // youtube-dl returns an individual json object per line
    public static IEnumerable<T> ReceiveMultiJsonResponse<T>(string binPath, string url, string parameterString)
    {
        return url.Valid(IsValidUrl) ? CreateJsonArray<T>(RunWebCommand(binPath, url, parameterString)) : Array.Empty<T>();
    }
    
    public static async Task<IEnumerable<T>> ReceiveMultiJsonResponseAsync<T>(string binPath, string url, string parameterString)
    {
        return url.Valid(IsValidUrl) ? CreateJsonArray<T>(await RunWebCommandAsync(binPath, url, parameterString)) : Array.Empty<T>();
    }

    private static IEnumerable<T> CreateJsonArray<T>(string jsonResponse)
    {
        return jsonResponse.HasValue()
            ? Deserialize<IEnumerable<T>>($"[{jsonResponse.Replace("\n", ",\n")}]") ?? Array.Empty<T>()
            : Array.Empty<T>();
    }

    #endregion

    #region Program Methods

    public static async Task OpenFileExplorer(string directory)
    {
        await RunProcessAsync(new ProcessStartInfo
        {
            Arguments = directory,
            FileName = _EXPLORER_EXECUTABLE
        });
    }

    public static async Task OpenTaskManager()
    {
        await TryStartProcessAsync(new ProcessStartInfo
        {
            CreateNoWindow = false,
            UseShellExecute = true,
            FileName = MergePaths(Environment.SystemDirectory, _TASK_MANAGER_EXECUTABLE),
            Arguments = string.Empty
        });
    }
    
    public static async Task OpenFileExplorerWithFileSelected(string filePath)
    {
        await OpenFileExplorer($"/select, {filePath.WrapQuotes()}");
    }
    
    public static async Task<bool> InstallProgram(string downloadUrl, string filename)
    {
        return downloadUrl.Valid(IsValidUrl) && 
               (await DownloadWebFileAsync(downloadUrl, Path.Join(Paths.Install, filename))).HasValue();
    }

    #endregion

    #region Web Methods
    
    private static readonly DomainParser _DomainParser = new(new WebTldRuleProvider());

    public static DomainInfo? ParseUrl(string? url)
    {
        return _DomainParser.Parse(url);
    }

    public static bool IsValidUrl(string url)
    {
        return _UrlPattern.IsMatch(url);
    }

    public static string DownloadTempFile(string url, string extension)
    {
        return url.Valid(IsValidUrl) ? DownloadWebFile(url, GetTempFilename(extension)) : string.Empty;
    }

    public static string DownloadWebFile(string url, string localPath)
    {
        if (url.Invalid(IsValidUrl))
            return string.Empty;
        
        ValidatePath(localPath, true);

        HttpResponseMessage response = SimpleWebQuery(url);

        return HandleResponse(response, localPath);
    }
    
    public static async Task<string> DownloadWebFileAsync(string url, string localPath)
    {
        if (url.Invalid(IsValidUrl))
            return string.Empty;

        ValidatePath(localPath, true);

        HttpResponseMessage response = await SimpleWebQueryAsync(url);

        return HandleResponse(response, localPath);
    }

    private static string HandleResponse(HttpResponseMessage response, string localPath)
    {
        if (response.IsSuccessStatusCode)
            return !LogExceptions(() => response.DownloadResponse(localPath)) ? string.Empty : localPath;
        Log(string.Format(Resources.FailedToDownload, response.ResponseCode()));
        return string.Empty;
    }

    public static HttpResponseMessage SimpleWebQuery(string url, HttpClientHandler? handler = null,
        HttpCompletionOption? completionOption = null)
    {
        var task = GetResponse(url, handler, completionOption);
        task.RunSynchronously();
        return task.Result;
    }
    
    public static async Task<HttpResponseMessage> SimpleWebQueryAsync(string url, HttpClientHandler? handler = null,
        HttpCompletionOption? completionOption = null)
    {
        return await GetResponse(url, handler, completionOption);
    }

    private static async Task<HttpResponseMessage> GetResponse(string url, HttpMessageHandler? handler = null, 
        HttpCompletionOption? completionOption = null)
    {
        if (url.Invalid(IsValidUrl))
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        using HttpClient client = CreateClient(handler);
        return completionOption is null ? await client.GetAsync(url) : 
            await client.GetAsync(url, (HttpCompletionOption) completionOption);
    }

    private static HttpClient CreateClient(HttpMessageHandler? handler = null)
    {
        return handler == null ? new HttpClient() : new HttpClient(handler);
    }

    #endregion

    #region File Methods

    public static Mutex CreateSingleInstanceLock(out bool isOnlyInstance)
    {
       return new Mutex(true, PROGRAM_NAME, out isOnlyInstance);
    }

    public static bool WarnIfFileExists(string filepath)
    {
        return !File.Exists(filepath) ||
               Modals.Confirmation(Resources.OverwriteFile, Properties.Captions.FileAlreadyExists);
    }
    
    public static void SaveToFile(string filepath, string content)
    {
        File.WriteAllText(filepath, content);
    }

    public static string GetTempFilename(string extension, string? prefix = null)
    {
        return MergePaths(TempPath, 
            $"{_PROGRAM_PREFIX}_{(prefix is null ? $"{prefix}_" : string.Empty)}{TimeStampDate}.{extension}");
    }
    
    public static async Task OpenFile(string filepath, bool openDownloadsIfNull)
    {
        if (filepath.HasValue() && File.Exists(filepath))
        {
            await OpenFileExplorerWithFileSelected(filepath);
        }
        else
        {
            // couldn't find folder, rolling back to just the folder with no select
            Log(string.Format(Resources.CouldNotOpenFile, filepath.WrapQuotes()));
            if (openDownloadsIfNull)
                await OpenDownloads();
        }
    }
    
    public static string? SaveCopy(string filepath)
    {
        SaveFileDialog saveFileDialog = new()
        {
            InitialDirectory = GetDirectory(filepath),  
            FileName = GetFilename(filepath),
            Filter = GetFileFilter(GetExtension(filepath), includeAll:true)
        };

        return saveFileDialog.Confirm() ? saveFileDialog.FileName : null;
    }

    public static string GetFilename(string filepath)
    {
        return filepath.Contains('.') ? filepath.BeforeLast(".") : filepath;
    }
    
    public static string GetExtension(string path)
    {
        return path.Contains('.') ? path.AfterLast(".") : path;
    }
    
    public static string ChangeExtension(string filepath, string extension)
    {
        return $"{GetFilename(filepath)}.{extension}";
    }

    public static string AppendSuffix(string filepath, string suffix, string separator = "")
    {
        return $"{GetFilename(filepath)}{separator}{suffix}.{GetExtension(filepath)}";
    }

    public static string GetDirectory(string path)
    {
        return path.Contains(Path.DirectorySeparatorChar) ? path.BeforeLast(PathSeparator) : path;
    }

    public static string GetFileFilter(string extension, bool includeAll = false)
    {
        return $@"{extension} file|*.{extension}{(includeAll ? $"|{Filters.AllFiles}" : string.Empty)}";
    }
    
    public static string ValidateFilename(string filepath)
    {
        return $@"{SanitizeFilename(GetFilename(filepath))}.{GetExtension(filepath)}";
    }

    public static string SanitizeFilename(string filename)
    {
        return _FilenamePattern.Replace(filename, "_").Replace(' ', '_');
    }
    
    public static void DeleteFileIfExists(string filepath)
    {
        if (File.Exists(filepath))
            File.Delete(filepath);
    }

    public static string CreateDownloadPath(string filename, params string[] folderHierarchy)
    {
        return Path.Combine(folderHierarchy.Prepend(Settings.Data.DefaultDownloadPath).Append(filename).ToArray());
    }
    
    public static long GetFileSize(string filepath)
    {
        return filepath.Valid(File.Exists) ? new FileInfo(filepath).Length : 0; //< in Bytes
    }
    
    public static string GetFileSizeFormatted(string filepath)
    {
        return GetSizeWithSuffix(GetFileSize(filepath));
    }
    
    private static readonly string[] _SizeSuffixes = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
    
    private static readonly string[] _SizeFullSuffixes = { "Bytes", "Kilobytes", "Megabytes", "Gigabytes", "Terabytes", 
        "Petabytes", "Exobytes", "Zetabytes", "Yotabytes" };

    public static string GetSizeWithSuffix(long value, int decimalPlaces = 1, bool useShortSuffixes = true)
    {
        if (decimalPlaces < 0)
            throw new ArgumentOutOfRangeException(nameof(decimalPlaces));
        
        switch (value)
        {
            case < 0:
                return $"-{GetSizeWithSuffix(-value, decimalPlaces)}";
            case 0:
                return string.Format($"{{0:n{decimalPlaces}}} {(useShortSuffixes ? _SizeSuffixes[0] : _SizeFullSuffixes[0])}", 0);
        }

        // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
        int mag = (int)Math.Log(value, 1024);

        // 1L << (mag * 10) == 2 ^ (10 * mag) 
        // [i.e. the number of bytes in the unit corresponding to mag]
        decimal adjustedSize = (decimal)value / (1L << (mag * 10));

        // make adjustment when the value is large enough that
        // it would round up to 1000 or more
        if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
        {
            mag += 1;
            adjustedSize /= 1024;
        }

        return string.Format($"{{0:n{decimalPlaces}}} {{1}}", adjustedSize, 
            useShortSuffixes ? _SizeSuffixes[mag] : _SizeFullSuffixes[mag]);
    }

    #endregion

    #region Clipboard Methods

    // TODO: Check if running in main thread...
    public static string GetClipboardText()
    {
        string content = string.Empty;

        void GetContent()
        {
            content = Clipboard.GetText().Trim();
        }
        
        RunSTA(GetContent);
        
        return content;
    }

    public static string? GetClipboardAsUrl()
    {
        string clipboard = GetClipboardText();
        return clipboard.Invalid(IsValidUrl) ? null : clipboard;
    }

    public static void SetClipboardText(string content)
    {
        void SetContent()
        {
            Clipboard.SetText(content);
        }
        
        RunSTA(SetContent);
    }

    #endregion

    #region Threading Methods

    public static Thread RunSTA(ThreadStart threadStart, bool startThread = true, bool blockCaller = true)
    {
        Thread thread = new(threadStart);
        #pragma warning disable CA1416
        thread.SetApartmentState(ApartmentState.STA);
        #pragma warning restore CA1416
        if (startThread)
            thread.Start();
        if (startThread && blockCaller)
            thread.Join();
        return thread;
    }

    #endregion

    #region Dialogs

    public static string? GetFilePathUsingDialog(string? initialDirectory = null, string? filter = null)
    {
        return RunFileDialog<OpenFileDialog>(initialDirectory, filter);
    }
    
    public static string? SaveFileUsingDialog(string? initialDirectory = null, string? filter = null)
    {
        return RunFileDialog<SaveFileDialog>(initialDirectory, filter);
    }
    
    public static string? RunFileDialog<T>(string? initialDirectory = null, string? filter = null) where T : FileDialog, new()
    {
        string? selectedPath = null;

        T fileDialog = new()
        {
            InitialDirectory = initialDirectory ?? Settings.Data.DefaultDownloadPath,
            Filter = filter ?? Filters.AllFiles
        };

        void DialogHandler()
        {
            if (!fileDialog.Confirm())
                return;
            selectedPath = fileDialog.FileName;
        }

        RunSTA(DialogHandler);

        return selectedPath.IsNullOrEmpty() ? null : selectedPath;
    }
    
    public static string? ReadFileUsingDialog(string? initialDirectory = null, string? filter = null)
    {
        return GetFilePathUsingDialog(initialDirectory, filter) is { } result ? File.ReadAllText(result) : null;
    }
    
    public static string? SelectFile(string initialPath = "")
    {
        FolderBrowserDialog folderBrowserDialog = new();
        
        if (initialPath.HasValue())
            folderBrowserDialog.InitialDirectory = initialPath;
        
        return folderBrowserDialog.Confirm() ? folderBrowserDialog.SelectedPath : null;
    }

    #endregion

    #region Administrative Functions

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
    
    #endregion

    #region Logging

    private static async Task<bool> LogExceptionsAsync(Func<Task> func)
    {
        try
        {
            await func();
            return true;
        }
        catch (Exception ex)
        {
            LogException(ex);
            return false;
        }
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

    public static void LogException(Exception exception)
    {
        Output.WriteLine(exception, Color.Crimson);
    }
    
    public static void LogException(string? message)
    {
        if (message.IsNullOrEmpty())
            return;
        Output.WriteLine(message!, Color.Crimson);
    }

    public static void Log(string? message)
    {
        if (message.IsNullOrEmpty())
            return;
        Output.WriteLine(message!);
    }
    
    #endregion

    #region Exceptions

    public class FileSystemException : Exception
    {
        public FileSystemException() { }
        
        public FileSystemException(string message) : base(message) { }
        
        public FileSystemException(string message, Exception innerException) : base(message, innerException) { }
    }

    #endregion
}