using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace DiegoG.Utilities
{
    [Serializable]
    public class Volume
    {
        public class VolumeMeasureProperty<T> where T : struct, IComparable, IConvertible, IFormattable, IComparable<T>, IEquatable<T>
        {
            public Units BaseUnit { get; set; }
            public T ConversionValue { get; set; }
            public VolumeMeasureProperty(Units unit, T value)
            {
                BaseUnit = unit;
                ConversionValue = value;
            }
        }

        [XmlType(TypeName = "VolumeUnits")]
        public enum Units
        {
            Cubicmeter,
            Liter,
            Milliliter,
            Gallon,
            Pint,
            Ounce,
        }

        private static readonly Dictionary<Units, Func<Volume, decimal>> Getters = new()
        {
            { Units.Cubicmeter, m => m.Cubicmeter },
            { Units.Liter, m => m.Liter},
            { Units.Milliliter, m => m.Milliliter },
            { Units.Gallon, m => m.Gallon },
            { Units.Pint, m => m.Pint },
            { Units.Ounce, m => m.Ounce },
        };

        private static readonly Dictionary<Units, Action<Volume, decimal>> Setters = new()
        {
            { Units.Cubicmeter, (m,v) => m.Cubicmeter = v },
            { Units.Liter, (m,v) => m.Liter = v},
            { Units.Milliliter, (m,v) => m.Milliliter = v },
            { Units.Gallon, (m,v) => m.Gallon = v },
            { Units.Pint, (m,v) => m.Pint = v },
            { Units.Ounce, (m, v) => m.Ounce = v },
        };

        public static ImmutableDictionary<Units, string> ShortUnits { get; }
        static Volume()
        {
            var builder = ImmutableDictionary.CreateBuilder<Units, string>();
            builder.Add(Units.Cubicmeter, "m^3");
            builder.Add(Units.Liter, "L");
            builder.Add(Units.Milliliter, "mL");
            builder.Add(Units.Gallon, "gal");
            builder.Add(Units.Pint, "pt");
            builder.Add(Units.Ounce, "oz");
            ShortUnits = builder.ToImmutable();
        }

        public const decimal cM3L = 1000M;
        public const decimal mLL = 0.001M;
        public const decimal GalL = 3.785411768M;
        public const decimal PtL = 0.473176473M;
        public const decimal LOz = 0.0284131M;

        public const decimal LcM3 = 1 / 1000M;
        public const decimal LmL = 1 / 0.001M;
        public const decimal LGal = 1 / 3.785411768M;
        public const decimal LPt = 1 / 0.473176473M;
        public const decimal OzL = 1 / 0.0284131M;

        public bool NotZero => Liter != 0;
        public decimal Liter { get; set; } = 0M;

        [IgnoreDataMember, JsonIgnore, XmlIgnore]
        public decimal Cubicmeter
        {
            get => Liter * LcM3;
            set => Liter = value * cM3L;
        }

        [IgnoreDataMember, JsonIgnore, XmlIgnore]
        public decimal Milliliter
        {
            get => Liter * LmL;
            set => Liter = value * mLL;
        }

        [IgnoreDataMember, JsonIgnore, XmlIgnore]
        public decimal Gallon
        {
            get => Liter * LGal;
            set => Liter = value * GalL;
        }

        [IgnoreDataMember, JsonIgnore, XmlIgnore]
        public decimal Pint
        {
            get => Liter * LPt;
            set => Liter = value * PtL;
        }

        [IgnoreDataMember, JsonIgnore, XmlIgnore]
        public decimal Ounce
        {
            get => Liter * LOz;
            set => Liter = value * OzL;
        }

        [JsonIgnore, IgnoreDataMember, XmlIgnore]
        public decimal this[Units index]
        {
            get => Getters[index](this);
            set => Setters[index](this, value);
        }
        public Volume() => Liter = 0;
        public Volume(decimal V, Units i) : this() => this[i] = V;
        public Volume(Length l1, Length l2, Length l3) : this()
            => Cubicmeter = (l1.Meter * l2.Meter * l3.Meter) / 3;

        /// <summary>
        /// Keep in mind, when setting this, that object will always be null when using parameterless CustomToString
        /// </summary>
        public static Func<object, Volume, string> CustomToStringBehaviour { get; set; }
        public string CustomToString(object stuff)
        {
            if (CustomToStringBehaviour is null)
                throw new InvalidOperationException("Cannot use this method if Length.CustomToStringBehaviour static property is not defined");
            return CustomToStringBehaviour(stuff, this);
        }
        public string CustomToString() => CustomToString(null);

        public override string ToString() => ToString(Units.Liter);
        public string ToString(string format) => ToString(Units.Liter, format);
        public string ToString(Units unit) => $"{this[unit]}{ShortUnits[unit]}";
        public string ToString(Units unit, string format) => $"{this[unit].ToString(format)}{ShortUnits[unit]}";
        public static bool operator >(Volume A, Volume B) => A.Liter > B.Liter;
        public static bool operator <(Volume A, Volume B) => A.Liter < B.Liter;
        public static bool operator >=(Volume A, Volume B) => A.Liter >= B.Liter;
        public static bool operator <=(Volume A, Volume B) => A.Liter <= B.Liter;
        public static bool operator ==(Volume A, Volume B) => A.Liter == B.Liter;
        public static bool operator !=(Volume A, Volume B) => !(A == B);
        public static Volume operator +(Volume A, Volume B) => new Volume(A.Liter + B.Liter, Units.Liter);
        public static Volume operator -(Volume A, Volume B) => new Volume(A.Liter - B.Liter, Units.Liter);
        public static Volume operator *(Volume A, Volume B) => new Volume(A.Liter * B.Liter, Units.Liter);
        public static Volume operator /(Volume A, Volume B) => new Volume(A.Liter / B.Liter, Units.Liter);
        public static Volume operator %(Volume A, Volume B) => new Volume(A.Liter % B.Liter, Units.Liter);
        public override bool Equals(object obj) => base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();

        public static Volume Zero => new Volume(0M, Units.Liter);
        public static Volume OneCubicmeter => new Volume(1, Units.Cubicmeter);
        public static Volume OneGallon => new Volume(1, Units.Gallon);
        public static Volume OneLiter => new Volume(1, Units.Liter);
        public static Volume OneMilliliter => new Volume(1, Units.Milliliter);
        public static Volume OneOunce => new Volume(1, Units.Ounce);
        public static Volume OnePint => new Volume(1, Units.Pint);
    }
}
