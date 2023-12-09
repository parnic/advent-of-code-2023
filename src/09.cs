namespace aoc2023;

internal class Day09 : Day
{
    private List<List<long>> sequences = new();
    internal override void Parse()
    {
        var lines = Util.Parsing.ReadAllLines($"{GetDay()}");
        foreach (var line in lines)
        {
            sequences.Add(new List<long>(line.Split().Select(long.Parse)));
        }
    }

    private List<long> GetDiffs(List<long> list)
    {
        List<long> ret = new(list.Count);
        for (int i = 1; i < list.Count; i++)
        {
            ret.Add(list[i] - list[i-1]);
        }

        return ret;
    }

    private List<List<long>> GetDiffLists(List<long> list)
    {
        List<List<long>> diffLists = new(list.Count);
        List<long> diffs = GetDiffs(list);
        diffLists.Add(diffs);
        while (diffs.Any(x => x != 0))
        {
            diffs = GetDiffs(diffs);
            diffLists.Add(diffs);
        }

        return diffLists;
    }

    private long GetNextNum(List<long> list)
    {
        List<List<long>> diffLists = GetDiffLists(list);

        long nextVal = 0;
        for (int i = diffLists.Count - 1; i > 0; i--)
        {
            nextVal = diffLists[i].Last() + diffLists[i - 1].Last();
            diffLists[i - 1].Add(nextVal);
        }

        return nextVal + list.Last();
    }

    private long GetPrevNum(List<long> list)
    {
        List<List<long>> diffLists = GetDiffLists(list);

        long nextVal = 0;
        for (int i = diffLists.Count - 1; i > 0; i--)
        {
            nextVal = diffLists[i - 1].First() - diffLists[i].First();
            diffLists[i - 1].Insert(0, nextVal);
        }

        return list.First() - nextVal;
    }

    internal override string Part1()
    {
        long total = 0;
        foreach (var seq in sequences)
        {
            long nextnum = GetNextNum(seq);
            total += nextnum;
        }

        return $"Sum of all next values: <+white>{total}";
    }

    internal override string Part2()
    {
        long total = 0;
        foreach (var seq in sequences)
        {
            long prevnum = GetPrevNum(seq);
            total += prevnum;
        }

        return $"Sum of all previous values: <+white>{total}";
    }
}
