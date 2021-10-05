using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace DiegoG.Utilities.Measures
{
    public class Length : Measure<Length.Units, Length>
    {
        public class LengthMeasureProperty
        {
            public Units BaseUnit { get; set; }
            public double ConversionValue { get; set; }
            public LengthMeasureProperty(Units unit, double value)
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

        public const double MCm = 100d; //Meter to Centimeter
        public const double MFt = 3.28084d; //Meter to Foot
        public const double MIn = 39.37008d; //Meter to Inch
        //
        public const double CmM = 1 / MCm; //Centimeter to Meter
        public const double FtM = 1 / MFt; //Foot to Meter
        public const double InM = 1 / MIn; //Inch to Meter

        [UnitProperty(nameof(Units.Meter))]
        public double Meter
        {
            get => DefaultValue;
            init => DefaultValue = value;
        }

        [UnitProperty(nameof(Units.Foot)), IgnoreDataMember, JsonIgnore, XmlIgnore]
        public double Foot
        {
            get => Meter * MFt;
            init => Meter = value * FtM;
        }

        [UnitProperty(nameof(Units.Inch)), IgnoreDataMember, JsonIgnore, XmlIgnore]
        public double Inch
        {
            get => Meter * MIn;
            init => Meter = value * InM;
        }

        [UnitProperty(nameof(Units.Square)), IgnoreDataMember, JsonIgnore, XmlIgnore]
        public double Square
        {
            get => Math.Floor(this[SquareUnitDefinition.BaseUnit] * SquareUnitDefinition.ConversionValue);
            init => this[SquareUnitDefinition.BaseUnit] = value / SquareUnitDefinition.ConversionValue;
        }

        [UnitProperty(nameof(Units.UserUnit)), IgnoreDataMember, JsonIgnore, XmlIgnore]
        public double UserUnit
        {
            get => this[UserUnitDefinition.BaseUnit] * UserUnitDefinition.ConversionValue;
            init => this[UserUnitDefinition.BaseUnit] = value / UserUnitDefinition.ConversionValue;
        }

        [UnitProperty(nameof(Units.Pixel)), IgnoreDataMember, JsonIgnore, XmlIgnore]
        public double Pixel
        {
            get => Math.Floor(this[PixelUnitDefinition.BaseUnit] * PixelUnitDefinition.ConversionValue);
            init => this[PixelUnitDefinition.BaseUnit] = value / PixelUnitDefinition.ConversionValue;
        }
        /// <summary>
        /// Floored, unchecked.
        /// </summary>
        public int PixelI { get { unchecked { return (int)Math.Floor(Pixel); } } }

        public Length() => Meter = 0;

        public Length(double V, Units i) : this() => this[i] = V;

        public Length(Length length) : base(length) { }

        public static Length OneFoot => new Length(1, Units.Foot);
        public static Length OneInch => new Length(1, Units.Inch);
        public static Length OneMeter => new Length(1, Units.Meter);
        public static Length OnePixel => new Length(1, Units.Pixel);
        public static Length OneSquare => new Length(1, Units.Square);
        public static Length OneUU => new Length(1, Units.UserUnit);
    }
}
