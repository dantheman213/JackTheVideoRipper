namespace JackTheVideoRipper;

public static class Threading
{
    public static TaskScheduler Scheduler { get; private set; } = null!;
    
    private static TaskFactory TaskFactory { get; set; } = null!;

    public static readonly CancellationTokenSource ProgramClosingCancellationTokenSource = new();
    
    private static CancellationToken ProgramClosingCancellationToken => ProgramClosingCancellationTokenSource.Token;

    private static bool _initialized;
    
    // Should be initialized in the main thread
    public static void InitializeScheduler()
    {
        if (_initialized)
            throw new Exception("Attempted to initialize the main thread scheduler twice!");
        Scheduler = TaskScheduler.FromCurrentSynchronizationContext();
        TaskFactory = new TaskFactory(CancellationToken.None, TaskCreationOptions.DenyChildAttach,
            TaskContinuationOptions.None, Scheduler);
        Core.ShutdownEvent += OnProgramShutdown;
        _initialized = true;
    }

    public static void OnProgramShutdown()
    {
        ProgramClosingCancellationTokenSource.Cancel();
    }
    
    // https://stackoverflow.com/questions/15428604/how-to-run-a-task-on-a-custom-taskscheduler-using-await

    public static Task RunInMainContext(Func<Task> func, CancellationToken? cancellationToken = null,
        TaskCreationOptions taskCreationOptions = TaskCreationOptions.None)
    {
        return TaskFactory.StartNew(func, cancellationToken ?? ProgramClosingCancellationToken,
            taskCreationOptions, Scheduler).Unwrap();
    }
    
    public static Task<T> RunInMainContext<T>(Func<Task<T>> func, CancellationToken? cancellationToken = null,
        TaskCreationOptions taskCreationOptions = TaskCreationOptions.None)
    {
        return TaskFactory.StartNew(func, cancellationToken ?? ProgramClosingCancellationToken,
            taskCreationOptions, Scheduler).Unwrap();
    }
    
    public static Task RunInMainContext(Action func, CancellationToken? cancellationToken = null,
        TaskCreationOptions taskCreationOptions = TaskCreationOptions.None)
    {
        return TaskFactory.StartNew(func, cancellationToken ?? ProgramClosingCancellationToken, 
            taskCreationOptions, Scheduler);
    }
    
    public static Task<T> RunInMainContext<T>(Func<T> func, CancellationToken? cancellationToken = null,
        TaskCreationOptions taskCreationOptions = TaskCreationOptions.None)
    {
        return TaskFactory.StartNew(func, cancellationToken ?? ProgramClosingCancellationToken,
            taskCreationOptions, Scheduler);
    }
}