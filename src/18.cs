using aoc2023.Util;

namespace aoc2023;

internal class Day18 : Day
{
    private List<(ivec2 dir, int dist, string color)> digplan = [];
    private List<ivec2> polygonVerts = [];
    private int perimeter = 0;

    internal override void Parse()
    {
        var lines = Parsing.ReadAllLines($"{GetDay()}").ToList();
        foreach (var line in lines)
        {
            var split = line.Split('(');
            var dir = split[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var color = split[1].Trim(')');
            var dirVec = dir[0] switch
            {
                "U" => ivec2.UP,
                "D" => ivec2.DOWN,
                "L" => ivec2.LEFT,
                "R" => ivec2.RIGHT,
                _ => throw new Exception(),
            };
            digplan.Add((dirVec, int.Parse(dir[1]), color));
        }
    }

    private bool[,] DigTrench_v1()
    {
        ivec2 currPos = ivec2.ZERO;
        polygonVerts.Add(currPos);
        long minx = long.MaxValue;
        long miny = long.MaxValue;
        long maxx = long.MinValue;
        long maxy = long.MinValue;
        foreach (var inst in digplan)
        {
            currPos += inst.dir * inst.dist;
            polygonVerts.Add(currPos);
            if (currPos.x < minx)
            {
                minx = currPos.x;
            }
            if (currPos.y < miny)
            {
                miny = currPos.y;
            }

            if (currPos.x > maxx)
            {
                maxx = currPos.x;
            }
            if (currPos.y > maxy)
            {
                maxy = currPos.y;
            }
        }

        long xoffset = -minx;
        long yoffset = -miny;
        bool[,] bounds = new bool[maxx + xoffset + 1, maxy + yoffset + 1];
        currPos = ivec2.ZERO;
        perimeter = 0;
        foreach (var inst in digplan)
        {
            bounds[currPos.x + xoffset, currPos.y + yoffset] = true;
            for (int i = 0; i < inst.dist; i++)
            {
                currPos += inst.dir;
                bounds[currPos.x + xoffset, currPos.y + yoffset] = true;
                perimeter++;
            }
        }

        return bounds;
    }

    private void Render(bool[,] grid)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Logger.Log(grid[x,y] ? Constants.SolidBlock.ToString() : " ");
            }
            Logger.LogLine("");
        }
    }

    private void FloodFill(bool[,] grid)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);
        Queue<ivec2> toPaint = new();
        bool last = false;
        int continuous = 0;
        for (int x = 0; x < width; x++)
        {
            if (grid[x, 1] != last)
            {
                last = grid[x, 1];
                continuous = 0;
            }
            else
            {
                continuous++;
            }

            if (!last && continuous == 0)
            {
                toPaint.Enqueue(new ivec2(x, 1));
                break;
            }
        }

        while (toPaint.TryDequeue(out ivec2 pt))
        {
            grid[pt.x, pt.y] = true;
            foreach (var n in pt.GetBoundedNeighbors(0, 0, width, height))
            {
                if (!grid[n.x, n.y] && !toPaint.Contains(n))
                {
                    toPaint.Enqueue(n);
                }
            }
        }
    }

    private static (List<ivec2>, long) DigTrench_v2(List<(ivec2 dir, int dist)> plan)
    {
        List<ivec2> verts = new(plan.Count + 1);
        var currPos = ivec2.ZERO;
        verts.Add(currPos);
        long perim = 0;
        foreach (var (dir, dist) in plan)
        {
            var last = currPos;
            currPos += dir * dist;
            perim += (long)(last - currPos).Length;
            verts.Add(currPos);
        }

        return (verts, perim);
    }

    // "shoelace theorem" plus accounting for perimeter...the perimeter adjustment feels weird, but sorta makes sense
    // in that the trench line can be considered to go through the middle of each square plus it shares a point with
    // the start and end, so...it just so happens that that worked out when i tried it. i'll take it.
    private static long ComputeArea(IReadOnlyList<ivec2> verts, long perim)
    {
        long s1 = 0;
        long s2 = 0;
        for (int i = 0; i < verts.Count - 1; i++)
        {
            var x1 = verts[i].x;
            var y2 = verts[i + 1].y;
            s1 += x1 * y2;
            var y1 = verts[i].y;
            var x2 = verts[i + 1].x;
            s2 += y1 * x2;
        }

        long area = System.Math.Abs(s1 - s2) / 2;
        long plusPerimeter = (perim / 2) + 1;
        return area + plusPerimeter;
    }

    internal override string Part1()
    {
        // var dug = DigTrench_v1();
        List<(ivec2 dir, int dist)> noColorPlan = new(digplan.Count);
        foreach (var (dir, dist, _) in digplan)
        {
            noColorPlan.Add((dir, dist));
        }

        var (verts, perim) = DigTrench_v2(noColorPlan);
        // Render(dug);
        var area = ComputeArea(verts, perim);
        // FloodFill(dug);
        // Render(dug);
        // int numFilled = 0;
        // int width = dug.GetLength(0);
        // int height = dug.GetLength(1);
        // for (int x = 0; x < width; x++)
        // {
        //     for (int y = 0; y < height; y++)
        //     {
        //         if (dug[x, y])
        //         {
        //             numFilled++;
        //         }
        //     }
        // }

        return $"Cubic meters: <+white>{area}";
    }

    internal override string Part2()
    {
        List<(ivec2 dir, int dist)> correctedPlan = new(digplan.Count);
        foreach (var (_, _, color) in digplan)
        {
            var hexDist = Convert.ToInt32(color[1..^1], 16);
            var hexDir = int.Parse(color[^1..]) switch
            {
                0 => ivec2.RIGHT,
                1 => ivec2.DOWN,
                2 => ivec2.LEFT,
                3 => ivec2.UP,
                _ => throw new Exception()
            };

            correctedPlan.Add((hexDir, hexDist));
        }

        var (verts, perim) = DigTrench_v2(correctedPlan);
        var area = ComputeArea(verts, perim);

        return $"Cubic meters of corrected instructions: <+white>{area}";
    }
}
