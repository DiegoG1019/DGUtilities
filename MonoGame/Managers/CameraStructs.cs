using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
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

            public override string ToString()
            {
                return $"X:{Position.X} Y:{Position.Y} R:{Rotation} Z:{ZoomLevel}";
            }

            public string ToString(string format)
            {
                return $"X:{Position.X.ToString(format)} Y:{Position.Y.ToString(format)} R:{Rotation.ToString(format)} Z:{((float)ZoomLevel).ToString(format)}";
            }

            public CameraPosition InvertedPos()
            {
                return new CameraPosition(new Vector2(-Position.X, -Position.Y), Rotation, ZoomLevel);
            }

        }

        public struct DrawData
        {
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
                    {
                        return (Rectangle)DestinationRectangle;
                    }
                    Vector2 newvector = (Position.ToVector2(Length.Units.Pixel) + Program.GameObject.Camera.current.Position);
                    newvector *= Scale;
                    newvector -= Origin;
                    Point newpoint = newvector.ToPoint();
                    return new Rectangle(newpoint.X, newpoint.Y, Texture.Width, Texture.Height);
                }
            }

            public DrawData(Texture2D texture, Vector2 position) :
                this(texture, position, 0f)
            { }
            public DrawData(Texture2D texture, Vector2 position, float layerdepth) :
                this(texture, position, layerdepth, Color.White)
            { }
            public DrawData(Texture2D texture, Vector2 position, float layerdepth, Color color) :
                this(texture, position, layerdepth, color, 0f)
            { }
            public DrawData(Texture2D texture, Vector2 position, float layerdepth, Color color, float rotation) :
                this(texture, position, layerdepth, color, rotation, new Vector2(1, 1))
            { }
            public DrawData(Texture2D texture, Vector2 position, float layerdepth, Color color, float rotation, Vector2 scale) :
                this(texture, position, layerdepth, color, rotation, scale, new Vector2(0, 0))
            { }
            public DrawData(Texture2D texture, Vector2 position, float layerdepth, Color color, float rotation, Vector2 scale, Vector2 origin) :
                this(texture, position, layerdepth, color, rotation, scale, origin, null, null)
            { }
            public DrawData(Texture2D texture, Vector2 position, float layerdepth, Color color, float rotation, Vector2 scale, Vector2 origin, Rectangle? sourcerectangle) :
                this(texture, position, layerdepth, color, rotation, scale, origin, sourcerectangle, null)
            { }
            public DrawData(Texture2D texture, Vector2 position, float layerdepth, Color color, float rotation, Vector2 scale, Vector2 origin, Rectangle? sourcerectangle, Rectangle? destinationrectangle) :
                this(texture, position, layerdepth, color, rotation, scale, origin, sourcerectangle, destinationrectangle, SpriteEffects.None)
            { }
            public DrawData(Texture2D texture, Vector2 position, float layerdepth, Color color, float rotation, Vector2 scale, Vector2 origin, Rectangle? sourcerectangle, Rectangle? destinationrectangle, SpriteEffects fx) :
                this()
            {
                Texture = texture;
                Position = new LengthVector2(position, Length.Units.Pixel);
                LayerDepth = layerdepth;
                Color = color;
                Rotation = rotation;
                Origin = origin;
                Scale = scale;
                Fx = fx;
                SourceRectangle = sourcerectangle;
                DestinationRectangle = destinationrectangle;
            }

        }
    }
}
