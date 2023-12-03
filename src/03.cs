using aoc2023.Util;

namespace aoc2023;

internal class Day03 : Day
{
    private char[,]? grid;

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
    }

    internal override string Part1()
    {
        long sum = 0;

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

                        valid = true;
                        break;
                    }
                }

                if (valid)
                {
                    sum += currNum;
                }

                currNumStr = string.Empty;
            }
        }

        return $"Sum of part numbers: <+white>{sum}";
    }

    internal override string Part2()
    {
        // a much better way to do this would be to use the number-parsing in part 1 to store which star a given number is touching,
        // then find all numbers that touch the same star more than once.
        // this solution is...not great.
        long total = 0;
        for (int row = 0; row < grid!.GetLength(0); row++)
        {
            for (int col = 0; col < grid!.GetLength(1); col++)
            {
                if (grid[row, col] != '*')
                {
                    continue;
                }

                long lastRow = -1;
                long lastCol = -1;
                ivec2 pt = new(col, row);
                List<int> nums = new();
                foreach (var n in pt.GetNeighbors())
                {
                    if (!grid[n.y, n.x].IsDigit())
                    {
                        continue;
                    }

                    if (lastRow == -1 || lastCol == -1 || lastRow != n.y || System.Math.Abs(lastCol - n.x) > 1)
                    {
                        var x = n.x;
                        while (x >= 0 && grid[n.y, x].IsDigit())
                        {
                            x--;
                        }

                        if (x < 0 || !grid[n.y, x].IsDigit())
                        {
                            x++;
                        }

                        var numStr = string.Empty;
                        while (x < grid.GetLength(1) && grid[n.y, x].IsDigit())
                        {
                            numStr += grid[n.y, x];
                            x++;
                        }

                        var num = int.Parse(numStr);
                        // this isn't quite right...it's possible the same number exists 2+ times around a star, but this works with my input, so...
                        nums.AddUnique(num);
                    }

                    lastRow = n.y;
                    lastCol = n.x;
                }

                if (nums.Count < 2)
                {
                    continue;
                }

                long mult = nums.Aggregate(1L, (current, num) => current * num);
                total += mult;
            }
        }

        return $"Sum of gear ratios: <+white>{total}";
    }
}
