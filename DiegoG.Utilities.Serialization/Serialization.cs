using DiegoG.Utilities.Collections;
using MessagePack;
using Serilog;
using System;
using System.Collections.Generic;
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
    /// This class supports the usage of OnSerialized and OnDeserialized attributes for Json as well as Binary serialization. For Json, use RegisterClassCallbacksJsonConverter
    /// </summary>
    public static class Serialization
    {
        [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
        public sealed class CustomConverterAttribute : Attribute
        {
            public CustomConverterAttribute() { }
        }

        public enum SerializationFormat { Binary, Xml, Json, MsgPk }

        public const string XmlExtension = ".xml";
        public const string JsonExtension = ".json";

        private static readonly Dictionary<SerializationFormat, string> ExtensionDict = new Dictionary<SerializationFormat, string>()
        {
            { SerializationFormat.Binary, "" },
            { SerializationFormat.Xml, XmlExtension },
            { SerializationFormat.Json, JsonExtension },
            { SerializationFormat.MsgPk, "" }
        };

        public static void Init()
        {
            Log.Information("Initializing DiegoG.Utilities.IO.Serialization");
            Log.Information("Initializing Json Serialization Settings");
            JsonSerializationSettings.Init();
            Log.Information("Initializing MsgPk Serialization Settings");
            MessagePackSerializationSettings.Init();
            
            Log.Information("Registering all CustomSerializers marked with CustomSerializerAttribute");
            foreach (var ty in ReflectionCollectionMethods.GetAllTypesWithAttribute(typeof(CustomConverterAttribute), false))
            {
                JsonSerializationSettings.JsonSerializerOptions.Converters.Add(Activator.CreateInstance(ty) as JsonConverter);
            }
        }

#pragma warning disable CS0618 // Type or member is obsolete; this does not involve any form of user input. Any possible threats presented here already exist in the object being copied
        public static object CopyByBinarySerialization(this object obj) => Deserialize<object>.Binary(Serialize.Binary(obj));

        public async static Task<object> CopyByBinarySerializationAsync(this object obj) => await Task.Run(() => Deserialize<object>.Binary(Serialize.Binary(obj)));
#pragma warning restore CS0618 // Type or member is obsolete



        public static class Parse
        {
            public static JsonDocument Json(string path, string file)
            {
                string fullpath = Path.Combine(path, file + JsonExtension);
                using StreamReader InFile = new StreamReader(new FileStream(fullpath, FileMode.Open, FileAccess.Read, FileShare.Read));
                string jsonstring = InFile.ReadToEnd();
                return JsonDocument.Parse(jsonstring);
            }
            public static Task<JsonDocument> JsonAsync(string path, string file) => Task.Run(() => Json(path, file));

            public static XmlDocument Xml(string path, string file)
            {
                string fullpath = Path.Combine(path, file + JsonExtension);
                using StreamReader InFile = new StreamReader(new FileStream(fullpath, FileMode.Open, FileAccess.Read, FileShare.Read));
                var xml = new XmlDocument();
                xml.LoadXml(InFile.ReadToEnd());
                return xml;
            }
            public static Task<XmlDocument> XmlAsync(string path, string file) => Task.Run(() => Xml(path, file));
        }

        public static class Deserialize
        {
            public static object Json(string jsonString, Type type) => JsonSerializer.Deserialize(jsonString, type, JsonSerializationSettings.JsonSerializerOptions);

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "C# language version retrocompatibility")]
            public static object Json(string path, string file, Type type)
            {
                string fullpath = Path.Combine(path, file + JsonExtension);
                using (StreamReader InFile = new StreamReader(new FileStream(fullpath, FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    string jsonstring = InFile.ReadToEnd();
                    return Json(jsonstring, type);
                }
            }

            public static async Task<object> JsonAsync(string jsonString, Type type) => await Task.Run(() => Json(jsonString, type));

            public static async Task<object> JsonAsync(string path, string file, Type type) => await Task.Run(() => Json(path, file, type));

            public static object MsgPk(byte[] MsgPkByteArray, Type type) => MessagePackSerializer.Deserialize(type, MsgPkByteArray, MessagePackSerializationSettings.MessagePackSerializerOptions);
            public static object MsgPk(string path, string file, Type type)
            {
                string fullpath = Path.Combine(path, file + JsonExtension);
                using FileStream InFile = new FileStream(fullpath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return MessagePackSerializer.Deserialize(type, InFile, MessagePackSerializationSettings.MessagePackSerializerOptions);
            }
            public static async Task<object> MsgPkAsync(byte[] MsgPkByteArray, Type type) => await Task.Run(() => MsgPk(MsgPkByteArray, type));
            public static async Task<object> MsgPkAsync(string path, string file, Type type) => await Task.Run(() => MsgPk(path, file, type));
        }

        public static class Deserialize<T>
        {
            private static Dictionary<SerializationFormat, Func<string, string, T>> DeserializationsDict { get; } = new Dictionary<SerializationFormat, Func<string, string, T>>()
            {
#pragma warning disable CS0618 // Type or member is obsolete
                { SerializationFormat.Binary, (p,f)=>Binary(p,f) },
#pragma warning restore CS0618 // Type or member is obsolete
                { SerializationFormat.Json, (p,f)=>Json(p,f) },
                { SerializationFormat.Xml, (p,f)=>Xml(p,f) },
                { SerializationFormat.MsgPk, (p,f)=>MsgPk(p,f) },
            };
            private static Dictionary<SerializationFormat, Func<string, string, Task<T>>> AsyncDeserializationsDict { get; } = new Dictionary<SerializationFormat, Func<string, string, Task<T>>>()
            {
#pragma warning disable CS0618 // Type or member is obsolete
                { SerializationFormat.Binary, (p,f)=>BinaryAsync(p,f) },
#pragma warning restore CS0618 // Type or member is obsolete
                { SerializationFormat.Json, (p,f)=>JsonAsync(p,f) },
                { SerializationFormat.Xml, (p,f)=>XmlAsync(p,f) },
                { SerializationFormat.MsgPk, (p,f)=>MsgPkAsync(p,f) },
            };

            public static T ByFormat(SerializationFormat f, string path, string file) => DeserializationsDict[f](path, file);

            public static Task<T> ByFormatAsync(SerializationFormat f, string path, string file) => AsyncDeserializationsDict[f](path, file);

            [Obsolete("BinaryFormatter serialization is obsolete and should not be used. See https://aka.ms/binaryformatter for more information.")]
            public static T Binary(Stream stm)
            {
                var binf = new BinaryFormatter();
                return (T)binf.Deserialize(stm);
            }

            [Obsolete("BinaryFormatter serialization is obsolete and should not be used. See https://aka.ms/binaryformatter for more information.")]
            public static T Binary(string path, string file) => Binary(File.OpenRead(Path.Combine(path, file)));

            [Obsolete("BinaryFormatter serialization is obsolete and should not be used. See https://aka.ms/binaryformatter for more information.")]
            public static async Task<T> BinaryAsync(Stream stm) => await Task<T>.Run(() => Binary(stm));

            [Obsolete("BinaryFormatter serialization is obsolete and should not be used. See https://aka.ms/binaryformatter for more information.")]
            public static async Task<T> BinaryAsync(string path, string file) => await Task<T>.Run(() => Binary(path, file));

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "C# language version retrocompatibility")]
            public static T Xml(string xmlString)
            {
                var t = typeof(T);
                var serializer = new XmlSerializer(t);
                using (StringReader sr = new StringReader(xmlString))
                {
                    return (T)serializer.Deserialize(sr);
                }
            }

            public static T Xml(string path, string file)
            {
                string fullpath = Path.Combine(path, file + XmlExtension);
                using StreamReader InFile = new StreamReader(new FileStream(fullpath, FileMode.Open, FileAccess.Read, FileShare.Read));
                string xmlstring = InFile.ReadToEnd();
                return Xml(xmlstring);
            }

            public static async Task<T> XmlAsync(string xmlString) => await Task<T>.Run(() => Xml(xmlString));

            public static async Task<T> XmlAsync(string path, string file) => await Task<T>.Run(() => Xml(path, file));

            public static T Json(string jsonString) => JsonSerializer.Deserialize<T>(jsonString, JsonSerializationSettings.JsonSerializerOptions);

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "C# language version retrocompatibility")]
            public static T Json(string path, string file)
            {
                string fullpath = Path.Combine(path, file + JsonExtension);
                using (StreamReader InFile = new StreamReader(new FileStream(fullpath, FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    string jsonstring = InFile.ReadToEnd();
                    return Json(jsonstring);
                }
            }

            public static async Task<T> JsonAsync(string jsonString) => await Task<T>.Run(() => Json(jsonString));

            public static async Task<T> JsonAsync(string path, string file) => await Task<T>.Run(() => Json(path, file));

            public static T MsgPk(byte[] MsgPkByteArray) => MessagePackSerializer.Deserialize<T>(MsgPkByteArray, MessagePackSerializationSettings.MessagePackSerializerOptions);
            public static T MsgPk(string path, string file)
            {
                string fullpath = Path.Combine(path, file + JsonExtension);
                using FileStream InFile = new FileStream(fullpath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return MessagePackSerializer.Deserialize<T>(InFile, MessagePackSerializationSettings.MessagePackSerializerOptions);
            }
            public static async Task<T> MsgPkAsync(byte[] MsgPkByteArray) => await Task.Run(() => MsgPk(MsgPkByteArray));
            public static async Task<T> MsgPkAsync(string path, string file) => await Task.Run(() => MsgPk(path, file));
        }

        public static class Serialize
        {
            private static Dictionary<SerializationFormat, Action<object, string, string>> SerializationsDict { get; } = new Dictionary<SerializationFormat, Action<object, string, string>>()
            {
#pragma warning disable CS0618 // Type or member is obsolete
                { SerializationFormat.Binary, (o,p,f)=>Binary(o,p,f) },
#pragma warning restore CS0618 // Type or member is obsolete
                { SerializationFormat.Json, (o,p,f)=>Json(o,p,f) },
                { SerializationFormat.Xml, (o,p,f)=>Xml(o,p,f) },
                { SerializationFormat.MsgPk, (o,p,f)=>MsgPk(o,p,f) },
            };
            private static Dictionary<SerializationFormat, Func<object, string, string, Task>> AsyncSerializationsDict { get; } = new Dictionary<SerializationFormat, Func<object, string, string, Task>>()
            {
#pragma warning disable CS0618 // Type or member is obsolete
                { SerializationFormat.Binary, (o,p,f)=>BinaryAsync(o,p,f) },
#pragma warning restore CS0618 // Type or member is obsolete
                { SerializationFormat.Json, (o,p,f)=>JsonAsync(o,p,f) },
                { SerializationFormat.Xml, (o,p,f)=>XmlAsync(o,p,f) },
                { SerializationFormat.MsgPk, (o,p,f)=>MsgPkAsync(o,p,f) }
            };

            public static void ByFormat(SerializationFormat f, object obj, string path, string file) => SerializationsDict[f](obj, path, file);

            public static Task ByFormatAsync(SerializationFormat f, object obj, string path, string file) => AsyncSerializationsDict[f](obj, path, file);

            [Obsolete("BinaryFormatter serialization is obsolete and should not be used. See https://aka.ms/binaryformatter for more information.")]
            public static Stream Binary(object obj)
            {
                var stm = new MemoryStream();
                new BinaryFormatter().Serialize(stm, obj);
                return stm;
            }

            [Obsolete("BinaryFormatter serialization is obsolete and should not be used. See https://aka.ms/binaryformatter for more information.")]
            public static Stream Binary(object obj, string path, string file)
            {
                string fullpath = Path.Combine(path, file);
                var stm = Binary(obj);
                var filestream = File.Create(fullpath);
                stm.Seek(0, SeekOrigin.Begin);
                stm.CopyTo(filestream);
                filestream.Close();
                return stm;
            }


            [Obsolete("BinaryFormatter serialization is obsolete and should not be used. See https://aka.ms/binaryformatter for more information.")]
            public static async Task<Stream> BinaryAsync(object obj) => await Task.Run(() => Binary(obj));

            [Obsolete("BinaryFormatter serialization is obsolete and should not be used. See https://aka.ms/binaryformatter for more information.")]
            public static async Task<Stream> BinaryAsync(object obj, string path, string file) => await Task.Run(() => Binary(obj, path, file));

            public static async Task<string> XmlAsync(object obj) => await Task.Run(() => Xml(obj));

            public static async Task<string> XmlAsync(object obj, string path, string file) => await Task.Run(() => Xml(obj, path, file));

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "C# language version retrocompatibility")]
            public static string Xml(object obj)
            {
                var t = obj.GetType();
                var serializer = new XmlSerializer(t);
                using (MemoryStream ms = new MemoryStream())
                {
                    serializer.Serialize(ms, obj);
                    ms.Position = 0;
                    return new StreamReader(ms).ReadToEnd();
                }
            }
            public static string Xml(object obj, string path, string file)
            {
                string fullpath = Path.Combine(path, file + XmlExtension);
                string xmlstring = Xml(obj);
                using (StreamWriter OutFile = new StreamWriter(new FileStream(fullpath, FileMode.Create, FileAccess.Write, FileShare.Read)))
                {
                    OutFile.WriteLine(xmlstring);
                }
                return xmlstring;
            }

            public static async Task<string> JsonAsync(object obj) => await Task<string>.Run(() => Json(obj));

            public static async Task<string> JsonAsync(object obj, string path, string file) => await Task<string>.Run(() => Json(obj, path, file));

            public static string Json(object obj) => JsonSerializer.Serialize(obj, JsonSerializationSettings.JsonSerializerOptions);

            public static string Json(object obj, string path, string file)
            {
                string fullpath = Path.Combine(path, file + JsonExtension);
                string jsonstring = Json(obj);
                using StreamWriter OutFile = new StreamWriter(new FileStream(fullpath, FileMode.Create, FileAccess.Write, FileShare.Read));
                OutFile.WriteLine(jsonstring);
                return jsonstring;
            }

            public static byte[] MsgPk(object obj) => MessagePackSerializer.Serialize(obj.GetType(), obj, MessagePackSerializationSettings.MessagePackSerializerOptions);
            public static byte[] MsgPk(object obj, string path, string file)
            {
                string fullpath = Path.Combine(path, file);
                byte[] bytearray = MsgPk(obj);
                using BinaryWriter OutFile = new(new FileStream(fullpath, FileMode.Create, FileAccess.Write, FileShare.Read));
                OutFile.Write(bytearray);
                return bytearray;
            }

            public static Task<byte[]> MsgPkAsync(object obj) => Task.Run(() => MsgPk(obj));
            public static Task<byte[]> MsgPkAsync(object obj, string path, string file) => Task.Run(() => MsgPk(obj, path, file));
        }
    }
}
