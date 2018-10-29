using System;
using System.Numerics;

namespace Emilie.Core.Numerics
{
    public struct USize
    {
        public uint Height { get; set; }
        public uint Width { get; set; }

        public USize(uint width, uint height) : this()
        {
            Width = width;
            Height = height;
        }

        public USize(float width, float height) : this()
        {
            Width = width.Round();
            Height = height.Round();
        }
    }

    public static class Maths
    {
        internal static bool IsNaN(this double d)
        {
            return Double.IsNaN(d);
        }

        public static double DegreesToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        public static float DegreesToRadians(float angle)
        {
            return (float)DegreesToRadians((double)angle);
        }

        public static double RadiansToDegrees(double angle)
        {
            return angle * (180.0 / Math.PI);
        }
        public static float RadiansToDegrees(float angle)
        {
            return (float)RadiansToDegrees((double)angle);
        }

        //////////////////////////////////////////////////////
        // GET POINT (Rectangle Helpers)
        /////////////////////////////////////////////////////

        public static Vector2 GetMidPoint(Vector2 pt1, Vector2 pt2)
        {
            var midX = (pt1.X + pt2.X) / 2f;
            var midY = (pt1.Y + pt2.Y) / 2f;
            return new Vector2(midX, midY);
        }

        public static float NextFloat(this Random random)
        {
            return (float)random.NextDouble();
        }

        public static Vector2 PointOnCircle(float radius, float radians, Vector2 origin)
        {
            float x = (float)(radius * Math.Cos(radians)) + origin.X;
            float y = (float)(radius * Math.Sin(radians)) + origin.Y;

            return new Vector2(x, y);
        }

        public static Vector2 RotatePoint(Vector2 p, Vector2 r, float a)
        {
            float s = (float)Math.Sin(a);
            float c = (float)Math.Cos(a);

            // translate point back to origin:
            r.X -= p.X;
            r.Y -= p.Y;

            // rotate point
            float xnew = (float)(r.X * c - r.Y * s);

            // xnew + toRotate.Y * s = toRotate.X * c;
            float ynew = (float)(r.X * s + r.Y * c);

            // translate point back:
            r.X = xnew + p.X;
            r.Y = ynew + p.Y;

            return r;
        }

        public static float Distance(Vector2 point1, Vector2 point2)
        {
            return (float)Math.Sqrt(Math.Pow((point1.X - point2.X), 2) + Math.Pow((point1.Y - point2.Y), 2));
        }

        public static float AngleBetweenPoints(Vector2 center, Vector2 point)
        {
            float xDiff = (float)(point.X - center.X);
            float yDiff = (float)(point.Y - center.Y);
            var degrees = (float)(Math.Atan2(yDiff, xDiff));
            return degrees;
        }

        public static float Clamp(float value, float min, float max)
        {
            value = Math.Min(value, max);
            return Math.Max(value, min);
        }

        public static uint GetScaledDimension(USize dimensions, uint targetWidth)
        {
            var thing = dimensions.Width / targetWidth;
            return ((float)(dimensions.Height / thing)).Round();
        }

        public static float GetScale(this USize size1, USize size2)
        {
            return (float)(size2.Width / size1.Width);
        }

        public static Vector2 Scale(this Vector2 vector, float scale)
        {
            return new Vector2(vector.X * scale, vector.Y * scale);
        }

        public static float Scale(this float _float, float scale)
        {
            return _float * scale;
        }

        public static uint Round(this float _float)
        {
            return (uint)Math.Round(_float, MidpointRounding.AwayFromZero);
        }

        public static uint Round(this double _double)
        {
            return (uint)Math.Round(_double, MidpointRounding.AwayFromZero);
        }
        public static Vector2 MidPoint(this Vector2 p1, Vector2 p2)
        {
            return new Vector2
            {
                X = p1.X + (p2.X - p1.X) / 2,
                Y = p1.Y + (p2.Y - p1.Y) / 2
            };
        }
    }
}
