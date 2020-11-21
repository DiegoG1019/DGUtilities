using System;

namespace DiegoG.Geometry.Shapes
{
    public class Triangle : DrawableShape
    {
        public Point A { get; set; }
        public Point B { get; set; }
        public Point C { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Triangle segment nomenclature")]
        public Segment a => new Segment(C, B);
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Triangle segment nomenclature")]
        public Segment b => new Segment(A, C);
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Triangle segment nomenclature")]
        public Segment c => new Segment(A, B);

        public Triangle(Point a, Point b, Point c)
        {
            A = a;
            B = b;
            C = c;
        }
        public double Semiperimeter => ((A + B + C) / 2).Scalar;
        public double Area => Math.Sqrt(Semiperimeter * (Semiperimeter - a.Length) * (Semiperimeter - b.Length) * (Semiperimeter - c.Length));
        public string AreaVerbose()
        {
            var s = Semiperimeter;
            var a = this.a.Length;
            var b = this.b.Length;
            var c = this.c.Length;
            return $"Area = Sqrt(s * (s - a) * (s - b) * (s - c)) => Area = Sqrt({s} * ({s}-{a}) * ({s}-{b}) * ({s}-{c}) = {Area}";
        }

        public override void Draw(MainForm canvas, System.Drawing.Color color)
        {
            throw new NotImplementedException();
        }
    }
}
