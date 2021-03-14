using DiegoG.Utilities.Collections;
using MessagePack;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace DiegoG.Utilities.IO
{
    /// <summary>
    /// This class supports the usage of OnSerialized and OnDeserialized attributes for all types of deserialization. There may only be one method attributed with OnSerialized or OnDeserialized on any given type, it must be an instance method and take no parameters.
    /// </summary>
    public static partial class Serialization
    {
        [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
        public sealed class CustomConverterAttribute : Attribute
        {
            public CustomConverterAttribute() { }
        }

        public enum SerializationFormat { Binary, Xml, Json, MsgPk }

        public const string XmlExtension = ".xml";
        public const string JsonExtension = ".json";

        public static bool IsInit { get; private set; } = false;

        private static bool Init()
        {
            if(IsInit)
                throw new InvalidOperationException("Cannot initialize twice");
            Log.Information("Initializing DiegoG.Utilities.IO.Serialization");
            Log.Information("Initializing Json Serialization Settings");
            JsonSerializationSettings.Init();
            Log.Information("Initializing MsgPk Serialization Settings");
            MessagePackSerializationSettings.Init();
            
            Log.Information("Registering all CustomSerializers marked with CustomSerializerAttribute");
            foreach (var ty in ReflectionCollectionMethods.GetAllTypesWithAttribute(typeof(CustomConverterAttribute), false))
                JsonSerializationSettings.JsonSerializerOptions.Converters.Add(Activator.CreateInstance(ty) as JsonConverter);
            IsInit = true;
            return IsInit;
        }

        private static bool TryInit() => !IsInit && Init();
        
#pragma warning disable CS0618 // Type or member is obsolete; this does not involve any form of user input. Any possible threats presented here already exist in the object being copied
        public static object CopyByBinarySerialization(this object obj) => Deserialize<object>.Binary(Serialize.Binary(obj));

        public async static Task<object> CopyByBinarySerializationAsync(this object obj) => await Task.Run(() => Deserialize<object>.Binary(Serialize.Binary(obj)));
#pragma warning restore CS0618 // Type or member is obsolete

        

        
    }
}
