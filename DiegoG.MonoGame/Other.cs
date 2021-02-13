using Microsoft.Xna.Framework;
using System;

namespace DiegoG.MonoGame
{
    public static class Other
    {
        public static Color TintColor(Color A, Color B)
        {
            byte r, g, b;

            B = Color.Multiply(B, 255f / B.A);

            r = (byte)MathHelper.Clamp((A.R + B.R) / 2, 0, 255);
            g = (byte)MathHelper.Clamp((A.G + B.G) / 2, 0, 255);
            b = (byte)MathHelper.Clamp((A.B + B.B) / 2, 0, 255);

            return new Color(r, g, b, A.A);

        }

        public static Vector2 HeadingVector(Vector2 origin, Vector2 destination)
        {
            double theta = Math.Atan2(origin.Y, destination.X);
            return new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
        }

        public static double GetVector2Distance(Vector2 A, Vector2 B) => Math.Sqrt(Math.Pow(B.X - A.X, 2f) + Math.Pow(B.Y - A.Y, 2f));
    }
}
