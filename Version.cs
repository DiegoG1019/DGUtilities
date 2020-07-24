using System;

namespace DiegoG.Utilities
{
    [Serializable]
    public struct Version
    {
        private readonly byte[] v;
        public Version(string p, byte w, byte z, byte y, byte x)
        {
            v = new byte[4];
            v[0] = w;
            v[1] = z;
            v[2] = y;
            v[3] = x;
            Preppendix = p;
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
                return $"{Major}.{Release}.{Minor}.{Addition}";
            }
        }
        public string Preppendix { get; private set; }
        public byte Major
        {
            get
            {
                return v[0];
            }
        }
        public byte Release
        {
            get
            {
                return v[1];
            }
        }
        public byte Minor
        {
            get
            {
                return v[2];
            }
        }
        public byte Addition
        {
            get
            {
                return v[1];
            }
        }

        public static bool operator==(Version a, Version b)
        {
            return a.Full == b.Full;
        }
        public static bool operator!=(Version a, Version b)
        {
            return !(a == b);
        }
        public static bool operator>(Version a, Version b)
        {
            return a.Major > b.Major || a.Release > b.Release || a.Minor > b.Minor || a.Addition > b.Addition;
        }
        public static bool operator<(Version a, Version b)
        {
            return a.Major < b.Major || a.Release < b.Release || a.Minor < b.Minor || a.Addition < b.Addition;
        }
        public static bool operator>=(Version a, Version b)
        {
            return a == b || a > b;
        }
        public static bool operator<=(Version a, Version b)
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
