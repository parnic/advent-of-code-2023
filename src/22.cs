using System.Diagnostics;
using aoc2023.Util;
using Math = System.Math;

namespace aoc2023;

internal class Day22 : Day
{
    private class block
    {
        public int startX;
        public int startY;
        public int startZ;
        public int endX;
        public int endY;
        public int endZ;

        public bool XYOverlaps(block other) => Math.Max(startX, other.startX) <= Math.Min(endX, other.endX) &&
                                             Math.Max(startY, other.startY) <= Math.Min(endY, other.endY);

        private block()
        {

        }

        public block(block other)
        {
            startX = other.startX;
            startY = other.startY;
            startZ = other.startZ;
            endX = other.endX;
            endY = other.endY;
            endZ = other.endZ;
        }

        public static block Parse(string line)
        {
            var split = line.Split('~');
            var start = split[0].Split(',').Select(int.Parse).ToArray();
            var end = split[1].Split(',').Select(int.Parse).ToArray();
            Debug.Assert(end[0] >= start[0]);
            Debug.Assert(end[1] >= start[1]);
            Debug.Assert(end[2] >= start[2]);

            return new block
            {
                startX = start[0],
                startY = start[1],
                startZ = start[2],
                endX = end[0],
                endY = end[1],
                endZ = end[2],
            };
        }

        public override string ToString() => $"[{startX},{startY},{startZ}] -> [{endX},{endY},{endZ}]";
    }

    private block[] blocks = [];

    internal override void Parse()
    {
        var lines = Parsing.ReadAllLines($"{GetDay()}");
        blocks = lines.Select(block.Parse).ToArray();
        Array.Sort(blocks, (a, b) => a.startZ.CompareTo(b.startZ));
    }

    private (block[], int) DropBlocks(IList<block> list)
    {
        block[] droppedBlocks = new block[list.Count];
        for (int i = 0; i < list.Count; i++)
        {
            droppedBlocks[i] = new block(list[i]);
        }

        int numDropped = 0;
        for (int i = 0; i < droppedBlocks.Length; i++)
        {
            var droppedBlock = droppedBlocks[i];
            int maxZ = 1;
            for (int j = 0; j < i; j++)
            {
                var checkAgainst = droppedBlocks[j];
                if (droppedBlock.XYOverlaps(checkAgainst))
                {
                    maxZ = Math.Max(maxZ, checkAgainst.endZ + 1);
                }
            }

            droppedBlock.endZ -= droppedBlock.startZ - maxZ;
            if (droppedBlock.startZ != maxZ)
            {
                numDropped++;
            }
            droppedBlock.startZ = maxZ;
        }

        return (droppedBlocks, numDropped);
    }

    private (Dictionary<int, List<int>> supporting, Dictionary<int, List<int>> supported) BuildSupportMap(block[] list)
    {
        Dictionary<int, List<int>> supportingIdxMap = [];
        Dictionary<int, List<int>> supportedByIdxMap = [];
        for (int upperIdx = 0; upperIdx < list.Length; upperIdx++)
        {
            supportingIdxMap.Add(upperIdx, []);
            supportedByIdxMap.Add(upperIdx, []);

            var upper = list[upperIdx];
            for (int lowerIdx = 0; lowerIdx < upperIdx; lowerIdx++)
            {
                var lower = list[lowerIdx];
                if (lower.XYOverlaps(upper) && upper.startZ == lower.endZ + 1)
                {
                    supportingIdxMap[lowerIdx].Add(upperIdx);
                    supportedByIdxMap[upperIdx].Add(lowerIdx);
                }
            }
        }

        return (supportingIdxMap, supportedByIdxMap);
    }

    internal override string Part1()
    {
        (block[] droppedBlocks, _) = DropBlocks(blocks);
        (Dictionary<int, List<int>> supportingIdxMap, Dictionary<int, List<int>> supportedByIdxMap) = BuildSupportMap(droppedBlocks);

        int total = 0;
        for (int i = 0; i < droppedBlocks.Length; i++)
        {
            if (supportingIdxMap[i].All(j => supportedByIdxMap[j].Count >= 2))
            {
                total++;
            }
        }

        return $"# bricks that can be safely disintegrated: <+white>{total}";
    }

    internal override string Part2()
    {
        (block[] droppedBlocks, _) = DropBlocks(blocks);
        (Dictionary<int, List<int>> supportingIdxMap, Dictionary<int, List<int>> supportedByIdxMap) = BuildSupportMap(droppedBlocks);

        long total = 0;
        for (int i = 0; i < droppedBlocks.Length; i++)
        {
            if (!supportingIdxMap[i].All(j => supportedByIdxMap[j].Count >= 2))
            {
                var testList = droppedBlocks.ToList();
                testList.RemoveAt(i);
                (_, int numDropped) = DropBlocks(testList);
                total += numDropped;
            }
        }

        return $"# bricks that would fall by removing load-bearing bricks: <+white>{total}";
    }
}
