using aoc2023.Util;

namespace aoc2023;

internal class Day11 : Day
{
    private readonly List<ivec2> grid = new();
    private int width;
    private int height;

    internal override void Parse()
    {
        var lines = Parsing.ReadAllLines($"{GetDay()}").ToList();
        for (int row = 0; row < lines.Count; row++)
        {
            for (int col = 0; col < lines[row].Length; col++)
            {
                if (lines[row][col] == '#')
                {
                    grid.Add(new ivec2(col, row));
                }

                width = col + 1;
            }

            height = row + 1;
        }
    }

    List<ivec2> ExpandGrid(int expandBy)
    {
        List<ivec2> result = grid.ToList();
        for (int row = 0; row < height; row++)
        {
            if (result.All(p => p.y != row))
            {
                var pointsToMove = result.Where(p => p.y > row).ToList();
                result.RemoveAll(p => pointsToMove.Contains(p));
                foreach (var pt in pointsToMove)
                {
                    result.Add(new ivec2(pt.x, pt.y + expandBy));
                }

                row += expandBy;
                height += expandBy;
            }
        }

        for (int col = 0; col < width; col++)
        {
            if (result.All(p => p.x != col))
            {
                var pointsToMove = result.Where(p => p.x > col).ToList();
                result.RemoveAll(p => pointsToMove.Contains(p));
                foreach (var pt in pointsToMove)
                {
                    result.Add(new ivec2(pt.x + expandBy, pt.y));
                }

                col += expandBy;
                width += expandBy;
            }
        }

        return result;
    }

    internal override string Part1()
    {
        List<ivec2> p1grid = ExpandGrid(1);
        long total = 0;
        while (p1grid.Count > 0)
        {
            var source = p1grid.First();
            p1grid.Remove(source);

            foreach (var pt in p1grid)
            {
                var dist = source.ManhattanDistanceTo(pt);
                total += dist;
            }
        }

        return $"<+white>{total}";
    }

    internal override string Part2()
    {
        // this should be 1 million, but that gives me a sort-of "off by one" problem that this solves. i don't know why yet.
        List<ivec2> p2grid = ExpandGrid(999999);
        long total = 0;
        while (p2grid.Count > 0)
        {
            var source = p2grid.First();
            p2grid.Remove(source);

            foreach (var pt in p2grid)
            {
                var dist = source.ManhattanDistanceTo(pt);
                total += dist;
            }
        }

        return $"<+white>{total}";
    }
}
