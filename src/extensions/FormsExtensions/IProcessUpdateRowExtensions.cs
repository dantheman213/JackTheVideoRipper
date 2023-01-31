using JackTheVideoRipper.interfaces;

namespace JackTheVideoRipper.extensions;

public static class IProcessUpdateRowExtensions
{
    public static void Pause(this IEnumerable<IProcessUpdateRow> processUpdateRows)
    {
        Parallel.ForEach(processUpdateRows, p => p.Pause());
    }
    
    public static void Resume(this IEnumerable<IProcessUpdateRow> processUpdateRows)
    {
        Parallel.ForEach(processUpdateRows, p => p.Resume());
    }

    public static void Kill(this IEnumerable<IProcessUpdateRow> processUpdateRows)
    {
        Parallel.ForEach(processUpdateRows, p => p.Kill());
    }

    public static void Stop(this IEnumerable<IProcessUpdateRow> processUpdateRows)
    {
        Parallel.ForEach(processUpdateRows, p => p.Stop());
    }

    public static async Task Update(this IEnumerable<IProcessUpdateRow> processUpdateRows)
    {
        await Parallel.ForEachAsync(processUpdateRows, async (p, _) => await p.Update());
    }
}