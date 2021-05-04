using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.Versioning;

namespace DiegoG.CLI
{
    public static class DConsole
    {

        //I do not implement a console descriptor, simply because 'Console' already does exactly that. WindowSize and, specially, input device, are done like that to allow combined usage with Console GUI Frameworks

        private static readonly Thread InputThread;
        private static readonly object Sync = new();

        private static AutoResetEvent GotInput, GotLine;

        private static ConsoleKeyInfo KeyBuffer;
        private static readonly List<char> LineBuffer = new(50);

        public static event Action<ConsoleKeyInfo> ControlKeyPressed;
        public static event Action<ConsoleKeyInfo> AltKeyPressed;
        public static event Action<ConsoleKeyInfo> KeyPressed;

        public static event Action<Rectangle> BufferSizeChanged;
        /// <summary>
        /// Invoked when the Window Position or Size changes, passes the current Window Size and Position as a rectangle
        /// </summary>
        public static event Action<Rectangle> WindowChanged;

        /// <summary>
        /// Could potentially break the application by taking too long between checks, serves as a throttle to avoid using too much CPU. NOT RECOMMENDED TO CHANGE.
        /// </summary>
        public static int InputDelayTime { get; set; } = 100;

        /// <summary>
        /// Defaults to Console.ReadKey; if it's ever set to null, an exception will be thrown.
        /// </summary>
        public static Func<ConsoleKeyInfo> InputDevice
        {
            private get => InputDevice_Field;
            set
            {
                if (value is null)
                    throw new ArgumentNullException(nameof(value));
                InputDevice_Field = value;
            }
        }
        static Func<ConsoleKeyInfo> InputDevice_Field = Console.ReadKey;

        public static Func<Rectangle> WindowDescriptor
        {
            get => WindowDescriptor_Field;
            set
            {
                if (value is null)
                    throw new ArgumentNullException(nameof(value));
                WindowDescriptor_Field = value;
            }
        }
        static Func<Rectangle> WindowDescriptor_Field = () => new(Console.WindowLeft, Console.WindowTop, Console.WindowWidth, Console.WindowHeight);

        /// <summary>
        /// Buffer should have a position of 0,0
        /// </summary>
        public static Func<Rectangle> BufferDescriptor
        {
            get => BufferDescriptor_Field;
            set
            {
                if (value is null)
                    throw new ArgumentNullException(nameof(value));
                BufferDescriptor_Field = value;
            }
        }
        static Func<Rectangle> BufferDescriptor_Field = ()=> new(0, 0, Console.BufferWidth, Console.BufferHeight);

        /// <summary>
        /// Same as WindowDescriptor, but with position set to 0,0
        /// </summary>
        public static Func<Rectangle> WindowSize => () =>
        {
            var win = WindowDescriptor_Field();
            return new(0, 0, win.Width, win.Height);
        };

        private static readonly DConsoleObject Instance = new();

        private static readonly IReadOnlyDictionary<ConsoleEffect, string> EffectDefs = new Dictionary<ConsoleEffect, string>()
        {
            { ConsoleEffect.None, None },

            { ConsoleEffect.Bold, None + Bold },
            { ConsoleEffect.Faint, None + Faint },

            { ConsoleEffect.Underline, None + Underline },
            { ConsoleEffect.Strikethrough, Strikethrough },

            { ConsoleEffect.BoldUnderline, None + Bold + Underline },
            { ConsoleEffect.BoldStrikethrough, None + Bold + Strikethrough },
            { ConsoleEffect.BoldStrikethroughUnderline, None + Bold + Strikethrough + Underline },

            { ConsoleEffect.FaintUnderline, None + Faint + Underline },
            { ConsoleEffect.FaintStrikethrough, None + Faint + Strikethrough },
            { ConsoleEffect.FaintStrikethroughUnderline, None + Faint + Strikethrough + Underline },
        };

