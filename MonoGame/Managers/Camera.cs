using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PrimitiveBuddy;
using Serilog;
using System;
using System.Collections.Generic;
using static MagicGame.Program;
using DiegoG.Utilities;

namespace DiegoG.MonoGame
{
    public partial class Camera //Camera.cs
    {
        public SpriteBatch SpriteBatch { get; set; }
        public Primitive Primitive { get; set; }
        private CameraPosition current { get; set; }
        private CameraPosition goal { get; set; }
        public Color? Tint { get; set; }

        public CameraPosition Current
        {
            get
            {
                return current.InvertedPos();
            }
        }
        public CameraPosition Goal
        {
            get
            {
                return goal.InvertedPos();
            }
        }

        public MouseState GetRelativeMouseState()
        {
            return GetRelativeMouseState(Program.GameObject.Window);
        }

        public MouseState GetRelativeMouseState(GameWindow gameWindow)
        {

            float curx = current.Position.X, cury = current.Position.Y;

            MouseState newms = Mouse.GetState(gameWindow);

            return new MouseState(
                (int)((newms.X + curx) / current.ZoomLevel),
                (int)((newms.Y + cury) / current.ZoomLevel),
                newms.ScrollWheelValue,
                newms.LeftButton,
                newms.MiddleButton,
                newms.RightButton,
                newms.XButton1,
                newms.XButton2,
                newms.HorizontalScrollWheelValue
                );

        }

        public Camera()
        {
            GraphicsDevice gd = Program.GameObject.GraphicsDevice;

            current = new CameraPosition(new Vector2(0, 0), 0, 1f);
            goal = new CameraPosition(new Vector2(0, 0), 0, 1f);

            SpriteBatch = new SpriteBatch(Program.GameObject.GraphicsDevice);

            Program.GameObject.KeyboardStateChanged += new MagicGameClass.KeyboardInput(KeyboardInput);
            Primitive = new Primitive(GameObject.GraphicsDevice, SpriteBatch);

            //TEST CODE
            SetInputReaction();

        }

        public void Move(float X, float Y)
        {
            Move(new Vector2(X, Y));
        }
        public void Move(Vector2 pos)
        {
            goal.Position += pos;
        }

        public void Set(float X, float Y)
        {
            Set(new Vector2(X, Y));
        }
        public void Set(Vector2 pos)
        {
            goal.Position = pos;
        }

        public void Rotate(float r)
        {
            goal.Rotation += r;
        }

        public void SetRotation(float r)
        {
            goal.Rotation = r;
        }

        public void Zoom(float z)
        {
            SetZoom(goal.ZoomLevel + z);
        }
        public void SetZoom(float z)
        {
            goal.ZoomLevel = new CFloat(z, goal.ZoomLevel.UpperLimit, goal.ZoomLevel.LowerLimit);
        }

        public bool isLocked { get; private set; }
        public IGameLiveObject LockedTo { get; private set; }
        public void LockTo(IGameLiveObject obj)
        {
            isLocked = true;
            LockedTo = obj;
        }
        public void ReleaseLock()
        {
            isLocked = false;
            LockedTo = null;
        }

        public void Update(GameTime gt)
        {

            if (isLocked)
            {
                var a = (w: GameObject.GraphicsManager.PreferredBackBufferWidth, h: GameObject.GraphicsManager.PreferredBackBufferHeight);
                goal.Position = new Vector2(
                    (-LockedTo.Position.X.Pixels) + (a.w / 2),
                    (-LockedTo.Position.Y.Pixels) + (a.h / 2)
                );
            }

            current.Position -= (current.Position - goal.Position) * 10f * new Vector2((float)gt.ElapsedGameTime.TotalSeconds);
            current.Rotation -= (current.Rotation - goal.Rotation) * 10f * (float)gt.ElapsedGameTime.TotalSeconds;
            current.ZoomLevel = new CFloat(
                current.ZoomLevel - (current.ZoomLevel - goal.ZoomLevel) * 10f * (float)gt.ElapsedGameTime.TotalSeconds,
                current.ZoomLevel.UpperLimit,
                current.ZoomLevel.LowerLimit
                );

            DebugPrint_lines = 0;

            VerbosePrint($"Current:({Current.ToString("0.00")}) | Goal:({Goal.ToString("0.00")})");

        }

        public void Draw(DrawData c)
        {
            try
            {
                Color newcolor = (Tint != null ? UtilMath.TintColor(c.Color, (Color)Tint) : c.Color);
                Vector2 pos = c.Position.ToVector2(Length.Units.Pixel) + current.Position;
                float depth = c.LayerDepth + (pos.Y / 10000);

                if (c.DestinationRectangle != null)
                {
                    Rectangle desrec = (Rectangle)c.DestinationRectangle;
                    desrec.Inflate(desrec.Width * (c.Scale.X + current.ZoomLevel), desrec.Height * (c.Scale.Y + current.ZoomLevel));
                    desrec.Location = pos.ToPoint();

                    SpriteBatch.Draw(c.Texture, desrec, c.SourceRectangle, newcolor, c.Rotation + current.Rotation, c.Origin, c.Fx, depth);
                    goto End;
                }
                SpriteBatch.Draw(c.Texture, pos, c.SourceRectangle, newcolor, c.Rotation + current.Rotation, c.Origin, c.Scale + new Vector2(current.ZoomLevel), c.Fx, depth);

                End:;
                DebugPrint($"Drawing on: {pos.X},{pos.Y} at depth {depth}");
                return;
            }
            catch (InvalidOperationException e)
            {
                const string str = "Perhaps Camera.Draw was called outside of Main Draw?";
                throw new InvalidOperationException(str, e);
            }
        }

