using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics.CodeAnalysis;

namespace DiegoG.CLI
{
    public static class DConsole
    {
        private static readonly Thread InputThread;
        private static readonly AutoResetEvent GetInput, GotInput;
        private static string Input;

        static DConsole()
        {
            GetInput = new AutoResetEvent(false);
            GotInput = new AutoResetEvent(false);
            InputThread = new Thread(Reader) { IsBackground = true };
            InputThread.Start();
        }

        public static string ClearLineString => new string(' ', Console.BufferWidth);

        public static ConsoleColor BackgroundColor
        {
            get => Console.BackgroundColor;
            set => Console.BackgroundColor = value;
        }
        public static ConsoleColor TextColor
        {
            get => Console.ForegroundColor;
            set => Console.ForegroundColor = value;
        }
        
        public static (int X, int Y) CursorPos
        {
            get => GetCursorPosition();
            set => SetCursorPosition(value);
        }
        public static int CursorX
        {
            get => Console.CursorLeft;
            set => Console.CursorLeft = value;
        }
        public static int CursorY
        {
            get => Console.CursorTop;
            set => Console.CursorTop = value;
        }

        public static void Beep() => Console.Beep();
        public static void Clear() => Console.Clear();
        public static void Write(object o) => Console.Write(o);
        public static void Write(string s, params object[] format) => Console.Write(s, format);
        public static void WriteLine(object o) => Console.WriteLine(o);
        public static void WriteLine(string s, params object[] format) => Console.WriteLine(s, format);
        public static void SetCursorPosition(int X, int Y) => Console.SetCursorPosition(X, Y);
        public static void SetCursorPosition((int x, int y) pos) => Console.SetCursorPosition(pos.x, pos.y);
        public static (int Left, int Top) GetCursorPosition() => (Console.CursorLeft, Console.CursorTop);
        /// <summary>
        /// Clears and sets the cursor at the specified position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void ClearAndSetCursor(int x, int y)
        {
            Clear();
            SetCursorPosition(x, y);
        }
        public static void ClearAndSetCursor((int x, int y) pos) => ClearAndSetCursor(pos.x, pos.y);
        public static void ClearLine(int y)
        {
            SetCursorPosition(0, y);
            Write(ClearLineString);
        }
        public static void ClearUntil(int y)
        {
            for (int line = 0; line <= y; line++)
                ClearLine(line);
        }
        public static void ClearFrom(int y, int linesToClear)
        {
            for (int line = y; line <= linesToClear; line++)
                ClearLine(line);
        } 

        private static void Reader()
        {
            while (true)
            {
                GetInput.WaitOne();
                Input = Console.ReadLine();
                GotInput.Set();
            }
        }

        /// <summary>
        /// Code sourced from https://stackoverflow.com/questions/57615/how-to-add-a-timeout-to-console-readline
        /// </summary>
        /// <param name="timeOutMillisecs"></param>
        /// <returns></returns>
        /// <exception cref="TimeoutException"></exception>
        public static string ReadLine(int timeOutMillisecs = Timeout.Infinite)
        {
            GetInput.Set();
            bool success = GotInput.WaitOne(timeOutMillisecs);
            if (success)
                return Input;
            else
                throw new TimeoutException("User did not provide input within the timelimit.");
        }

        public static bool TryReadLine([MaybeNullWhen(false)]out string input, int timeOutMillisecs = Timeout.Infinite)
        {
            GetInput.Set();
            bool success = GotInput.WaitOne(timeOutMillisecs);
            if (success)
            {
                input = Input;
                return true;
            }
            input = null;
            return false;
        }
    }
}
