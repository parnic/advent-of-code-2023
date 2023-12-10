using System.Diagnostics;

namespace aoc2023.Util;

public static class Testing
{
    internal static void StartTestSet(string name)
    {
        Logger.LogLine($"<underline>test: {name}<r>");
    }

    internal static void StartTest(string label)
    {
        Logger.LogLine($"<magenta>{label}<r>");
    }

    internal static void TestCondition(Func<bool> a, bool printResult = true)
    {
        if (a.Invoke() == false)
        {
            Debug.Assert(false);
            if (printResult)
            {
                Logger.LogLine("<red>x<r>");
            }
        }
        else
        {
            if (printResult)
            {
                Logger.LogLine("<green>✓<r>");
            }
        }
    }
}