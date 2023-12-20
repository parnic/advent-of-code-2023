namespace aoc2023;

internal class Day20 : Day
{
    private record command(bool state, string source, string target);

    abstract class module(string n, IList<string> o)
    {
        protected readonly string name = n;
        public readonly List<string> outputs = [..o];

        public abstract void Signal(string source, bool state, Queue<command> queue);
        public abstract void Reset();
    }

    private class flipflop(string n, IList<string> o) : module(n, o)
    {
        private bool state;

        public override void Signal(string source, bool pulse, Queue<command> queue)
        {
            if (pulse)
            {
                return;
            }

            state = !state;
            foreach (var r in outputs)
            {
                queue.Enqueue(new command(state, name, r));
            }
        }

        public override void Reset()
        {
            state = false;
        }
    }

    private class conjunction(string n, IList<string> o) : module(n, o)
    {
        public readonly Dictionary<string, bool> inputs = [];

        public override void Signal(string source, bool state, Queue<command> queue)
        {
            inputs[source] = state;
            bool send = !inputs.All(r => r.Value);

            foreach (var r in outputs)
            {
                queue.Enqueue(new command(send, name, r));
            }
        }

        public override void Reset()
        {
            foreach (var i in inputs)
            {
                inputs[i.Key] = false;
            }
        }
    }

    private readonly Dictionary<string, module> modules = [];
    private readonly List<string> broadcastReceivers = [];

    internal override void Parse()
    {
        var lines = Util.Parsing.ReadAllLines($"{GetDay()}");
        foreach (var line in lines)
        {
            var split = line.Split(" -> ");
            if (split[0].StartsWith('%'))
            {
                var n = split[0][1..];
                var receivers = split[1].Split(", ");
                modules.Add(n, new flipflop(n, receivers));
            }
            else if (split[0].StartsWith('&'))
            {
                var n = split[0][1..];
                var receivers = split[1].Split(", ");
                modules.Add(n, new conjunction(n, receivers));
            }
            else if (split[0] == "broadcaster")
            {
                broadcastReceivers.AddRange(split[1].Split(", "));
            }
            else
            {
                throw new Exception();
            }
        }

        foreach (var m in modules.Where(m => m.Value is conjunction))
        {
            foreach (var i in modules.Where(m2 => m2.Value.outputs.Contains(m.Key)))
            {
                (m.Value as conjunction)!.inputs.TryAdd(i.Key, false);
            }
        }
    }

    internal override string Part1()
    {
        modules.ForEach(m => m.Value.Reset());
        Queue<command> q = [];

        long lows = 0;
        long highs = 0;
        for (int i = 0; i < 1000; i++)
        {
            lows++;
            foreach (var r in broadcastReceivers)
            {
                q.Enqueue(new command(false, "broadcaster", r));
            }

            while (q.TryDequeue(out command? result))
            {
                if (result.state)
                {
                    highs++;
                }
                else
                {
                    lows++;
                }

                if (modules.TryGetValue(result.target, out module? value))
                {
                    value.Signal(result.source, result.state, q);
                }
            }
        }

        return $"{lows} low signals * {highs} high signals = <+white>{lows*highs}";
    }

    internal override string Part2()
    {
        modules.ForEach(m => m.Value.Reset());
        Queue<command> q = [];

        var rxin = modules.First(m => m.Value.outputs.Contains("rx"));
        var rxinin = modules.Where(m => m.Value.outputs.Contains(rxin.Key)).ToDictionary();
        Dictionary<string, long> loops = [];

        bool seenRx = false;
        long presses = 0;
        while (!seenRx)
        {
            presses++;
            foreach (var r in broadcastReceivers)
            {
                q.Enqueue(new command(false, "broadcaster", r));
            }

            while (q.TryDequeue(out command? result))
            {
                if (result is {target: "rx", state: false})
                {
                    seenRx = true;
                    break;
                }

                if (!modules.TryGetValue(result.target, out module? value))
                {
                    continue;
                }

                if (!result.state && rxinin.Any(m => m.Key == result.target))
                {
                    if (loops.TryAdd(result.target, presses))
                    {
                        if (loops.Count == rxinin.Count)
                        {
                            seenRx = true;
                            break;
                        }
                    }
                }

                value.Signal(result.source, result.state, q);
            }
        }

        // technically this should use LCM, but they're all prime.
        return $"rx will receive a low pulse after <+white>{loops.Aggregate(1L, (curr, m) => curr * m.Value)}<+black> button presses";
    }
}
