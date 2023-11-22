namespace aoc2023.Util;

public static class Math
{
    public static ulong GCD(ulong a, ulong b)
    {
        while (true)
        {
            if (b == 0)
            {
                return a;
            }

            var a1 = a;
            a = b;
            b = a1 % b;
        }
    }

    public static ulong LCM(params ulong[] nums)
    {
        var num = nums.Length;
        switch (num)
        {
            case 0:
                return 0;
            case 1:
                return nums[0];
        }

        var ret = lcm(nums[0], nums[1]);
        for (var i = 2; i < num; i++)
        {
            ret = lcm(nums[i], ret);
        }

        return ret;
    }

    private static ulong lcm(ulong a, ulong b)
    {
        return (a * b) / GCD(a, b);
    }

    public static long Modulo(long numer, long denom)
    {
        // long q = numer / denom;
        long r = numer % denom;
        if (r < 0)
        {
            if (denom > 0)
            {
                // q = q - 1;
                r = r + denom;
            }
            else
            {
                // q = q + 1;
                r = r - denom;
            }
        }

        return r;
    }
}