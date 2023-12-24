using System.Numerics;
using aoc2023.Util;
using Microsoft.Z3;

namespace aoc2023;

internal class Day24 : Day
{
    class path
    {
        public readonly ivec3 startPos;
        public readonly ivec3 velocity;

        public readonly BigInteger a;
        public readonly BigInteger b;
        public readonly BigInteger c;

        public path(ivec3 p, ivec3 v)
        {
            startPos = p;
            velocity = v;

            a = velocity.y;
            b = -velocity.x;
            c = velocity.y * startPos.x - velocity.x * startPos.y;
        }

        public override string ToString() => $"{startPos} @ {velocity}";
    }

    private readonly List<path> hailstones = [];
    internal override void Parse()
    {
        var lines = Parsing.ReadAllLines($"{GetDay()}");
        foreach (var line in lines)
        {
            var split = line.Split(" @ ").Select(x => x.Replace(" ", "")).ToList();
            path p = new(ivec3.Parse(split[0]), ivec3.Parse(split[1]));
            hailstones.Add(p);
        }
    }

    internal override string Part1()
    {
        BigInteger lowerBound = 200000000000000;
        BigInteger upperBound = 400000000000000;
        long total = 0;
        for (int i = 0; i < hailstones.Count; i++)
        {
            for (int j = 0; j < i; j++)
            {
                var (a1, b1, c1) = (hailstones[i].a, hailstones[i].b, hailstones[i].c);
                var (a2, b2, c2) = (hailstones[j].a, hailstones[j].b, hailstones[j].c);
                if (a1 * b2 == b1 * a2)
                {
                    continue;
                }

                var x = (c1 * b2 - c2 * b1) / (a1 * b2 - a2 * b1);
                var y = (c2 * a1 - c1 * a2) / (a1 * b2 - a2 * b1);
                if (x < lowerBound || x > upperBound || y < lowerBound || y > upperBound)
                {
                    continue;
                }

                if ((x - hailstones[i].startPos.x) * hailstones[i].velocity.x >= 0 && (y - hailstones[i].startPos.y) * hailstones[i].velocity.y >= 0 &&
                    (x - hailstones[j].startPos.x) * hailstones[j].velocity.x >= 0 && (y - hailstones[j].startPos.y) * hailstones[j].velocity.y >= 0)
                {
                    total++;
                }
            }
        }

        return $"Intersections: <+white>{total}";
    }

    internal override string Part2()
    {
        var ctx = new Context();
        var solver = ctx.MkSolver();

        // Coordinates of the stone
        var x = ctx.MkIntConst("x");
        var y = ctx.MkIntConst("y");
        var z = ctx.MkIntConst("z");

        // Velocity of the stone
        var vx = ctx.MkIntConst("vx");
        var vy = ctx.MkIntConst("vy");
        var vz = ctx.MkIntConst("vz");

        // For each iteration, we will add 3 new equations and one new condition to the solver.
        // We want to find 9 variables (x, y, z, vx, vy, vz, t0, t1, t2) that satisfy all the equations, so a system of 9 equations is enough.
        for (var i = 0; i < 3; i++)
        {
            var t = ctx.MkIntConst($"t{i}"); // time for the stone to reach the hail
            var hail = hailstones[i];

            var px = ctx.MkInt(hail.startPos.x);
            var py = ctx.MkInt(hail.startPos.y);
            var pz = ctx.MkInt(hail.startPos.z);

            var pvx = ctx.MkInt(hail.velocity.x);
            var pvy = ctx.MkInt(hail.velocity.y);
            var pvz = ctx.MkInt(hail.velocity.z);

            var xLeft = ctx.MkAdd(x, ctx.MkMul(t, vx)); // x + t * vx
            var yLeft = ctx.MkAdd(y, ctx.MkMul(t, vy)); // y + t * vy
            var zLeft = ctx.MkAdd(z, ctx.MkMul(t, vz)); // z + t * vz

            var xRight = ctx.MkAdd(px, ctx.MkMul(t, pvx)); // px + t * pvx
            var yRight = ctx.MkAdd(py, ctx.MkMul(t, pvy)); // py + t * pvy
            var zRight = ctx.MkAdd(pz, ctx.MkMul(t, pvz)); // pz + t * pvz

            solver.Add(t >= 0); // time should always be positive - we don't want solutions for negative time
            solver.Add(ctx.MkEq(xLeft, xRight)); // x + t * vx = px + t * pvx
            solver.Add(ctx.MkEq(yLeft, yRight)); // y + t * vy = py + t * pvy
            solver.Add(ctx.MkEq(zLeft, zRight)); // z + t * vz = pz + t * pvz
        }

        solver.Check();
        var model = solver.Model;

        var rx = model.Eval(x);
        var ry = model.Eval(y);
        var rz = model.Eval(z);

        long result = long.Parse(rx.ToString()) + long.Parse(ry.ToString()) + long.Parse(rz.ToString());

        return $"Added coordinates of rock-throwing position: <+white>{result}";
    }
}
