using DiegoG.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using System.Collections.ObjectModel;

namespace DiegoG.Utilities.Measures
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    sealed class UnitProperty : Attribute
    {
        public UnitProperty(string unitName) => UnitName = unitName;

        public string UnitName { get; init; }
    }

    /// <summary>
    /// The base class for Measure, Tolerance, NotZero and DefaultValue are declared and defined here. Do not inherit from this, use Measure`Units, T` instead 
    /// </summary>
    public abstract record Measure
    {
        /// <summary>
        /// Change with caution, Not recommended if already made comparisons. Set to 0 for exact comparisons
        /// </summary>
        public static decimal Tolerance { get; set; } = .00001M;

        [IgnoreDataMember, JsonIgnore, XmlIgnore]
        public bool NotZero => DiegoGMath.TolerantCompare(DefaultValue, 0M, Tolerance) != 0;

        protected decimal DefaultValue { get; set; }
        
        public Prefix CurrentPrefix { get; init; }
        public string CurrentPrefixName => PrefixNames[CurrentPrefix];
        public string CurrentPrefixShortName => ShortPrefixNames[CurrentPrefix];
        public decimal CurrentPrefixValue => PrefixValues[CurrentPrefix];

        protected Measure(Measure measure) => DefaultValue = measure.DefaultValue;
        public enum Prefix : byte
        { Yotta, Zetta, Exa, Peta, Tera, Giga, Mega, Kilo, Hecto, Deka, n, deci, centi, milli, micro, nano, pico, femto, atto, zepto, yocto }
        public virtual ReadOnlyDictionary<Prefix, decimal> PrefixValues => StaticValues;
        public static ReadOnlyDictionary<Prefix, string> PrefixNames { get; } = new
        (
            new Dictionary<Prefix, string>()
            {
                { Prefix.Yotta,  "Yotta" },
                { Prefix.Zetta,  "Zetta" },
                { Prefix.Exa,  "Exa" },
                { Prefix.Peta,  "Peta" },
                { Prefix.Tera,  "Tera" },
                { Prefix.Giga,  "Giga" },
                { Prefix.Mega,  "Mega" },
                { Prefix.Kilo,  "Kilo" },
                { Prefix.Hecto,  "Hecto" },
                { Prefix.Deka,  "Deka" },
                { Prefix.n,  "" },
                { Prefix.deci,  "deci" },
                { Prefix.centi,  "centi" },
                { Prefix.milli,  "milli" },
                { Prefix.micro,  "micro" },
                { Prefix.nano,  "nano" },
                { Prefix.pico,  "pico" },
                { Prefix.femto,  "femto" },
                { Prefix.atto,  "atto" },
                { Prefix.zepto,  "zepto" },
                { Prefix.yocto,  "yocto" },
            }
        ); 
        public static ReadOnlyDictionary<Prefix, string> ShortPrefixNames { get; } = new
         (
             new Dictionary<Prefix, string>()
             {
                { Prefix.Yotta,  "Y" },
                { Prefix.Zetta,  "Z" },
                { Prefix.Exa,  "E" },
                { Prefix.Peta,  "P" },
                { Prefix.Tera,  "T" },
                { Prefix.Giga,  "G" },
                { Prefix.Mega,  "M" },
                { Prefix.Kilo,  "K" },
                { Prefix.Hecto,  "H" },
                { Prefix.Deka,  "D" },
                { Prefix.n,  "" },
                { Prefix.deci,  "d" },
                { Prefix.centi,  "c" },
                { Prefix.milli,  "m" },
                { Prefix.micro,  "μ" },
                { Prefix.nano,  "n" },
                { Prefix.pico,  "p" },
                { Prefix.femto,  "f" },
                { Prefix.atto,  "a" },
                { Prefix.zepto,  "z" },
                { Prefix.yocto,  "y" },
             }
         );
        protected readonly static ReadOnlyDictionary<Prefix, decimal> StaticValues = new
        (
            new Dictionary<Prefix, decimal>()
            {
                { Prefix.Yotta,  10e24M },
                { Prefix.Zetta,  10e21M },
                { Prefix.Exa,  10e18M },
                { Prefix.Peta,  10e15M },
                { Prefix.Tera,  10e12M },
                { Prefix.Giga,  10e9M },
                { Prefix.Mega,  10e6M },
                { Prefix.Kilo,  10e3M },
                { Prefix.Hecto,  10e2M },
                { Prefix.Deka,  10e1M },
                { Prefix.n,  1 },
                { Prefix.deci,  10e-1M },
                { Prefix.centi,  10e-2M },
                { Prefix.milli,  10e-3M },
                { Prefix.micro,  10e-6M },
                { Prefix.nano,  10e-9M },
                { Prefix.pico,  10e-12M },
                { Prefix.femto,  10e-15M },
                { Prefix.atto,  10e-18M },
                { Prefix.zepto,  10e-21M },
                { Prefix.yocto,  10e-24M },
            }
        );
    }

    /// <summary>
    /// The record where all equality comparisons and arithmetic are defined. Do not inherit from this, use Measure`Unit, T` instead
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract record Measure<T> : Measure where T : Measure<T>, new()
    {
        public int CompareTo(T other) => DiegoGMath.TolerantCompare(DefaultValue, other.DefaultValue, Tolerance);

        public bool Equals(T B) => CompareTo(B) == 0;

        public bool GreaterThan(T B) => CompareTo(B) == 1;

        public bool LessThan(T B) => CompareTo(B) == -1;

        public bool NotEquals(T B) => !Equals(B);

        public bool GreaterOrEqualThan(T B) => GreaterThan(B) || Equals(B);

        public bool LessOrEqualThan(T B) => LessThan(B) || Equals(B);

        public static bool operator >(Measure<T> A, T B) => A.GreaterThan(B);

        public static bool operator <(Measure<T> A, T B) => A.LessThan(B);

        public static bool operator >=(Measure<T> A, T B) => A.GreaterOrEqualThan(B);

        public static bool operator <=(Measure<T> A, T B) => A.LessOrEqualThan(B);

        public static bool operator ==(Measure<T> A, T B) => A.Equals(B);

        public static bool operator !=(Measure<T> A, T B) => A.NotEquals(B);

        public virtual T Add(Measure<T> B) => NewT(DefaultValue + B.DefaultValue);

        public virtual T Sub(Measure<T> B) => NewT(DefaultValue - B.DefaultValue);

        public virtual T Mult(Measure<T> B) => NewT(DefaultValue * B.DefaultValue);

        public virtual T Div(Measure<T> B) => NewT(DefaultValue / B.DefaultValue);

        public virtual T Mod(Measure<T> B) => NewT(DefaultValue % B.DefaultValue);

        public static T operator +(Measure<T> A, T B) => A.Add(B);

        public static T operator -(Measure<T> A, T B) => A.Sub(B);

        public static T operator *(Measure<T> A, T B) => A.Mult(B);

        public static T operator /(Measure<T> A, T B) => A.Div(B);

        public static T operator %(Measure<T> A, T B) => A.Mod(B);

        private static T NewT(decimal V) => new T { DefaultValue = V };
    }

    /// <summary>
    /// The base class for all Measures. Set the DefaultUnit's property counterpart to wrap DefaultValue, and define DefaultUnit in the same place as ShortUnits: in a static constructor. All Units should be labeled with an appropriate UnitProperty attribute. This is the record from which Measures should inherit.
    /// </summary>
    /// <typeparam name="Units"></typeparam>
    /// <typeparam name="T"></typeparam>
    public abstract record Measure<Units, T> : Measure<T>, IComparable<T>, IEquatable<T> where Units : struct, Enum where T : Measure<Units, T>, new()
    {
        protected static Units DefaultUnit { get; set; }
#nullable enable
        public static Func<object?, Measure<Units, T>, string> CustomToStringBehaviour { get; set; }
#nullable disable

        protected static Dictionary<Units, string> ShortUnitsDict { get; set; }

        public static IEnumerable<Units> AllUnits { get; } = CollectionExtensionMethods.GetEnumValues<Units>();
        public static ReadOnlyIndexedProperty<Units, string> ShortUnits { get; } = new(d => ShortUnitsDict[d]);

        [JsonIgnore, IgnoreDataMember, XmlIgnore]
        public virtual decimal this[Units index]
        {
            get => Getters[index](this);
            protected set => Setters[index](this, value);
        }

        protected static readonly Dictionary<Units, Func<Measure<Units, T>, decimal>> Getters = new();
        protected static readonly Dictionary<Units, Action<Measure<Units, T>, decimal>> Setters = new();

        static Measure()
        {
            var props = ReflectionCollectionMethods<T>.GetAllInstancePropertiesWithAttribute(typeof(UnitProperty));
            foreach (var (prop, attr) in props)
            {
                if (!prop.DeclaringType.IsSubclassOf(typeof(Measure<Units, T>)))
                {
                    continue;
                }

                var unit = (Units)Enum.Parse(typeof(Units), (attr.First() as UnitProperty).UnitName);
                Getters.Add
                       (
                           unit,
                           m => (decimal)prop.GetValue(m)
                       );
                Setters.Add
                       (
                            unit,
                            (t, m) => prop.SetValue(t, m)
                       );
            }
        }

        private void SetIndexedProperties()
        {
            AsFloat = new(u => (float)this[u]);
            AsDouble = new(u => (double)this[u]);
            Prefixed = new(u => this[u] * PrefixValues[CurrentPrefix]);
        }
        public Measure()
        {
            DefaultValue = 0;
            SetIndexedProperties();
        }

        public Measure(decimal V, Units i, Prefix prefix = Prefix.n)
        {
            CurrentPrefix = prefix;
            this[i] = V / CurrentPrefixValue;
            SetIndexedProperties();
        }

        public Measure(Measure<Units, T> measure) : base(measure) => SetIndexedProperties();

        public ReadOnlyIndexedProperty<Units, decimal> Prefixed { get; private set; }
        public ReadOnlyIndexedProperty<Units, double> AsDouble { get; private set; }
        public ReadOnlyIndexedProperty<Units, float> AsFloat { get; private set; }

        public virtual string CustomToString(object stuff)
             => CustomToStringBehaviour is null
                ? throw new InvalidOperationException("Cannot use this method if CustomToStringBehaviour static property is not defined")
                : CustomToStringBehaviour!(stuff, this);
        public virtual string CustomToString() => CustomToString(null);

        public override string ToString() => ToString(DefaultUnit);

        public string ToString(string format) => ToString(DefaultUnit, format);

        public string ToString(Units unit) => $"{this[unit]}{CurrentPrefixShortName}{ShortUnitsDict[unit]}";

        public string ToString(Units unit, string format) => $"{(this[unit]*CurrentPrefixValue).ToString(format)}{CurrentPrefixShortName}{ShortUnitsDict[unit]}";

        public string ToString(Units unit, Prefix prefix) => $"{this[unit] * PrefixValues[prefix]}{ShortPrefixNames[prefix]}{ShortUnitsDict[unit]}";

        public string ToString(Units unit, Prefix prefix, string format) => $"{(this[unit] * PrefixValues[prefix]).ToString(format)}{ShortPrefixNames[prefix]}{ShortUnitsDict[unit]}";

        public override int GetHashCode() => HashCode.Combine(DefaultValue.GetHashCode(), DefaultUnit.GetHashCode());

        /// <summary>
        /// Corret format (without brackets) {value}{shortunit} i.e. 15mts
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T Parse(string str) 
            => TryParse(str, out T measure)
                ? measure
                : throw new FormatException($"Input string was not in a correct format: {str} for type {typeof(T).Name}");

        /// <summary>
        /// Corret format (without brackets) {value}{shortunit} i.e. 15mts
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool TryParse(string str, [MaybeNullWhen(false)] out T measure)
        {
            measure = null;
            foreach (var unit in ShortUnitsDict)
            {
                if (str.EndsWith(unit.Value))
                {
                    var ns = str.Split(unit.Value, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    if (ns.Length == 1)
                    {
                        if (!decimal.TryParse(ns[0], out decimal val))
                            return false;

                        measure = NewT(val, unit.Key);
                        return true;
                    }
                    return false;
                }
            }

            return false;
        }

        public static T Zero => NewT(0, DefaultUnit);

        /// <summary>
        /// Not being able to provide constructor parameter constraints is dumb af
        /// </summary>
        /// <param name="V"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private static T NewT(decimal V, Units i)
        {
            var newt = new T();
            newt[i] = V;
            return newt;
        }
    }
}
