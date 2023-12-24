using aoc2023.Util;

namespace aoc2023;

internal class Day23 : Day
{
    private IEnumerable<ivec2> GetNeighbors(ivec2 pos, bool oneWaySlopes = true)
    {
        if (grid[pos] != null && oneWaySlopes)
        {
            yield return (pos + grid[pos]!).Value;
        }
        else
        {
            foreach (var p in pos.GetOrthogonalNeighbors())
            {
                if (grid.ContainsKey(p))
                {
                    yield return p;
                }
            }
        }
    }

    private int width;
    private int height;
    private readonly Dictionary<ivec2, ivec2?> grid = [];
    private ivec2 start = ivec2.ZERO;
    private ivec2 end = ivec2.ZERO;

    internal override void Parse()
    {
        var lines = Parsing.ReadAllLines($"{GetDay()}").ToList();
        height = lines.Count;
        for (int row = 0; row < height; row++)
        {
            width = lines[row].Length;
            for (int col = 0; col < width; col++)
            {
                if (lines[row][col] == '#')
                {
                    continue;
                }

                if (lines[row][col] == '.')
                {
                    var pt = new ivec2(col, row);
                    grid.Add(pt, null);
                    if (row == 0)
                    {
                        start = pt;
                    }
                    else if (row == height - 1)
                    {
                        end = pt;
                    }
                    continue;
                }

                grid.Add(new ivec2(col, row), lines[row][col] switch
                {
                    '>' => ivec2.RIGHT,
                    '<' => ivec2.LEFT,
                    '^' => ivec2.UP,
                    'v' => ivec2.DOWN,
                    _ => throw new Exception(),
                });
            }
        }
    }

    private void Render(Dictionary<(ivec2, ivec2), int> visited)
    {
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                var pt = new ivec2(col, row);
                if (!grid.ContainsKey(pt))
                {
                    Logger.Log(Constants.SolidBlock.ToString());
                    continue;
                }

                bool skip = false;
                foreach (var v in visited)
                {
                    if (v.Key.Item1 == pt)
                    {
                        Logger.Log("O");
                        skip = true;
                        break;
                    }
                }

                if (skip)
                {
                    continue;
                }

                if (grid[pt] == null)
                {
                    Logger.Log(" ");
                    continue;
                }

                if (grid[pt] == ivec2.RIGHT)
                {
                    Logger.Log(">");
                }
                if (grid[pt] == ivec2.DOWN)
                {
                    Logger.Log("v");
                }
                if (grid[pt] == ivec2.UP)
                {
                    Logger.Log("^");
                }
                if (grid[pt] == ivec2.LEFT)
                {
                    Logger.Log("<");
                }
            }
            Logger.LogLine("");
        }
    }

    int GetLongestDistanceFrom(Dictionary<ivec2, List<(ivec2, int)>> graph, ivec2 pt, HashSet<ivec2> seen)
    {
        if (pt == end)
        {
            return 0;
        }

        var m = int.MinValue;
        seen.Add(pt);
        foreach (var pos in graph[pt])
        {
            if (!seen.Contains(pos.Item1))
            {
                m = System.Math.Max(m, GetLongestDistanceFrom(graph, pos.Item1, seen) + pos.Item2);
            }
        }

        seen.Remove(pt);

        return m;
    }

    internal override string Part1()
    {
        Dictionary<ivec2, List<(ivec2, int)>> graph = [];

        HashSet<ivec2> forks = [start, end];
        foreach (var pt in grid)
        {
            if (GetNeighbors(pt.Key).Count() >= 3)
            {
                forks.Add(pt.Key);
                graph.Add(pt.Key, []);
            }
        }

        HashSet<ivec2> seen = [];
        foreach (var forkPoint in forks)
        {
            Stack<(ivec2, int)> s = [];
            s.Push((forkPoint, 0));
            seen.Clear();
            seen.Add(forkPoint);

            while (s.TryPop(out (ivec2 pos, int cost) next))
            {
                if (next.cost != 0 && forks.Contains(next.pos))
                {
                    graph.TryAdd(forkPoint, []);
                    graph[forkPoint].Add((next.pos, next.cost));
                    continue;
                }

                foreach (var neighbor in GetNeighbors(next.pos))
                {
                    if (seen.Contains(neighbor))
                    {
                        continue;
                    }

                    s.Push((neighbor, next.cost + 1));
                    seen.Add(neighbor);
                }
            }
        }

        seen.Clear();
        int dist = GetLongestDistanceFrom(graph, start, seen);

        // Render();

        return $"# steps in longest hike: <+white>{dist}";
    }

    internal override string Part2()
    {
        Dictionary<ivec2, List<(ivec2, int)>> graph = [];

        HashSet<ivec2> forks = [start, end];
        foreach (var pt in grid)
        {
            if (GetNeighbors(pt.Key, false).Count() >= 3)
            {
                forks.Add(pt.Key);
                graph.Add(pt.Key, []);
            }
        }

        HashSet<ivec2> seen = [];
        foreach (var forkPoint in forks)
        {
            Stack<(ivec2, int)> s = [];
            s.Push((forkPoint, 0));
            seen.Clear();
            seen.Add(forkPoint);

            while (s.TryPop(out (ivec2 pos, int cost) next))
            {
                if (next.cost != 0 && forks.Contains(next.pos))
                {
                    graph.TryAdd(forkPoint, []);
                    graph[forkPoint].Add((next.pos, next.cost));
                    continue;
                }

                foreach (var neighbor in GetNeighbors(next.pos, false))
                {
                    if (seen.Contains(neighbor))
                    {
                        continue;
                    }

                    s.Push((neighbor, next.cost + 1));
                    seen.Add(neighbor);
                }
            }
        }

        seen.Clear();
        int dist = GetLongestDistanceFrom(graph, start, seen);

        return $"# steps in longest hike ignoring slopes: <+white>{dist}";
    }
}
