using aoc2023.Util;

namespace aoc2023;

internal class Day17 : Day
{
    private int[,] heatmap = new int[1, 1];
    private int width;
    private int height;
    internal override void Parse()
    {
        var lines = Parsing.ReadAllLines($"{GetDay()}").ToList();
        height = lines.Count;
        width = lines[0].Length;
        heatmap = new int[width, height];
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                heatmap[col, row] = int.Parse(lines[row][col].ToString());
            }
        }
    }

    private static readonly ivec2[] directions = [ivec2.LEFT, ivec2.UP, ivec2.RIGHT, ivec2.DOWN];
    List<(ivec2 pos, int dirIdx, int steps)> GetNeighbors(ivec2 position, ivec2 direction, int steps, int minSteps, int maxSteps)
    {
        List<(ivec2 pos, int dirIdx, int steps)> neighbors = [];

        var idx = directions.IndexOf(direction);
        List<int> newDirectionsIdx = new(directions.Length);
        if (steps < maxSteps)
        {
            newDirectionsIdx.Add(idx);
        }

        if (steps >= minSteps)
        {
            for (int i = 1; i < directions.Length; i++)
            {
                newDirectionsIdx.Add((idx + i) % directions.Length);
            }
        }

        foreach (var dirIdx in newDirectionsIdx)
        {
            int stepsToDir = directions[dirIdx] == direction ? steps : 0;
            neighbors.Add((position + directions[dirIdx], dirIdx, stepsToDir + 1));
        }

        return neighbors;
    }

    private int Solve(int minSteps, int maxSteps)
    {
        int result = int.MaxValue;
        ivec2 goal = new(width - 1, height - 1);

        PriorityQueue<(ivec2 pos, ivec2 dir, int steps), int> pq = new();
        pq.Enqueue((new ivec2(0, 0), ivec2.RIGHT, 0), heatmap[0, 0]);
        pq.Enqueue((new ivec2(0, 0), ivec2.DOWN, 0), heatmap[0, 0]);
        Dictionary<int, int> visited = [];
        while (pq.TryDequeue(out (ivec2 pos, ivec2 dir, int steps) element, out int cost))
        {
            foreach (var neighbor in GetNeighbors(element.pos, element.dir, element.steps, minSteps, maxSteps))
            {
                if (!neighbor.pos.IsWithinRange(0, 0, width - 1, height - 1))
                {
                    continue;
                }

                int nextCost = cost + heatmap[neighbor.pos.x, neighbor.pos.y];
                var hash = HashCode.Combine(neighbor.pos, neighbor.dirIdx, neighbor.steps);
                visited.TryAdd(hash, int.MaxValue);
                if (nextCost < visited[hash] && nextCost < result)
                {
                    visited[hash] = nextCost;
                    if (neighbor.pos == goal && element.steps >= minSteps)
                    {
                        result = nextCost;
                    }
                    else
                    {
                        pq.Enqueue((neighbor.pos, directions[neighbor.dirIdx], neighbor.steps), nextCost);
                    }
                }
            }
        }

        return result + heatmap[goal.x, goal.y];
    }

    internal override string Part1()
    {
        var result = Solve(0, 3);
        return $"Least heat loss with 3 maximum steps in one direction: <+white>{result}";
    }

    internal override string Part2()
    {
        var result = Solve(4, 10);
        return $"Least heat loss with 4 minimum and 10 maximum steps in one direction: <+white>{result}";
    }
}
