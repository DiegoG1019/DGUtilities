using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPoint = System.Drawing.Point;

namespace DiegoG.Utilities.Replacements
{
    /// <summary>
    /// Usage of this structure may complicate things by adding an unnecessary amount of explicit conversions, as well as conversions in general are unoptimal
    /// Use only if you don't have system.drawing or prefer using this in a large portion of your code.
    /// </summary>
    public struct Point : IEquatable<Point>
    {
        public int X { get; init; }
        public int Y { get; init; }
        public Point(int xy) : this(xy, xy) { }
        public Point(int x, int y)
        { X = x; Y = y; }

        public static Point operator +(Point a, Point b) => new(b.X + a.X, b.Y + a.Y);
        public static Point operator -(Point a, Point b) => new(b.X - a.X, b.Y - a.Y);
        public static Point operator -(Point p) => new(-p.X, -p.Y);
        public static bool operator ==(Point a, Point b) => (a.X == b.X) && (a.Y == b.Y);
        public static bool operator !=(Point a, Point b) => !(a == b);

        public static implicit operator (int X, int Y)(Point p) => (p.X, p.Y);
        public static implicit operator Point((int x, int y) n) => new(n.x, n.y);
        public static implicit operator Point(int xy) => new(xy);
        public static implicit operator DPoint(Point p) => new(p.X, p.Y);
        public static implicit operator Point(DPoint p) => new(p.X, p.Y);

        public static Point Zero => new(0);
        public static Point One => new(1);

        public override bool Equals(object obj)
            => obj is Point s && this == s;
        public bool Equals(Point p)
            => p == this;
        public override int GetHashCode()
            => base.GetHashCode();
    }
}
