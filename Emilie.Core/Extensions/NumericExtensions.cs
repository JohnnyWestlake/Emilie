using System;
using System.Numerics;

namespace UIC.Core.Extensions
{
    public static class NumericExtensions
    {

        public static bool IsNaN(this double d) => Double.IsNaN(d);

        #region Radian - Degree Conversions

        public static double DegreesToRadians(double angle) => Math.PI * angle / 180.0;

        public static float DegreesToRadians(float angle) => (float)DegreesToRadians((double)angle);

        public static double RadiansToDegrees(double angle) => angle * (180.0 / Math.PI);
        public static float RadiansToDegrees(float angle) => (float)RadiansToDegrees((double)angle);

        #endregion

        public static Vector2 RotatePoint(Vector2 toRotate, Vector2 pivot, float radians)
        {
            float s = (float)Math.Sin(radians);
            float c = (float)Math.Cos(radians);

            // translate point back to origin:
            toRotate.X -= pivot.X;
            toRotate.Y -= pivot.Y;

            // rotate point
            float xnew = (toRotate.X * c - toRotate.Y * s);
            float ynew = (toRotate.X * s + toRotate.Y * c);

            // translate point back:
            toRotate.X = xnew + pivot.X;
            toRotate.Y = ynew + pivot.Y;

            return toRotate;
        }
    }
}
