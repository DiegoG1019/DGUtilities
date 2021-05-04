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

            static uint prevMode;
            static bool EnabledFx = false;
            public static void InitConsoleEffects()
            {
                if (EnabledFx)
                    throw new InvalidOperationException("ConsoleEffects are already enabled");
                var handle = GetStdHandle(STD_OUTPUT_HANDLE);

                GetConsoleMode(handle, out uint mode);
                prevMode = mode;
                mode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;
                SetConsoleMode(handle, mode);
                EnabledFx = true;
            }
            public static void DisableConsoleEffects()
            {
                if (!EnabledFx)
                    throw new InvalidOperationException("ConsoleEffects are not yet enabled");
                var handle = GetStdHandle(STD_OUTPUT_HANDLE);
                SetConsoleMode(handle, prevMode);
                EnabledFx = false;
            }
        }
    }
}
