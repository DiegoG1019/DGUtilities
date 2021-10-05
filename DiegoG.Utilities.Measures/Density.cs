using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace DiegoG.Utilities.Measures
{
    public class Density
    {
        private Volume VolumeLocal { get; set; }
        private Mass MassLocal { get; set; }

        [JsonIgnore, XmlIgnore, IgnoreDataMember]
        public double this[Mass.Units munit, Volume.Units vunit]
        {
            get => MassLocal[munit] / VolumeLocal[vunit];
            init
            {
                //If we set Volume to 1, then we simply need to calculate Mass by multiplying density times 1, which is just density.
                //This way, even if Mass is 0, then it won't throw DivideByZeroException
                VolumeLocal = new(1, vunit);
                MassLocal = new(value, munit);
            }
        }

        public double KgOverCubicMeter
        {
            get => this[Mass.Units.Kilogram, Volume.Units.Cubicmeter];
            init
            {
                //If we set Volume to 1, then we simply need to calculate Mass by multiplying density times 1, which is just density.
                //This way, even if Mass is 0, then it won't throw DivideByZeroException
                VolumeLocal = new(1, Volume.Units.Cubicmeter);
                MassLocal = new(value, Mass.Units.Kilogram);
            }
        }

        public Density(Density density) => KgOverCubicMeter = density.KgOverCubicMeter;
        public Density(double density, Mass.Units munit, Volume.Units vunit) => this[munit, vunit] = density;

        /// <summary>
        /// Density is assumed to be defined as Kg/m^3 (Kilogram over Cubic Meter)
        /// </summary>
        public Density(double density) => KgOverCubicMeter = density;

        public Density(Mass mass, Volume volume)
        {
            if (!volume.NotZero)
            {
                throw new ArgumentException($"{nameof(volume)} cannot be 0");
            }

            VolumeLocal = volume;
            MassLocal = mass;
        }

        /// <summary>
        /// Corret format (without brackets) {value}{mass.shortunit}/{value}{volume.shortunit}
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Density Parse(string str)
        {
            DGHelper.ThrowIfNull((str, nameof(str)));
            var s = str.Split('/');
            if (s.Length == 2)
            {
                return new Density(Mass.Parse(s[0]), Volume.Parse(s[1]));
            }

            throw new FormatException($"Input string was not in a correct format: {str}");
        }

        public static bool TryParse(string str, [MaybeNullWhen(false)] out Density density)
        {
            density = null;
            DGHelper.ThrowIfNull((str, nameof(str)));
            var s = str.Split('/');
            if (s.Length == 2 && Mass.TryParse(s[0], out Mass mss) && Volume.TryParse(s[1], out Volume vlm))
            {
                density = new Density(mss, vlm);
                return true;
            }
            return false;
        }

    }
}