        public void DrawString(string fontname, string text, Vector2 position, Color color, Vector2 origin, float rotation = 0, float scale = 1, SpriteEffects fx = SpriteEffects.None, float layer = 0)
        {
            DrawString(Assets.GetSpriteFont(fontname), text, position, color, origin, rotation, scale, fx, layer);
        }
        public void DrawString(SpriteFont font, string text, Vector2 position, Color color, Vector2 origin, float rotation = 0, float scale = 1, SpriteEffects fx = SpriteEffects.None, float layer = 0)
        {
            if(font == null)
            {
                font = Assets.GetSpriteFont("Default");
            }
            SpriteBatch.DrawString(font, text, position + current.Position, color, rotation + current.Rotation, origin, scale + current.ZoomLevel, fx, layer);
        }

        private int DebugPrint_lines = 0;
        public void DebugPrint(string s)
        {
            if (Configurations.System.Debug)
            {
                SpriteBatch.DrawString(Assets.GetSpriteFont("DebugFont"), s, new Vector2(5, 15 * DebugPrint_lines + 5), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, Layer.DebugText);
                DebugPrint_lines += s.Split('\n').Length + 1;
            }
        }

        public void VerbosePrint(string s)
        {
            if (Configurations.System.Verbose)
            {
                SpriteBatch.DrawString(Assets.GetSpriteFont("DebugFont"), s, new Vector2(5, 15 * DebugPrint_lines + 5), Color.Crimson, 0, Vector2.Zero, 1, SpriteEffects.None, Layer.DebugText);
                DebugPrint_lines += s.Split('\n').Length + 1;
            }
        }

        //Primitives
        /// <summary>
        /// Draw Circle
        /// </summary>
        /// <param name="position"></param>
        /// <param name="radius"></param>
        /// <param name="color"></param>
        public void DrawShape(Vector2 position, float radius, Color color)
        {
            Primitive.Circle(position + current.Position, radius, color);
        }
        /// <summary>
        /// Draw Line
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="color"></param>
        public void DrawShape(Vector2 start, Vector2 end, Color color)
        {
            Primitive.Line(start + current.Position, end + current.Position, color);
        }
        /// <summary>
        /// Draw Pie
        /// </summary>
        /// <param name="position"></param>
        /// <param name="radius"></param>
        /// <param name="startAngle"></param>
        /// <param name="sweepAngle"></param>
        /// <param name="color"></param>
        public void DrawShape(Vector2 position, float radius, float startAngle, float sweepAngle, Color color)
        {
            Primitive.Pie(position + current.Position, radius, startAngle, sweepAngle, color);
        }
        /// <summary>
        /// Draw Point
        /// </summary>
        /// <param name="position"></param>
        /// <param name="color"></param>
        public void DrawShape(Vector2 position, Color color)
        {
            Primitive.Point(position + current.Position, color);
        }
        /// <summary>
        /// Draw Rectangle
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="color"></param>
        public void DrawShape(Rectangle rect, Color color)
        {
            var nrect = new Rectangle(
                (int)(rect.X + current.Position.X),
                (int)(rect.Y + current.Position.Y),
                rect.Width,
                rect.Height
                );
            Primitive.Rectangle(nrect, color);
        }
        /// <summary>
        /// Draw Sine Wave
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="frequency"></param>
        /// <param name="amplitude"></param>
        /// <param name="color"></param>
        public void DrawShape(Vector2 start, Vector2 end, float frequency, float amplitude, Color color)
        {
            Primitive.SineWave(start + current.Position, end + current.Position, frequency, amplitude, color);
        }

        //TEST-CODE
        Dictionary<Keys, Vector2> InputReaction;
        private void SetInputReaction()
        {
            InputReaction = new Dictionary<Keys, Vector2>
            {
                { Keys.Up, DirectionVector2.Up*30 },
                { Keys.Down, DirectionVector2.Down*30 },
                { Keys.Left, DirectionVector2.Left*30 },
                { Keys.Right, DirectionVector2.Right*30 }
            };
        }
        private void KeyboardInput(KeyboardState ksprev, KeyboardState ksnow)
        {
            if (ksnow.IsKeyDown(Keys.LeftControl))
            {
                foreach (Keys k in ksnow.GetPressedKeys())
                {
                    if (InputReaction.ContainsKey(k))
                    {
                        Move(InputReaction[k]);
                    }
                    if (k == Keys.L)
                    {
                        if (isLocked)
                        {
                            Log.Verbose("Releasing Camera");
                            ReleaseLock();
                        }
                        Log.Verbose("Locking Camera to Player");
                        //LockedTo();
                    }
                }
            }
        }

    }
}
