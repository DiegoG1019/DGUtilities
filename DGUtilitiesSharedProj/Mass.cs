﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace DiegoG.Utilities
{
    public class Mass
    {
        public class MassMeasureProperty<T> where T : struct, IComparable, IConvertible, IFormattable, IComparable<T>, IEquatable<T>
        {
            public Units BaseUnit { get; set; }
            public T ConversionValue { get; set; }
            public MassMeasureProperty(Units unit, T value)
            {
                BaseUnit = unit;
                ConversionValue = value;
            }
        }

        public static MassMeasureProperty<float> UserUnitDefinition { get; set; } = new MassMeasureProperty<float>(Units.Kilogram, 1);

        private static Dictionary<Units, Func<Mass, decimal>> Getters = new Dictionary<Units, Func<Mass, decimal>>()
        {
            { Units.Kilogram, (m) => m.Kilogram },
            { Units.Pound, (m) => m.Pound },
            { Units.UserUnits, (m) => (decimal)m.UserUnit }
        };
        private static Dictionary<Units, Action<Mass, decimal>> Setters = new Dictionary<Units, Action<Mass, decimal>>()
        {
            { Units.Kilogram, (m,v) => m.Kilogram = v },
            { Units.Pound, (m,v) => m.Pound = v },
            { Units.UserUnits, (m,v) => m.UserUnit = (float)v }
        };
        private float mass;
        private Units kilogram;

        public enum Units { Kilogram, Pound, UserUnits }
        public static ImmutableDictionary<Units, string> ShortUnits { get; }
        static Mass()
        {
            var builder = ImmutableDictionary.CreateBuilder<Units, string>();
            builder.Add(Units.Kilogram, "Kg.");
            builder.Add(Units.Pound, "Lbs.");
            ShortUnits = builder.ToImmutable();
        }
        public const decimal KgLb = 2.20462M;
        public const decimal LbKg = 0.453592M;
        public decimal Kilogram { get; set; }

        [IgnoreDataMember, JsonIgnore, XmlIgnore]
        public decimal Pound
        {
            get => Kilogram * KgLb;
            set => Kilogram = value * LbKg;
        }

        [IgnoreDataMember, JsonIgnore, XmlIgnore]
        public float UserUnit
        {
            get => (float)this[UserUnitDefinition.BaseUnit] * UserUnitDefinition.ConversionValue;
            set => this[UserUnitDefinition.BaseUnit] = (decimal)(value / UserUnitDefinition.ConversionValue);
        }
        [JsonIgnore, IgnoreDataMember, XmlIgnore]
        public decimal this[Units index]
        {
            get => Getters[index](this);
            set => Setters[index](this, value);
        }
        public Mass() => Kilogram = 0;
        public Mass(decimal V, Units i) : this() => this[i] = V;
        public string ToString(Units unit) => $"{this[unit]}{ShortUnits[unit]}";
        public static bool operator >(Mass A, Mass B) => A.Kilogram > B.Kilogram;
        public static bool operator <(Mass A, Mass B) => !(A > B);
        public static bool operator >=(Mass A, Mass B) => A.Kilogram >= B.Kilogram;
        public static bool operator <=(Mass A, Mass B) => !(A >= B);
        public static bool operator ==(Mass A, Mass B) => A.Kilogram == B.Kilogram;
        public static bool operator !=(Mass A, Mass B) => !(A == B);
        public static Mass operator +(Mass A, Mass B) => new Mass(A.Kilogram + B.Kilogram, Units.Kilogram);
        public static Mass operator -(Mass A, Mass B) => new Mass(A.Kilogram - B.Kilogram, Units.Kilogram);
        public static Mass operator /(Mass A, Mass B) => new Mass(A.Kilogram / B.Kilogram, Units.Kilogram);
        public static Mass operator *(Mass A, Mass B) => new Mass(A.Kilogram * B.Kilogram, Units.Kilogram);
        public static Mass operator %(Mass A, Mass B) => new Mass(A.Kilogram % B.Kilogram, Units.Kilogram);
        public override bool Equals(object obj) => base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();

        public static Mass Zero => new Mass(0, Units.Kilogram);

    }
}
