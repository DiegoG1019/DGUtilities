using DiegoG.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace DiegoG.Utilities.Measures
{
    [System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    sealed class UnitProperty : Attribute
    {
        public UnitProperty(string unitName)
            => UnitName = unitName;
        public string UnitName { get; init; }
    }

    /// <summary>
    /// The base class for all Measures. This class cannot be instantiated. Set the DefaultUnit's property counterpart to wrap DefaultValue, and define DefaultUnit in the same place as ShortUnits: in a static constructor. All Units should be labeled with an appropriate UnitProperty attribute
    /// </summary>
    /// <typeparam name="Units"></typeparam>
    /// <typeparam name="T"></typeparam>
    public abstract class Measure<Units, T> : IComparable<T>, IEquatable<T> where Units : Enum where T : Measure<Units, T>, new()
    {
        protected static Units DefaultUnit { get; set; }
        public static Func<object, Measure<Units, T>, string> CustomToStringBehaviour { get; set; }
        public static ImmutableDictionary<Units, string> ShortUnits { get; protected set; }

        /// <summary>
        /// Change with caution, Not recommended if already made comparisons. Set to 0 for exact comparisons
        /// </summary>
        public static decimal Tolerance { get; set; } = .00001M;

        protected decimal DefaultValue { get; set; }

        [IgnoreDataMember, JsonIgnore, XmlIgnore]
        public bool NotZero => DiegoGMath.TolerantCompare(DefaultValue, 0M, Tolerance) != 0;

        [JsonIgnore, IgnoreDataMember, XmlIgnore]
        public virtual decimal this[Units index]
        {
            get => Getters[index](this);
            set => Setters[index](this, value);
        }

        protected static readonly Dictionary<Units, Func<Measure<Units, T>, decimal>> Getters = new();
        protected static readonly Dictionary<Units, Action<Measure<Units, T>, decimal>> Setters = new();

        static Measure()
        {
            var props = ReflectionCollectionMethods<T>.GetAllInstancePropertiesWithAttribute(typeof(UnitProperty));
            foreach (var (prop, attr) in props)
            {
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

        public Measure() => DefaultValue = 0;
        public Measure(decimal V, Units i) => this[i] = V;
        public Measure(Measure<Units, T> measure) => DefaultValue = measure.DefaultValue;

        public virtual string CustomToString(object stuff)
        {
            if (CustomToStringBehaviour is null)
                throw new InvalidOperationException("Cannot use this method if CustomToStringBehaviour static property is not defined");
            return CustomToStringBehaviour(stuff, this);
        }
        public virtual string CustomToString() => CustomToString(null);
        public override string ToString() => ToString(DefaultUnit);
        public string ToString(string format) => ToString(DefaultUnit, format);
        public string ToString(Units unit) => $"{this[unit]}{ShortUnits[unit]}";
        public string ToString(Units unit, string format) => $"{this[unit].ToString(format)}{ShortUnits[unit]}";

        public bool GreaterThan(T B) => CompareTo(B) == 1;
        public bool LessThan(T B) => CompareTo(B) == -1;
        public bool Equals(T B) => CompareTo(B) == 0;
        public bool NotEquals(T B) => !Equals(B);
        public bool GreaterOrEqualThan(T B) => GreaterThan(B) || Equals(B);
        public bool LessOrEqualThan(T B) => LessThan(B) || Equals(B);
        public override bool Equals(object obj)
        => (obj is Measure<Units, T> s) && Equals(s);
        public virtual T Add(Measure<Units, T> B)
            => NewT(DefaultValue + B.DefaultValue, DefaultUnit);
        public virtual T Sub(Measure<Units, T> B)
            => NewT(DefaultValue - B.DefaultValue, DefaultUnit);
        public virtual T Mult(Measure<Units, T> B)
            => NewT(DefaultValue * B.DefaultValue, DefaultUnit);
        public virtual T Div(Measure<Units, T> B)
            => NewT(DefaultValue / B.DefaultValue, DefaultUnit);
        public virtual T Mod(Measure<Units, T> B)
            => NewT(DefaultValue % B.DefaultValue, DefaultUnit);
        public static bool operator >(Measure<Units, T> A, T B) => A.GreaterThan(B);
        public static bool operator <(Measure<Units, T> A, T B) => A.LessThan(B);
        public static bool operator >=(Measure<Units, T> A, T B) => A.GreaterOrEqualThan(B);
        public static bool operator <=(Measure<Units, T> A, T B) => A.LessOrEqualThan(B);
        public static bool operator ==(Measure<Units, T> A, T B) => A.Equals(B);
        public static bool operator !=(Measure<Units, T> A, T B) => A.NotEquals(B);
        public static T operator +(Measure<Units, T> A, T B) => A.Add(B);
        public static T operator -(Measure<Units, T> A, T B) => A.Sub(B);
        public static T operator *(Measure<Units, T> A, T B) => A.Mult(B);
        public static T operator /(Measure<Units, T> A, T B) => A.Div(B);
        public static T operator %(Measure<Units, T> A, T B) => A.Mod(B);
        public override int GetHashCode() => base.GetHashCode();

        public int CompareTo(T other) => DiegoGMath.TolerantCompare(DefaultValue, other.DefaultValue, Tolerance);

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