        private const string None = "\x1B[0m";
        private const string Bold = "\x1B[1m";
        private const string Faint = "\x1B[2m";
        private const string Underline = "\x1B[4m";
        private const string Strikethrough = "\x1B[9m";

        public enum ConsoleEffect
        {
            None,

            Bold,
            Faint,

            Underline,
            Strikethrough,

            BoldUnderline,
            BoldStrikethrough,
            BoldStrikethroughUnderline,

            FaintUnderline,
            FaintStrikethrough,
            FaintStrikethroughUnderline,
        }


        static DConsole()
        {
            GotInput = new AutoResetEvent(false);
            GotLine = new AutoResetEvent(false);

            InputThread = new Thread(Reader) { IsBackground = true };
            InputThread.Start();
        }
        
        /// <summary>
        /// Only necessary on Windows
        /// </summary>
        [SupportedOSPlatform("Windows")]
        public static void EnableConsoleEffects() => StaticSetup.Windows.InitConsoleEffects();
        /// <summary>
        /// Only necessary on Windows
        /// </summary>
        [SupportedOSPlatform("Windows")]
        public static void DisableConsoleEffects() => StaticSetup.Windows.DisableConsoleEffects();

        private static void Reader()
        {
            while (true)
            {
                KeyBuffer = InputDevice();

                if (KeyBuffer.Key == ConsoleKey.Enter)
                    GotLine.Set();
                else
                    LineBuffer.Add(KeyBuffer.KeyChar);

                GotInput.Set();

                var winRect = WindowDescriptor();
                if (Width != winRect.Width || Height != winRect.Height || WindowX != winRect.X || WindowY != winRect.Y)
                    WindowChanged?.Invoke(WindowDescriptor());

                var bufRect = BufferDescriptor();
                if (BufferHeight != bufRect.Height || BufferWidth != bufRect.Width)
                    BufferSizeChanged?.Invoke(BufferDescriptor());
                                                                                                     
                KeyPressed?.Invoke(KeyBuffer);
                if (KeyBuffer.Modifiers is ConsoleModifiers.Control)
                    ControlKeyPressed?.Invoke(KeyBuffer);
                if (KeyBuffer.Modifiers is ConsoleModifiers.Alt)
                    AltKeyPressed?.Invoke(KeyBuffer);
            }
        }

        public static string ClearLineString => new string(' ', BufferDescriptor().Width);

        public static ConsoleEffect Effect { get; set; }

        public static ConsoleColor BackgroundColor { get; set; }
        public static ConsoleColor TextColor { get; set; }

        public static (int X, int Y) CursorPos
        {
            get => GetCursorPosition();
            set => SetCursorPosition(value);
        }

        public static int MiddleX => Width / 2;
        public static int MiddleY => Height / 2;

        public static int BufferWidth => BufferDescriptor().Width;
        public static int BufferHeight => BufferDescriptor().Height;

        public static int Width => WindowDescriptor().Width;
        public static int Height => WindowDescriptor().Height;

        public static int WindowX { get; private set; } = WindowDescriptor().X;
        public static int WindowY { get; private set; } = WindowDescriptor().Y;

        public static int PlaceMiddleX(string s) => MiddleX - s.Length / 2;

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

