using System;

namespace DiegoG.Utilities
{
    [Serializable]
    public struct Version
    {
        public static Version Assembly { get; } = new Version("", 0, 0, 1, 0);

        private readonly byte[] v;
        public Version(string preppendix, byte major, byte build, byte minor, byte addition)
        {
            v = new byte[4];
            v[0] = major;
            v[1] = build;
            v[2] = minor;
            v[3] = addition;
            Preppendix = preppendix;
        }
        public string Full
        {
            get
            {
                return $"{Preppendix}-{Short}";
            }
        }
        public string Short
        {
            get
            {
                return $"{Major}.{Build}.{Minor}.{Addition}";
            }
        }
        public string Preppendix { get; private set; }
        public byte Major => v[0];
        public byte Build => v[1];
        public byte Minor => v[2];
        public byte Addition => v[3];

        private static readonly bool[] comparison = new bool[5];
        public static bool[] CompareLargerThan(Version a, Version b)
        {
            comparison[0] = a.Major > b.Major;
            comparison[1] = a.Major > b.Major;
            comparison[2] = a.Build > b.Build;
            comparison[3] = a.Minor > b.Minor;
            comparison[4] = a.Addition > b.Addition;
            return comparison;
        }
        public static bool[] CompareEqualTo(Version a, Version b)
        {
            comparison[0] = a.Preppendix == b.Preppendix;
            comparison[1] = a.Major == b.Major;
            comparison[2] = a.Build == b.Build;
            comparison[3] = a.Minor == b.Minor;
            comparison[4] = a.Addition == b.Addition;
            return comparison;
        }
        public static bool[] CompareLessThan(Version a, Version b)
        {
            var c = CompareLargerThan(a, b);
            for (int i = 0; i < c.Length; i++)
            {
                comparison[i] = !c[i];
            }
            return comparison;
        }

        public static bool operator ==(Version a, Version b)
        {
            return a.Full == b.Full;
        }
        public static bool operator !=(Version a, Version b)
        {
            return !(a == b);
        }
        public static bool operator >(Version a, Version b)
        {
            return a.Major > b.Major || a.Build > b.Build || a.Minor > b.Minor || a.Addition > b.Addition;
        }
        public static bool operator <(Version a, Version b)
        {
            return a.Major < b.Major || a.Build < b.Build || a.Minor < b.Minor || a.Addition < b.Addition;
        }
        public static bool operator >=(Version a, Version b)
        {
            return a == b || a > b;
        }
        public static bool operator <=(Version a, Version b)
        {
            return a == b || a < b;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
