using Emilie.Core.Numerics;
using System;
using System.Numerics;
using Windows.Foundation;

namespace Emilie.UWP.Maths
{
    public static class FrameworkMaths
    {

        //////////////////////////////////////////////////////
        // GET POINT (Rectangle Helpers)
        /////////////////////////////////////////////////////

        public static Point GetBottomLeftPoint(this Rect rect)
        {
            return new Point(rect.Left, rect.Bottom);
        }

        public static Point GetTopLeftPoint(this Rect rect)
        {
            return new Point(rect.Left, rect.Top);
        }
        public static Point GetTopRightPoint(this Rect rect)
        {
            return new Point(rect.Right, rect.Top);
        }

        public static Point GetBottomRightPoint(this Rect rect)
        {
            return new Point(rect.Right, rect.Bottom);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="rotationCenter">Relative to canvas (not object)</param>
        /// <param name="angle">rotation angle in radians</param>
        /// <returns></returns>
        public static Point GetRotatedBottomRightPoint(this Rect rect, Point rotationCenter, float angle)
        {
            return RotatePoint(rotationCenter, rect.GetBottomRightPoint(), angle);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="rotationCenter">Relative to canvas (not object)</param>
        /// <param name="angle">rotation angle in radians</param>
        /// <returns></returns>
        public static Point GetRotatedTopRightPoint(this Rect rect, Point rotationCenter, float angle)
        {
            return RotatePoint(rotationCenter, rect.GetTopRightPoint(), angle);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="rotationCenter">Relative to canvas (not object)</param>
        /// <param name="angle">rotation angle in radians</param>
        /// <returns></returns>
        public static Point GetRotatedTopLeftPoint(this Rect rect, Point rotationCenter, float angle)
        {
            return RotatePoint(rotationCenter, rect.GetTopLeftPoint(), angle);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="rotationCenter">Relative to canvas (not object)</param>
        /// <param name="angle">rotation angle in radians</param>
        /// <returns></returns>
        public static Point GetRotatedBottomLeftPoint(this Rect rect, Point rotationCenter, float angle)
        {
            return RotatePoint(rotationCenter, rect.GetBottomLeftPoint(), angle);
        }

        public static Point GetMidPoint(Point pt1, Point pt2)
        {
            var midX = (pt1.X + pt2.X) / 2;
            var midY = (pt1.Y + pt2.Y) / 2;
            return new Point(midX, midY);
        }

        public static Vector2 GetCenter(this Rect rect)
        {
            return new Vector2(
                (float)(rect.Left + (rect.Width / 2d)),
                (float)(rect.Top + (rect.Height / 2d)));
        }

        //public static UnRotateVector2(Vector2 pivot, Vector2 rotatedPoint, float angle)
        //{
        //    float _sinA = (float)Math.Sin(angle);
        //    float _cosA = (float)Math.Cos(angle);

        //    float _x = rotatedPoint.X - pivot.X;
        //    float _y = rotatedPoint.Y - pivot.Y;

        //    float __x = (float)(rotatedPoint.X * _cosA) - (rotatedPoint.Y * _sinA);

        //    x

        //    return new Vector2(0,0);
        //}


        public static Point RotatePoint(Point p, Point r, float a)
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

        public static float AngleBetweenPoints(Point center, Point point)
        {
            float xDiff = (float)(point.X - center.X);
            float yDiff = (float)(point.Y - center.Y);
            var degrees = (float)(Math.Atan2(yDiff, xDiff));
            return degrees;
        }

        public static uint GetScaledDimension(Size dimensions, uint targetWidth)
        {
            var thing = dimensions.Width / targetWidth;
            return ((float)(dimensions.Height / thing)).Round();
        }

        public static float GetScale(this Size size1, Size size2)
        {
            return (float)(size2.Width / size1.Width);
        }

        public static Point Scale(this Point point, float scale)
        {
            return new Point(point.X * scale, point.Y * scale);
        }

        public static Rect Scale(this Rect rect, float scale)
        {
            if (rect.IsEmpty)
                return rect;

            return new Rect(
                rect.X * scale,
                rect.Y * scale,
                rect.Width * scale,
                rect.Height * scale);
        }

        public static Rect CenterScale(this Rect rect, float scale)
        {
            if (rect.IsEmpty)
                return rect;

            Rect scaled = new Rect(
                rect.X * scale,
                rect.Y * scale,
                rect.Width * scale,
                rect.Height * scale);

            return rect.Center(scaled);
        }

        public static Size Scale(this Size size, float scale)
        {
            return new Size(size.Width * scale, size.Height * scale);
        }
        /// <summary>
        /// Expands the dimensions of the box in all directions by the given radius, keeping the same center point
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Rect Expand(this Rect rect, float radius)
        {
            if (rect.IsEmpty)
                return rect;

            return new Rect(
                rect.X -= radius,
                rect.Y -= radius,
                rect.Width += (radius * 2),
                rect.Height += (radius * 2));
        }

        public static Rect Round(this Rect r)
        {
            return new Rect(
                new Point(Math.Floor(r.Left), Math.Floor(r.Top)),
                new Point(Math.Ceiling(r.Right), Math.Ceiling(r.Bottom)));
        }

        public static Rect Offset(this Rect r, Vector2 offset)
        {
            if (r.IsEmpty)
                return r;

            return new Rect(r.X + offset.X, r.Y + offset.Y, r.Width, r.Height);
        }

        /// <summary>
        /// Sets the centre point of one rectangle to match the centre point of another
        /// </summary>
        /// <param name="r"></param>
        /// <param name="toCenter"></param>
        /// <returns></returns>
        public static Rect Center(this Rect r, Rect toCenter)
        {
            if (r.IsEmpty)
                return r;

            return new Rect
            {
                X = r.X + ((r.Width - toCenter.Width) / 2d),
                Y = r.Y + ((r.Height - toCenter.Height) / 2d),
                Width = toCenter.Width,
                Height = toCenter.Height
            };
        }

        public static Rect FillInside(this Rect toCenter, Rect parent, bool centre = true)
        {
            Size from = new Size(toCenter.Width, toCenter.Height);
            Size final = ScaleSize(from, parent.Width, parent.Height);

            Rect rect = new Rect(0, 0, final.Width, final.Height);
            return (centre) ? parent.Center(rect) : rect;
        }

        /// <summary>
        /// Note : returns results rounded as integers
        /// </summary>
        /// <param name="from"></param>
        /// <param name="maxWidth"></param>
        /// <param name="maxHeight"></param>
        /// <returns></returns>
        public static Size ScaleSize(Size from, double? maxWidth, double? maxHeight)
        {
            if (!maxWidth.HasValue && !maxHeight.HasValue) throw new ArgumentException("At least one scale factor (toWidth or toHeight) must not be null.");
            if (from.Height == 0 || from.Width == 0) throw new ArgumentException("Cannot scale size from zero.");

            double? widthScale = null;
            double? heightScale = null;

            if (maxWidth.HasValue)
            {
                widthScale = maxWidth.Value / (double)from.Width;
            }
            if (maxHeight.HasValue)
            {
                heightScale = maxHeight.Value / (double)from.Height;
            }

            double scale = Math.Min((double)(widthScale ?? heightScale),
                                     (double)(heightScale ?? widthScale));

            return new Size((int)Math.Floor(from.Width * scale), (int)Math.Ceiling(from.Height * scale));
        }

        public static Size ScaleSizeDouble(Size from, double? maxWidth, double? maxHeight)
        {
            if (!maxWidth.HasValue && !maxHeight.HasValue) throw new ArgumentException("At least one scale factor (toWidth or toHeight) must not be null.");
            if (from.Height == 0 || from.Width == 0) throw new ArgumentException("Cannot scale size from zero.");

            double? widthScale = null;
            double? heightScale = null;

            if (maxWidth.HasValue)
            {
                widthScale = maxWidth.Value / (double)from.Width;
            }
            if (maxHeight.HasValue)
            {
                heightScale = maxHeight.Value / (double)from.Height;
            }

            double scale = Math.Min((double)(widthScale ?? heightScale),
                                     (double)(heightScale ?? widthScale));

            return new Size(from.Width * scale, from.Height * scale);
        }

        public static Size Size(this Rect r)
        {
            return new Size(r.Width, r.Height);
        }

        public static USize USize(this Rect r)
        {
            return new USize(r.Width.Round(), r.Height.Round());
        }

        public static bool Intersects(this Rect rect1, Rect rect2)
        {
            Rect r = rect1;
            r.Intersect(rect2);
            return r != Rect.Empty;
        }
    }
}
