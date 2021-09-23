using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;

namespace DiegoG.Utilities.IO.Files
{
    public static class Helper
    {
        public static readonly IEnumerable<byte> NextBatchSignal;
        public static readonly int NextBatchSignalLength;

        public static readonly string WindowsIllegalCharactersRegex = @"[\\/:*?""<>\|]";
        public static readonly string WindowsInvalidFileNamesRegex = @"(^com[0-9])|(^lpt[0-9])|(^con)|(^prn)|(^aux)|(^nul)";

        public static readonly string OSXIllegalCharactersRegex = @"[:]";
        public static readonly string OSXInvalidFileNamesRegex = @"^\.";

        public static readonly string LinuxIllegalCharactersRegex = @"[/]";
        public static readonly string LinuxInvalidFileNamesRegex = @"";

        static Helper()
        {
            byte[] b = Encoding.ASCII.GetBytes("\u001c\u001e\u0001");
            NextBatchSignal = Array.AsReadOnly(b);
            NextBatchSignalLength = b.Length;
        }

        private static string? IllegalCharactersRegex = null;
        public static string GetIllegalCharactersRegex()
            =>    IllegalCharactersRegex ??=
                  RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? WindowsIllegalCharactersRegex
                : RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                ? OSXIllegalCharactersRegex
                : RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                ? LinuxIllegalCharactersRegex
                : throw new InvalidOperationException("This OS Platform has no supported regexes");

        private static string? InvalidFileNamesRegex = null;
        public static string GetInvalidFileNamesRegex()
            =>    InvalidFileNamesRegex ??=
                  RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? WindowsIllegalCharactersRegex
                : RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                ? OSXIllegalCharactersRegex
                : RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                ? LinuxIllegalCharactersRegex
                : throw new InvalidOperationException("This OS Platform has no supported regexes");
    }
}
