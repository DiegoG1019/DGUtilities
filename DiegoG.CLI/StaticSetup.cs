using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace DiegoG.CLI
{
    internal static class StaticSetup
    {
        [SupportedOSPlatform("Windows")]
        internal static class Windows
        {
            const int STD_OUTPUT_HANDLE = -11;
            const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 4;

            [DllImport("kernel32.dll", SetLastError = true)]
            static extern IntPtr GetStdHandle(int nStdHandle);

            [DllImport("kernel32.dll")]
            static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

            [DllImport("kernel32.dll")]
            static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

            public static void InitConsoleEffects()
            {
                var handle = GetStdHandle(STD_OUTPUT_HANDLE);
                uint mode;
                GetConsoleMode(handle, out mode);
                mode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;
                SetConsoleMode(handle, mode);
            }
        }
    }
}
