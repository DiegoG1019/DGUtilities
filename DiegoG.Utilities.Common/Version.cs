using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace DiegoG.Utilities
{
    [Serializable]
    public struct Version
    {
#warning Remember to update this
        public static Version Assembly { get; } = new Version("", 0, 0, 6, 0);
        public Version(string preppendix, byte major, byte build, byte minor, byte addition)
        {
            Major = major;
            Build = build;
            Minor = minor;
            Addition = addition;
            Preppendix = preppendix;
        }
        [JsonIgnore, IgnoreDataMember, XmlIgnore]
        public string Full => $"{Preppendix}-{Short}";
        [JsonIgnore, IgnoreDataMember, XmlIgnore]
        public string Short => $"{Major}.{Build}.{Minor}.{Addition}";
        public string Preppendix { get; set; }
        public byte Major { get; set; }
        public byte Build { get; set; }
        public byte Minor { get; set; }
        public byte Addition { get; set; }

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
                comparison[i] = !c[i];
            return comparison;
        }

        public static bool operator ==(Version a, Version b) => a.Full == b.Full;
        public static bool operator !=(Version a, Version b) => !(a == b);
        public static bool operator >(Version a, Version b) => a.Major > b.Major || a.Build > b.Build || a.Minor > b.Minor || a.Addition > b.Addition;
        public static bool operator <(Version a, Version b) => a.Major < b.Major || a.Build < b.Build || a.Minor < b.Minor || a.Addition < b.Addition;
        public static bool operator >=(Version a, Version b) => a == b || a > b;
        public static bool operator <=(Version a, Version b) => a == b || a < b;
        public override bool Equals(object obj) => base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();
    }
}
