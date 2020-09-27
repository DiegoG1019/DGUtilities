using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DiegoG.Utilities
{
    public static class Serialization
    {
        public const string XmlExtension = ".xml";
        public const string JsonExtension = ".json";
        public static class Deserialize<T>
        {
            public static T Xml(string xmlString)
            {
                var t = typeof(T);
                var serializer = new XmlSerializer(t);
                using(StringReader sr = new StringReader(xmlString))
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

            public static async Task<T> XmlAsync(string xmlString)
            {
                var tsk = new Task<T>(new Func<T>(delegate { return Xml(xmlString); }));
                return await tsk;
            }
            public static async Task<T> XmlAsync(string path, string file)
            {
                var tsk = new Task<T>(new Func<T>(delegate { return Xml(path, file); }));
                return await tsk;
            }

            public static T Json(string jsonString)
            {
                return JsonSerializer.Deserialize<T>(jsonString);
            }
            public static T Json(string path, string file)
            {
                string fullpath = Path.Combine(path, file + JsonExtension);
                using (StreamReader InFile = new StreamReader(new FileStream(fullpath, FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    string jsonstring = InFile.ReadToEnd();
                    return Json(jsonstring);
                }
            }

            public static async Task<T> JsonAsync(string jsonString)
            {
                var tsk = new Task<T>(new Func<T>(delegate { return Json(jsonString); }));
                return await tsk;
            }
            public static async Task<T> JsonAsync(string path, string file)
            {
                var tsk = new Task<T>(new Func<T>(delegate { return Json(path, file); }));
                return await tsk;
            }

        }

        public static class Serialize
        {

            public static async Task<string> XmlAsync(object obj)
            {
                var tsk = new Task<string>(new Func<string>(delegate { return Xml(obj); }));
                return await tsk;
            }
            public static async Task<string> XmlAsync(object obj, string path, string file)
            {
                var tsk = new Task<string>(new Func<string>(delegate { return Xml(obj, path, file); }));
                return await tsk;
            }

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

            public static async Task<string> JsonAsync(object obj)
            {
                var tsk = new Task<string>(new Func<string>(delegate { return Json(obj); }));
                return await tsk;
            }
            public static async Task<string> JsonAsync(object obj, string path, string file)
            {
                var tsk = new Task<string>(new Func<string>(delegate { return Json(obj, path, file); }));
                return await tsk;
            }

            public static string Json(object obj)
            {
                return JsonSerializer.Serialize(obj);
            }
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
