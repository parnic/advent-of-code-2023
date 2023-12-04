namespace aoc2023;

internal class Day04 : Day
{
    private List<List<int>> cardNums = new();
    private List<List<int>> winningNums = new();

    internal override void Parse()
    {
        var lines = Util.Parsing.ReadAllLines($"{GetDay()}");
        foreach (var line in lines)
        {
            var split = line.Split('|');
            var cardsplit = split[0].Split(':');
            cardNums.Add(cardsplit[1].Split(' ').Where(s => !string.IsNullOrEmpty(s)).Select(int.Parse).ToList());
            winningNums.Add(split[1].Split(' ').Where(s => !string.IsNullOrEmpty(s)).Select(int.Parse).ToList());
        }
    }

    internal override string Part1()
    {
        long total = 0;
        for (int cardIdx = 0; cardIdx < cardNums.Count; cardIdx++)
        {
            long cardtotal = 0;
            foreach (var num in cardNums[cardIdx])
            {
                if (winningNums[cardIdx].Contains(num))
                {
                    if (cardtotal == 0)
                    {
                        cardtotal = 1;
                    }
                    else
                    {
                        cardtotal *= 2;
                    }
                }
            }

            total += cardtotal;
        }

        return $"Total value of cards counted once: <+white>{total}";
    }

    internal override string Part2()
    {
        Dictionary<int, int> cardCopies = new();
        for (int i = 0; i < cardNums.Count; i++)
        {
            cardCopies.Add(i, 1);
        }

        for (int cardIdx = 0; cardIdx < cardNums.Count; cardIdx++)
        {
            int numMatches = 0;
            foreach (var num in cardNums[cardIdx])
            {
                if (winningNums[cardIdx].Contains(num))
                {
                    numMatches++;
                    cardCopies[cardIdx + numMatches] += cardCopies[cardIdx];
                }
            }
        }

        long total = cardCopies.Sum(c => c.Value);
        return $"Total number of cards: <+white>{total}";
    }
}
