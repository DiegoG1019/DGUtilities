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
        private static void CallOnSerializedIfExists(object instance)
            => instance.GetType().GetMethods().Where(s => s.GetCustomAttributes(typeof(OnSerializedAttribute), false).Any()).SingleOrDefault()?.Invoke(instance, null);
        private static void CallOnSerializingIfExists(object instance)
            => instance.GetType().GetMethods().Where(s => s.GetCustomAttributes(typeof(OnSerializingAttribute), false).Any()).SingleOrDefault()?.Invoke(instance, null);
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
                TryInit();
                CallOnSerializingIfExists(obj);
                var stm = new MemoryStream();
                new BinaryFormatter().Serialize(stm, obj);
                CallOnSerializedIfExists(obj);
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

            public static string Xml(object obj)
            {
                TryInit();
                CallOnSerializingIfExists(obj);
                var t = obj.GetType();
                var serializer = new XmlSerializer(t);
                using MemoryStream ms = new MemoryStream();
                serializer.Serialize(ms, obj);
                ms.Position = 0;
                CallOnSerializedIfExists(obj);
                return new StreamReader(ms).ReadToEnd();
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

            public static string Json(object obj)
            {
                TryInit();
                CallOnSerializingIfExists(obj);
                var s = JsonSerializer.Serialize(obj, JsonSerializationSettings.JsonSerializerOptions);
                CallOnSerializedIfExists(obj);
                return s;
            }

            public static string Json(object obj, string path, string file)
            {
                string fullpath = Path.Combine(path, file + JsonExtension);
                string jsonstring = Json(obj);
                using StreamWriter OutFile = new(new FileStream(fullpath, FileMode.Create, FileAccess.Write, FileShare.Read));
                OutFile.WriteLine(jsonstring);
                return jsonstring;
            }

            public static byte[] MsgPk(object obj)
            {
                TryInit();
                CallOnSerializingIfExists(obj);
                var s = MessagePackSerializer.Serialize(obj.GetType(), obj, MessagePackSerializationSettings.MessagePackSerializerOptions);
                CallOnSerializedIfExists(obj);
                return s;
            }
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