        private static void SetEffects() { Console.Write(EffectDefs[Effect]); Console.ForegroundColor = TextColor; Console.BackgroundColor = BackgroundColor; }
        public static DConsoleObject Beep() { Console.Beep(); return Instance; }
        public static DConsoleObject Write(object o) { SetEffects(); Console.Write(o); return Instance; }
        public static DConsoleObject Write(string s, params object[] format) { SetEffects(); Console.Write(s, format); return Instance; }
        public static DConsoleObject WriteLine(object o) { Console.Write(EffectDefs[Effect]); Console.WriteLine(o); return Instance; }
        public static DConsoleObject WriteLine(string s, params object[] format) { SetEffects(); Console.WriteLine(s, format); return Instance; }
        public static DConsoleObject SetCursorPosition(int X, int Y) { Console.SetCursorPosition(X, Y); return Instance; }
        public static DConsoleObject SetCursorPosition((int x, int y) pos) { Console.SetCursorPosition(pos.x, pos.y); return Instance; }
        public static DConsoleObject FWrite(string s, ConsoleColor? textc = null, ConsoleColor? backc = null, ConsoleEffect? fx = null, int? x = null, int? y = null, params object[] format)
        {
            lock (Sync)
            {
                TextColor = textc ?? TextColor;
                BackgroundColor = backc ?? BackgroundColor;
                Effect = fx ?? Effect;
                CursorX = x ?? CursorX;
                CursorY = y ?? CursorY;
                return Write(s, format);
            }
        }
        public static DConsoleObject FWriteL(string s, ConsoleColor? textc = null, ConsoleColor? backc = null, ConsoleEffect? fx = null, int? x = null, int? y = null, params object[] format) => FWrite(s + "\n", textc, backc, fx, x, y, format);
        public static DConsoleObject InputField(int? x = null, int? y = null, string s = "> ", ConsoleColor? textc = null, ConsoleColor? backc = null, ConsoleEffect? fx = null)
                => FWrite(s, textc, backc, fx, x, y);
        public static DConsoleObject Clear(ConsoleColor? color = null)
        {
            lock (Sync)
            {
                var c = BackgroundColor;
                BackgroundColor = color ?? c; 
                Console.Clear();
                BackgroundColor = c;
                return Instance; 
            }
        }

        public static DConsoleObject DrawRectangle(Rectangle rectangle, ConsoleColor? color = null, char filler = ' ', ConsoleColor? fillerColor = null)
        {
            if (!WindowDescriptor().Contains(rectangle))
                throw new ArgumentException("Provided Rectangle must be contained within Console buffer (Position + Size cannot go beyond buffer bounds)", nameof(rectangle));
            lock (Sync)
            {
                var clsy = new string(' ', rectangle.Width);
                CursorPos = (rectangle.X, rectangle.Y);
                for(int i = 0; i < rectangle.Height; i++)
                {
                    Console.Write(clsy);
                    CursorY++;
                }
            }
            return Instance;
        }

        /// <summary>
        /// Draws a rectangle with the specified or current color, with the specified stroke width
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="strokewidth"></param>
        /// <param name="color"></param>
        /// <param name="filler"></param>
        /// <param name="fillerColor"></param>
        /// <returns></returns>
        public static DConsoleObject DrawRectangle(Rectangle rectangle, int strokewidth, ConsoleColor? color = null, char filler = ' ', ConsoleColor? fillerColor = null)
        {
            if (!WindowDescriptor().Contains(rectangle))
                throw new ArgumentException("Provided Rectangle must be contained within Console buffer (Position + Size cannot go beyond buffer bounds)",nameof(rectangle));

            var clsy = new string(filler, rectangle.Width);
            var clsx = string.Concat(Enumerable.Repeat(filler + "\n", rectangle.Height));

            (int X, int Y) topPointer = (rectangle.X, rectangle.Y);
            (int X, int Y) bottomPointer = (rectangle.X, rectangle.Bottom);
            (int X, int Y) leftPointer = (rectangle.X, rectangle.Y);
            (int X, int Y) rightPointer = (rectangle.Right, rectangle.Y);

            lock (Sync)
            {
                var bc = BackgroundColor;
                var tc = TextColor;
                BackgroundColor = color ?? bc;
                TextColor = fillerColor ?? tc;

                for(int i = 0; i < strokewidth; i++)
                {
                    CursorPos = topPointer;
                    Write(clsy);
                    topPointer.Y++;

                    CursorPos = bottomPointer;
                    Write(clsy);
                    bottomPointer.Y--;

                    CursorPos = leftPointer;
                    Write(clsx);
                    leftPointer.X++;

                    CursorPos = rightPointer;
                    for(int y = 0; y <= rectangle.Height; y++)
                    {
                        Write(filler);
                        CursorY++;
                    }
                    rightPointer.X--;
                }

                BackgroundColor = bc;
                TextColor = tc;
            }

            return Instance;
        }

