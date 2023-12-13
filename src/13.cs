using aoc2023.Util;

namespace aoc2023;

internal class Day13 : Day
{
    private int part1;
    private int part2;
    internal override void Parse()
    {
        var lines = Parsing.ReadAllLines($"{GetDay()}");

        List<string> grid = new();
        void ProcessGrid()
        {
            (int res1, int res2) = CheckGrid(grid);
            part1 += res1;
            part2 += res2;
        }

        foreach (var line in lines)
        {
            if (line.Length == 0)
            {
                ProcessGrid();
                grid.Clear();
            }
            else
            {
                grid.Add(line);
            }
        }

        ProcessGrid();
    }

    (int, int) CheckColumns(IList<string> grid)
    {
        int part1Columns = 0;
        int part2Columns = 0;

        for (int checkCol = 1; checkCol < grid[0].Length; checkCol++)
        {
            int columnSpread = System.Math.Min(checkCol, grid[0].Length - checkCol);
            int leftCol = checkCol - 1;
            int rightCol = checkCol;

            int matches = 0;

            for (int dx = 0; dx < columnSpread; dx++)
            {
                int leftMatch = leftCol - dx;
                int rightMatch = rightCol + dx;
                matches += grid.Count(l => l[leftMatch] == l[rightMatch]);
            }

            int maxCount = columnSpread * grid.Count;
            if (matches == maxCount)
            {
                part1Columns = checkCol;
            }
            else if (matches == maxCount - 1)
            {
                part2Columns = checkCol;
            }
        }

        return (part1Columns, part2Columns);
    }

    (int, int) CheckRows(IList<string> grid)
    {
        int part1Rows = 0;
        int part2Rows = 0;

        for (int checkRow = 1; checkRow < grid.Count; checkRow++)
        {
            int rowSpread = System.Math.Min(checkRow, grid.Count - checkRow);
            int upRow = checkRow - 1;
            int downRow = checkRow;

            int matches = 0;

            for (int dy = 0; dy < rowSpread; dy++)
            {
                int upMatch = upRow - dy;
                int downMatch = downRow + dy;

                string lineUp = grid[upMatch];
                string lineDown = grid[downMatch];

                for (int i = 0; i < lineUp.Length; i++)
                {
                    int c1 = lineUp[i];
                    int c2 = lineDown[i];
                    if (c1 == c2)
                    {
                        matches++;
                    }
                }
            }

            int maxCount = rowSpread * grid[0].Length;
            if (matches == maxCount)
            {
                part1Rows = checkRow;
            }
            else if (matches == maxCount - 1)
            {
                part2Rows = checkRow;
            }
        }

        return (part1Rows, part2Rows);
    }

    (int, int) CheckGrid(IList<string> grid)
    {
        (int part1Lefts, int part2Lefts) = CheckColumns(grid);
        (int part1Ups, int part2Ups) = CheckRows(grid);

        return (part1Lefts + part1Ups * 100, part2Lefts + part2Ups * 100);
    }

    internal override string Part1()
    {
        return $"Columns and rows total: <+white>{part1}";
    }

    internal override string Part2()
    {
        return $"Smudged columns and rows total: <+white>{part2}";
    }
}