using DiegoG.Geometry.Shapes;
using System;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;
using Point = DiegoG.Geometry.Shapes.Point;

namespace DiegoG.Geometry
{
    public static class Program
    {
        public static MainForm MainForm { get; private set; }
        public static DrawingList DrawingList { get; } = new DrawingList();
        public static void Main()
        {
            MainForm = new MainForm()
            {
                BackColor = Color.LightGray,
                FormBorderStyle = FormBorderStyle.Sizable,
                Bounds = Screen.PrimaryScreen.Bounds,
                TopMost = true,
            };

            Application.EnableVisualStyles();
            Application.Run(MainForm);

            DrawingList.Add(new Quadrilateral("Ano", new Point(2, 4), new Point(9, 5), new Point(8, 2), new Point(1, 1)));

            while (true)
                DrawingList.DrawAll();

        }
    }

    public static class Utilities
    {
        public static Vector2 VectorFromPoints(Point A, Point B)
            => new Vector2(B.X - A.X, B.Y - A.Y);
        public static double Vector2Scalar(Vector2 A, Vector2 B)
            => (A.X * B.X) + (A.Y * B.Y);
        public static double RadToDeg(double rad) => (180 / Math.PI) * rad;
    }
}
