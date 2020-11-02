using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DiegoG.Utilities;

namespace DiegoG.MonoGame
{
    public partial class Camera
    {
        public class CameraPosition
        {
            public Vector2 Position { get; set; }
            public float Rotation { get; set; }
            public CFloat ZoomLevel { get; set; }

            public CameraPosition(Vector2 position, float rotation, float zoom)
            {
                Position = position;
                Rotation = rotation;
                ZoomLevel = new CFloat(zoom, 0.01f, 3f);
            }

            public override string ToString() => $"X:{Position.X} Y:{Position.Y} R:{Rotation} Z:{ZoomLevel}";
            public string ToString(string format)
                => $"X:{Position.X.ToString(format)} Y:{Position.Y.ToString(format)} R:{Rotation.ToString(format)} Z:{((float)ZoomLevel).ToString(format)}";
            public CameraPosition InvertedPos() => new CameraPosition(new Vector2(-Position.X, -Position.Y), Rotation, ZoomLevel);
        }

        public struct DrawData
        {
            public Camera Camera { get; set; }
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
                    Vector2 newvector = (Position.ToVector2(Length.Units.Pixel) + Camera.current.Position);
                    newvector *= Scale;
                    newvector -= Origin;
                    Point newpoint = newvector.ToPoint();
                    return new Rectangle(newpoint.X, newpoint.Y, Texture.Width, Texture.Height);
                }
            }

            public DrawData(Camera camera) : this()
            {
                Camera = camera;
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
}
