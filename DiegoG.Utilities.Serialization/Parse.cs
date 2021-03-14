using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;

namespace DiegoG.Utilities.IO
{
    public static partial class Serialization
    {
        public static class Parse
        {
            public static JsonDocument Json(string path, string file)
            {
                TryInit();
                string fullpath = Path.Combine(path, file + JsonExtension);
                using StreamReader InFile = new StreamReader(new FileStream(fullpath, FileMode.Open, FileAccess.Read, FileShare.Read));
                string jsonstring = InFile.ReadToEnd();
                return JsonDocument.Parse(jsonstring);
            }

            public static JsonDocument Json(string jsonstring)
            {
                TryInit();
                return JsonDocument.Parse(jsonstring);
            }

            public static Task<JsonDocument> JsonAsync(string path, string file) => Task.Run(() => Json(path, file));
            public static Task<JsonDocument> JsonAsync(string jsonstring) => Task.Run(() => Json(jsonstring));

            public static XmlDocument Xml(string path, string file)
            {
                TryInit();
                string fullpath = Path.Combine(path, file + XmlExtension);
                using StreamReader InFile = new StreamReader(new FileStream(fullpath, FileMode.Open, FileAccess.Read, FileShare.Read));
                var xml = new XmlDocument();
                xml.LoadXml(InFile.ReadToEnd());
                return xml;
            }

            public static XmlDocument Xml(string xmlstring)
            {
                TryInit();
                var xml = new XmlDocument();
                xml.LoadXml(xmlstring);
                return xml;
            }

            public static Task<XmlDocument> XmlAsync(string path, string file) => Task.Run(() => Xml(path, file));
            public static Task<XmlDocument> XmlAsync(string xmlstring) => Task.Run(() => Xml(xmlstring));
        }
    }
}
