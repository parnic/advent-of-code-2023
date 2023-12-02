namespace aoc2023;

internal class Day02 : Day
{
    struct gamepull
    {
        public int red;
        public int green;
        public int blue;
    }

    private Dictionary<int, List<gamepull>> games = new();
    internal override void Parse()
    {
        var lines = Util.Parsing.ReadAllLines($"{GetDay()}");
        foreach (var line in lines)
        {
            var game = new List<gamepull>();
            var gamedata = line.Split(": ");
            var gameNum = int.Parse(gamedata[0].Split(' ')[1]);
            var pulls = gamedata[1].Split("; ");
            foreach (var pull in pulls)
            {
                var p = new gamepull();
                var cubes = pull.Split(", ");
                foreach (var cube in cubes)
                {
                    var sp = cube.Split(' ');
                    var val = int.Parse(sp[0]);
                    if (sp[1] == "red")
                    {
                        p.red = val;
                    }
                    else if (sp[1] == "green")
                    {
                        p.green = val;
                    }
                    else if (sp[1] == "blue")
                    {
                        p.blue = val;
                    }
                }

                game.Add(p);
            }

            games.Add(gameNum, game);
        }
    }

    internal override string Part1()
    {
        long possible = games.Where(g => g.Value.All(p => p.red <= 12 && p.green <= 13 && p.blue <= 14))
            .Sum(g => g.Key);
        return $"Sum of IDs of possible 12r/13g/14b games: <+white>{possible}";
    }

    internal override string Part2()
    {
        long total = games.Sum(g => g.Value.Max(p => p.red) * g.Value.Max(p => p.green) * g.Value.Max(p => p.blue));
        return $"Sum of power of minimum cube sets: <+white>{total}";
    }
}
