using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;

namespace DiegoG.Utilities
{
    public static class Serialization
    {
        public const string XmlExtension = ".xml";
        public const string JsonExtension = ".json";
        public static class Deserialize
        {
            public static T Xml<T>(string xmlString)
            {
                var t = typeof(T);
                var serializer = new XmlSerializer(t);
                using StringReader sr = new StringReader(xmlString);
                return (T)serializer.Deserialize(sr);
            }
            public static T Xml<T>(string path, string file)
            {
                string fullpath = Path.Combine(path, file + XmlExtension);
                using (StreamReader InFile = new StreamReader(new FileStream(fullpath, FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    string xmlstring = InFile.ReadToEnd();
                    return Xml<T>(xmlstring);
                }
            }

            public static T Json<T>(string jsonString)
            {
                return JsonSerializer.Deserialize<T>(jsonString);
            }
            public static T Json<T>(string path, string file)
            {
                string fullpath = Path.Combine(path, file + JsonExtension);
                using (StreamReader InFile = new StreamReader(new FileStream(fullpath, FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    string jsonstring = InFile.ReadToEnd();
                    return Json<T>(jsonstring);
                }
            }
        }

        public static class Serialize
        {
            public static string Xml(object obj)
            {
                var t = obj.GetType();
                var serializer = new XmlSerializer(t);
                using MemoryStream ms = new MemoryStream();
                serializer.Serialize(ms, obj);
                ms.Position = 0;
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
