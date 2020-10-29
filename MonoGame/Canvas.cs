using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MagicGame
{
    using static Program;
    public class Canvas : IDisposable
    {

        public RenderTarget2D RenderTarget { get; set; }
        public SpriteBatch SpriteBatch { get; set; }
        public Rectangle Dimensions { get; set; }

        private int DebugPrint_lines = 0;

        public Canvas(int width, int height)
        {
            RenderTarget = new RenderTarget2D(GameObject.GraphicsDevice, width, height);
            Dimensions = new Rectangle(0, 0, width, height);
        }

        private void _header()
        {
            GameObject.GraphicsDevice.SetRenderTarget(RenderTarget);
            SpriteBatch.Begin();
        }
        private void _footer()
        {
            SpriteBatch.End();
            GameObject.GraphicsDevice.SetRenderTarget(null);
        }

        public void Clear()
        {
            GameObject.GraphicsDevice.Clear(Color.Transparent);
            DebugPrint_lines = 0;
        }

        public void DrawStringTo(SpriteFont spriteFont, string text, Vector2 position, Color color)
        {
            _header();
            //
            SpriteBatch.DrawString(spriteFont, text, position, color);
            //
            _footer();
        }

        public void DrawTo(Camera.DrawData c, bool clear = false, string debug = "", string verbose = "")
        {
            _header();
            //

            if (clear)
            {
                Clear();
            }

            if (c.DestinationRectangle != null)
            {
                Rectangle desrec = (Rectangle)c.DestinationRectangle;
                desrec.Inflate(desrec.Width * c.Scale.X, desrec.Height * c.Scale.Y);
                desrec.Location = c.Position.ToVector2(Length.Units.Pixel).ToPoint();

                SpriteBatch.Draw(c.Texture, desrec, c.SourceRectangle, c.Color, c.Rotation, c.Origin, c.Fx, c.LayerDepth);
                goto enddraw;
            }
            SpriteBatch.Draw(c.Texture, c.Position.ToVector2(Length.Units.Pixel), c.SourceRectangle, c.Color, c.Rotation, c.Origin, c.Scale, c.Fx, c.LayerDepth);

            enddraw:;

            if (Configurations.System.Debug && (debug != null || debug != ""))
            {
                SpriteBatch.DrawString(Assets.GetSpriteFont("Default"), debug, new Vector2(5, 15 * DebugPrint_lines + 5), Color.Black);
                DebugPrint_lines++;
            }
            if (Configurations.System.Verbose && (verbose != null || verbose != ""))
            {
                SpriteBatch.DrawString(Assets.GetSpriteFont("Default"), verbose, new Vector2(5, 15 * DebugPrint_lines + 5), Color.Red);
                DebugPrint_lines++;
            }

            //
            _footer();
        }

        public void Dispose()
        {
            SpriteBatch.Dispose();
            RenderTarget.Dispose();
        }

        public static implicit operator RenderTarget2D(Canvas c)
        {
            return c.RenderTarget;
        }

    }
}
