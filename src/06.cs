namespace aoc2023;

internal class Day06 : Day
{
    private List<int> times = new();
    private List<int> dists = new();
    private long time = 0;
    private long dist = 0;
    internal override void Parse()
    {
        var lines = Util.Parsing.ReadAllLines($"{GetDay()}").ToList();
        var timeline = lines[0].Split(':');
        times = timeline[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
        time = long.Parse(timeline[1].Replace(" ", ""));
        var distline = lines[1].Split(':');
        dists = distline[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
        dist = long.Parse(distline[1].Replace(" ", ""));
    }

    internal override string Part1()
    {
        long total = 1;
        for (int i = 0; i < times.Count; i++)
        {
            long numWins = 0;
            bool hasWon = false;
            for (int chargetime = 1; chargetime < times[i]; chargetime++)
            {
                var d = chargetime * (times[i] - chargetime);
                if (d > dists[i])
                {
                    hasWon = true;
                    numWins++;
                }
                else if (hasWon)
                {
                    break;
                }
            }

            total *= numWins;
        }

        return $"Multiplied number of wins total: <+white>{total}";
    }

    internal override string Part2()
    {
        long numWins = 0;
        bool hasWon = false;
        for (int chargetime = 1; chargetime < time; chargetime++)
        {
            var d = chargetime * (time - chargetime);
            if (d > dist)
            {
                hasWon = true;
                numWins++;
            }
            else if (hasWon)
            {
                break;
            }
        }

        return $"Number of wins with correct kerning: <+white>{numWins}";
    }
}
