namespace aoc2023;

internal class Day15 : Day
{
    class lens
    {
        public string label = string.Empty;
        public int length;

        public override string ToString() => $"[{label} {length}]";
    }

    private List<string> sequence = [];
    internal override void Parse()
    {
        var str = Util.Parsing.ReadAllText($"{GetDay()}");
        sequence = [.. str.Split(',')];
    }

    private byte GetSequenceHash(string seq)
    {
        int val = 0;
        foreach (var ch in seq)
        {
            val += ch;
            val *= 17;
            val %= 256;
        }

        return (byte)val;
    }

    internal override string Part1()
    {
        long total = sequence.Sum(s => GetSequenceHash(s));
        return $"Sum of each sequence's hash: <+white>{total}";
    }

    internal override string Part2()
    {
        List<lens>[] boxes = new List<lens>[256];
        for (int i = 0; i < 256; i++)
        {
            boxes[i] = [];
        }

        foreach (var seq in sequence)
        {
            var opIdx = seq.IndexOfAny(['-', '=']);
            var label = seq[..opIdx];
            int box = GetSequenceHash(label);
            var op = seq[opIdx];
            if (op == '=')
            {
                int len = int.Parse(seq[(opIdx + 1)..]);
                int idx = boxes[box].FindIndex(l => l.label == label);
                if (idx >= 0)
                {
                    boxes[box][idx].length = len;
                }
                else
                {
                    boxes[box].Add(new lens(){label = label, length = len});
                }

                continue;
            }

            boxes[box].RemoveAll(l => l.label == label);
        }

        long total = 0;
        for (int i = 0; i < 256; i++)
        {
            if (!boxes[i].Any())
            {
                continue;
            }

            for (int j = 0; j < boxes[i].Count; j++)
            {
                long boxval = i + 1;
                boxval *= (j + 1);
                boxval *= boxes[i][j].length;
                total += boxval;
            }
        }

        return $"Focusing power of final configuration: <+white>{total}";
    }
}
