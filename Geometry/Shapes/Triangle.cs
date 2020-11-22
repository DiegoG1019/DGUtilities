using System;
using System.Linq;

namespace DiegoG.Geometry.Shapes
{
    public class Triangle : DrawableShape
    {

        public enum TriangleType
        {
            Isosceles, Equilateral, Escalene,
            Acutangle, Obtusangle, Rectangle
        }

        public Point A { get; set; }
        public Point B { get; set; }
        public Point C { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Triangle segment nomenclature")]
        public Segment a => new Segment($"{Label}:a", C, B);
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Triangle segment nomenclature")]
        public Segment b => new Segment($"{Label}:b", A, C);
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Triangle segment nomenclature")]
        public Segment c => new Segment($"{Label}:c", A, B);

        public double[] AllAngles => new double[] { CAB, ABC, BCA };

        public double CAB => Segment.AngleDeg(b, c);
        public double ABC => Segment.AngleDeg(c, a);
        public double BCA => Segment.AngleDeg(a, b);

        public Segment[] AllSegments => new Segment[] { a, b, c };
        public Segment Base => AllSegments.Where(s => s.Length == AllSegments.Max(se => se.Length)).FirstOrDefault();

        public Triangle(string label, Point a, Point b, Point c) : base(label)
        {
            A = a;
            B = b;
            C = c;
        }


        public static TriangleType Clasify(Triangle t)
        {
            TriangleType type = TriangleType.Escalene;

            bool isosceles = true;
            var nonbasesegments = t.AllSegments.Where(s => !ReferenceEquals(t.Base, s)).ToList();
            for (int i = 0; i < nonbasesegments.Count - 1; i++)
                isosceles = isosceles && nonbasesegments[i] == nonbasesegments[i + 1];
            
            if (isosceles)
                type = TriangleType.Isosceles;
            else
                if (t.a == t.b && t.b == t.c)
                type = TriangleType.Equilateral;

            if (t.AllAngles.Where(s => s == 90f).Count() > 0)
                return type | TriangleType.Rectangle;

            var acuteangles = t.AllAngles.Where(s => s < 90f).Count();
            if (acuteangles == 3)
                type |= TriangleType.Acutangle;
            else
                type |= TriangleType.Obtusangle;
            return type;
        }

        public double Semiperimeter => (a.Length + b.Length + c.Length) / 2;
        public double AreaHeron => Math.Sqrt(Semiperimeter * (Semiperimeter - a.Length) * (Semiperimeter - b.Length) * (Semiperimeter - c.Length));
        public string AreaVerbose()
        {
            var s = Semiperimeter;
            var a = this.a.Length;
            var b = this.b.Length;
            var c = this.c.Length;
            return $"Area = Sqrt(s * (s - a) * (s - b) * (s - c)) => Area = Sqrt({s} * ({s}-{a}) * ({s}-{b}) * ({s}-{c}) = {AreaHeron}";
        }

        public override void Draw(MainForm canvas, System.Drawing.Color color)
        {
            throw new NotImplementedException();
        }
    }
}
