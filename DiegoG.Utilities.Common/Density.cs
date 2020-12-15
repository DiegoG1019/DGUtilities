using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace DiegoG.Utilities
{
    public class Density
    {
        private Volume VolumeLocal { get; set; }
        private Mass MassLocal { get; set; }

        [JsonIgnore, XmlIgnore, IgnoreDataMember]
        public decimal this[Mass.Units munit, Volume.Units vunit]
        {
            get => MassLocal[munit] / VolumeLocal[vunit];
            set
            {
                //If we set Volume to 1, then we simply need to calculate Mass by multiplying density times 1, which is just density.
                //This way, even if Mass is 0, then it won't throw DivideByZeroException
                VolumeLocal = new(1, vunit);
                MassLocal = new(value, munit);
            }
        }

        public decimal KgOverCubicMeter
        {
            get => this[Mass.Units.Kilogram, Volume.Units.Cubicmeter];
            set
            {
                //If we set Volume to 1, then we simply need to calculate Mass by multiplying density times 1, which is just density.
                //This way, even if Mass is 0, then it won't throw DivideByZeroException
                VolumeLocal = new(1, Volume.Units.Cubicmeter);
                MassLocal = new(value, Mass.Units.Kilogram);
            }
        }
        public Density(decimal density, Mass.Units munit, Volume.Units vunit)
            => this[munit, vunit] = density;
        /// <summary>
        /// Density is assumed to be defined as Kg/m^3 (Kilogram over Cubic Meter)
        /// </summary>
        public Density(decimal density)
            => KgOverCubicMeter = density;
        public Density(Mass mass, Volume volume)
        {
            if (!volume.NotZero)
                throw new ArgumentException($"{nameof(volume)} cannot be 0");
            VolumeLocal = volume;
            MassLocal = mass;
        }
    }
}