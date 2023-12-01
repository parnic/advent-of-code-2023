namespace aoc2023;

internal class Day01 : Day
{
    private readonly List<string> numStrs = new()
    {
        "1",
        "one",
        "2",
        "two",
        "3",
        "three",
        "4",
        "four",
        "5",
        "five",
        "6",
        "six",
        "7",
        "seven",
        "8",
        "eight",
        "9",
        "nine",
    };

    private readonly Dictionary<string, string> strs = new()
    {
        {"one", "1" },
        {"two", "2" },
        {"three", "3" },
        {"four", "4" },
        {"five", "5" },
        {"six", "6" },
        {"seven", "7" },
        {"eight", "8" },
        {"nine", "9" },
    };

    private readonly List<int> nums = new();
    private readonly List<int> num2 = new();
    internal override void Parse()
    {
        var lines = Util.Parsing.ReadAllLines($"{GetDay()}");
        foreach (var line in lines)
        {
            try
            {
                var firstNum = line.First(c => c >= '1' && c <= '9');
                var lastNum = line.Last(c => c >= '1' && c <= '9');
                nums.Add(int.Parse($"{firstNum}{lastNum}"));
            }
            catch { }

            List<int> digits = new();
            for (int i = 0; i < line.Length; i++)
            {
                var s = line[..(i+1)];
                foreach (var ns in numStrs)
                {
                    if (s.EndsWith(ns))
                    {
                        s = ns;
                        break;
                    }
                }

                if (numStrs.Contains(s))
                {
                    if (strs.TryGetValue(s, out var str))
                    {
                        s = str;
                    }
                    digits.Add(s[0] - '0');
                }
            }

            num2.Add((digits.First() * 10) + digits.Last());
        }
    }

    internal override string Part1()
    {
        long total = nums.Sum();
        return $"Calibration sum for literal digits: <+white>{total}";
    }

    internal override string Part2()
    {
        long total = num2.Sum();
        return $"Calibration sum for spelled and literal digits: <+white>{total}";
    }
}
