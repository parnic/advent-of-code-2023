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
}
