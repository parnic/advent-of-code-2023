using System.Text;
using aoc2023.Util;

namespace aoc2023;

internal class Day14 : Day
{
    private char[,] platform = new char[1, 1];
    private int width;
    private int height;
    internal override void Parse()
    {
        var lines = Parsing.ReadAllLines($"{GetDay()}").ToList();
        platform = new char[lines[0].Length, lines.Count];
        for (int row = 0; row < lines.Count; row++)
        {
            for (int col = 0; col < lines[row].Length; col++)
            {
                platform[col, row] = lines[row][col];
                width = col + 1;
            }

            height = row + 1;
        }
    }

    private void Render(char[,] grid)
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Logger.Log($"{grid[j,i]}");
            }
            Logger.LogLine("");
        }
    }

    private void Tilt(char[,] grid, ivec2 direction)
    {
        int initialX = 0;
        int initialY = 0;
        var xCond = (int v) => v < width;
        var yCond = (int v) => v < height;
        var xInc = (int x) => x + 1;
        var yInc = (int y) => y + 1;

        if (direction.x > 0 || direction.y > 0)
        {
            initialX = width - 1;
            initialY = height - 1;
            xCond = v => v >= 0;
            yCond = v => v >= 0;
            xInc = x => x - 1;
            yInc = y => y - 1;
        }

        for (int row = initialY; yCond(row); row = yInc(row))
        {
            for (int col = initialX; xCond(col); col = xInc(col))
            {
                if (grid[col, row] != 'O')
                {
                    continue;
                }

                var next = new ivec2(col, row);
                while (true)
                {
                    next += direction;
                    if (next.x < 0 || next.y < 0 || next.x >= width || next.y >= height)
                    {
                        break;
                    }

                    if (grid[next.x, next.y] != '.')
                    {
                        break;
                    }

                    grid[next.x - direction.x, next.y - direction.y] = '.';
                    grid[next.x, next.y] = 'O';
                }
            }
        }
    }

    private long GetLoad(char[,] grid)
    {
        long total = 0;
        for (int row = 0; row < height; row++)
        {
            int multiplier = height - row;
            for (int col = 0; col < width; col++)
            {
                if (grid[col, row] == 'O')
                {
                    total += multiplier;
                }
            }
        }

        return total;
    }

    private string Stringify(char[,] grid)
    {
        StringBuilder sb = new(width * height);
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                sb.Append(grid[col, row]);
            }
        }

        return sb.ToString();
    }

    private char[,] Arrayify(string grid)
    {
        char[,] result = new char[width, height];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                result[j, i] = grid[(i * height) + j];
            }
        }

        return result;
    }

    internal override string Part1()
    {
        char[,] part1 = new char[width, height];
        Array.Copy(platform, part1, width * height);

        Tilt(part1, new ivec2(0, -1));

        long total = GetLoad(part1);

        return $"Load on north support beam: <+white>{total}";
    }

    internal override string Part2()
    {
        char[,] part2 = new char[width, height];
        Array.Copy(platform, part2, width * height);

        List<string> seen = [];

        var northDir = new ivec2(0, -1);
        var eastDir = new ivec2(1, 0);
        var southDir = new ivec2(0, 1);
        var westDir = new ivec2(-1, 0);
        long loopCycle = 0;
        long cycle;
        for (cycle = 0; cycle < 1000000000L; cycle++)
        {
            Tilt(part2, northDir);
            Tilt(part2, westDir);
            Tilt(part2, southDir);
            Tilt(part2, eastDir);

            var result = Stringify(part2);
            if (seen.Any(g => g == result))
            {
                int periodStart = seen.FindIndex(g => g == result);
                int periodLen = seen.Count - periodStart;
                loopCycle = ((1000000000L - periodStart) % periodLen) + periodStart;
                break;
            }
            seen.Add(result);
        }

        var finalGrid = Arrayify(seen[(int)loopCycle - 1]);
        var load = GetLoad(finalGrid);
        return $"North load after 1000000000 (determined after {cycle + 1}) cycles would be: <+white>{load}";
    }
}
