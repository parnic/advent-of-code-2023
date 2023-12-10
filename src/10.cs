using System.Diagnostics;
using aoc2023.Util;

namespace aoc2023;

internal class Day10 : Day
{
    enum tiletype
    {
        ns,
        ew,
        ne,
        nw,
        sw,
        se,
        ground,
        start,
    }

    private readonly Dictionary<ivec2, tiletype> grid = [];
    private readonly Dictionary<ivec2, int> pipeLocations = [];
    private ivec2 startLoc;
    private int width;
    private int height;
    internal override void Parse()
    {
        var lines = Parsing.ReadAllLines($"{GetDay()}").ToList();
        for (int row = 0; row < lines.Count; row++)
        {
            for (int col = 0; col < lines[row].Length; col++)
            {
                var loc = new ivec2(col, row);
                var tileType = lines[row][col] switch
                {
                    '|' => tiletype.ns,
                    '-' => tiletype.ew,
                    'L' => tiletype.ne,
                    'J' => tiletype.nw,
                    '7' => tiletype.sw,
                    'F' => tiletype.se,
                    '.' => tiletype.ground,
                    'S' => tiletype.start,
                    _ => throw new Exception(),
                };

                width = col + 1;

                grid.Add(loc, tileType);
            }

            height = row + 1;
        }

        FindStartTileType();
        PopulatePipeLoop();
    }

    private void FindStartTileType()
    {
        var start = grid.First(t => t.Value == tiletype.start);
        List<tiletype> possibleTypes =
        [
            tiletype.ns,
            tiletype.ew,
            tiletype.ne,
            tiletype.nw,
            tiletype.sw,
            tiletype.se
        ];

        // this only works as written for start points not on the outside edge. i'm certain there are much better ways
        // of doing this...
        foreach (var n in start.Key.GetOrthogonalNeighbors())
        {
            if (n.x < start.Key.x)
            {
                if (grid[n] == tiletype.ew || grid[n] == tiletype.ne || grid[n] == tiletype.se)
                {
                    possibleTypes.Remove(tiletype.ns);
                    possibleTypes.Remove(tiletype.ne);
                    possibleTypes.Remove(tiletype.se);
                }

                if (grid[n] == tiletype.ground)
                {
                    possibleTypes.Remove(tiletype.ew);
                    possibleTypes.Remove(tiletype.sw);
                    possibleTypes.Remove(tiletype.nw);
                }
            }
            else if (n.x > start.Key.x)
            {
                if (grid[n] == tiletype.ew || grid[n] == tiletype.nw || grid[n] == tiletype.sw)
                {
                    possibleTypes.Remove(tiletype.ns);
                    possibleTypes.Remove(tiletype.nw);
                    possibleTypes.Remove(tiletype.sw);
                }

                if (grid[n] == tiletype.ground)
                {
                    possibleTypes.Remove(tiletype.ew);
                    possibleTypes.Remove(tiletype.ne);
                    possibleTypes.Remove(tiletype.se);
                }
            }
            else if (n.y > start.Key.y)
            {
                if (grid[n] == tiletype.ns || grid[n] == tiletype.ne || grid[n] == tiletype.nw)
                {
                    possibleTypes.Remove(tiletype.ew);
                    possibleTypes.Remove(tiletype.ne);
                    possibleTypes.Remove(tiletype.nw);
                }

                if (grid[n] == tiletype.ground)
                {
                    possibleTypes.Remove(tiletype.ns);
                    possibleTypes.Remove(tiletype.se);
                    possibleTypes.Remove(tiletype.sw);
                }
            }
            else if (n.y < start.Key.y)
            {
                if (grid[n] == tiletype.ns || grid[n] == tiletype.sw || grid[n] == tiletype.se)
                {
                    possibleTypes.Remove(tiletype.ew);
                    possibleTypes.Remove(tiletype.sw);
                    possibleTypes.Remove(tiletype.se);
                }

                if (grid[n] == tiletype.ground)
                {
                    possibleTypes.Remove(tiletype.ns);
                    possibleTypes.Remove(tiletype.ne);
                    possibleTypes.Remove(tiletype.nw);
                }
            }
        }
        Debug.Assert(possibleTypes.Count == 1);
        startLoc = start.Key;
        grid[start.Key] = possibleTypes[0];
    }

