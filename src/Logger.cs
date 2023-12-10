using System.Diagnostics;

namespace aoc2023;

internal class Logger
{
    private static readonly Dictionary<string, string> colorCodes = new()
    {
        { "r", "\u001b[0m" },
        { "black", "\u001b[30m" },
        { "red", "\u001b[31m" },
        { "green", "\u001b[32m" },
        { "yellow", "\u001b[33m" },
        { "blue", "\u001b[34m" },
        { "magenta", "\u001b[35m" },
        { "cyan", "\u001b[36m" },
        { "white", "\u001b[37m" },
        { "+black", "\u001b[30;1m" },
        { "+red", "\u001b[31;1m" },
        { "+green", "\u001b[32;1m" },
        { "+yellow", "\u001b[33;1m" },
        { "+blue", "\u001b[34;1m" },
        { "+magenta", "\u001b[35;1m" },
        { "+cyan", "\u001b[36;1m" },
        { "+white", "\u001b[37;1m" },
        { "bgBlack", "\u001b[40m" },
        { "bgRed", "\u001b[41m" },
        { "bgGreen", "\u001b[42m" },
        { "bgYellow", "\u001b[43m" },
        { "bgBlue", "\u001b[44m" },
        { "bgMagenta", "\u001b[45m" },
        { "bgCyan", "\u001b[46m" },
        { "bgWhite", "\u001b[47m" },
        { "+bgBlack", "\u001b[40;1m" },
        { "+bgRed", "\u001b[41;1m" },
        { "+bgGreen", "\u001b[42;1m" },
        { "+bgYellow", "\u001b[43;1m" },
        { "+bgBlue", "\u001b[44;1m" },
        { "+bgMagenta", "\u001b[45;1m" },
        { "+bgCyan", "\u001b[46;1m" },
        { "+bgWhite", "\u001b[47;1m" },
        { "bold", "\u001b[1m" },
        { "underline", "\u001b[4m" },
        { "reverse", "\u001b[7m" },
    };

    public static void LogLine(string msg)
    {
        Console.WriteLine(InsertColorCodes(msg));
        Debug.WriteLine(StripColorCodes(msg));
    }

    public static void Log(string msg)
    {
        Console.Write(InsertColorCodes(msg));
        Debug.Write(StripColorCodes(msg));
    }

    private static string InsertColorCodes(string msg)
    {
        foreach (var code in colorCodes)
        {
            msg = msg.Replace($"<{code.Key}>", code.Value, StringComparison.CurrentCultureIgnoreCase);
        }

        return msg;
    }

    private static string StripColorCodes(string msg)
    {
        foreach (var code in colorCodes)
        {
            msg = msg.Replace($"<{code.Key}>", string.Empty, StringComparison.CurrentCultureIgnoreCase);
        }

        return msg;
    }
}
