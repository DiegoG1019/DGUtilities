using DiegoG.DnDTDesktop.Properties;
using System.Collections.Immutable;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace DiegoG.Utilities
{
    public class Length
    {
        public enum Units
        {
            Meter, Foot, Inch, Square
        }

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
        public decimal Foot
        {
            get => Meter * MFt;
            set => Meter = value * FtM;
        }
        public decimal Inch
        {
            get => Meter * MIn;
            set => Meter = value * InM;
        }
        public int Square
        {
            get => (int)(Foot / Settings.Default.SquareSize);
            set => Foot = value * Settings.Default.SquareSize;
        }

        [JsonIgnore, IgnoreDataMember, XmlIgnore]
        public decimal this[Units index]
        {
            get
            {
                switch (index)
                {
                    case Units.Meter:
                        return Meter;
                    case Units.Square:
                        return Square;
                    case Units.Foot:
                        return Foot;
                    case Units.Inch:
                        return Inch;
                    default:
                        return Meter;
                }
            }
            set
            {
                switch (index)
                {
                    case Units.Meter:
                        Meter = value;
                        break;
                    case Units.Square:
                        Square = (int)value;
                        break;
                    case Units.Foot:
                        Foot = value;
                        break;
                    case Units.Inch:
                        Inch = value;
                        break;
                    default:
                        Meter = value;
                        break;
                }
            }
        }
        public Length(decimal v, Units a)
        {
            switch (a)
            {
                case Units.Meter:
                    Meter = v;
                    break;
                case Units.Inch:
                    Inch = v;
                    break;
                case Units.Foot:
                    Foot = v;
                    break;
                case Units.Square:
                    Square = (int)v;
                    break;
            }
        }
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
    }
}
