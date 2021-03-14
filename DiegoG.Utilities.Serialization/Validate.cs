using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiegoG.Utilities.IO
{
    public static partial class Serialization
    {
        public static class Validate
        {
            public static bool Json(string jsonstr)
            {
                TryInit();
#if DEBUG
                var r = jsonstr.StartsWith('{') && jsonstr.EndsWith('}') || jsonstr.StartsWith('[') && jsonstr.EndsWith(']');
                return r;
#else
                return jsonstr.StartsWith('{') && jsonstr.EndsWith('}') || jsonstr.StartsWith('[') && jsonstr.EndsWith(']');
#endif
            }

            public static bool Json(string path, string file, out string jsonstr)
            {
                string fullpath = Path.Combine(path, file + JsonExtension);
                using StreamReader InFile = new StreamReader(new FileStream(fullpath, FileMode.Open, FileAccess.Read, FileShare.Read));
                jsonstr = InFile.ReadToEnd();
                return Json(jsonstr);
            }
        }
    }
}
