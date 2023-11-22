using System.Diagnostics;

namespace aoc2023;

internal static class Extensions
{
    public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
    {
        foreach (T item in enumeration)
        {
            action(item);
        }
    }

    public static (double elapsed, string unit) ConvertToHumanReadable(this Stopwatch stopwatch)
    {
        var elapsed = 1.0d * stopwatch.ElapsedTicks / Stopwatch.Frequency;
        var unit = "s";
        if (elapsed < 0.001)
        {
            elapsed *= 1e+6;
            unit = "us";
        }
        else if (elapsed < 1)
        {
            elapsed *= 1000;
            unit = "ms";
        }
        else if (elapsed < 60)
        {
            unit = "s";
        }
        else if (elapsed < 60 * 60)
        {
            elapsed /= 60;
            unit = "m";
        }

        return (elapsed, unit);
    }
}
