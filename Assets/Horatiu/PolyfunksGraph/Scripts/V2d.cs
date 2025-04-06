namespace Polyfunks
{
    using System;
    using UnityEngine;

    [Serializable]
    public struct V2d
    {
        public static V2d zero = new V2d(0, 0);

        public double x;
        public double y;

        public V2d(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public V2d RotateRadians(double radians)
        {
            var ca = Math.Cos(radians);
            var sa = Math.Sin(radians);
            return new V2d(ca * x - sa * y, sa * x + ca * y);
        }

        public Vector3 ToVector3()
        {
            return new Vector3((float)x, (float)y, 0);
        }
    }
}