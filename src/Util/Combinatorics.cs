namespace aoc2023.Util;

public static class Combinatorics
{
    public static IEnumerable<IEnumerable<T>> GetPermutations<T>(IList<T> list)
    {
        Action<IList<T>, int>? helper = null;
        List<IEnumerable<T>> res = new();

        helper = (arr, n) =>
        {
            if (n == 1)
            {
                var tmp = new T[arr.Count];
                arr.CopyTo(tmp, 0);
                res.Add(tmp);
            }
            else
            {
                for (var i = 0; i < n; i++)
                {
                    // ReSharper disable once AccessToModifiedClosure
                    helper!(arr, n - 1);
                    if (n % 2 == 1)
                    {
                        (arr[i], arr[n - 1]) = (arr[n - 1], arr[i]);
                    }
                    else
                    {
                        (arr[0], arr[n - 1]) = (arr[n - 1], arr[0]);
                    }
                }
            }
        };

        helper(list, list.Count);
        return res;
    }
}
