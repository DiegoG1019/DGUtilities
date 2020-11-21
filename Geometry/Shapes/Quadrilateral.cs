using System.Drawing;
using System.Linq;

namespace DiegoG.Geometry.Shapes
{
    public class Quadrilateral : DrawableShape
    {
        public enum QuadType
        {
            Square, Rectangle, Diamond, Rhomboid, RectangleTrapezoid, IsoscelesTrapezoid, ScaleneTrapezoid, Undefined
        }
        public Point A { get; set; }
        public Point B { get; set; }
        public Point C { get; set; }
        public Point D { get; set; }

        public Segment AB => new Segment(A, B);
        public Segment BC => new Segment(B, C);
        public Segment CD => new Segment(C, D);
        public Segment DA => new Segment(D, A);
        public Segment AC => new Segment(A, C);
        public Segment BD => new Segment(B, D);

        public Segment[] AllSegments => new Segment[] { AB, BC, CD, DA, AC, BD };
        public double[] AllAngles => new double[] { DAB, ABC, BCD, CDA };

        public double DAB => Segment.AngleDeg(DA, AB);
        public double ABC => Segment.AngleDeg(AB, BC);
        public double BCD => Segment.AngleDeg(BC, CD);
        public double CDA => Segment.AngleDeg(CD, DA);

        public Segment LargestSegment => AllSegments.Where(s => s.Length == AllSegments.Max(se => se.Length)).FirstOrDefault();

        public double Area => new Triangle(A, B, C).Area + new Triangle(A, D, C).Area;

        public QuadType Clasification => Clasify();
        public QuadType Clasify()
        {
            bool ConsecutiveSidesEqual = AB == BC;
            bool OppositeSidesEqual = AB == CD;
            bool DiagonalSidesEqual = AC == BD;
            bool AllSidesEqual = ConsecutiveSidesEqual && OppositeSidesEqual;
            bool AllSegmentsEqual = AllSidesEqual && DiagonalSidesEqual;

            bool ConsecutiveAnglesEqual = DAB == ABC;
            bool OppositeAnglesEqual = DAB == BCD;
            bool ConsecutiveAnglesSupplementary = !ConsecutiveAnglesEqual && (ABC + BCD == 180);


            bool ABCDParallel = AB.IsParallel(CD);

            bool DABCParallel = DA.IsParallel(BC);

            bool NonParallelSidesEqual = (!ABCDParallel && AB == CD) || (!DABCParallel && DA == CD);

            int RightAngleCount = AllAngles.Where(a => a == 90).Count();

            if (AllSegmentsEqual)
                return QuadType.Square;

            if (RightAngleCount == 4 && OppositeSidesEqual)
                return QuadType.Rectangle;

            if (AllSidesEqual && !DiagonalSidesEqual && OppositeAnglesEqual && ConsecutiveAnglesSupplementary)
                return QuadType.Diamond;

            if (OppositeSidesEqual && !ConsecutiveSidesEqual && OppositeAnglesEqual && ConsecutiveAnglesSupplementary)
                return QuadType.Rhomboid;

            if (RightAngleCount == 2)
                return QuadType.RectangleTrapezoid;

            if (NonParallelSidesEqual)
                return QuadType.IsoscelesTrapezoid;

            if (AB != BC != (CD != DA))
                return QuadType.ScaleneTrapezoid;

            return QuadType.Undefined;
        }

        public Quadrilateral(string name, Point a, Point b, Point c, Point d)
        {
            Label = name;
            A = a;
            B = b;
            C = c;
            D = d;
        }

        public override void Draw(MainForm canvas, Color color)
        {
            var p = new Pen(color, 1f);
            foreach (Segment s in AllSegments)
            {
                s.Draw(canvas, color);
            }
            canvas.DrawTo(g => g.DrawString("A", canvas.Font, new SolidBrush(color), A + LabelOffset));
            canvas.DrawTo(g => g.DrawString("B", canvas.Font, new SolidBrush(color), B + LabelOffset));
            canvas.DrawTo(g => g.DrawString("C", canvas.Font, new SolidBrush(color), C + LabelOffset));
            canvas.DrawTo(g => g.DrawString("D", canvas.Font, new SolidBrush(color), D + LabelOffset));
            AC.Intersect(CD, out Point ACCD);
            canvas.DrawTo(g => g.DrawString(Clasification.ToString(), canvas.Font, new SolidBrush(color), ACCD + LabelOffset));
        }
    }
}
