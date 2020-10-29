using DiegoG.Utilities;
using Microsoft.Xna.Framework;

namespace DiegoG.MonoGame
{
    public class LengthVector2
    {
        public Length X { get; set; }
        public Length Y { get; set; }

        public Vector2 ToVector2()
        {
            return ToVector2(Length.Units.Meter);
        }
        public Vector2 ToVector2(Length.Units unit)
        {
            return new Vector2(X.GetValue(unit), Y.GetValue(unit));
        }

        public static LengthVector2 operator +(LengthVector2 a, LengthVector2 b)
        {
            return new LengthVector2(
                new Length(a.X.Meters + b.X.Meters, Length.Units.Meter),
                new Length(a.Y.Meters + b.Y.Meters, Length.Units.Meter)
                );
        }
        public static LengthVector2 operator -(LengthVector2 a, LengthVector2 b)
        {
            return new LengthVector2(
                new Length(a.X.Meters - b.X.Meters, Length.Units.Meter),
                new Length(a.Y.Meters - b.Y.Meters, Length.Units.Meter)
                );
        }
        public static LengthVector2 operator -(LengthVector2 a)
        {
            return new LengthVector2(
                new Length(-a.X.Meters, Length.Units.Meter),
                new Length(-a.Y.Meters, Length.Units.Meter)
                );
        }

        public LengthVector2(Vector2 v, Length.Units unit) :
            this(new Length(v.X, unit), new Length(v.Y, unit))
        { }
        public LengthVector2(float x, float y, Length.Units unit) :
            this(new Length(x, unit), new Length(y, unit))
        { }
        public LengthVector2(Length value) :
            this(value, value)
        { }
        public LengthVector2(Length x, Length y) :
            this()
        {
            X = x;
            Y = y;
        }

        public static readonly LengthVector2 Zero = new LengthVector2(0f, 0f);

        public static LengthVector2 Clamp(LengthVector2 v, Length max, Length min)
        {
            var newv = (x: v.X, y: v.Y);

            if (v.X > max)
            {
                newv.x = max;
            }
            else
            {
                if (v.X < min)
                {
                    newv.x = min;
                }
            }

            if (v.Y > max)
            {
                newv.y = max;
            }
            else
            {
                if (v.Y < min)
                {
                    newv.y = min;
                }
            }

            return new LengthVector2(newv.x, newv.y);

        }

    }
}
