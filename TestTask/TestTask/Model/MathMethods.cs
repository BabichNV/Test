using LiveCharts.Defaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTask.Model
{
    public static class MathMethods
    {
        public static int Comparator(double x, double y)
        {
            if (x < y - double.Epsilon)
                return -1;
            if (Math.Abs(x - y) < double.Epsilon)
                return 0;
            if (x > y + double.Epsilon)
                return 1;
            return 0;
        }

        public static ObservablePoint GetIntersectionPoint (ObservablePoint p11, ObservablePoint p12,
            ObservablePoint p21, ObservablePoint p22)
        {
            double A1 = p12.Y - p11.Y;
            double B1 = p11.X - p12.X;
            double C1 = (A1 * p11.X + B1 * p11.Y);

            double A2 = p22.Y - p21.Y;
            double B2 = p21.X - p22.X;
            double C2 = (A2 * p21.X + B2 * p21.Y);

            double det = A1 * B2 - A2 * B1;
            double x = (C1 * B2 - C2 * B1) / det;
            double y = (A1 * C2 - A2 * C1) / det;
            return new ObservablePoint(x, y);
        }

        public static bool IsInSegment(ObservablePoint point, ObservablePoint leftpoint, ObservablePoint rightpoint)
        {
            return IsInSegment(point.X, Math.Min(leftpoint.X, rightpoint.X), Math.Max(rightpoint.X, leftpoint.X)) &&
                IsInSegment(point.Y, Math.Min(leftpoint.Y, rightpoint.Y), Math.Max(rightpoint.Y, leftpoint.Y));
        }

        public static bool IsInSegment(double point, double leftpoint, double rightpoint)
        {
            if (Comparator(leftpoint, point) < 1 && Comparator(point, rightpoint) < 1)
                return true;
            else
                return false;
        }

        public static IntersectionType GetIntersectionType(ObservablePoint p11, ObservablePoint p12,
            ObservablePoint p21, ObservablePoint p22)
        {
            double B1 = p11.X - p12.X;
            double A1 = p12.Y - p11.Y;
            double B2 = p21.X - p22.X;
            double A2 = p22.Y - p21.Y;

            double C1 = -(A1 * p11.X + B1 * p11.Y);
            if (Comparator(B1 * A2 - B2 * A1, 0) == 0)
            {
                if (Comparator(A1 * p21.X + B1 * p21.Y, C1) == 0)
                {
                    if (IsInSegment(p21, p11, p12))
                    {
                        return IntersectionType.FirstSecondSegment;
                    }
                    if (IsInSegment(p12, p21, p22))
                    {
                        return IntersectionType.SecondFirstSegment;
                    }
                }
                return IntersectionType.None;
            }
            else
            {
                return IntersectionType.One;
            }
        }

        public static bool IsEqual(ObservablePoint p1, ObservablePoint p2)
        {
            return (Comparator(p1.X, p2.X) == 0 && Comparator(p1.Y, p2.Y) == 0);
        }
        public static double GetY (double x, ObservablePoint p1, ObservablePoint p2)
        {
            double A = p2.Y - p1.Y;
            double B = p1.X - p2.X;
            double C = (A * p1.X + B * p1.Y);

            double y = (C - A * x) / B;
            return y;
        }
    }
}
