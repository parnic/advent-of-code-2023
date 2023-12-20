namespace aoc2023;

internal class Day19 : Day
{
    private readonly Dictionary<string, List<string>> workflows = [];
    private readonly List<Dictionary<char, long>> partList = [];

    internal override void Parse()
    {
        var lines = Util.Parsing.ReadAllLines($"{GetDay()}");
        int mode = 0;
        foreach (var line in lines)
        {
            if (mode == 0)
            {
                if (line.Length == 0)
                {
                    mode++;
                    continue;
                }

                var split = line.Split('{');
                var name = split[0];
                var rulesStr = split[1].Trim('}');
                var rules = rulesStr.Split(',');
                workflows.Add(name, [..rules]);
                continue;
            }

            var ratings = line.Trim('{', '}').Split(',');
            Dictionary<char, long> rating = [];
            foreach (var r in ratings)
            {
                var s2 = r.Split('=');
                rating.Add(s2[0][0], long.Parse(s2[1]));
            }
            partList.Add(rating);
        }
    }

    private static (Dictionary<char, (int low, int high)>, Dictionary<char, (int low, int high)>) GetSplitInterval(
        Dictionary<char, (int low, int high)> items, char ch, char op, int val)
    {
        var itemsB = items.ToDictionary();
        var itemsF = items.ToDictionary();
        itemsB[ch] = (items[ch].low, op == '<' ? val - 1 : val);
        itemsF[ch] = (op == '<' ? val : val + 1, items[ch].high);
        return (itemsB, itemsF);
    }

    private long GetCombinations(string workflowName, Dictionary<char, (int low, int high)> items)
    {
        if (workflowName == "A")
        {
            return 1L * (items['x'].high - items['x'].low + 1) * (items['m'].high - items['m'].low + 1) *
                   (items['a'].high - items['a'].low + 1) * (items['s'].high - items['s'].low + 1);
        }

        if (workflowName == "R")
        {
            return 0;
        }

        var result = 0L;
        var currItems = items.ToDictionary();
        foreach (var currentWorkflow in workflows[workflowName])
        {
            if (!currentWorkflow.Contains(':'))
            {
                result += GetCombinations(currentWorkflow, currItems);
                break;
            }

            var ch = currentWorkflow[0];
            var op = currentWorkflow[1];
            var val = int.Parse(currentWorkflow[2..currentWorkflow.IndexOf(':')]);
            var targetWorkflow = currentWorkflow[(currentWorkflow.IndexOf(':') + 1)..];

            var (itemsB, itemsF) = GetSplitInterval(currItems, ch, op, val);
            var inItems = itemsB;
            var outItems = itemsF;
            if (op == '>')
            {
                inItems = itemsF;
                outItems = itemsB;
            }

            currItems = outItems;
            result += GetCombinations(targetWorkflow, inItems);
        }

        return result;
    }

    internal override string Part1()
    {
        List<Dictionary<char, long>> accepted = [];
        foreach (var part in partList)
        {
            var currentWorkflow = workflows["in"];
            bool bAccepted = false;
            int idx = 0;
            while (true)
            {
                var str = currentWorkflow[idx];
                if (str.Contains(':'))
                {
                    var ch = str[0];
                    var op = str[1];
                    var val = long.Parse(str[2..str.IndexOf(':')]);
                    var target = str[(str.IndexOf(':') + 1)..];

                    if ((op == '<' && part[ch] < val) || (op == '>' && part[ch] > val))
                    {
                        str = target;
                    }
                    else
                    {
                        idx++;
                        continue;
                    }
                }

                if (str == "A")
                {
                    bAccepted = true;
                    break;
                }

                if (str == "R")
                {
                    break;
                }

                currentWorkflow = workflows[str];
                idx = 0;
            }

            if (bAccepted)
            {
                accepted.Add(part);
            }
        }

        long total = accepted.Sum(a => a.Sum(v => v.Value));
        return $"Accepted rating sum: <+white>{total}";
    }

    internal override string Part2()
    {
        Dictionary<char, (int low, int high)> items = [];
        items.Add('x', (1, 4000));
        items.Add('m', (1, 4000));
        items.Add('a', (1, 4000));
        items.Add('s', (1, 4000));
        var result = GetCombinations("in", items);

        return $"Distinct workflow combinations: <+white>{result}";
    }
}
