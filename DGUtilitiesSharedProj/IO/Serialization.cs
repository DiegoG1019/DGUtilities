using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DiegoG.Utilities.IO
{
    public static class Serialization
    {
        public const string XmlExtension = ".xml";
        public const string JsonExtension = ".json";

        public static object CopyByBinarySerialization(object obj) => Deserialize<object>.Binary(Serialize.Binary(obj));

        public static class Deserialize<T>
        {
            public static T Binary(Stream stm)
            {
                var binf = new BinaryFormatter();
                return (T)binf.Deserialize(stm);
            }
            public static T Binary(string path, string file) => Binary(File.OpenRead(Path.Combine(path, file)));
            public static async Task<T> BinaryAsync(Stream stm) => await new Task<T>(new Func<T>(delegate { return Binary(stm); }));
            public static async Task<T> BinaryAsync(string path, string file) => await new Task<T>(new Func<T>(delegate { return Binary(path, file); }));
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
                using (StreamReader InFile = new StreamReader(new FileStream(fullpath, FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    string xmlstring = InFile.ReadToEnd();
                    return Xml(xmlstring);
                }
            }

            public static async Task<T> XmlAsync(string xmlString) => await new Task<T>(new Func<T>(delegate { return Xml(xmlString); }));
            public static async Task<T> XmlAsync(string path, string file) => await new Task<T>(new Func<T>(delegate { return Xml(path, file); }));

            public static T Json(string jsonString) => JsonSerializer.Deserialize<T>(jsonString);
            public static T Json(string path, string file)
            {
                string fullpath = Path.Combine(path, file + JsonExtension);
                using (StreamReader InFile = new StreamReader(new FileStream(fullpath, FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    string jsonstring = InFile.ReadToEnd();
                    return Json(jsonstring);
                }
            }

            public static async Task<T> JsonAsync(string jsonString) => await new Task<T>(new Func<T>(delegate { return Json(jsonString); }));
            public static async Task<T> JsonAsync(string path, string file) => await new Task<T>(new Func<T>(delegate { return Json(path, file); }));
        }

        public static class Serialize
        {

            public static Stream Binary(object obj)
            {
                var stm = new MemoryStream();
                new BinaryFormatter().Serialize(stm, obj);
                return stm;
            }
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

            public static async Task<Stream> BinaryAsync(object obj) => await new Task<Stream>(new Func<Stream>(delegate { return Binary(obj); }));
            public static async Task<Stream> BinaryAsync(object obj, string path, string file) => await new Task<Stream>(new Func<Stream>(delegate { return Binary(obj, path, file); }));
            public static async Task<string> XmlAsync(object obj) => await new Task<string>(new Func<string>(delegate { return Xml(obj); }));
            public static async Task<string> XmlAsync(object obj, string path, string file) => await new Task<string>(new Func<string>(delegate { return Xml(obj, path, file); }));
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

            public static async Task<string> JsonAsync(object obj) => await new Task<string>(new Func<string>(delegate { return Json(obj); }));
            public static async Task<string> JsonAsync(object obj, string path, string file) => await new Task<string>(new Func<string>(delegate { return Json(obj, path, file); }));
            public static string Json(object obj) => JsonSerializer.Serialize(obj);
            public static string Json(object obj, string path, string file)
            {
                string fullpath = Path.Combine(path, file + JsonExtension);
                string jsonstring = Json(obj);
                using (StreamWriter OutFile = new StreamWriter(new FileStream(fullpath, FileMode.Create, FileAccess.Write, FileShare.Read)))
                {
                    OutFile.WriteLine(jsonstring);
                }
                return jsonstring;
            }
        }

    }
}
