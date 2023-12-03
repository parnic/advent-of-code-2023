using aoc2023.Util;

namespace aoc2023;

internal class Day03 : Day
{
    private char[,]? grid;
    private readonly List<int> partNums = new();
    private readonly Dictionary<ivec2, List<int>> gears = new();

    internal override void Parse()
    {
        var lines = Util.Parsing.ReadAllLines($"{GetDay()}").ToList();
        grid = new char[lines.Count, lines[0].Length];
        for (int row = 0; row < lines.Count; row++)
        {
            for (int col = 0; col < lines[row].Length; col++)
            {
                grid[row, col] = lines[row][col];
            }
        }

        string currNumStr = "";
        for (int row = 0; row < grid!.GetLength(0); row++)
        {
            for (int col = 0; col < grid!.GetLength(1); col++)
            {
                if (grid[row, col].IsDigit())
                {
                    currNumStr += grid[row, col];
                }

                if ((grid[row, col].IsDigit() && col != grid!.GetLength(1) - 1) || currNumStr.Length == 0)
                {
                    continue;
                }

                bool valid = false;
                var currNum = int.Parse(currNumStr);
                for (int i = 1; i <= currNumStr.Length && !valid; i++)
                {
                    ivec2 pt = new(col - i, row);
                    foreach (var n in pt.GetNeighbors())
                    {
                        if (n.x < 0 || n.x >= grid.GetLength(1) || n.y < 0 || n.y >= grid.GetLength(0))
                        {
                            continue;
                        }

                        if (grid[n.y, n.x] == '.' || grid[n.y, n.x].IsDigit())
                        {
                            continue;
                        }

                        if (grid[n.y, n.x] == '*')
                        {
                            if (!gears.ContainsKey(n))
                            {
                                gears.Add(n, new List<int>());
                            }

                            gears[n].Add(currNum);
                        }

                        valid = true;
                    }
                }

                if (valid)
                {
                    partNums.Add(currNum);
                }

                currNumStr = string.Empty;
            }
        }
    }

    internal override string Part1()
    {
        long total = partNums.Sum();

        return $"Sum of part numbers: <+white>{total}";
    }

    internal override string Part2()
    {
        long total = gears.Where(gear => gear.Value.Count == 2)
            .Sum(gear => gear.Value.Aggregate(1L, (current, num) => current * num));

        return $"Sum of gear ratios: <+white>{total}";
    }
}
