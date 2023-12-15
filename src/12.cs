using aoc2023.Util;

namespace aoc2023;

internal class Day12 : Day
{
    private readonly List<(string puzzle, List<int> slots)> springList = [];

    internal override void Parse()
    {
        var lines = Parsing.ReadAllLines($"{GetDay()}");
        foreach (var line in lines)
        {
            var split = line.Split(' ');
            var nums = split[1].Split(',').Select(int.Parse);
            springList.Add((split[0], nums.ToList()));
        }
    }

    HashSet<string> GeneratePossibilities(string input)
    {
        Queue<string> possibilities = new();
        possibilities.Enqueue(input);
        while (true)
        {
            var p = possibilities.Peek();
            if (!p.Contains('?'))
            {
                break;
            }

            possibilities.Dequeue();
            for (int j = 0; j < p.Length; j++)
            {
                if (p[j] == '#' || p[j] == '.')
                {
                    continue;
                }

                possibilities.Enqueue(p.ReplaceAt(j, '#'));
                possibilities.Enqueue(p.ReplaceAt(j, '.'));
                break;
            }
        }

        return possibilities.ToHashSet();
    }

    bool IsValidString(string p, List<int> springs)
    {
        int group = 0;
        int count = 0;
        int i;
        for (i = 0; i <= p.Length; i++)
        {
            if (i == p.Length)
            {
                if (group >= 0 && group < springs.Count && springs[group] == count)
                {
                    group++;
                    count = 0;
                }

                break;
            }
            if (p[i] == '#')
            {
                count++;
                if (group >= 0 && group < springs.Count)
                {
                    if (count > springs[group])
                    {
                        break;
                    }
                }
                continue;
            }

            if (p[i] == '.')
            {
                if (group >= 0 && group < springs.Count)
                {
                    if (springs[group] == count)
                    {
                        group++;
                        count = 0;
                    }
                    else if (count > 0)
                    {
                        break;
                    }
                }
            }
        }

        var isValid = i == p.Length && group == springs.Count && count == 0;
        return isValid;
    }

    long GetValidStrings(HashSet<string> possibilities, List<int> springs)
    {
        long total = 0;
        foreach (var p in possibilities)
        {
            if (IsValidString(p, springs))
            {
                total++;
            }
        }

        return total;
    }

    long Solve(List<(string puzzle, List<int> slots)> list)
    {
        long numMatching = 0;
        foreach (var line in list)
        {
            HashSet<string> possibilities = GeneratePossibilities(line.puzzle);
            long thisValid = GetValidStrings(possibilities, line.slots);
            numMatching += thisValid;
        }

        return numMatching;
    }
    // all of the above was my original implementation that worked for part 1, but was far, far too slow for part 2

    // this is a solution very heavily cribbed from a post on the subreddit as i was just not understanding how to
    // optimize my existing solution appropriately.
    long GetPermutations(string row, List<int> pattern)
    {
        Dictionary<(int group, long amount), long> permutations = [];
        permutations.Add((0, 0), 1);

        foreach (var ch in row)
        {
            List<(int groupID, long groupAmount, long permutations)> next = [];
            foreach (var kvp in permutations)
            {
                int groupID = kvp.Key.group;
                long groupAmount = kvp.Key.amount;
                if (ch != '#')
                {
                    if (groupAmount == 0)
                    {
                        next.Add((groupID, groupAmount, kvp.Value));
                    }
                    else if (groupAmount == pattern[groupID])
                    {
                        next.Add((groupID + 1, 0, kvp.Value));
                    }
                }

                if (ch != '.')
                {
                    if (groupID < pattern.Count && groupAmount < pattern[groupID])
                    {
                        next.Add((groupID, groupAmount + 1, kvp.Value));
                    }
                }
            }

            permutations.Clear();
            foreach (var tuple in next)
            {
                var key = (tuple.groupID, tuple.groupAmount);
                if (!permutations.TryAdd(key, tuple.permutations))
                {
                    permutations[key] += tuple.permutations;
                }
            }
        }

        long total = 0;
        foreach (var kvp in permutations)
        {
            if (isValid(kvp.Key.group, kvp.Key.amount))
            {
                total += kvp.Value;
            }
        }

        return total;

        bool isValid(int groupID, long groupAmount)
        {
            return groupID == pattern.Count || (groupID == pattern.Count - 1 && groupAmount == pattern[groupID]);
        }
    }

    internal override string Part1()
    {
        long total = Solve(springList);

        return $"<+white>{total}";
    }

    internal override string Part2()
    {
        List<(string puzzle, List<int> slots)> unfolded = new(springList.Count);
        foreach (var folded in springList)
        {
            string puzzle2 = string.Empty;
            List<int> springs = new(folded.slots.Count * 5);
            for (int i = 0; i < 5; i++)
            {
                if (puzzle2.Length > 0)
                {
                    puzzle2 += "?";
                }
                puzzle2 += folded.puzzle;
                springs.AddRange(folded.slots);
            }

            unfolded.Add((puzzle2, springs));
        }

        long total = unfolded.Sum(item => GetPermutations(item.puzzle, item.slots));

        return $"<+white>{total}";
    }
}
