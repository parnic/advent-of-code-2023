namespace aoc2023;

internal class Day05 : Day
{
    record map(long destStart, long sourceStart, long len)
    {
        public long Translate(long input)
        {
            if (input >= sourceStart && input < sourceStart + len)
            {
                var offset = input - sourceStart;
                return destStart + offset;
            }

            return input;
        }

        public long ReverseTranslate(long input)
        {
            if (input >= destStart && input < destStart + len)
            {
                var offset = input - destStart;
                return sourceStart + offset;
            }

            return input;
        }
    }

    private long TranslateInMapSet(List<map> set, long input)
    {
        foreach (var m in set)
        {
            var translated = m.Translate(input);
            if (translated != input)
            {
                return translated;
            }
        }

        return input;
    }

    private long ReverseTranslateInMapSet(List<map> set, long input)
    {
        foreach (var m in set)
        {
            var translated = m.ReverseTranslate(input);
            if (translated != input)
            {
                return translated;
            }
        }

        return input;
    }

    private List<long> seeds = new();
    private List<List<map>> maps = new();
    internal override void Parse()
    {
        var lines = Util.Parsing.ReadAllLines($"{GetDay()}");
        int mapNum = -1;
        foreach (var line in lines)
        {
            if (seeds.Count == 0)
            {
                var split = line.Split(": ");
                seeds.AddRange(split[1].Split(' ').Select(long.Parse));
                continue;
            }
            if (string.IsNullOrWhiteSpace(line))
            {
                mapNum++;
                continue;
            }

            if (line.Contains(':'))
            {
                maps.Add(new());
                continue;
            }

            var nums = line.Split(' ').Select(long.Parse).ToArray();
            maps[mapNum].Add(new(nums[0], nums[1], nums[2]));
        }
    }

    internal override string Part1()
    {
        long lowest = long.MaxValue;
        foreach (var seed in seeds)
        {
            long curr = seed;
            foreach (var set in maps)
            {
                curr = TranslateInMapSet(set, curr);
            }

            if (curr < lowest)
            {
                lowest = curr;
            }
        }

        return $"Lowest location number: <+white>{lowest}";
    }

    internal override string Part2()
    {
        List<(long start, long len)> seedranges = new();
        for (int i = 0; i < seeds.Count; i += 2)
        {
            seedranges.Add((seeds[i], seeds[i+1]));
        }

        for (long i = 0; i < long.MaxValue; i++)
        {
            long pos = i;
            for (int j = maps.Count - 1; j >= 0; j--)
            {
                pos = ReverseTranslateInMapSet(maps[j], pos);
            }

            foreach (var range in seedranges)
            {
                if (pos >= range.start && pos < range.start + range.len)
                {
                    return $"Lowest location number in seed ranges is at seed {pos}: <+white>{i}";
                }
            }
        }

        throw new Exception("didn't find it...");
    }
}
