using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace DiegoG.Utilities.Measures
{
    public class Length : Measure<Length.Units, Length>
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
        static Length()
        {
            var builder = ImmutableDictionary.CreateBuilder<Units, string>();
            builder.Add(Units.Meter, "Mts");
            builder.Add(Units.Foot, "\"");
            builder.Add(Units.Inch, "\'");
            builder.Add(Units.Square, "Square");
            ShortUnits = builder.ToImmutable();
            DefaultUnit = Units.Meter;
        }

        public const decimal MCm = 100M; //Meter to Centimeter
        public const decimal MFt = 3.28084M; //Meter to Foot
        public const decimal MIn = 39.37008M; //Meter to Inch
        //
        public const decimal CmM = 1 / MCm; //Centimeter to Meter
        public const decimal FtM = 1 / MFt; //Foot to Meter
        public const decimal InM = 1 / MIn; //Inch to Meter

        [UnitProperty(nameof(Units.Meter))]
        public decimal Meter
        {
            get => DefaultValue;
            set => DefaultValue = value;
        }

        [UnitProperty(nameof(Units.Foot)), IgnoreDataMember, JsonIgnore, XmlIgnore]
        public decimal Foot
        {
            get => Meter * MFt;
            set => Meter = value * FtM;
        }

        [UnitProperty(nameof(Units.Inch)), IgnoreDataMember, JsonIgnore, XmlIgnore]
        public decimal Inch
        {
            get => Meter * MIn;
            set => Meter = value * InM;
        }

        [UnitProperty(nameof(Units.Square)), IgnoreDataMember, JsonIgnore, XmlIgnore]
        public int Square
        {
            get => (int)(this[SquareUnitDefinition.BaseUnit] * SquareUnitDefinition.ConversionValue);
            set => this[SquareUnitDefinition.BaseUnit] = value / SquareUnitDefinition.ConversionValue;
        }

        [UnitProperty(nameof(Units.UserUnit)), IgnoreDataMember, JsonIgnore, XmlIgnore]
        public decimal UserUnit
        {
            get => this[UserUnitDefinition.BaseUnit] * UserUnitDefinition.ConversionValue;
            set => this[UserUnitDefinition.BaseUnit] = value / UserUnitDefinition.ConversionValue;
        }

        [UnitProperty(nameof(Units.Pixel)), IgnoreDataMember, JsonIgnore, XmlIgnore]
        public int Pixel
        {
            get => (int)(this[PixelUnitDefinition.BaseUnit] * PixelUnitDefinition.ConversionValue);
            set => this[PixelUnitDefinition.BaseUnit] = value / PixelUnitDefinition.ConversionValue;
        }

        public Length() => Meter = 0;
        public Length(decimal V, Units i) : this() => this[i] = V;
        public Length(Length length) => Meter = length.Meter;

        public static Length OneFoot => new Length(1, Units.Foot);
        public static Length OneInch => new Length(1, Units.Inch);
        public static Length OneMeter => new Length(1, Units.Meter);
        public static Length OnePixel => new Length(1, Units.Pixel);
        public static Length OneSquare => new Length(1, Units.Square);
        public static Length OneUU => new Length(1, Units.UserUnit);
    }
}