        public static (int Left, int Top) GetCursorPosition() => (Console.CursorLeft, Console.CursorTop);

        /// <summary>
        /// Clears and sets the cursor at the specified position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static DConsoleObject ClearAndSetCursor(int x, int y, ConsoleColor? color = null)
        {
            Clear();
            return SetCursorPosition(x, y);
        }
        public static DConsoleObject ClearAndSetCursor((int x, int y) pos, ConsoleColor? color = null) => ClearAndSetCursor(pos.x, pos.y, color);
        public static DConsoleObject ClearLine(int y, ConsoleColor? color = null)
        {
            lock (Sync)
            {
                var c = BackgroundColor;
                BackgroundColor = color ?? c;
                SetCursorPosition(0, y);
                BackgroundColor = c;
            }
            return Write(ClearLineString);
        }
        public static DConsoleObject ClearUntil(int y, ConsoleColor? color = null)
        {
            for (int line = 0; line <= y; line++)
                ClearLine(line);
            return Instance;
        }
        public static DConsoleObject ClearFrom(int y, int linesToClear, ConsoleColor? color = null)
        {
            for (int line = y; line <= linesToClear; line++)
                ClearLine(line);
            return Instance;
        } 

        private static string GetLineBuffer()
        {
            var s = new string(LineBuffer.ToArray());
            LineBuffer.Clear();
            return s;
        }
        private static ConsoleKeyInfo GetKeyBuffer() { GetLineBuffer(); return KeyBuffer; }


        /// <summary>
        /// Code sourced from https://stackoverflow.com/questions/57615/how-to-add-a-timeout-to-console-readline
        /// </summary>
        /// <param name="timeOutMillisecs"></param>
        /// <returns></returns>
        /// <exception cref="TimeoutException"></exception>
        public static string ReadLine(int timeOutMillisecs = Timeout.Infinite)
        {
            bool success = GotLine.WaitOne(timeOutMillisecs);
            return success ? GetLineBuffer() : throw new TimeoutException("User did not provide input within the timelimit.");
        }

