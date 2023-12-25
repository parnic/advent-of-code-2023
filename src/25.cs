using aoc2023.Util;

namespace aoc2023;

internal class Day25 : Day
{
    private Dictionary<string, List<string>> connections = [];
    internal override void Parse()
    {
        var lines = Util.Parsing.ReadAllLines($"{GetDay()}");
        foreach (var line in lines)
        {
            var split = line.Split(": ");
            var right = split[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            connections.TryAdd(split[0], []);
            foreach (var r in right)
            {
                connections[split[0]].AddUnique(r);

                connections.TryAdd(r, []);
                connections[r].AddUnique(split[0]);
            }
        }

        // Logger.LogLine("graph G {");
        // foreach (var c in connections)
        // {
        //     Logger.LogLine($"{c.Key} -- {{ {string.Join(' ', c.Value)} }}");
        // }
        // Logger.LogLine("}");
    }

    internal override string Part1()
    {
        var br = connections.ToDictionary();
        foreach (var b in br)
        {
            br[b.Key] = [..b.Value];
            if (b.Key == "nqq")
            {
                br[b.Key].Remove("pxp");
            }

            if (b.Key == "pxp")
            {
                br[b.Key].Remove("nqq");
            }

            if (b.Key == "jxb")
            {
                br[b.Key].Remove("ksq");
            }

            if (b.Key == "ksq")
            {
                br[b.Key].Remove("jxb");
            }

            if (b.Key == "kns")
            {
                br[b.Key].Remove("dct");
            }

            if (b.Key == "dct")
            {
                br[b.Key].Remove("kns");
            }
        }

        List<string> h1 = ["pxp", "dct", "ksq"];
        List<string> h2 = ["nqq", "jxb", "kns"];
        bool bfound = false;
        while (!bfound)
        {
            bfound = true;
            int c = h1.Count;
            for (int i = 0; i < c; i++)
            {
                foreach (var n in br[h1[i]])
                {
                    if (!h1.Contains(n))
                    {
                        bfound = false;
                        h1.Add(n);
                    }
                }
            }
        }

        bfound = false;
        while (!bfound)
        {
            bfound = true;
            var c = h2.Count;
            for (int i = 0; i < c; i++)
            {
                foreach (var n in br[h2[i]])
                {
                    if (!h2.Contains(n))
                    {
                        bfound = false;
                        h2.Add(n);
                    }
                }
            }
        }

        return $"{h1.Count} * {h2.Count} = <+white>{h1.Count * h2.Count}";
    }

    internal override string Part2()
    {
        return $"<red>M<green>e<red>r<green>r<red>y <green>C<red>h<green>r<red>i<green>s<red>t<green>m<red>a<green>s<red>!";
    }
}
