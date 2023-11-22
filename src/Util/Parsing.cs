using System.Reflection;
using System.Text;

namespace aoc2023.Util;

public static class Parsing
{
    private static readonly char[] StripPreamble = { (char)8745, (char)9559, (char)9488, };
    private static readonly Encoding[] StripBOMsFromEncodings = { Encoding.UTF8, Encoding.Unicode, Encoding.BigEndianUnicode, };
    private static void ReadData(string inputName, Action<string> processor)
    {
        if (Console.IsInputRedirected)
        {
            bool processedSomething = false;
            for (int i = 0; Console.In.ReadLine() is { } line; i++)
            {
                if (i == 0)
                {
                    if (line[0..StripPreamble.Length].SequenceEqual(StripPreamble))
                    {
                        line = line[StripPreamble.Length..];
                    }
                    else
                    {
                        foreach (var encoding in StripBOMsFromEncodings)
                        {
                            if (line.StartsWith(encoding.GetString(encoding.GetPreamble()), StringComparison.Ordinal))
                            {
                                line = line.Replace(encoding.GetString(encoding.GetPreamble()), "", StringComparison.Ordinal);
                            }
                        }
                    }
                }
                processor(line);
                if (!string.IsNullOrEmpty(line))
                {
                    processedSomething = true;
                }
            }

            if (processedSomething)
            {
                return;
            }
        }

        var filename = $"inputs/{inputName}.txt";
        if (File.Exists(filename))
        {
            if (Directory.Exists(Path.GetDirectoryName(filename)!) && File.Exists(filename))
            {
                foreach (var line in File.ReadLines(filename))
                {
                    processor(line);
                }

                return;
            }
        }

        // typeof(Logger) is not technically correct since what we need is the "default namespace,"
        // but "default namespace" is a Project File concept, not a C#/.NET concept, so it's not
        // accessible at runtime. instead, we assume Logger is also part of the "default namespace"
        var resourceName = $"{typeof(Logger).Namespace}.inputs.{inputName}.txt";
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        using StreamReader reader = new(stream!);
        while (reader.ReadLine() is { } readLine)
        {
            processor(readLine);
        }
    }

    internal static string ReadAllText(string filename)
    {
        string contents = string.Empty;
        ReadData(filename, (line) => contents = line);
        return contents;
    }

    internal static IEnumerable<string> ReadAllLines(string filename)
    {
        List<string> lines = new();
        ReadData(filename, (line) => lines.Add(line));
        return lines;
    }

    internal static IEnumerable<long> ReadAllLinesAsInts(string filename)
    {
        return ReadAllLines(filename).Select(long.Parse);
    }
}