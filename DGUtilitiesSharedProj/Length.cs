using DiegoG.DnDTDesktop.Properties;

namespace DiegoG.Utilities
{
    public class Length
    {
        public enum Units
        {
            Meter, Foot, Inch, Square
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
            get
            {
                return Meter * MFt;
            }
            set
            {
                Meter = value * FtM;
            }
        }
        public decimal Inch
        {
            get
            {
                return Meter * MIn;
            }
            set
            {
                Meter = value * InM;
            }
        }
        public int Square
        {
            get
            {
                return (int)(Foot / Settings.Default.SquareSize);
            }
            set
            {
                Foot = value * Settings.Default.SquareSize;
            }
        }
        public Length(decimal V, Units a)
        {
            if (a == Units.Foot)
            {
                Foot = V;
                return;
            }
            if (a == Units.Meter)
            {
                Meter = V;
                return;
            }
            if (a == Units.Inch)
            {
                Inch = V;
                return;
            }
            if (a == Units.Square)
            {
                Square = (int)V;
                return;
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
