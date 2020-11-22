using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace DiegoG.Geometry.Shapes
{
    public abstract class DrawableShape
    {
        public float Zoom { get; set; } = 0;
        public static Point LabelOffset => new Point(6, 0);
        public static bool DrawCoordinates { get; set; } = false;
        public string Label { get; set; } = "";
        public int DrawID { get; set; }
        public abstract void Draw(MainForm canvas, Color color);
        public DrawableShape(string label)
            => Label = label;
    }

    public class DrawingList : IEnumerable<DrawableShape>
    {
        private List<(DrawableShape shape, Color color)> List { get; } = new List<(DrawableShape, Color)>();
        public void Add(DrawableShape shape)
        {
            shape.DrawID = List.Count;
            List.Add((shape, NextColor));
        }
        public void Clear()
            => List.Clear();
        public void Remove(int drawid)
            => List.RemoveAt(drawid);
        public void Remove(DrawableShape shape)
            => List.RemoveAt(List.FindIndex(i => ReferenceEquals(shape, i.shape)));

        public List<Color> ColorList { get; } = new List<Color>() { Color.Red, Color.Blue, Color.Purple, Color.Green, Color.Yellow, Color.AliceBlue, Color.Aqua, Color.Aquamarine };

        private int NextColorInt = 0;
        public Color NextColor
        {
            get
            {
                if (NextColorInt >= ColorList.Count)
                    NextColorInt = 0;

                NextColorInt++;
                return ColorList[NextColorInt - 1];
            }
        }

        public void DrawAll()
        {
            foreach (var (shape, color) in List)
                shape.Draw(Program.MainForm, color);
        }
        public IEnumerator<DrawableShape> GetEnumerator()
        {
            foreach (var (shape, _) in List)
                yield return shape;
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
