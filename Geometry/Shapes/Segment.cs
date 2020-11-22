using System;
using System.Drawing;
using System.Numerics;

namespace DiegoG.Geometry.Shapes
{
    public class Segment : DrawableShape
    {
        public Point Start { get; private set; }
        public Point End { get; private set; }
        public Point Mid => (Start + End) / 2;
        public string WrittenEquation => $"y={Slope}x+{(Slope * -Start.X) + Start.Y}";
        public double Slope { get; private set; }
        public double Length
            => Math.Sqrt(Math.Pow(End.X - Start.X, 2) + Math.Pow(End.Y - Start.Y, 2));
        public double InclinationDeg => Utilities.RadToDeg(Math.Atan(Slope));
        public double B => 1;
        public double A => -Slope;
        public double C => Start.Y - (Slope * Start.X);
        public Vector2 Vector => new Vector2(End.X - Start.X, End.Y - Start.Y);
        public Segment(string label, Point a, Point b) : base(label)
        {
            Start = a;
            End = b;
            Slope = (End.Y - Start.Y) / (double)(End.X - Start.X);
        }
        public Segment(string label, double slope, Point a) : base(label)
        {
            Start = a;
            End = new Point(0, 0);
            Slope = slope;
        }
        public static double Angle(Segment A, Segment B)
            => Math.Acos(Utilities.Vector2Scalar(A.Vector, B.Vector) / (A.Vector.Length() * B.Vector.Length()));
        public static double AngleDeg(Segment A, Segment B)
            => Utilities.RadToDeg(Angle(A, B));

        public static bool operator >(Segment A, Segment B)
            => A.Length > B.Length;
        public static bool operator <(Segment A, Segment B)
            => A.Length < B.Length;
        public static bool operator ==(Segment A, Segment B)
            => A.Length == B.Length;
        public static bool operator !=(Segment A, Segment B)
            => !(A == B);
        public bool IsParallel(Segment other)
            => Slope == other.Slope;
        public bool IsVertical => Math.Abs(End.X - Start.X) < 0.00001f;
        public bool Intersect(Segment other, out Point intersection)
        {
            intersection = new Point(0, 0);
            if (IsVertical && other.IsVertical)
            {
                return false;
            }

            if (IsVertical || other.IsVertical)
            {
                intersection = GetIntersectionPointIfOneIsVertical(other, this);
                return true;
            }
            double delta = (Start * other.End).Scalar - (other.Start * End).Scalar;
            bool hasIntersection = Math.Abs(delta - 0) > 0.0001f;
            if (hasIntersection)
            {
                double x = (other.End.Scalar * C - End.Scalar * other.C) / delta;
                double y = (Start.Scalar * other.C - other.Start.Scalar * C) / delta;
                intersection = new Point((float)x, (float)y);
            }
            return hasIntersection;
        }
        private static Point GetIntersectionPointIfOneIsVertical(Segment line1, Segment line2)
        {
            Segment verticalLine = line2.IsVertical ? line2 : line1;
            Segment nonVerticalLine = line2.IsVertical ? line1 : line2;

            float y = (verticalLine.Start.X - nonVerticalLine.Start.X) *
                       (nonVerticalLine.End.Y - nonVerticalLine.Start.Y) /
                       (nonVerticalLine.End.X - nonVerticalLine.Start.X) +
                       nonVerticalLine.Start.Y;
            float x = line1.IsVertical ? line1.Start.X : line2.Start.X;
            return new Point(x, y);
        }

        public override bool Equals(object obj)
        {
            if (obj is Segment point)
            {
                return this == point;
            }

            return false;
        }
        public override int GetHashCode()
            => base.GetHashCode();

        public static string VerboseAngleDeg(Segment A, Segment B)
        {
            var vecA = Utilities.VectorFromPoints(A.Start, A.End);
            var vecB = Utilities.VectorFromPoints(B.Start, B.End);
            var modA = vecA.Length();
            var modB = vecB.Length();
            var scalar = Utilities.Vector2Scalar(vecA, vecB);

            return $"vecA = (LineA.PointB.X - LineA.PointA.X, LineA.PointB.Y - LineA.PointA.Y) = ({A.End.X} - {A.Start.X}, {A.End.Y} - {A.Start.Y}) = ({A.End.X - A.Start.X}, {A.End.Y - A.Start.Y})\n" +
            $"vecB = (LineB.PointB.X - LineB.PointA.X, LineB.PointB.Y - LineB.PointA.Y) = ({B.End.X} - {B.Start.X}, {B.End.Y} - {B.Start.Y}) = ({B.End.X - B.Start.X}, {B.End.Y - B.Start.Y})\n" +
            $"Angle = ArcCosine( (vecA * vecB) / (modA * modB) ) = ArcCosine( ({vecA} * {vecB}) / ({modA} * {modB}) ) = ArcCosine( {scalar} / {modA * modB} ) = ArcCosine({scalar / (modA * modB)}) = {AngleDeg(A, B)}º";
        }

        public override void Draw(MainForm canvas, Color color)
        {
            var p = new Pen(color, 1f);
            canvas.DrawTo(g => g.DrawLine(p, Start, End));
            canvas.DrawTo(g => g.DrawString(Label, canvas.Font, new SolidBrush(color), Mid));
        }
    }
}
