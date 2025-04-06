using System;
using System.Collections;
using UnityEngine;

namespace Polyfunks
{
    public static class MathUtils
    {
        public const double PI = Math.PI;
        public const double TAU = Math.PI * 2.0;

        public static double InverseLerp(double a, double b, double t)
        {
            return Clamp01(InverseLerpUnclamped(a, b, t));
        }

        public static double InverseLerpUnclamped(double a, double b, double t)
        {
            if (b == a)
                return b;
            return (t - a) / (b - a);
        }

        public static double Lerp(double a, double b, double t)
        {
            return LerpUnclamped(a, b, Clamp01(t));
        }

        public static double LerpUnclamped(double a, double b, double t)
        {
            // if t == 0, we want a. if t == 1, we want b.
            return (1 - t) * a + (t * b);
        }

        public static double Clamp(double value, double min, double max)
        {
            return Math.Max(min, Math.Min(value, max));
        }

        public static double Clamp01(double value)
        {
            return Clamp(value, 0.0, 1.0);
        }

        public static double StepTowards(double value, double target, double maxDelta)
        {
            var delta = target - value;
            //delta = Math.Abs(delta) * Math.Sign(delta);
            value += Math.Min(maxDelta, Math.Abs(delta)) * Math.Sign(delta);
            return value;
        }

    }
}