using System.ComponentModel;
using System.Diagnostics;
using System.IO.Compression;
using System.Management;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using JackTheVideoRipper.extensions;
using JackTheVideoRipper.interfaces;
using JackTheVideoRipper.models;
using JackTheVideoRipper.models.processes;
using JackTheVideoRipper.Properties;
using Nager.PublicSuffix;
using Newtonsoft.Json;
using SpecialFolder = System.Environment.SpecialFolder;

namespace JackTheVideoRipper;

public static class FileSystem
{
    #region Data Members

    private static readonly Regex _FilenamePattern = new("[^a-zA-Z0-9 -]", RegexOptions.Compiled);

    private static readonly Regex _UrlPattern = 
        new(@"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static readonly string TempPath = Path.GetTempPath();

    public static readonly char DirectorySeparatorChar = Path.DirectorySeparatorChar;

    public const string RUN_AS_ADMIN = "runas";
     
    #endregion

    #region Embedded Types

    public static class Paths
    {
        public static readonly string AppPath       = Path.GetDirectoryName(MainModule?.FileName).ValueOrDefault();
        public static readonly string Local         = GetSpecialFolderPath(SpecialFolder.LocalApplicationData);
        public static readonly string Common        = GetSpecialFolderPath(SpecialFolder.CommonApplicationData);
        public static readonly string Root          = MergePaths(Common, AppInfo.ProgramName);
        public static readonly string Install       = MergePaths(Root, "bin");
        public static readonly string Settings      = MergePaths(Local, AppInfo.ProgramName);
        public static readonly string UserProfile   = Environment.ExpandEnvironmentVariables("%userprofile%");
        public static readonly string Download      = MergePaths(UserProfile, "Downloads");
        public static readonly string UserData      = MergePaths(Settings, "UserData");
        public static readonly string Config        = MergePaths(Settings, "settings");
        public static readonly string Data          = MergePaths(Settings, "data");
        public static readonly string System        = Environment.SystemDirectory;
    }

    #endregion

    #region Attributes

    public static string TempFile => Path.GetTempFileName();
    
    public static Version OSVersion => Environment.OSVersion.Version;

    public static FileVersionInfo FileVersionInfo => FileVersionInfo.GetVersionInfo(ExecutingAssembly.Location);

    public static string VersionInfo => FileVersionInfo.FileVersion.ValueOrDefault();
    
    public static string TimeStampDate => $"{DateTime.Now:yyyyMMddhmmsstt}";

    private static string GetProcessQuery(int pid) => string.Format(RFileSystem.SelectProcessById, pid);

    public static Assembly ExecutingAssembly => Assembly.GetExecutingAssembly();
    
    public static WindowsIdentity CurrentUser => WindowsIdentity.GetCurrent();

    public static WindowsPrincipal CurrentUserPrincipal => new(CurrentUser);
    
    public static ProcessModule? MainModule => Process.GetCurrentProcess().MainModule;

    #endregion

    #region System Methods

    public static Mutex CreateSingleInstanceLock(out bool isOnlyInstance)
    {
        return new Mutex(true, AppInfo.ProgramName, out isOnlyInstance);
    }

    #endregion

    #region Directory Methods

    public static string GetSpecialFolderPath(SpecialFolder specialFolder)
    {
        return Environment.GetFolderPath(specialFolder);
    }
    
    public static string MergePaths(IEnumerable<string> parts)
    {
        return Path.Combine(parts.ToArray());
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

    public static void OpenDownloads()
    {
        OpenFolder(Settings.Data.DefaultDownloadPath);
    }
    
    public static void OpenFolder(string? folderPath)
    {
        if (folderPath.Valid(FolderExists))
        {
            OpenFileExplorer(folderPath!);
        }
        else
        {
            Modals.Warning(string.Format(Messages.DirectoryDoesNotExist, folderPath!.WrapQuotes()));
        }
    }

    public static string CreateFolderIfNotExists(string directory)
    {
        if (directory.Valid(IsValidPath) && !FolderExists(directory))
            CreateFolder(directory);
        return directory;
    }

    public static void CreatePathIfNoneExists(string filepath)
    {
        Directory.CreateDirectory(GetDirectory(filepath));
    }

    public static string ProgramPath(string executablePath)
    {
        return CreateInstallPath(executablePath);
    }
    
    public static string GetDownloadPath(string filename)
    {
        return CreateDownloadPath(ValidateFilename(filename));
    }
    
    private static void ValidateFilePath(string filepath, bool deleteFileIfExists = false)
    {
        if (filepath.Invalid(IsValidPath))
            throw new InvalidPathException();
        CreatePathIfNoneExists(filepath);
        if (deleteFileIfExists)
            DeleteFileIfExists(filepath);
    }

    #endregion

    #region Process Methods

    // Useful for single file (NOT A ZIP ARCHIVE)
    public static AsyncProcess GetWebResourceHandle(string url, string downloadDirectory = "",
        bool useShellExecute = false)
    {
        return CreateAsyncProcess(new ProcessStartInfo(url)
        {
            WorkingDirectory = downloadDirectory,
            UseShellExecute = useShellExecute
        }, timeoutPeriod: 60000);
    }

    public static Process CreateProcess(ProcessStartInfo processStartInfo, bool enableRaisingEvents = false)
    {
        return new Process
        {
            StartInfo = processStartInfo,
            EnableRaisingEvents = enableRaisingEvents
        };
    }
    
    public static AsyncProcess CreateAsyncProcess(ProcessStartInfo processStartInfo, int timeoutPeriod = -1)
    {
        return new AsyncProcess(timeoutPeriod)
        {
            StartInfo = processStartInfo
        };
    }

    public static Process CreateProcess(string bin, string parameters, string workingDir = "", bool runAsAdmin = false, 
        bool executeShell = false, bool redirect = true, bool enableRaisingEvents = false)
    {
        if (bin.Invalid(IsValidPath))
            throw new FileSystemException(RFileSystem.EmptyBinPath);

        return CreateProcess(new ProcessStartInfo
        {
            WindowStyle                 = ProcessWindowStyle.Hidden,
            FileName                    = bin,
            Arguments                   = parameters,
            WorkingDirectory            = workingDir.ValueOrDefault(Paths.AppPath),
            UseShellExecute             = executeShell,
            RedirectStandardError       = redirect,
            RedirectStandardOutput      = redirect,
            RedirectStandardInput       = redirect,
            CreateNoWindow              = true,
            Verb                        = runAsAdmin ? RUN_AS_ADMIN : string.Empty
        },  enableRaisingEvents);
    }

    public static AsyncProcess CreateAsyncProcess(string bin, string parameters, string workingDir = "",
        bool runAsAdmin = false, bool redirectInput = true, int timeoutPeriod = -1)
    {
        if (bin.Invalid(IsValidPath))
            throw new FileSystemException(RFileSystem.EmptyBinPath);

        return CreateAsyncProcess(new ProcessStartInfo
        {
            WindowStyle                 = ProcessWindowStyle.Hidden,
            FileName                    = bin,
            Arguments                   = parameters,
            WorkingDirectory            = workingDir.ValueOrDefault(Paths.AppPath),
            RedirectStandardInput       = redirectInput,
            CreateNoWindow              = true,
            Verb                        = runAsAdmin ? RUN_AS_ADMIN : string.Empty
        },  timeoutPeriod);
    }
    
    public static string TryRunProcess(Process process)
    {
        return TryStartProcess(process).GetOutput();
    }
    
    public static bool RunProcess(ProcessStartInfo processStartInfo)
    {
        Process process = CreateProcess(processStartInfo);
        return process.Start();
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
    
    public static async Task<ProcessResult> RunProcessAsync(string filepath, string args, string workingDir = "",
        bool runAsAdmin = false, bool redirectInput = true, int timeoutPeriod = -1)
    {
        return await CreateAsyncProcess(filepath, args, workingDir, runAsAdmin, redirectInput, timeoutPeriod).Run();
    }
    
    public static async Task<ProcessResult> RunProcessAsync(ProcessStartInfo processStartInfo, int timeoutPeriod = -1)
    {
        return await CreateAsyncProcess(processStartInfo, timeoutPeriod).Run();
    }

    public static Process TryStartProcess(ProcessStartInfo processStartInfo)
    {
        return TryStartProcess(CreateProcess(processStartInfo));
    }

    public static Process TryStartProcess(Process process)
    {
        void LogIfFailed()
        {
            if (!process.Start()) Log(RFileSystem.ProcessAlreadyRunning);
        }
        
        LogExceptions(LogIfFailed);
        return process;
    }
    
    public static async Task<ProcessResult> TryStartProcessAsync(ProcessStartInfo processStartInfo, int timeoutPeriod = -1)
    {
        return await TryStartProcessAsync(CreateAsyncProcess(processStartInfo, timeoutPeriod));
    }

    public static async Task<ProcessResult> TryStartProcessAsync(AsyncProcess process)
    {
        return await LogExceptionsAsync(async () => await process.Run()) ?? new ProcessResult();
    }

    #endregion

    #region Command Methods

    public static string RunCommand(string binPath, string paramString, string workingDir = "", bool runAsAdmin = false)
    {
        return RunProcess(CreateProcess(binPath, paramString, workingDir, runAsAdmin));
    }
    
    public static async Task<ProcessResult> RunCommandAsync(string binPath, string paramString,
        string workingDir = "", bool runAsAdmin = false, bool redirectInput = true, int timeoutPeriod = -1)
    {
        return await CreateAsyncProcess(binPath, paramString, workingDir, runAsAdmin, redirectInput, timeoutPeriod).Run();
    }
    
    public static async Task<ProcessResult> RunCommandAsync<T>(string binPath, string paramString,
        string workingDir = "", bool runAsAdmin = false, bool redirectInput = true, int timeoutPeriod = -1)
    {
        
        return await CreateAsyncProcess(binPath, paramString, workingDir, runAsAdmin, redirectInput, timeoutPeriod).Run();
    }

    public static string RunWebCommand(string binPath, string url, string parameterString)
    {
        return url.Valid(IsValidUrl) ? RunCommand(binPath, $"{parameterString} {url}") : string.Empty;
    }
    
    public static async Task<ProcessResult?> RunWebCommandAsync(string binPath, string url, string parameterString)
    {
        return url.Valid(IsValidUrl) ? 
            await RunCommandAsync(binPath, $"{parameterString} {url}") :
            default;
    }

    #endregion

    #region JSON Methods

    public static string Serialize(object? obj)
    {
        return JsonConvert.SerializeObject(obj);
    }

    public static void WriteJsonToFile(string filepath, object? obj)
    {
        SaveToFile(filepath, Serialize(obj));
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

    public static async Task<string> GetJsonResponse(string binPath, string url, string parameterString)
    {
        return (await RunWebCommandAsync(binPath, url, parameterString))?.Output ?? string.Empty;
    }
    
    public static async Task<T?> ReceiveJsonResponseAsync<T>(string binPath, string url, string parameterString)
    {
        return url.Valid(IsValidUrl) ? Deserialize<T>(await GetJsonResponse(binPath, url, parameterString)) : default;
    }
        
    // youtube-dl returns an individual json object per line
    public static IEnumerable<T> ReceiveMultiJsonResponse<T>(string binPath, string url, string parameterString)
    {
        return CreateMultiJson<T>(url, RunWebCommand(binPath, url, parameterString));
    }

    public static async Task<IEnumerable<T>> ReceiveMultiJsonResponseAsync<T>(string binPath, string url,
        string parameterString)
    {
        return CreateMultiJson<T>(url, await GetJsonResponse(binPath, url, parameterString));
    }

    public static IEnumerable<T> CreateMultiJson<T>(string url, string jsonResponse)
    {
        return url.Valid(IsValidUrl) ? CreateJsonArray<T>(jsonResponse) : Array.Empty<T>();
    }

    private static IEnumerable<T> CreateJsonArray<T>(string jsonResponse)
    {
        if (jsonResponse.IsNullOrEmpty())
            return Array.Empty<T>();

        string reformatted = jsonResponse.Replace("\n", ",\n").Wrap("[", "]");
        
        return Deserialize<IEnumerable<T>>(reformatted) ?? Array.Empty<T>();
    }

    #endregion

    #region Program Methods
    
    public static void OpenWebPage(string url)
    {
        if (url.Invalid(IsValidUrl))
            return;
        
        OpenFileExplorer(url);
    }

    public static void OpenFileExplorer(string directory)
    {
        bool startedSuccessfully = RunProcess(new ProcessStartInfo
        {
            Arguments = directory,
            FileName = Executables.Explorer
        });
        
        if (!startedSuccessfully)
        {
            throw new ProcessFailedToStartException(RFileSystem.CouldNotStartExplorer);
        }
    }

    public static void OpenTaskManager()
    {
        bool startedSuccessfully;
        try
        {
            startedSuccessfully = RunProcess(new ProcessStartInfo
            {
                CreateNoWindow = false,
                UseShellExecute = true,
                FileName = CreateSystemPath(Executables.TaskManager)
            });
        }
        catch (Win32Exception win32Exception)
        {
            // User Cancelled
            if (win32Exception.NativeErrorCode == 1223)
                return;
            throw new ProcessFailedToStartException(RFileSystem.CouldNotStartTaskManager, win32Exception);
        }
        catch (Exception exception)
        {
            throw new ProcessFailedToStartException(RFileSystem.CouldNotStartTaskManager, exception);
        }

        if (!startedSuccessfully)
        {
            throw new ProcessFailedToStartException(RFileSystem.CouldNotStartTaskManager);
        }
    }

    public static void OpenFileExplorerWithFileSelected(string filePath)
    {
        OpenFileExplorer($"/select, {filePath.WrapQuotes()}");
    }
    
    public static async Task<bool> InstallProgram(string downloadUrl, string filename)
    {
        if (CreateInstallPath(filename) is not { } installPath)
            return false;
        return downloadUrl.Valid(IsValidUrl) && (await DownloadWebFileAsync(downloadUrl, installPath)).HasValue();
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
        return DownloadWebFile(url, GetTempFilename(extension));
    }

    private static bool ValidateWebFile(string url, string? localPath, bool deleteFileIfExists = true)
    {
        if (url.Invalid(IsValidUrl) || localPath.Invalid(IsValidPath))
            return false;
        
        ValidateFilePath(localPath!, deleteFileIfExists);
        return true;
    }

    public static string DownloadWebFile(string url, string? localPath, bool deleteFileIfExists = true)
    {
        if (!ValidateWebFile(url, localPath, deleteFileIfExists))
            return string.Empty;

        HttpResponseMessage response = SimpleWebQuery(url);

        return HandleResponse(response, localPath!);
    }
    
    public static async Task<string?> DownloadWebFileAsync(string url, string localPath, bool deleteFileIfExists = true)
    {
        if (!ValidateWebFile(url, localPath, deleteFileIfExists))
            return string.Empty;

        return await GetResource(url, localPath);
    }

    private static string HandleResponse(HttpResponseMessage response, string localPath)
    {
        if (localPath.Invalid(IsValidPath))
            return string.Empty;
        if (response.IsSuccessStatusCode)
            return LogExceptions(() => response.DownloadResponse(localPath)) ?? string.Empty;
        Log(string.Format(Messages.FailedToDownload, response.ResponseCode()));
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
        using HttpClient client = CreateClientWithUserAgent(handler);
        
        return completionOption is not { } option ? await client.GetAsync(url) : 
            await client.GetAsync(url, option);
    }

    private static async Task<string?> GetResource(string resourceUrl, string localPath, HttpMessageHandler? handler = null)
    {
        if (resourceUrl.Invalid(IsValidUrl) || localPath.Invalid(IsValidPath))
            return null;
        
        // Open Client
        using HttpClient client = CreateClientWithUserAgent(handler);

        // Get the name of the Resource we're requesting
        if (await client.GetResourceFileName(resourceUrl) is not { } filename)
            return null;

        return await client.DownloadResourceAsync(resourceUrl, MergePaths(localPath, filename));
    }

    private static IEnumerable<string> SystemInformation => new[]
    {
        Environment.OSVersion.VersionString,
        Environment.Is64BitOperatingSystem ? "Win64" : "Win32",
        Environment.Is64BitOperatingSystem ? "x64"   : "x32"
    };

    private static string UserAgentString()
    {
        return $"{AppInfo.ProgramName}/{VersionInfo} ({UserAgentOperatingSystemInfo})";
    }

    private static string UserAgentOperatingSystemInfo => SystemInformation.Merge("; ");

    // Lets endpoints know to send us responses relevant to our operating system (e.g. exe/zip for windows)
    private static ProductInfoHeaderValue[] ProductInfoHeaders => new[]
    {
        new ProductInfoHeaderValue(AppInfo.ProgramName, VersionInfo),
        new ProductInfoHeaderValue($"({UserAgentOperatingSystemInfo})")
    };

    private static HttpClient CreateClient(HttpMessageHandler? handler = null)
    {
        return handler == null ? new HttpClient() : new HttpClient(handler);
    }
    
    private static HttpClient CreateClientWithUserAgent(HttpMessageHandler? handler = null)
    {
        return handler == null ? 
            new HttpClient
            {
                DefaultRequestHeaders = { UserAgent = { ProductInfoHeaders[0], ProductInfoHeaders[1] } }
            } :
            new HttpClient(handler)
            {
                DefaultRequestHeaders = { UserAgent = { ProductInfoHeaders[0], ProductInfoHeaders[1] } }
            };
    }

    #endregion

    #region File Methods

    public static bool IsValidPath(string path)
    {
        return path.IndexOfAny(Path.GetInvalidPathChars()) == -1;
    }
    
    public static bool WarnIfFileExists(string filepath)
    {
        return !Exists(filepath) || ConfirmOverwrite();
    }

    public static bool Exists(string? filepath)
    {
        return File.Exists(filepath);
    }

    public static void DeleteFileIfExists(string filepath)
    {
        if (Exists(filepath))
            File.Delete(filepath);
    }

    public static bool WarnAndDeleteIfExists(string filepath)
    {
        if (!Exists(filepath))
            return true;

        return ConfirmOverwrite() && TryDelete(filepath);
    }

    public static bool TryDelete(string filepath)
    {
        return LogExceptions(() => File.Delete(filepath));
    }
    
    public static void SaveToFile(string filepath, string content)
    {
        File.WriteAllText(filepath, content);
    }

    public static string GetTempFilename(string extension, string? prefix = null, string separator = "_")
    {
        return MergePaths(TempPath, prefix is null ?
            $"{AppInfo.ProgramPrefix}{separator}{TimeStampDate}.{extension}" :
            $"{AppInfo.ProgramPrefix}{separator}{prefix}{separator}{TimeStampDate}.{extension}");
    }
    
    public static void OpenFile(string filepath, bool openDownloadsIfNull)
    {
        if (filepath.Valid(Exists))
        {
            OpenFileExplorerWithFileSelected(filepath);
        }
        else
        {
            // couldn't find folder, rolling back to just the folder with no select
            Log(string.Format(RFileSystem.CouldNotOpenFile, filepath.WrapQuotes()));
            if (openDownloadsIfNull)
                OpenDownloads();
        }
    }
    
    public static string GetFilepathWithoutExtension(string filepath)
    {
        return filepath.BeforeLast('.');
    }

    public static string GetFilenameWithoutExtension(string filepath)
    {
        return GetFilename(filepath).BeforeLast('.');
    }
    
    public static string GetFilename(string filepath)
    {
        return Path.GetFileName(filepath);
    }

    public static string GetExtension(string path)
    {
        return path.AfterLast('.');
    }
    
    public static string ChangeExtension(string filepath, string extension)
    {
        return $"{GetFilepathWithoutExtension(filepath)}.{extension}";
    }

    public static string AppendSuffix(string filepath, string suffix, string separator = "")
    {
        return $"{GetFilepathWithoutExtension(filepath)}{separator}{suffix}.{GetExtension(filepath)}";
    }

    public static string GetDirectory(string? path)
    {
        if (path.IsNullOrEmpty())
            return string.Empty;
        return path!.Contains(DirectorySeparatorChar) ? path.BeforeLast(DirectorySeparatorChar) : path;
    }

    public static string GetFileFilter(string extension, bool includeAll = false)
    {
        return $@"{extension} file|*.{extension}{(includeAll ? $"|{FileFilters.AllFiles}" : string.Empty)}";
    }
    
    public static string ValidateFilename(string filepath, string replacement = "_")
    {
        return $@"{SanitizeFilename(GetFilenameWithoutExtension(filepath), replacement)}.{GetExtension(filepath)}";
    }

    public static string SanitizeFilename(string filename, string replacement = "_")
    {
        return _FilenamePattern.Replace(filename, replacement).Replace(" ", replacement);
    }

    public static string CreateDownloadPath(string? filename, params string[] folderHierarchy)
    {
        return filename.Valid(IsValidPath)
            ? MergePaths(folderHierarchy.Prepend(Settings.Data.DefaultDownloadPath).Append(filename!))
            : string.Empty;
    }
    
    public static string CreateInstallPath(string? filename, params string[] folderHierarchy)
    {
        return filename.Valid(IsValidPath)
            ? MergePaths(folderHierarchy.Prepend(Paths.Install).Append(filename!))
            : string.Empty;
    }
    
    public static string CreateSystemPath(string? filename, params string[] folderHierarchy)
    {
        return filename.Valid(IsValidPath)
            ? MergePaths(folderHierarchy.Prepend(Paths.System).Append(filename!))
            : string.Empty;
    }
    
    public static long GetFileSize(string filepath)
    {
        return filepath.Valid(Exists) ? new FileInfo(filepath).Length : 0; //< in Bytes
    }
    
    public static string GetFileSizeFormatted(string filepath)
    {
        return GetFileSizeFormatted(GetFileSize(filepath));
    }
    
    public static bool TryParseFileSize(string sizeString, out long fileSize)
    {
        fileSize = -1;
        
        string[] tokens = sizeString.Split();

        if (tokens.Length < 2 || !long.TryParse(tokens[0], out long baseSize))
            return false;

        string suffix = tokens[1].ToUpper();

        if (!_SuffixToSizeMultiplierDict.ContainsKey(suffix))
            return false;

        fileSize = _SuffixToSizeMultiplierDict[suffix] * baseSize;

        return true;
    }

    public static long ParseFileSize(string sizeString)
    {
        if (sizeString.IsNullOrEmpty())
            return 0;
        double baseSize = double.Parse(sizeString.BeforeFirstLetter());
        string suffix = sizeString.AfterFirstLetter(true);
        return (long) Math.Floor(_SuffixToSizeMultiplierDict[suffix.ToUpper()] * baseSize);
    }

    private static readonly string[] _SizeSuffixes = { "B", "KB", "MB", "GB", "TB", "PB", "XB" };
    
    private static readonly string[] _SizeFullSuffixesSingular = { "Byte", "Kilobyte", "Megabyte", "Gigabyte",
        "Terabyte", "Petabyte", "Exobyte" };
    
    private static readonly string[] _SizeFullSuffixes = { "Bytes", "Kilobytes", "Megabytes", "Gigabytes",
        "Terabytes", "Petabytes", "Exobytes" };
    
    private static readonly Dictionary<string, long> _SuffixToSizeMultiplierDict = new()
    {
        // Bytes
        { _SizeSuffixes[0],                         1 },
        { _SizeFullSuffixes[0].ToUpper(),           1 },
        { _SizeFullSuffixesSingular[0].ToUpper(),   1 },
        
        // Kilobytes
        { _SizeSuffixes[1],                         1<<10 },
        { _SizeFullSuffixes[1].ToUpper(),           1<<10 },
        { _SizeFullSuffixesSingular[1].ToUpper(),   1<<10 },
        
        // Megabytes
        { _SizeSuffixes[2],                         1<<20 },
        { _SizeFullSuffixes[2].ToUpper(),           1<<20 },
        { _SizeFullSuffixesSingular[2].ToUpper(),   1<<20 },
        
        // Gigabytes
        { _SizeSuffixes[3],                         1<<30 },
        { _SizeFullSuffixes[3].ToUpper(),           1<<30 },
        { _SizeFullSuffixesSingular[3].ToUpper(),   1<<30 },
        
        // Terabytes
        { _SizeSuffixes[4],                         1L<<40 },
        { _SizeFullSuffixes[4].ToUpper(),           1L<<40 },
        { _SizeFullSuffixesSingular[4].ToUpper(),   1L<<40 },
        
        // Petabytes
        { _SizeSuffixes[5],                         1L<<50 },
        { _SizeFullSuffixes[5].ToUpper(),           1L<<50 },
        { _SizeFullSuffixesSingular[5].ToUpper(),   1L<<50 },
        
        // Exobytes
        { _SizeSuffixes[6],                         1L<<60 },
        { _SizeFullSuffixes[6].ToUpper(),           1L<<60 },
        { _SizeFullSuffixesSingular[6].ToUpper(),   1L<<60 }
    };

    public static string FileSizeSuffix(int n, bool useShortSuffixes) =>
        useShortSuffixes ? _SizeSuffixes[n] : _SizeFullSuffixes[n];

    public static string FileSizeFormatString(double adjustedSize = 0, int decimalPlaces = 1, int magnitude = 0, 
        bool useShortSuffixes = true)
    {
        return string.Format($"{{0:n{decimalPlaces}}} {FileSizeSuffix(magnitude, useShortSuffixes)}", adjustedSize);
    }

    public static string GetFileSizeFormatted(long value, int decimalPlaces = 1, bool useShortSuffixes = true)
    {
        if (decimalPlaces < 0)
            throw new ArgumentOutOfRangeException(nameof(decimalPlaces));

        if (value <= 0)
        {
            return value switch
            {
                < 0 => $"-{GetFileSizeFormatted(-value, decimalPlaces)}",
                _ => FileSizeFormatString()
            };
        }

        // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
        int magnitude = (int) Math.Floor(Math.Log(value, 1024));

        // 1L << (mag * 10) == 2 ^ (10 * mag) 
        // [i.e. the number of bytes in the unit corresponding to mag]
        double adjustedSize = (double) value / (1L << (magnitude * 10));

        // make adjustment when the value is large enough that
        // it would round up to 1000 or more
        if (Math.Round(adjustedSize, decimalPlaces) >= 1024)
        {
            magnitude += 1;
            adjustedSize /= 1024;
        }

        return FileSizeFormatString(adjustedSize, decimalPlaces, magnitude, useShortSuffixes);
    }

    public static string MoveFile(string filepath, string destinationDirectory, bool overwrite = true)
    {
        if (filepath.IsNullOrEmpty() || destinationDirectory.IsNullOrEmpty())
            return string.Empty;
        string destinationFilepath = MergePaths(destinationDirectory, GetFilename(filepath));
        File.Move(filepath, destinationFilepath, overwrite);
        return destinationFilepath;
    }

    public static bool IsFolderEmpty(string path)
    {
        return Directory.GetFiles(path).None() && Directory.GetDirectories(path).None();
    }

    public static string? MoveFolder(string path, string destinationDirectory, bool moveChildren = false,
        bool throwWhenFailed = false)
    {
        if (path.IsNullOrEmpty())
            return string.Empty;
        if (!IsFolderEmpty(path) && !moveChildren)
        {
            if (throwWhenFailed)
                throw new MoveFailedException();
            return null;
        }
        string destinationPath = MergePaths(destinationDirectory, Path.GetFileName(path));
        MoveChildFolders(path, destinationPath);
        MoveFiles(path, destinationPath);
        Directory.Move(path, destinationPath);
        return destinationPath;
    }

    public static bool FolderExists(string? folderPath)
    {
        return folderPath.Valid(IsValidPath) && Directory.Exists(folderPath);
    }

    public static string GetFolderName(string? path)
    {
        return Path.GetDirectoryName(path) ?? string.Empty;
    }
    
    public static bool MoveChildFolders(string rootDirectory, string destinationDirectory)
    {
        CreateFolderIfNotExists(MergePaths(destinationDirectory, GetFolderName(rootDirectory)));
        return MoveFolders(Directory.GetDirectories(rootDirectory), destinationDirectory);
    }

    public static bool MoveFolders(IEnumerable<string> folderPaths, string destinationDirectory)
    {
        return folderPaths.Select(dir => FolderExists(MoveFolder(dir, destinationDirectory, true))).All();
    }

    public static bool MoveFiles(string directory, string destinationDirectory, bool overwrite = true)
    {
        bool success = Directory.GetFiles(directory)
            .Select(file => MoveFile(file, destinationDirectory, overwrite))
            .All(Exists);
        return success && Directory.GetDirectories(directory)
            .Select(dir => MoveFolder(dir, destinationDirectory, true))
            .All(FolderExists);
    }
    
    public static string CopyFile(string filepath, string destinationDirectory, bool overwrite = true)
    {
        if (filepath.IsNullOrEmpty() || destinationDirectory.IsNullOrEmpty())
            return string.Empty;
        string destinationFilepath = MergePaths(destinationDirectory, GetFilename(filepath));
        File.Copy(filepath, destinationFilepath, overwrite);
        return destinationFilepath;
    }
    
    public static bool CopyFiles(string directory, string destinationDirectory, bool overwrite = true)
    {
        return Directory.GetFiles(directory)
            .Select(file => CopyFile(file, destinationDirectory, overwrite))
            .All(Exists);
    }
    
    public static async Task<string?> ExtractResourceFromUrl(string url, string filepath)
    {
        if (await DownloadWebFileAsync(url, TempPath) is not { } downloadPath || downloadPath.Invalid(Exists))
            return null;

        if (await ExtractResource(downloadPath) is not { } path || path.Invalid(FolderExists))
            return null;

        return MoveFiles(path, filepath) ? path : null;
    }

    public static async Task<string?> ExtractResource(string filepath)
    {
        string? result = GetExtension(filepath).ToLower() switch
        {
            "zip"   => ExtractZip(filepath),
            "gz"    => await ExtractTarGz(filepath),
            "tar"   => await ExtractTar(filepath),
            _       => throw new UnsupportedArchiveFormatException()
        };

        return result;
    }

    public static string ExtractZip(string filepath, string? outputDirectory = null)
    {
        outputDirectory ??= TempPath;
        string extractPath = MergePaths(outputDirectory, GetFilenameWithoutExtension(filepath));
        
        if (!Exists(filepath))
            ZipFile.CreateFromDirectory(outputDirectory, filepath);
        
        ZipFile.ExtractToDirectory(filepath, extractPath, true);
        return extractPath;
    }
    
    public static async Task<string> ExtractTarGz(string filename, string? outputDirectory = null)
    {
        outputDirectory ??= TempPath;
        await using FileStream stream = File.OpenRead(filename);
        await ExtractTarGz(stream, outputDirectory);
        return MergePaths(outputDirectory, filename);
    }

    public static async Task ExtractTarGz(Stream stream, string? outputDirectory = null)
    {
        outputDirectory ??= TempPath;
        const int chunkSize = 4096;
        
        // A GZipStream is not seekable, so copy it first to a MemoryStream
        await using GZipStream gzip = new(stream, CompressionMode.Decompress);
        
        using MemoryStream memStr = new();
        int readPosition;
        byte[] buffer = new byte[chunkSize];
        do
        {
            readPosition = await gzip.ReadAsync(buffer.AsMemory(0, chunkSize));
            await memStr.WriteAsync(buffer.AsMemory(0, readPosition));
        } while (readPosition == chunkSize);

        memStr.Seek(0, SeekOrigin.Begin);
        await ExtractTar(memStr, outputDirectory);
    }

    public static async Task<string> ExtractTar(string filename, string? outputDirectory = null)
    {
        outputDirectory ??= TempPath;
        await using FileStream stream = File.OpenRead(filename);
        await ExtractTar(stream, outputDirectory);
        return MergePaths(outputDirectory, filename);
    }

    private static string GetAsciiString(byte[] buffer, int count)
    {
        return Encoding.ASCII.GetString(buffer, 0, count);
    }
    
    private static string GetAsciiString(Memory<byte> buffer, int count)
    {
        return Encoding.ASCII.GetString(buffer.ToArray(), 0, count);
    }

    private const int _CHUNK_SIZE = 512;
    
    public static async Task ExtractTar(Stream stream, string? outputDirectory = null)
    {
        outputDirectory ??= TempPath;
        byte[] buffer = new byte[100];
        while (true)
        {
            int result = await stream.ReadAsync(buffer);
            
            string name = GetAsciiString(buffer, result).Trim('\0');
            if (name.IsNullOrEmpty())
                break;
            
            //stream.Seek(24, SeekOrigin.Current);
            stream.Seek(result, SeekOrigin.Current);
            Memory<byte> sizeMemory = buffer.AsMemory(0, 12);
            int result2 = await stream.ReadAsync(sizeMemory);
            long size = Convert.ToInt64(GetAsciiString(sizeMemory, result2).Trim(), 8);

            stream.Seek(376L, SeekOrigin.Current);
            string output = MergePaths(outputDirectory, name);
            
            // Directory, skip
            if (name.EndsWith('/'))
            {
                Directory.CreateDirectory(output);
            }
            else
            {
                await stream.WriteToFileAsync(output, (int) size);
            }

            stream.Seek((_CHUNK_SIZE - stream.Position % _CHUNK_SIZE) % _CHUNK_SIZE, SeekOrigin.Current);
        }
    }

    #endregion

    #region Clipboard Methods

    public static string GetClipboardText()
    {
        return RunSTA(Clipboard.GetText).Trim();
    }

    public static string? GetClipboardAsUrl()
    {
        return GetClipboardText().ValidOrDefault(IsValidUrl);
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
    
    public static T RunSTA<T>(Func<T> func)
    {
        T result = default!;
        Thread thread = new(() => result = func());
        #pragma warning disable CA1416
        thread.SetApartmentState(ApartmentState.STA);
        #pragma warning restore CA1416
        thread.Start();
        thread.Join();
        return result;
    }
    
    public static async Task<T> RunSTAAsync<T>(Func<T> func)
    {
        T result = default!;
        Thread thread = new(() => result = func());
        #pragma warning disable CA1416
        thread.SetApartmentState(ApartmentState.STA);
        #pragma warning restore CA1416
        thread.Start();
        await Task.Run(thread.Join);
        return result;
    }

    #endregion

    #region Dialogs
    
    private static bool ConfirmOverwrite()
    {
        return Modals.Confirmation(Messages.OverwriteFile, Captions.FileExists);
    }

    public static string? SelectFile(string? initialDirectory = null, string? filename = null, string? filter = null)
    {
        return RunFileDialog<OpenFileDialog>(initialDirectory, filename, filter);
    }
    
    public static string? SelectFileMulti(string? initialDirectory = null, string? filename = null,
        string? filter = null)
    {
        return RunFileDialog<OpenFileDialog>(initialDirectory, filename, filter, true);
    }
    
    public static string? SaveFileUsingDialog(string? initialDirectory = null, string? filename = null,
        string? filter = null)
    {
        return RunFileDialog<SaveFileDialog>(initialDirectory, filename, filter);
    }
    
    public static string? RunFileDialog<T>(string? initialDirectory = null, string? filename = null, 
        string? filter = null, bool multiSelect = false) where T : FileDialog, new()
    {
        string? selectedPath = null;

        T fileDialog = new()
        {
            InitialDirectory = initialDirectory ?? Settings.Data.LastOpenedFilepath,
            Filter = filter ?? FileFilters.AllFiles,
            FileName = filename
        };

        if (fileDialog is OpenFileDialog dialog)
        {
            dialog.Multiselect = multiSelect;
        }

        void DialogHandler()
        {
            if (!fileDialog.Confirm())
                return;
            selectedPath = fileDialog.FileName;
        }

        RunSTA(DialogHandler);

        if (selectedPath.Valid(Exists))
            Settings.Data.LastOpenedFilepath = GetDirectory(selectedPath);

        return selectedPath.IsNullOrEmpty() ? null : selectedPath;
    }
    
    public static string? ReadFileUsingDialog(string? initialDirectory = null, string? filter = null)
    {
        return SelectFile(initialDirectory, filter:filter) is { } result ? File.ReadAllText(result) : null;
    }
    
    public static string? SelectFolder(string? initialPath = null)
    {
        FolderBrowserDialog folderBrowserDialog = new()
        {
            InitialDirectory = initialPath ?? Settings.Data.LastOpenedFilepath
        };

        string? result = null;

        RunSTA(() =>
        {
            if (folderBrowserDialog.Confirm())
                result = folderBrowserDialog.SelectedPath;
        });
        
        if (result.Valid(FolderExists))
            Settings.Data.LastOpenedFilepath = result!;
        
        return result;
    }
    
    public static string? SaveCopy(string filepath)
    {
        return RunFileDialog<SaveFileDialog>(GetDirectory(filepath), 
                                             GetFilenameWithoutExtension(filepath),
                                             GetFileFilter(GetExtension(filepath), includeAll: true));
    }

    #endregion

    #region Administrative Functions

    public static bool UserIsAdmin()
    {
        return CurrentUserPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    private static ManagementObjectSearcher? GetProcessSearcher(int pid)
    {
        return LogExceptions(() => new ManagementObjectSearcher(GetProcessQuery(pid)));
    }

    public static void TryKillProcessAndChildren(int pid)
    {
        if (GetProcessSearcher(pid) is not { } process)
            return;

        foreach (ManagementObject manager in process.Get().Cast<ManagementObject>())
        {
            LogExceptions(() => manager.TryKillProcessAndChildren());
        }

        TryKillProcess(pid);
    }

    private static void TryKillProcess(int pid)
    {
        if (Process.GetProcessById(pid) is not { HasExited: false } process )
            return;

        LogExceptions(process.Kill);
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
    
    private static async Task<T?> LogExceptionsAsync<T>(Func<Task<T>> func)
    {
        try
        {
            return await func();
        }
        catch (Exception ex)
        {
            LogException(ex);
            return default;
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
    
    private static T? LogExceptions<T>(Func<T> func)
    {
        try
        {
            return func();
        }
        catch (Exception ex)
        {
            LogException(ex);
            return default;
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

    public class InvalidPathException : FileSystemException
    {
        public InvalidPathException() { }
        
        public InvalidPathException(string message) : base(message) { }
        
        public InvalidPathException(string message, Exception innerException) : base(message, innerException) { }
    }
    
    public class InvalidUrlException : FileSystemException
    {
        public InvalidUrlException() { }
        
        public InvalidUrlException(string message) : base(message) { }
        
        public InvalidUrlException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class ProcessFailedToStartException : FileSystemException
    {
        public ProcessFailedToStartException() { }
        
        public ProcessFailedToStartException(string message) : base(message) { }
        
        public ProcessFailedToStartException(string message, Exception innerException) : base(message, innerException) { }
    }
    
    public class MoveFailedException : FileSystemException
    {
        public MoveFailedException() { }
        
        public MoveFailedException(string message) : base(message) { }
        
        public MoveFailedException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class UnsupportedArchiveFormatException : FileSystemException
    {
    }

    #endregion
}