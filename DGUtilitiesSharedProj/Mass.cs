namespace DiegoG.Utilities
{
    public class Mass
    {
        public enum Units
        {
            Kilogram, Pound
        }
        public const decimal KgLb = 2.20462M;
        public const decimal LbKg = 0.453592M;
        public decimal Kilogram { get; set; }
        public decimal Pound
        {
            get
            {
                return Kilogram * KgLb;
            }
            set
            {
                Kilogram = value * LbKg;
            }
        }
        public Mass(decimal V, Units i)
        {
            if (i == Units.Kilogram)
            {
                Kilogram = V;
                return;
            }
            if (i == Units.Pound)
            {
                Pound = V;
                return;
            }
        }
        public static bool operator >(Mass A, Mass B) => A.Kilogram > B.Kilogram;
        public static bool operator <(Mass A, Mass B) => !(A > B);
        public static bool operator >=(Mass A, Mass B) => A.Kilogram >= B.Kilogram;
        public static bool operator <=(Mass A, Mass B) => !(A >= B);
        public static bool operator ==(Mass A, Mass B) => A.Kilogram == B.Kilogram;
        public static bool operator !=(Mass A, Mass B) => !(A == B);
        public override bool Equals(object obj) => base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();
    }
}
