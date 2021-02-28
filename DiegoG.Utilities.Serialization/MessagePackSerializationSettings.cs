using MessagePack;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiegoG.Utilities.IO
{
    public static class MessagePackSerializationSettings
    {
        public static void Init()
        {
            Log.Debug("Initializing MessagePackSerializationSettings");
            MessagePackSerializerOptions = MessagePack.MessagePackSerializerOptions.Standard;
        }
        public static MessagePackSerializerOptions MessagePackSerializerOptions { get; private set; }
    }
}
