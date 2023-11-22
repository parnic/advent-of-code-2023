namespace aoc2023.Util;

public static class Bisect
{
    // Bisect takes a known-good low and known-bad high value as the bounds
    // to bisect, and a function to test each value for success or failure.
    // If the function succeeds, the value is adjusted toward the maximum,
    // and if the function fails, the value is adjusted toward the minimum.
    // The final value is returned when the difference between the success
    // and the failure is less than or equal to the acceptance threshold
    // (usually 1, for integers).
    public static double Find(double low, double high, double threshold, Func<double, bool> tryFunc)
    {
        while (System.Math.Abs(high - low) > threshold)
        {
            var currVal = low + ((high - low) / 2);
            var success = tryFunc(currVal);
            if (success)
            {
                low = currVal;
            }
            else
            {
                high = currVal;
            }
        }

        return low;
    }

    public static double Find(long low, long high, long threshold, Func<long, bool> tryFunc)
    {
        while (System.Math.Abs(high - low) > threshold)
        {
            var currVal = low + ((high - low) / 2);
            var success = tryFunc(currVal);
            if (success)
            {
                low = currVal;
            }
            else
            {
                high = currVal;
            }
        }

        return low;
    }
}