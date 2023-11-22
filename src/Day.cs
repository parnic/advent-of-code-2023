namespace aoc2023;

internal abstract class Day : IDisposable
{
    public void Dispose()
    {
        Logger.Log("");
    }

    internal void Go(bool runPart1, bool runPart2)
    {
        Logger.Log($"<reverse>{GetType().Name}<r>");

        using (new Timer("Parsing"))
        {
            Parse();
        }

        if (runPart1)
        {
            using var stopwatch = new Timer();
            var response = Part1();
            stopwatch.Stop();
            if (!string.IsNullOrEmpty(response))
            {
                Logger.Log($"<+black>> part1: {response}<r>");
            }
            else
            {
                stopwatch.Disabled = true;
            }
        }

        if (runPart2)
        {
            using var stopwatch = new Timer();
            var response = Part2();
            stopwatch.Stop();
            if (!string.IsNullOrEmpty(response))
            {
                Logger.Log($"<+black>> part2: {response}<r>");
            }
            else
            {
                stopwatch.Disabled = true;
            }
        }
    }

    internal int GetDayNum()
    {
        if (int.TryParse(GetType().Name["Day".Length..], out int dayNum))
        {
            return dayNum;
        }

        return -1;
    }

    internal string GetDay() => $"{GetDayNum():00}";

    internal virtual void Parse() { }
    internal virtual string Part1() { return string.Empty; }
    internal virtual string Part2() { return string.Empty; }
}
