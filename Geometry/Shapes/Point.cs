﻿using System.Drawing;

namespace DiegoG.Geometry.Shapes
{
    public struct Point
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Point(float x, float y)
        {
            X = x;
            Y = y;
        }

        public double Scalar => X + Y;
        public static Point operator -(Point A, Point B)
            => new Point(A.X - B.X, A.Y - B.Y);
        public static Point operator +(Point A, Point B)
            => new Point(A.X + B.X, A.Y + B.Y);
        public static Point operator *(Point A, Point B)
            => new Point(A.X * B.X, A.Y * B.Y);
        public static Point operator /(Point A, Point B)
            => new Point(A.X / B.X, A.Y / B.Y);
        public static Point operator /(Point A, float b)
            => new Point(A.X / b, A.Y / b);
        public static Point operator *(Point A, float b)
            => new Point(A.X * b, A.Y * b);
        public static bool operator ==(Point A, Point B)
            => A.X == B.X && A.Y == B.Y;
        public static bool operator !=(Point A, Point B)
            => !(A == B);
        public override string ToString()
            => $"Point: ({X}, {Y})";
        public override bool Equals(object obj)
        {
            if (obj is Point point)
                return this == point;
            return false;
        }

        public static implicit operator PointF(Point point)
            => new PointF(point.X, point.Y);
        public static implicit operator Point(PointF point)
            => new Point(point.X, point.Y);

        public override int GetHashCode()
            => base.GetHashCode();
    }
}
