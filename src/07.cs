using System.Diagnostics;
using aoc2023.Util;
using Math = System.Math;

namespace aoc2023;

internal class Day07 : Day
{
    private static readonly char[] order = new[]
    {
        '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A'
    };

    private Dictionary<List<int>, int> hands = new();

    internal override void Parse()
    {
        var lines = Util.Parsing.ReadAllLines($"{GetDay()}");
        foreach (var line in lines)
        {
            var split = line.Split(' ');
            List<int> hand = new();
            foreach (var card in split[0])
            {
                hand.Add(order.IndexOf(card));
            }

            hands.Add(hand, int.Parse(split[1]));
        }
    }

    private int GetHandValue(List<int> hand)
    {
        Dictionary<int, int> cardCounts = new();
        foreach (var card in hand)
        {
            if (!cardCounts.TryAdd(card, 1))
            {
                cardCounts[card]++;
            }
        }

        // five of a kind
        if (cardCounts.Count == 1)
        {
            return 7;
        }

        // four of a kind
        if (cardCounts.Any(c => c.Value == 4))
        {
            return 6;
        }

        // full house
        if (cardCounts.Count == 2 && cardCounts.ContainsValue(2) && cardCounts.ContainsValue(3))
        {
            return 5;
        }

        // three of a kind
        if (cardCounts.Count == 3 && cardCounts.ContainsValue(3) && cardCounts.Count(c => c.Value == 1) == 2)
        {
            return 4;
        }

        // two pair
        if (cardCounts.Count(c => c.Value == 2) == 2 && cardCounts.ContainsValue(1))
        {
            return 3;
        }

        // one pair
        if (cardCounts.Count(c => c.Value == 2) == 1 && cardCounts.Count(c => c.Value == 1) == 3)
        {
            return 2;
        }

        return 1;
    }

    private int GetHandValuePart2(List<int> hand)
    {
        Dictionary<int, int> cardCounts = new();
        int numWilds = 0;
        foreach (var card in hand)
        {
            if (card == -1)
            {
                numWilds++;
            }
            else if (!cardCounts.TryAdd(card, 1))
            {
                cardCounts[card]++;
            }
        }

        if (numWilds == 0)
        {
            return GetHandValue(hand);
        }

        // five of a kind
        if (cardCounts.Any(c => c.Value + numWilds == 5) || numWilds == 5)
        {
            return 7;
        }

        // four of a kind
        if (cardCounts.Any(c => c.Value + numWilds == 4))
        {
            return 6;
        }

        // full house
        {
            int wilds = numWilds;
            bool hasTwo = false;
            bool hasThree = false;
            foreach (var cc in cardCounts)
            {
                if (cc.Value + wilds >= 3 && !hasThree)
                {
                    wilds -= Math.Max(0, 3 - cc.Value);
                    hasThree = true;
                }
                else if (cc.Value + wilds >= 2 && !hasTwo)
                {
                    wilds -= Math.Max(0, 2 - cc.Value);
                    hasTwo = true;
                }
            }

            if (hasTwo && hasThree)
            {
                return 5;
            }
        }

        // three of a kind
        if (cardCounts.Any(c => c.Value + numWilds == 3))
        {
            return 4;
        }

        // one pair
        if (cardCounts.Count(c => c.Value == 2) == 1 || numWilds >= 1)
        {
            return 2;
        }

        return 1;
    }

    private bool HandWinsTie(List<int> hand, List<int> other)
    {
        for (int j = 0; j < hand.Count; j++)
        {
            if (hand[j] > other[j])
            {
                return true;
            }
            if (other[j] > hand[j])
            {
                break;
            }
        }

        return false;
    }

    List<(List<int> hand, int bet, int value)> SortHands(Dictionary<List<int>, int> h, Func<List<int>, int> valueFunc)
    {
        List<(List<int> hand, int bet, int value)> ordered = new();
        foreach (var hand in h)
        {
            bool inserted = false;
            var value = valueFunc(hand.Key);
            for (int i = 0; i < ordered.Count && !inserted; i++)
            {
                if (value > ordered[i].value)
                {
                    ordered.Insert(i, (hand.Key, hand.Value, value));
                    inserted = true;
                    break;
                }

                if (value == ordered[i].value)
                {
                    if (HandWinsTie(hand.Key, ordered[i].hand))
                    {
                        ordered.Insert(i, (hand.Key, hand.Value, value));
                        inserted = true;
                        break;
                    }
                }
            }

            if (!inserted)
            {
                ordered.Add((hand.Key, hand.Value, value));
            }
        }

        return ordered;
    }

    internal override string Part1()
    {
        var ordered = SortHands(hands, GetHandValue);
        long total = 0;
        for (int i = 0; i < ordered.Count; i++)
        {
            total += (ordered.Count - i) * ordered[i].bet;
        }
        return $"Total winnings: <+white>{total}";
    }

    internal override string Part2()
    {
        Dictionary<List<int>, int> handsp2 = [];
        foreach (var hand in hands)
        {
            var l = new List<int>(hand.Key);
            for (int i = 0; i < l.Count; i++)
            {
                if (l[i] == order.IndexOf('J'))
                {
                    l[i] = -1;
                }
            }
            handsp2.Add(l, hand.Value);
        }

        var ordered = SortHands(handsp2, GetHandValuePart2);
        long total = 0;
        for (int i = 0; i < ordered.Count; i++)
        {
            total += (ordered.Count - i) * ordered[i].bet;
        }

        return $"Total winnings: <+white>{total}";
    }
}