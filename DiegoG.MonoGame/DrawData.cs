using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using DiegoG.Utilities.Measures;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiegoG.MonoGame
{
    public class DrawData
    {
        public Func<Vector2> RelativeTo { get; set; }
        public Rectangle? SourceRectangle { get; set; }
        public Rectangle? DestinationRectangle { get; set; }
        public Texture2D Texture { get; set; }
        public LengthVector2 Position { get; set; }
        public float LayerDepth { get; set; }
        public Color Color { get; set; }
        public float Rotation { get; set; }
        public Vector2 Origin { get; set; }
        public Vector2 Scale { get; set; }
        public SpriteEffects Fx { get; set; }

        public Rectangle DrawBox
        {
            get
            {
                if (DestinationRectangle != null)
                    return (Rectangle)DestinationRectangle;
                Vector2 newvector = (Position.ToVector2(Length.Units.Pixel) + RelativeTo());
                newvector *= Scale;
                newvector -= Origin;
                Point newpoint = newvector.ToPoint();
                return new Rectangle(newpoint.X, newpoint.Y, Texture.Width, Texture.Height);
            }
        }

        public DrawData()
        {
            Texture = null;
            Position = new LengthVector2(Vector2.Zero, Length.Units.Pixel);
            LayerDepth = 0f;
            Color = Color.White;
            Rotation = 0f;
            Scale = Vector2.One;
            Origin = Vector2.Zero;
            SourceRectangle = null;
            DestinationRectangle = null;
            Fx = SpriteEffects.None;
        }
    }
}
