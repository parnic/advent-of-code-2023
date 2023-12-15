namespace aoc2023.Util;

public static class Extensions
{
    public static bool IsDigit(this char c) => c >= '0' && c <= '9';

    public static void AddUnique<T>(this ICollection<T> list, T elem)
    {
        if (!list.Contains(elem))
        {
            list.Add(elem);
        }
    }

    public static int IndexOf<T>(this ICollection<T> list, T elem) where T : IEquatable<T>
    {
        for (int idx = 0; idx < list.Count; idx++)
        {
            if (list.ElementAt(idx).Equals(elem))
            {
                return idx;
            }
        }

        return -1;
    }

    public static string ReplaceFirst(this string str, char ch, char replace)
    {
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] == ch)
            {
                return str.ReplaceAt(i, replace);
            }
        }

        return str;
    }

    public static string ReplaceAt(this string str, int index, char replace)
    {
        return str[..index] + replace + str[(index + 1)..];
    }

    public static bool SequenceEquals<T>(this T[,] a, T[,] b) => a.Rank == b.Rank
                                                                 && Enumerable.Range(0, a.Rank).All(d=> a.GetLength(d) == b.GetLength(d))
                                                                 && a.Cast<T>().SequenceEqual(b.Cast<T>());
}
