using aoc2023.Util;

namespace aoc2023;

internal class Day21 : Day
{
    private int width;
    private int height;
    private readonly HashSet<ivec2> garden = [];
    private ivec2 start = new(0, 0);

    internal override void Parse()
    {
        var lines = Parsing.ReadAllLines($"{GetDay()}").ToList();
        height = lines.Count;
        for (int y = 0; y < height; y++)
        {
            width = lines[y].Length;
            for (int x = 0; x < width; x++)
            {
                if (lines[y][x] == '.' || lines[y][x] == 'S')
                {
                    var pt = new ivec2(x, y);
                    garden.Add(pt);

                    if (lines[y][x] == 'S')
                    {
                        start = pt;
                    }
                }
            }
        }
    }

    private long FillGrid(long startY, long startX, long steps)
    {
        HashSet<(long, long)> filled = [];
        HashSet<(long, long)> visited = [];
        Queue<(long y, long x, long step)> q = [];
        q.Enqueue((startY, startX, steps));

        while (q.TryDequeue(out (long y, long x, long step) currLoc))
        {
            if (currLoc.step % 2 == 0)
            {
                filled.Add((currLoc.y, currLoc.x));
            }

            if (currLoc.step == 0)
            {
                continue;
            }

            foreach (var (newY, newX) in ((long, long c)[]) [(currLoc.y + 1, c: currLoc.x), (currLoc.y - 1, c: currLoc.x), (currLoc.y, currLoc.x + 1), (currLoc.y, currLoc.x - 1)])
            {
                if (newY < 0 || newY >= height || newX < 0 || newX >= width || !garden.Contains(new ivec2(newY, newX)) ||
                    !visited.Add((newY, newX)))
                {
                    continue;
                }

                q.Enqueue((newY, newX, currLoc.step - 1));
            }
        }

        return filled.Count;
    }

    internal override string Part1()
    {
        HashSet<ivec2> next = [start];
        HashSet<ivec2> reachableNow = [];
        const int numSteps = 64;
        for (int step = 0; step < numSteps; step++)
        {
            HashSet<ivec2> curr = [..next];
            next.Clear();
            foreach (var pos in curr)
            {
                foreach (var neighbor in pos.GetBoundedOrthogonalNeighbors(0, 0, width, height))
                {
                    if (!garden.Contains(neighbor))
                    {
                        continue;
                    }

                    if (step == numSteps - 1)
                    {
                        reachableNow.Add(neighbor);
                    }

                    next.Add(neighbor);
                }
            }
        }

        long numVisited = reachableNow.Count;
        return $"Garden plots reachable in 24 steps: <+white>{numVisited}";
    }

    internal override string Part2()
    {
        const long steps = 26501365;
        long gridWidth = steps / width - 1;
        long odd = (long)System.Math.Pow(gridWidth / 2 * 2 + 1, 2);
        long even = (long)System.Math.Pow((gridWidth + 1) / 2 * 2, 2);

        long oddPoints = FillGrid(start.y, start.x, width * 2 + 1);
        long eventPoints = FillGrid(start.y, start.x, width * 2);

        long cornerTop = FillGrid(width - 1, start.x, width - 1);
        long cornerRight = FillGrid(start.y, 0, width - 1);
        long cornerBottom = FillGrid(0, start.x, width - 1);
        long cornerLeft = FillGrid(start.y, width - 1, width - 1);

        long smallTopRight = FillGrid(width - 1, 0, width / 2 - 1);
        long smallTopLeft = FillGrid(width - 1, width - 1, width / 2 - 1);
        long smallBottomRight = FillGrid(0, 0, width / 2 - 1);
        long smallBottomLeft = FillGrid(0, width - 1, width / 2 - 1);

        long largeTopRight = FillGrid(width - 1, 0, width * 3 / 2 - 1);
        long largeTopLeft = FillGrid(width - 1, width - 1, width * 3 / 2 - 1);
        long largeBottomRight = FillGrid(0, 0, width * 3 / 2 - 1);
        long largeBottomLeft = FillGrid(0, width - 1, width * 3 / 2 - 1);

        long numVisited = odd * oddPoints +
                          even * eventPoints +
                          cornerTop + cornerRight + cornerBottom + cornerLeft +
                          (gridWidth + 1) * (smallTopRight + smallTopLeft + smallBottomRight + smallBottomLeft) +
                          gridWidth * (largeTopRight + largeTopLeft + largeBottomRight + largeBottomLeft);
        return $"Garden plots reachable in {steps:N0} steps: <+white>{numVisited}";
    }
}
