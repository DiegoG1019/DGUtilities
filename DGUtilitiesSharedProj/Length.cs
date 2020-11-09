using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace DiegoG.Utilities
{
    public class Length
    {
        public class LengthMeasureProperty<T> where T : struct, IComparable, IConvertible, IFormattable, IComparable<T>, IEquatable<T>
        {
            public Units BaseUnit { get; set; }
            public T ConversionValue { get; set; }
            public LengthMeasureProperty(Units unit, T value)
            {
                BaseUnit = unit;
                ConversionValue = value;
            }
        }

        public static LengthMeasureProperty<int> SquareUnitDefinition { get; set; } = new LengthMeasureProperty<int>(Units.Meter, 1);
        public static LengthMeasureProperty<decimal> UserUnitDefinition { get; set; } = new LengthMeasureProperty<decimal>(Units.Meter, 1);
        public static LengthMeasureProperty<int> PixelUnitDefinition { get; set; } = new LengthMeasureProperty<int>(Units.Meter, 1);

        [XmlType(TypeName = "LengthUnits")]
        public enum Units { Meter, Foot, Inch, Square, UserUnit, Pixel }

        private static readonly Dictionary<Units, Func<Length, decimal>> Getters = new Dictionary<Units, Func<Length, decimal>>()
        {
            { Units.Meter, (m) => m.Meter },
            { Units.Foot, (m) => m.Foot },
            { Units.Inch, (m) => m.Inch },
            { Units.Square, (m) => m.Square },
            { Units.UserUnit, (m) => m.UserUnit },
            { Units.Pixel, (m) => m.Pixel }
        };
        private static readonly Dictionary<Units, Action<Length, decimal>> Setters = new Dictionary<Units, Action<Length, decimal>>()
        {
            { Units.Meter, (m,v) => m.Meter = v },
            { Units.Foot, (m,v) => m.Foot = v },
            { Units.Inch, (m,v) => m.Inch = v },
            { Units.Square, (m,v) => m.Square = (int)v },
            { Units.UserUnit, (m,v) => m.UserUnit = v },
            { Units.Pixel, (m,v) => m.Pixel = (int)v }
        };

        public static ImmutableDictionary<Units, string> ShortUnits { get; }
        static Length()
        {
            var builder = ImmutableDictionary.CreateBuilder<Units, string>();
            builder.Add(Units.Meter, "Mts");
            builder.Add(Units.Foot, "\"");
            builder.Add(Units.Inch, "\'");
            builder.Add(Units.Square, "Square");
            ShortUnits = builder.ToImmutable();
        }

        public const decimal MCm = 100M; //Meter to Centimeter
        public const decimal MFt = 3.28084M; //Meter to Foot
        public const decimal MIn = 39.37008M; //Meter to Inch
        //
        public const decimal CmM = 0.01M; //Centimeter to Meter
        public const decimal FtM = 0.3048M; //Foot to Meter
        public const decimal InM = 0.0254M; //Inch to Meter
        public decimal Meter { get; set; } = 0;

        [IgnoreDataMember, JsonIgnore, XmlIgnore]
        public decimal Foot
        {
            get => Meter * MFt;
            set => Meter = value * FtM;
        }

        [IgnoreDataMember, JsonIgnore, XmlIgnore]
        public decimal Inch
        {
            get => Meter * MIn;
            set => Meter = value * InM;
        }

        [IgnoreDataMember, JsonIgnore, XmlIgnore]
        public int Square
        {
            get => (int)(this[SquareUnitDefinition.BaseUnit] * SquareUnitDefinition.ConversionValue);
            set => this[SquareUnitDefinition.BaseUnit] = value / SquareUnitDefinition.ConversionValue;
        }
        [IgnoreDataMember, JsonIgnore, XmlIgnore]
        public decimal UserUnit
        {
            get => this[UserUnitDefinition.BaseUnit] * UserUnitDefinition.ConversionValue;
            set => this[UserUnitDefinition.BaseUnit] = value / UserUnitDefinition.ConversionValue;
        }
        [IgnoreDataMember, JsonIgnore, XmlIgnore]
        public int Pixel
        {
            get => (int)(this[PixelUnitDefinition.BaseUnit] * PixelUnitDefinition.ConversionValue);
            set => this[PixelUnitDefinition.BaseUnit] = value / PixelUnitDefinition.ConversionValue;
        }

        [JsonIgnore, IgnoreDataMember, XmlIgnore]
        public decimal this[Units index]
        {
            get => Getters[index](this);
            set => Setters[index](this, value);
        }
        public Length() => Meter = 0;
        public Length(decimal V, Units i) : this() => this[i] = V;

        /// <summary>
        /// Keep in mind, when setting this, that object will always be null when using parameterless CustomToString
        /// </summary>
        public static Func<object, Length, string> CustomToStringBehaviour { get; set; }
        public string CustomToString(object stuff)
        {
            if (CustomToStringBehaviour is null)
                throw new InvalidOperationException("Cannot use this method if Length.CustomToStringBehaviour static property is not defined");
            return CustomToStringBehaviour(stuff, this);
        }
        public string CustomToString() => CustomToString(null);

        public override string ToString() => ToString(Units.Meter);
        public string ToString(string format) => ToString(Units.Meter, format);
        public string ToString(Units unit) => $"{this[unit]}{ShortUnits[unit]}";
        public string ToString(Units unit, string format) => $"{this[unit].ToString(format)}{ShortUnits[unit]}";
        public static bool operator >(Length A, Length B) => A.Meter > B.Meter;
        public static bool operator <(Length A, Length B) => A.Meter < B.Meter;
        public static bool operator >=(Length A, Length B) => A.Meter >= B.Meter;
        public static bool operator <=(Length A, Length B) => !(A >= B);
        public static bool operator ==(Length A, Length B) => A.Meter == B.Meter;
        public static bool operator !=(Length A, Length B) => !(A == B);
        public static Length operator +(Length A, Length B) => new Length(A.Meter + B.Meter, Units.Meter);
        public static Length operator -(Length A, Length B) => new Length(A.Meter - B.Meter, Units.Meter);
        public static Length operator *(Length A, Length B) => new Length(A.Meter * B.Meter, Units.Meter);
        public static Length operator /(Length A, Length B) => new Length(A.Meter / B.Meter, Units.Meter);
        public static Length operator %(Length A, Length B) => new Length(A.Meter % B.Meter, Units.Meter);
        public override bool Equals(object obj) => base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();

        public static Length Zero => new Length(0M, Units.Meter);
    }
}
