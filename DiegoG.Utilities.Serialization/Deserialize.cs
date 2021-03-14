using MessagePack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DiegoG.Utilities.IO
{
    public static partial class Serialization
    {
        private static void CallOnDeserializedIfExists(Type type, object instance)
            => type.GetMethods().Where(s => s.GetCustomAttributes(typeof(OnDeserializedAttribute), false).Any()).SingleOrDefault()?.Invoke(instance, null);
        public static class Deserialize
        {

            public static object Json(string jsonString, Type type)
            {
                TryInit();
                var ob = JsonSerializer.Deserialize(jsonString, type, JsonSerializationSettings.JsonSerializerOptions);
                CallOnDeserializedIfExists(type, ob);
                return ob;
            }

            public static object Json(string path, string file, Type type)
            {
                string fullpath = Path.Combine(path, file + JsonExtension);
                using StreamReader InFile = new StreamReader(new FileStream(fullpath, FileMode.Open, FileAccess.Read, FileShare.Read));
                string jsonstring = InFile.ReadToEnd();
                return Json(jsonstring, type);
            }

            public static async Task<object> JsonAsync(string jsonString, Type type) => await Task.Run(() => Json(jsonString, type));

            public static async Task<object> JsonAsync(string path, string file, Type type) => await Task.Run(() => Json(path, file, type));

            public static object MsgPk(byte[] MsgPkByteArray, Type type)
            {
                TryInit();
                var ob = MessagePackSerializer.Deserialize(type, MsgPkByteArray, MessagePackSerializationSettings.MessagePackSerializerOptions);
                CallOnDeserializedIfExists(type, ob);
                return ob;
            }
            public static object MsgPk(string path, string file, Type type)
            {
                TryInit();
                string fullpath = Path.Combine(path, file + JsonExtension);
                using FileStream InFile = new FileStream(fullpath, FileMode.Open, FileAccess.Read, FileShare.Read);
                var ob = MessagePackSerializer.Deserialize(type, InFile, MessagePackSerializationSettings.MessagePackSerializerOptions);
                CallOnDeserializedIfExists(type, ob);
                return ob;
            }
            public static async Task<object> MsgPkAsync(byte[] MsgPkByteArray, Type type) => await Task.Run(() => MsgPk(MsgPkByteArray, type));
            public static async Task<object> MsgPkAsync(string path, string file, Type type) => await Task.Run(() => MsgPk(path, file, type));
        }
        public static class Deserialize<T>
        {
            private static readonly Type TType = typeof(T);
            private static Dictionary<SerializationFormat, Func<string, string, T>> DeserializationsDict { get; } = new Dictionary<SerializationFormat, Func<string, string, T>>()
            {
#pragma warning disable CS1062 // Type or member is obsolete
                { SerializationFormat.Binary, Binary },
#pragma warning restore CS1062 // Type or member is obsolete
                { SerializationFormat.Json, Json },
                { SerializationFormat.Xml, Xml },
                { SerializationFormat.MsgPk, MsgPk },
            };
            private static Dictionary<SerializationFormat, Func<string, string, Task<T>>> AsyncDeserializationsDict { get; } = new Dictionary<SerializationFormat, Func<string, string, Task<T>>>()
            {
#pragma warning disable CS1062 // Type or member is obsolete
                { SerializationFormat.Binary, BinaryAsync },
#pragma warning restore CS1062 // Type or member is obsolete
                { SerializationFormat.Json, JsonAsync },
                { SerializationFormat.Xml, XmlAsync },
                { SerializationFormat.MsgPk, MsgPkAsync },
            };

            public static T ByFormat(SerializationFormat f, string path, string file) => DeserializationsDict[f](path, file);

            public static Task<T> ByFormatAsync(SerializationFormat f, string path, string file) => AsyncDeserializationsDict[f](path, file);

            [Obsolete("BinaryFormatter serialization is obsolete and should not be used. See https://aka.ms/binaryformatter for more information.")]
            public static T Binary(Stream stm)
            {
                TryInit();
                var binf = new BinaryFormatter();
                var obj = (T)binf.Deserialize(stm);
                CallOnDeserializedIfExists(TType, obj);
                return obj;
            }

            [Obsolete("BinaryFormatter serialization is obsolete and should not be used. See https://aka.ms/binaryformatter for more information.")]
            public static T Binary(string path, string file) => Binary(File.OpenRead(Path.Combine(path, file)));

            [Obsolete("BinaryFormatter serialization is obsolete and should not be used. See https://aka.ms/binaryformatter for more information.")]
            public static async Task<T> BinaryAsync(Stream stm) => await Task<T>.Run(() => Binary(stm));

            [Obsolete("BinaryFormatter serialization is obsolete and should not be used. See https://aka.ms/binaryformatter for more information.")]
            public static async Task<T> BinaryAsync(string path, string file) => await Task<T>.Run(() => Binary(path, file));

            public static T Xml(string xmlString)
            {
                TryInit();
                var t = typeof(T);
                var serializer = new XmlSerializer(t);
                using StringReader sr = new StringReader(xmlString);
                var obj = (T)serializer.Deserialize(sr);
                CallOnDeserializedIfExists(TType, obj);
                return obj;
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

            public static T Json(string jsonString)
            {
                TryInit();
                var obj = JsonSerializer.Deserialize<T>(jsonString, JsonSerializationSettings.JsonSerializerOptions);
                CallOnDeserializedIfExists(TType, obj);
                return obj;
            }

            public static T Json(string path, string file)
            {
                string fullpath = Path.Combine(path, file + JsonExtension);
                using StreamReader InFile = new StreamReader(new FileStream(fullpath, FileMode.Open, FileAccess.Read, FileShare.Read));
                string jsonstring = InFile.ReadToEnd();
                return Json(jsonstring);
            }

            public static async Task<T> JsonAsync(string jsonString) => await Task<T>.Run(() => Json(jsonString));

            public static async Task<T> JsonAsync(string path, string file) => await Task<T>.Run(() => Json(path, file));

            public static T MsgPk(byte[] MsgPkByteArray)
            {
                TryInit();
                var obj = MessagePackSerializer.Deserialize<T>(MsgPkByteArray, MessagePackSerializationSettings.MessagePackSerializerOptions);
                CallOnDeserializedIfExists(TType, obj);
                return obj;
            }
            public static T MsgPk(string path, string file)
            {
                TryInit();
                string fullpath = Path.Combine(path, file + JsonExtension);
                using FileStream InFile = new FileStream(fullpath, FileMode.Open, FileAccess.Read, FileShare.Read);
                var obj = MessagePackSerializer.Deserialize<T>(InFile, MessagePackSerializationSettings.MessagePackSerializerOptions);
                CallOnDeserializedIfExists(TType, obj);
                return obj;
            }
            public static async Task<T> MsgPkAsync(byte[] MsgPkByteArray) => await Task.Run(() => MsgPk(MsgPkByteArray));
            public static async Task<T> MsgPkAsync(string path, string file) => await Task.Run(() => MsgPk(path, file));
        }
    }
}
