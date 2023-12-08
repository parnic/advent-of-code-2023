using Math = aoc2023.Util.Math;

namespace aoc2023;

internal class Day08 : Day
{
    private string directions = "";
    private readonly Dictionary<string, (string left, string right)> nodes = [];
    internal override void Parse()
    {
        var lines = Util.Parsing.ReadAllLines($"{GetDay()}").ToList();
        directions = lines[0];
        foreach (var line in lines.Skip(2))
        {
            var split = line.Split(" = ");
            var choices = split[1].Trim('(', ')').Split(", ");
            nodes.Add(split[0], (choices[0], choices[1]));
        }
    }

    internal override string Part1()
    {
        var currNode = "AAA";
        int i;
        for (i = 0; currNode != "ZZZ"; i++)
        {
            if (directions[i % directions.Length] == 'R')
            {
                currNode = nodes[currNode].right;
            }
            else
            {
                currNode = nodes[currNode].left;
            }
        }

        return $"Steps from AAA to ZZZ: <+white>{i}";
    }

    internal override string Part2()
    {
        List<string> currNodes = [];
        foreach (var node in nodes)
        {
            if (node.Key.EndsWith('A'))
            {
                currNodes.Add(node.Key);
            }
        }

        List<long> dists = [];
        for (int n = 0; n < currNodes.Count; n++)
        {
            long i;
            for (i = 0; !currNodes[n].EndsWith('Z'); i++)
            {
                if (directions[(int)(i % directions.Length)] == 'R')
                {
                    currNodes[n] = nodes[currNodes[n]].right;
                }
                else
                {
                    currNodes[n] = nodes[currNodes[n]].left;
                }
            }
            dists.Add(i);
        }

        long totalDist = Math.LCM([.. dists]);
        return $"Steps before all nodes ending in A are simultaneously on a node ending in Z: <+white>{totalDist}";
    }
}
