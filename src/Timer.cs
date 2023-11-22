using System.Diagnostics;

namespace aoc2023;

internal class Timer : IDisposable
{
    private readonly Stopwatch stopwatch = Stopwatch.StartNew();
    private readonly string? name;
    private bool stopped;
    public bool Disabled { get; set; }

    public Timer(string? inName = null)
    {
        name = inName;
    }

    public void Stop()
    {
        if (stopped)
        {
            return;
        }

        stopwatch.Stop();
        stopped = true;
    }

    public void Dispose()
    {
        Stop();
        if (Disabled)
        {
            return;
        }

        var (elapsed, unit) = stopwatch.ConvertToHumanReadable();
        var color = "<red>";
        if (unit == "us" || (unit == "ms" && elapsed < 10))
        {
            color = "<green>";
        }
        else if (unit == "ms" && elapsed < 250)
        {
            color = "<yellow>";
        }
        Logger.Log($"<cyan>{name}{(!string.IsNullOrEmpty(name) ? " t" : "T")}ook {color}{elapsed:N1}{unit}<r>");
    }
}
