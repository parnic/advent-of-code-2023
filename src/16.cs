using aoc2023.Util;

namespace aoc2023;

internal class Day16 : Day
{
    private enum tiletype
    {
        empty,
        fmirror,
        bmirror,
        vsplitter,
        hsplitter,
    }

    private readonly Dictionary<tiletype, Dictionary<ivec2, ivec2>> reflectMap = [];

    private record beaminfo(ivec2 loc, ivec2 dir);

    private tiletype[,] grid = new tiletype[1,1];
    private int width;
    private int height;

    internal override void Parse()
    {
        reflectMap.Add(tiletype.fmirror, new Dictionary<ivec2, ivec2>()
        {
            {ivec2.RIGHT, ivec2.UP},
            {ivec2.DOWN, ivec2.LEFT},
            {ivec2.LEFT, ivec2.DOWN},
            {ivec2.UP, ivec2.RIGHT},
        });
        reflectMap.Add(tiletype.bmirror, new Dictionary<ivec2, ivec2>()
        {
            {ivec2.RIGHT, ivec2.DOWN},
            {ivec2.DOWN, ivec2.RIGHT},
            {ivec2.LEFT, ivec2.UP},
            {ivec2.UP, ivec2.LEFT},
        });

        var lines = Parsing.ReadAllLines($"{GetDay()}").ToList();
        grid = new tiletype[lines[0].Length, lines.Count];
        for (int row = 0; row < lines.Count; row++)
        {
            for (int col = 0; col < lines[row].Length; col++)
            {
                grid[col, row] = lines[row][col] switch
                {
                    '-' => tiletype.hsplitter,
                    '|' => tiletype.vsplitter,
                    '\\' => tiletype.bmirror,
                    '/' => tiletype.fmirror,
                    '.' => tiletype.empty,
                    _ => throw new Exception(),
                };

                width = col + 1;
            }

            height = row + 1;
        }
    }

    HashSet<beaminfo> PropagateLight(beaminfo entryBeam)
    {
        HashSet<beaminfo> beamdata = [entryBeam];
        Queue<beaminfo> currBeams = [];
        currBeams.Enqueue(beamdata.First());
        while (currBeams.Count != 0)
        {
            var beam = currBeams.Dequeue();
            var nextBeamLoc = beam.loc + beam.dir;
            if (nextBeamLoc.x < 0 || nextBeamLoc.y < 0 || nextBeamLoc.x >= width || nextBeamLoc.y >= height)
            {
                continue;
            }

            var nextTile = grid[nextBeamLoc.x, nextBeamLoc.y];
            if (nextTile == tiletype.empty
                || (nextTile == tiletype.hsplitter && (beam.dir == ivec2.RIGHT || beam.dir == ivec2.LEFT))
                || (nextTile == tiletype.vsplitter && (beam.dir == ivec2.UP || beam.dir == ivec2.DOWN)))
            {
                var nextBeam = beam with {loc = nextBeamLoc};
                if (beamdata.Add(nextBeam))
                {
                    currBeams.Enqueue(nextBeam);
                }

                continue;
            }

            if (nextTile == tiletype.hsplitter)
            {
                var beam1 = new beaminfo(nextBeamLoc, ivec2.LEFT);
                var beam2 = new beaminfo(nextBeamLoc, ivec2.RIGHT);
                if (beamdata.Add(beam1))
                {
                    currBeams.Enqueue(beam1);
                }

                if (beamdata.Add(beam2))
                {
                    currBeams.Enqueue(beam2);
                }

                continue;
            }

            if (nextTile == tiletype.vsplitter)
            {
                var beam1 = new beaminfo(nextBeamLoc, ivec2.UP);
                var beam2 = new beaminfo(nextBeamLoc, ivec2.DOWN);
                if (beamdata.Add(beam1))
                {
                    currBeams.Enqueue(beam1);
                }

                if (beamdata.Add(beam2))
                {
                    currBeams.Enqueue(beam2);
                }

                continue;
            }

            if (nextTile is tiletype.bmirror or tiletype.fmirror)
            {
                ivec2 nextDir = reflectMap[nextTile][beam.dir];
                var nextBeam = new beaminfo(nextBeamLoc, nextDir);
                if (beamdata.Add(nextBeam))
                {
                    currBeams.Enqueue(nextBeam);
                }
            }
        }

        return beamdata;
    }

    long NumEnergized(HashSet<beaminfo> beamdata)
    {
        var energizedTiles = beamdata.DistinctBy(b => b.loc).Where(b => b.loc is {x: >= 0, y: >= 0} && b.loc.x < width && b.loc.y < height);
        long energized = energizedTiles.Count();
        return energized;
    }

    internal override string Part1()
    {
        var beamdata = PropagateLight(new beaminfo(new ivec2(-1, 0), ivec2.RIGHT));
        long energized = NumEnergized(beamdata);
        return $"<+white>{energized}";
    }

    internal override string Part2()
    {
        long highest = 0;
        for (int i = 0; i < width; i++)
        {
            var fromUp = PropagateLight(new beaminfo(new ivec2(i, -1), ivec2.DOWN));
            var val = NumEnergized(fromUp);
            if (val > highest)
            {
                highest = val;
            }

            var fromDown = PropagateLight(new beaminfo(new ivec2(i, height), ivec2.UP));
            val = NumEnergized(fromDown);
            if (val > highest)
            {
                highest = val;
            }
        }
        for (int i = 0; i < height; i++)
        {
            var fromLeft = PropagateLight(new beaminfo(new ivec2(-1, i), ivec2.RIGHT));
            var val = NumEnergized(fromLeft);
            if (val > highest)
            {
                highest = val;
            }

            var fromRight = PropagateLight(new beaminfo(new ivec2(width, i), ivec2.LEFT));
            val = NumEnergized(fromRight);
            if (val > highest)
            {
                highest = val;
            }
        }
        return $"<+white>{highest}";
    }
}