    private void PopulatePipeLoop()
    {
        pipeLocations.Add(startLoc, 0);
        Queue<(ivec2 loc, int dist)> toCheck = new();
        toCheck.Enqueue((startLoc, 0));
        while (toCheck.Count != 0)
        {
            var curr = toCheck.Dequeue();
            foreach (var n in GetValidOrthogonalNeighbors(curr.loc))
            {
                if (pipeLocations.TryAdd(n, curr.dist + 1))
                {
                    toCheck.Enqueue((n, curr.dist + 1));
                }
            }
        }
    }

    IEnumerable<ivec2> GetValidOrthogonalNeighbors(ivec2 p)
    {
        var neighbors = p.GetOrthogonalNeighbors();
        foreach (var n in neighbors)
        {
            if (n.x < p.x && (grid[p] == tiletype.ew || grid[p] == tiletype.nw || grid[p] == tiletype.sw))
            {
                yield return n;
            }
            else if (n.x > p.x && (grid[p] == tiletype.ew || grid[p] == tiletype.ne || grid[p] == tiletype.se))
            {
                yield return n;
            }
            else if (n.y > p.y && (grid[p] == tiletype.ns || grid[p] == tiletype.se || grid[p] == tiletype.sw))
            {
                yield return n;
            }
            else if (n.y < p.y && (grid[p] == tiletype.ns || grid[p] == tiletype.ne || grid[p] == tiletype.nw))
            {
                yield return n;
            }
        }
    }

    internal override string Part1()
    {
        // render your map!
        // for (int row = 0; row < height; row++)
        // {
        //     for (int col = 0; col < width; col++)
        //     {
        //         if (visited.ContainsKey(new ivec2(col, row)))
        //         {
        //             Console.Write(Constants.SolidBlock);
        //         }
        //         else
        //         {
        //             Console.Write('.');
        //         }
        //     }
        //     Console.WriteLine();
        // }

        int furthestLocation = pipeLocations.Max(v => v.Value);
        return $"Furthest point from start requires #steps: <+white>{furthestLocation}";
    }

    internal override string Part2()
    {
        // flood fill the outside (i don't think this is necessary, but it was my initial naive attempt)
        HashSet<ivec2> excluded = [new ivec2(0, 0)];
        Queue<ivec2> toCheck = new();
        toCheck.Enqueue(new ivec2(0, 0));
        while (toCheck.Count != 0)
        {
            var curr = toCheck.Dequeue();
            foreach (var n in curr.GetOrthogonalNeighbors())
            {
                if (n.x < 0 || n.y < 0 || n.x >= width || n.y >= height)
                {
                    continue;
                }

                if (excluded.Contains(n) || pipeLocations.ContainsKey(n))
                {
                    continue;
                }

                excluded.Add(n);
                toCheck.Enqueue(n);
            }
        }

        int interior = 0;
        for (int row = 0; row < height; row++)
        {
            bool bInside = false;
            for (int col = 0; col < width; col++)
            {
                var pt = new ivec2(col, row);
                if (excluded.Contains(pt))
                {
                    continue;
                }

                // each time we cross over a part of the shape that has a north-facing component, we are entering
                // or exiting the interior of the shape. checking for a north-facing component allows us to ensure
                // we solve the edge case of a top or bottom edge of the shape; we're effectively comparing against
                // the row before us to see if this point is enclosed.
                if (pipeLocations.ContainsKey(pt) &&
                    (grid[pt] == tiletype.ne || grid[pt] == tiletype.ns || grid[pt] == tiletype.nw))
                {
                    bInside = !bInside;
                    continue;
                }

                if (bInside && !pipeLocations.ContainsKey(pt))
                {
                    interior++;
                }
            }
        }

        return $"Number of spaces interior to the pipeline: <+white>{interior}";
    }
}
