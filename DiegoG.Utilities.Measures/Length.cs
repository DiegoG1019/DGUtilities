using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace DiegoG.Utilities.Measures
{
    public record Length : Measure<Length.Units, Length>
    {
        public class LengthMeasureProperty
        {
            public Units BaseUnit { get; set; }
            public decimal ConversionValue { get; set; }
            public LengthMeasureProperty(Units unit, decimal value)
            {
                BaseUnit = unit;
                ConversionValue = value;
            }
        }

        public static LengthMeasureProperty SquareUnitDefinition { get; set; } = new LengthMeasureProperty(Units.Meter, 1);
        public static LengthMeasureProperty UserUnitDefinition { get; set; } = new LengthMeasureProperty(Units.Meter, 1);
        public static LengthMeasureProperty PixelUnitDefinition { get; set; } = new LengthMeasureProperty(Units.Meter, 1);

        [XmlType(TypeName = "LengthUnits")]
        public enum Units { Meter, Foot, Inch, Square, UserUnit, Pixel }
        static Length()
        {
            ShortUnitsDict.Add(Units.Meter, "Mts");
            ShortUnitsDict.Add(Units.Foot, "Ft");
            ShortUnitsDict.Add(Units.Inch, "In");
            ShortUnitsDict.Add(Units.Square, "Sqr");
            ShortUnitsDict.Add(Units.UserUnit, "UU");
            ShortUnitsDict.Add(Units.Pixel, "Px");
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
            init => DefaultValue = value;
        }
        public double MeterD => (double)Meter;
        public float MeterF => (float)Meter;

        [UnitProperty(nameof(Units.Foot)), IgnoreDataMember, JsonIgnore, XmlIgnore]
        public decimal Foot
        {
            get => Meter * MFt;
            init => Meter = value * FtM;
        }
        public double FootD => (double)Foot;
        public float FootF => (float)Foot;

        [UnitProperty(nameof(Units.Inch)), IgnoreDataMember, JsonIgnore, XmlIgnore]
        public decimal Inch
        {
            get => Meter * MIn;
            init => Meter = value * InM;
        }
        public double InchD => (double)Inch;
        public float InchF => (float)Inch;

        [UnitProperty(nameof(Units.Square)), IgnoreDataMember, JsonIgnore, XmlIgnore]
        public decimal Square
        {
            get => Math.Floor(this[SquareUnitDefinition.BaseUnit] * SquareUnitDefinition.ConversionValue);
            init => this[SquareUnitDefinition.BaseUnit] = value / SquareUnitDefinition.ConversionValue;
        }
        public double SquareD => (double)Square;
        public float SquareF => (float)Square;

        [UnitProperty(nameof(Units.UserUnit)), IgnoreDataMember, JsonIgnore, XmlIgnore]
        public decimal UserUnit
        {
            get => this[UserUnitDefinition.BaseUnit] * UserUnitDefinition.ConversionValue;
            init => this[UserUnitDefinition.BaseUnit] = value / UserUnitDefinition.ConversionValue;
        }
        public double UserUnitD => (double)UserUnit;
        public float UserUnitF => (float)UserUnit;

        [UnitProperty(nameof(Units.Pixel)), IgnoreDataMember, JsonIgnore, XmlIgnore]
        public decimal Pixel
        {
            get => Math.Floor(this[PixelUnitDefinition.BaseUnit] * PixelUnitDefinition.ConversionValue);
            init => this[PixelUnitDefinition.BaseUnit] = value / PixelUnitDefinition.ConversionValue;
        }
        public double PixelD => (double)Pixel;
        public float PixelF => (float)Pixel;
        /// <summary>
        /// Floored, unchecked.
        /// </summary>
        public int PixelI { get { unchecked { return (int)Math.Floor(Pixel); } } }

        public Length() => Meter = 0;

        public Length(decimal V, Units i) : this() => this[i] = V;

        public Length(Length length) : base(length) { }

        public static Length OneFoot => new Length(1, Units.Foot);
        public static Length OneInch => new Length(1, Units.Inch);
        public static Length OneMeter => new Length(1, Units.Meter);
        public static Length OnePixel => new Length(1, Units.Pixel);
        public static Length OneSquare => new Length(1, Units.Square);
        public static Length OneUU => new Length(1, Units.UserUnit);
    }
}
