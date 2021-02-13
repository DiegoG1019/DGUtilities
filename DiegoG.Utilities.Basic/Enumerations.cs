using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace DiegoG.Utilities.Enumerations
{
    [Serializable, JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Directions
    {
        [EnumMember(Value = "Up")]
        Up,

        [EnumMember(Value = "Down")]
        Down,

        [EnumMember(Value = "Left")]
        Left,

        [EnumMember(Value = "Right")]
        Right,

        [EnumMember(Value = "UpRight")]
        UpRight,

        [EnumMember(Value = "UpLeft")]
        UpLeft,

        [EnumMember(Value = "DownRight")]
        DownRight,

        [EnumMember(Value = "DownLeft")]
        DownLeft
    }

    [Serializable, JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Verbosity
    {
        [EnumMember(Value = "Normal")]
        Normal,

        [EnumMember(Value = "Debug")]
        Debug,

        [EnumMember(Value = "Verbose")]
        Verbose
    }

    [Serializable, JsonConverter(typeof(JsonStringEnumConverter))]
    public enum NumberTypes
    {
        Byte,
        SByte,
        Int16,
        UInt16,
        Int32,
        UInt32,
        Int64,
        UInt64,
        Half,
        Single,
        Double,
        Decimal
    }

    public static class MemberCount
    {
        public static readonly int Directions = Enum.GetNames(typeof(Directions)).Length;
    }
    public static class EnumUtil
    {
        public static IEnumerable<T> GetValues<T>() => Enum.GetValues(typeof(T)).Cast<T>();
    }

}
