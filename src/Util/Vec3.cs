namespace aoc2023.Util;

public readonly struct ivec3 : IEquatable<ivec3>, IComparable<ivec3>, IComparable
{
    public readonly long x = 0;
    public readonly long y = 0;
    public readonly long z = 0;

    public static readonly ivec3 ZERO = new ivec3(0, 0, 0);
    public static readonly ivec3 ONE = new ivec3(1, 1, 1);

    public static readonly ivec3 LEFT = new ivec3(-1, 0, 0);
    public static readonly ivec3 RIGHT = new ivec3(1, 0, 0);
    public static readonly ivec3 UP = new ivec3(0, -1, 0);
    public static readonly ivec3 DOWN = new ivec3(0, 1, 0);
    public static readonly ivec3 FORWARD = new ivec3(0, 0, 1);
    public static readonly ivec3 BACKWARD = new ivec3(0, 0, -1);
    public static readonly ivec3[] DIRECTIONS = {LEFT, RIGHT, UP, DOWN, FORWARD, BACKWARD};

    public ivec3(long xv, long yv, long zv)
    {
        x = xv;
        y = yv;
        z = zv;
    }

    public bool IsZero() => x == 0 && y == 0 && z == 0;
    public long Sum => x + y + z;
    public long Product => x * y * z;
    public long MaxElement => System.Math.Max(x, System.Math.Max(y, z));
    public long MinElement => System.Math.Min(x, System.Math.Min(y, z));

    public ivec3 GetRotatedLeft() => new ivec3(y, -x, z);

    public ivec3 GetRotatedRight() => new ivec3(-y, x, z);

    public long Dot(ivec3 v) => (x * v.x) + (y * v.y) + (z * v.z);
    public long LengthSquared => (x * x) + (y * y) +  (z * z);
    public float Length => MathF.Sqrt(LengthSquared);

    public long ManhattanDistance => Abs(this).Sum;
    public long ManhattanDistanceTo(ivec3 other) => System.Math.Abs(x - other.x) + System.Math.Abs(y - other.y) + System.Math.Abs(z - other.z);

    public bool IsTouching(ivec3 other) => ManhattanDistanceTo(other) == 1;

    public IEnumerable<ivec3> GetNeighbors(ivec3? min = null, ivec3? max = null)
    {
        foreach (var d in DIRECTIONS)
        {
            var n = this + d;
            if ((min == null || n >= min) && (max == null || n <= max))
            {
                yield return n;
            }
        }
    }

    public ivec3 GetBestDirectionTo(ivec3 p)
    {
        ivec3 diff = p - this;
        if (diff.IsZero())
        {
            return ZERO;
        }

        ivec3 dir = diff / Abs(diff).MaxElement;
        return Sign(dir);
    }

    // get a point in the 8 cells around me closest to p
    public ivec3 GetNearestNeighbor(ivec3 p)
    {
        ivec3 dir = GetBestDirectionTo(p);
        return this + dir;
    }

    public long this[long i] => (i == 0) ? x : (i == 1) ? y : z;

    public static ivec3 operator +(ivec3 v) => v;
    public static ivec3 operator -(ivec3 v) => new ivec3(-v.x, -v.y, -v.z);
    public static ivec3 operator +(ivec3 a, ivec3 b) => new ivec3(a.x + b.x, a.y + b.y, a.z + b.z);
    public static ivec3 operator -(ivec3 a, ivec3 b) => new ivec3(a.x - b.x, a.y - b.y, a.z - b.z);
    public static ivec3 operator *(ivec3 a, ivec3 b) => new ivec3(a.x * b.x, a.y * b.y, a.z * b.z);
    public static ivec3 operator *(long a, ivec3 v) => new ivec3(a * v.x, a * v.y, a * v.z);
    public static ivec3 operator *(ivec3 v, long a) => new ivec3(a * v.x, a * v.y, a * v.z);
    public static ivec3 operator /(ivec3 v, long a) => new ivec3(v.x / a, v.y / a, v.z / a);
    public static bool operator ==(ivec3 a, ivec3 b) => (a.x == b.x) && (a.y == b.y) && (a.z == b.z);
    public static bool operator !=(ivec3 a, ivec3 b) => (a.x != b.x) || (a.y != b.y) || (a.z != b.z);
    public static bool operator <(ivec3 a, ivec3 b) => (a.x < b.x) && (a.y < b.y) && (a.z < b.z);
    public static bool operator <=(ivec3 a, ivec3 b) => (a.x <= b.x) && (a.y <= b.y) && (a.z <= b.z);
    public static bool operator >(ivec3 a, ivec3 b) => (a.x > b.x) && (a.y > b.y) && (a.z > b.z);
    public static bool operator >=(ivec3 a, ivec3 b) => (a.x >= b.x) && (a.y >= b.y) && (a.z >= b.z);

    public bool Equals(ivec3 other)
    {
        return x == other.x && y == other.y && z == other.z;
    }

    public override bool Equals(object? obj)
    {
        return obj is ivec3 other && Equals(other);
    }

    public int CompareTo(ivec3 other)
    {
        if (this < other)
        {
            return -1;
        }

        if (this > other)
        {
            return 1;
        }

        return 0;
    }

    public int CompareTo(object? obj)
    {
        if (ReferenceEquals(null, obj)) return 1;
        return obj is ivec3 other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(ivec3)}");
    }

    public static ivec3 Sign(ivec3 v) => new ivec3(System.Math.Sign(v.x), System.Math.Sign(v.y), System.Math.Sign(v.z));
    public static ivec3 Min(ivec3 a, ivec3 b) => new ivec3(System.Math.Min(a.x, b.x), System.Math.Min(a.y, b.y), System.Math.Min(a.z, b.z));
    public static ivec3 Max(ivec3 a, ivec3 b) => new ivec3(System.Math.Max(a.x, b.x), System.Math.Max(a.y, b.y), System.Math.Max(a.z, b.z));

    public static ivec3 Clamp(ivec3 v, ivec3 lh, ivec3 rh) => Min(rh, Max(lh, v));

    public static ivec3 Abs(ivec3 v) => new ivec3(System.Math.Abs(v.x), System.Math.Abs(v.y), System.Math.Abs(v.z));

    public static ivec3 Mod(ivec3 val, long den)
    {
        long x = val.x % den;
        long y = val.y % den;
        long z = val.z % den;

        if (x < 0)
        {
            x += den;
        }

        if (y < 0)
        {
            y += den;
        }

        if (z < 0)
        {
            z += den;
        }

        return new ivec3(x, y, z);
    }

    public static long Dot(ivec3 a, ivec3 b) => (a.x * b.x) + (a.y * b.y) + (a.z * b.z);

    public static ivec3 Parse(string s)
    {
        string[] parts = s.Split(',', 3);
        long x = long.Parse(parts[0]);
        long y = long.Parse(parts[1]);
        long z = long.Parse(parts[2]);
        return new ivec3(x, y, z);
    }

    public override int GetHashCode() => HashCode.Combine(x, y, z);

    public override string ToString() => $"{x},{y},{z}";
}
