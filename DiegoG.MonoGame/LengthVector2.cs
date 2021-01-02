using DiegoG.Utilities;
using DiegoG.Utilities.Measures;
using Microsoft.Xna.Framework;
using System;
using static DiegoG.Utilities.Other;

namespace DiegoG.MonoGame
{
    public class LengthVector2
    {
        public Length X { get; set; }
        public Length Y { get; set; }

        public Vector2 ToVector2() => ToVector2(Length.Units.Meter);
        public Vector2 ToVector2(Length.Units unit) => new Vector2((float)X[unit], (float)Y[unit]);
        public static LengthVector2 operator +(LengthVector2 a, LengthVector2 b)
        {
            return new LengthVector2(
                new Length(a.X.Meter + b.X.Meter, Length.Units.Meter),
                new Length(a.Y.Meter + b.Y.Meter, Length.Units.Meter)
                );
        }
        public static LengthVector2 operator -(LengthVector2 a, LengthVector2 b)
        {
            return new LengthVector2(
                new Length(a.X.Meter - b.X.Meter, Length.Units.Meter),
                new Length(a.Y.Meter - b.Y.Meter, Length.Units.Meter)
                );
        }
        public static LengthVector2 operator -(LengthVector2 a)
        {
            return new LengthVector2(
                new Length(-a.X.Meter, Length.Units.Meter),
                new Length(-a.Y.Meter, Length.Units.Meter)
                );
        }

        public LengthVector2(Vector2 v, Length.Units unit) :
            this(new Length((decimal)v.X, unit), new Length((decimal)v.Y, unit))
        { }
        public LengthVector2(float x, float y, Length.Units unit) :
            this(new Length((decimal)x, unit), new Length((decimal)y, unit))
        { }
        public LengthVector2(Length value) :
            this(value, value)
        { }
        public LengthVector2(Length x, Length y)
        {
            X = x;
            Y = y;
        }

        public static readonly LengthVector2 Zero = new LengthVector2(0f, 0f, Length.Units.Meter);

        public static void Clamp(LengthVector2 v, Length max, Length min)
        {
            v.X = new(CapNumber(v.X.Meter, min.Meter, max.Meter), Length.Units.Meter);
            v.Y = new(CapNumber(v.Y.Meter, min.Meter, max.Meter), Length.Units.Meter);
        }

        internal static void Clamp(LengthVector2 v, float max, float min)
        {
            v.X = new(CapNumber(v.X.Meter, (decimal)min, (decimal)max), Length.Units.Meter);
            v.Y = new(CapNumber(v.Y.Meter, (decimal)min, (decimal)max), Length.Units.Meter);
        }
    }
}