        public static bool TryReadLine([MaybeNullWhen(false)]out string input, int timeOutMillisecs = Timeout.Infinite)
        {
            bool success = GotLine.WaitOne(timeOutMillisecs);
            if (success)
            {
                input = GetLineBuffer();
                return true;
            }
            input = null;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="onComplete">An action to execute upon succesful completion. Receives the Line read as a input</param>
        /// <param name="onFailure">An action to execute upon failure. Receives the Line read as a input</param>
        /// <returns></returns>
        public static Task<string> ReadLineAsync(int timeoutms = Timeout.Infinite, Action<string> onComplete = null, Action<string> onFailure = null) => Task.Run(() =>
        {
            if (TryReadLine(out var outp, timeoutms))
                onComplete?.Invoke(outp);
            else
                onFailure?.Invoke(outp);
            return outp;
        });

        public static ConsoleKeyInfo ReadKey(int timeOutMillisecs = Timeout.Infinite)
        {
            bool success = GotInput.WaitOne(timeOutMillisecs);
            return success ? GetKeyBuffer() : throw new TimeoutException("User did not provide input within the timelimit.");
        }

        public static bool TryReadKey([MaybeNullWhen(false)] out ConsoleKeyInfo input, int timeOutMillisecs = Timeout.Infinite)
        {
            bool success = GotInput.WaitOne(timeOutMillisecs);
            if (success)
            {
                input = GetKeyBuffer();
                return true;
            }
            input = default;
            return false;
        }

        public static Task<ConsoleKeyInfo> ReadKeyAsync(Action onComplete = null, Action onFailure = null) => Task.Run(() =>
        {
            if (TryReadKey(out var outp))
                onComplete?.Invoke();
            else
                onFailure?.Invoke();
            return outp;
        });

        /// <summary>
        /// Facilitates method chaining
        /// </summary>
        public sealed class DConsoleObject
        {
            public string ReadLine(int timeOutMillisecs = Timeout.Infinite) 
                => DConsole.ReadLine(timeOutMillisecs);

            public bool TryReadLine([MaybeNullWhen(false)] out string input, int timeOutMillisecs = Timeout.Infinite)
                => DConsole.TryReadLine(out input, timeOutMillisecs);

            public Task<string> ReadLineAsync(int timeoutMs = Timeout.Infinite, Action<string> onComplete = null, Action<string> onFailure = null)
                => DConsole.ReadLineAsync(timeoutMs, onComplete, onFailure);

            public ConsoleKeyInfo ReadKey(int timeOutMillisecs = Timeout.Infinite)
                => DConsole.ReadKey(timeOutMillisecs);

            public bool TryReadKey([MaybeNullWhen(false)] out ConsoleKeyInfo input, int timeOutMillisecs = Timeout.Infinite)
                => DConsole.TryReadKey(out input, timeOutMillisecs);

            public Task<ConsoleKeyInfo> ReadKeyAsync(Action onComplete = null, Action onFailure = null)
                => DConsole.ReadKeyAsync(onComplete, onFailure);
            
            /// <summary>
            /// Clears and sets the cursor at the specified position
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            public DConsoleObject ClearAndSetCursor(int x, int y, ConsoleColor? color = null)
                => DConsole.ClearAndSetCursor(x, y, color);
            public DConsoleObject ClearAndSetCursor((int x, int y) pos, ConsoleColor? color = null)
                => DConsole.ClearAndSetCursor(pos, color);
            public DConsoleObject ClearLine(int y, ConsoleColor? color = null)
                => DConsole.ClearLine(y, color);
            public DConsoleObject ClearUntil(int y, ConsoleColor? color = null)
                => DConsole.ClearUntil(y, color);
            public DConsoleObject ClearFrom(int y, int linesToClear, ConsoleColor? color = null)
                => DConsole.ClearFrom(y, linesToClear, color);
            public DConsoleObject Beep()
                => DConsole.Beep();
            public DConsoleObject Clear(ConsoleColor? color = null)
                => DConsole.Clear(color);
            public DConsoleObject Write(object o)
                => DConsole.Write(o);
            public DConsoleObject Write(string s, params object[] format)
                => DConsole.Write(s, format);
            public DConsoleObject WriteLine(object o)
                => DConsole.WriteLine(o);
            public DConsoleObject WriteLine(string s, params object[] format)
                => DConsole.WriteLine(s, format);
            public DConsoleObject SetCursorPosition(int X, int Y)
                => DConsole.SetCursorPosition(X, Y);
            public DConsoleObject SetCursorPosition((int x, int y) pos)
                => DConsole.SetCursorPosition(pos);
            public DConsoleObject FWrite(string s, ConsoleColor? textc = null, ConsoleColor? backc = null, ConsoleEffect? fx = null, int? x = null, int? y = null, params object[] format)
                => DConsole.FWrite(s, textc, backc, fx, x, y, format); 
            public static DConsoleObject FWriteL(string s, ConsoleColor? textc = null, ConsoleColor? backc = null, ConsoleEffect? fx = null, int? x = null, int? y = null, params object[] format) => DConsole.FWriteL(s, textc, backc, fx, x, y, format);
            public DConsoleObject InputField(int? x = null, int? y = null, string s = "> ", ConsoleColor? textc = null, ConsoleColor? backc = null, ConsoleEffect? fx = null)
                => DConsole.InputField(x, y, s, textc, backc, fx);
            internal DConsoleObject() { }
        }
    }
}
