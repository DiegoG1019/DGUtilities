using DiegoG.Utilities;
using DiegoG.Utilities.Collections;
using Serilog;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Version = DiegoG.Utilities.Version;
using System.Threading.Tasks;

namespace DiegoG.CLI
{
    public interface ICommand
    {
        /// <summary>
        /// The action to be taken when the command is invoked
        /// </summary>
        /// <param name="args">The argument list given, index 0 is NOT the executable's name</param>
        /// <returns>The return value of the command. Consider returning strings that can be used as arguments by other commands. If the method cannot be made async, consider returning Task.FromResult(YourResult)</returns>
        Task<string> Action(string[] args);
        /// <summary>
        /// Explains the purpose and effects of the command
        /// </summary>
        string HelpExplanation { get; }
        /// <summary>
        /// Explains the usage and syntax of the command
        /// </summary>
        string HelpUsage { get; }
        /// <summary>
        /// Defines the trigger of the command (Case Insensitive)
        /// </summary>
        string Trigger { get; }
        /// <summary>
        /// An alternate, usually shortened way to call the command. Set to null to ignore, can not be duplicate with any of the aliases or triggers
        /// </summary>
        string Alias { get; }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class CLICommandAttribute : Attribute
    { }

    public sealed class CommandList : IEnumerable<ICommand>
    {
        private readonly Dictionary<string, ICommand> dict = new Dictionary<string, ICommand>();

        public int Count { get; private set; }
        public ICommand this[string commandName]
        {
            get
            {
                if(HasCommand(commandName))
                    return dict[commandName] ?? throw new InvalidCommandException($"Command '{commandName}' does not exist.");
                throw new InvalidCommandException($"Command '{commandName}' does not exist.");
            }
        }
        internal void Add(ICommand cmd)
        {
            Count++;
            dict.Add(cmd.Trigger.ToLower(), cmd);
            if (cmd.Alias is not null)
                dict.Add(cmd.Alias.ToLower(), cmd);
        }
        public bool HasCommand(string cmd) => dict.ContainsKey(cmd);

        public IEnumerator<ICommand> GetEnumerator()
        {
            foreach (var cmd in dict.Values)
                yield return cmd;
        }

        internal CommandList() { }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public static class Commands
    {
        public static Version Version { get; } = new("DiegoG.CLI", 0,0,0,0);

        [DllImport("shell32.dll", SetLastError = true)]
        static extern IntPtr CommandLineToArgvW([MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, out int pNumArgs);

        public static string[] CommandLineToArgs(string commandLine)
        {
            var argv = CommandLineToArgvW(commandLine, out int argc);
            if (argv == IntPtr.Zero)
                throw new System.ComponentModel.Win32Exception();
            try
            {
                var args = new string[argc];
                for (var i = 0; i < args.Length; i++)
                {
                    var p = Marshal.ReadIntPtr(argv, i * IntPtr.Size);
                    args[i] = Marshal.PtrToStringUni(p);
                }

                return args;
            }
            finally
            {
                Marshal.FreeHGlobal(argv);
            }
        }

        public const string CommandConcatenator = ">>";
        public static readonly string ExeName = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;

        public class CommandCallEventArgs : EventArgs
        {
            public string[] Args { get; init; }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="Args">This array will be COPIED, not reference passed</param>
            public CommandCallEventArgs(string[] args)
            {
                Args = new string[args.Length];
                args.CopyTo(Args, 0);
            }
        }

        public static event EventHandler<CommandCallEventArgs> CommandCalled;

        static Commands()
        {
            try
            {
                foreach (var ty in ReflectionCollectionMethods.GetAllTypesWithAttribute(typeof(CLICommandAttribute), false))
                    CommandList.Add(Activator.CreateInstance(ty) as ICommand);
            }
            catch (Exception e)
            {
                throw new TypeLoadException($"All classes attributed with CLICommandAttribute must not be generic, abstract, or static, must have a parameterless constructor, and must implement ICommand directly or indirectly. CLICommandAttribute is not inheritable. Check inner exception for more details.", e);
            }
        }
        public static CommandList CommandList { get; } = new();

        /// <summary>
        /// Loads the file as a string, and calls the first element as a command, and all other elements as its arguments. Commands can be concatenated using ">>". If the first element equal's the program's executable name, it is omitted.This command appends the ExeName to the start, as it assumes it is not present in the file.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <exception cref="InvalidCommandException">Throws when a given command is invalid, or an unspecified Exception is thrown.</exception>
        /// <exception cref="InvalidCommandArgumentException">Throws when the arguments to a given command are invalid</exception>
        public static async Task<string> CallFromFile(string filepath) => await Call(File.ReadAllText(filepath));
        /// <summary>
        /// Calls the first element as a command, and all other elements as its arguments. Commands can be concatenated using ">>". If the first element equal's the program's executable name, it is omitted. This command appends the ExeName to the start, as it assumes it is not present.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <exception cref="InvalidCommandException">Throws when a given command is invalid, or an unspecified Exception is thrown.</exception>
        /// <exception cref="InvalidCommandArgumentException">Throws when the arguments to a given command are invalid</exception>
        public static async Task<string> Call(string args) => await Call(CommandLineToArgs($"\"{ExeName}\""+args));

        /// <summary>
        /// Calls the first element as a command, and all other elements as its arguments. Commands can be concatenated using ">>". If the first element equal's the program's executable name, it is omitted.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <exception cref="InvalidCommandException">Throws when a given command is invalid, or an unspecified Exception is thrown.</exception>
        /// <exception cref="InvalidCommandArgumentException">Throws when the arguments to a given command are invalid</exception>
        public static async Task<string> Call(params string[] args)
        {
            if (args is null)
                throw new ArgumentNullException(nameof(args));
            if(args[0] == ExeName)
                args = args[1..^0];
            args = args.ToLower().ToArray();
            Log.Debug($"Command.Call({args.Flatten()})");
            return await Task.Run
            (
                async () =>
                {
                    string r = null;
                    foreach (var i in args.Split(CommandConcatenator).Reverse())
                    {
                        var s = i.FirstOrDefault();
                        if (s is null || s == string.Empty)
                            continue;
                        var li = i.ToList();
                        if (r is not null)
                            li.Add(r);
                        var liarr = li.ToArray();
                        try
                        {
                            r = await CommandList[s].Action(liarr);
                        }
                        catch(InvalidCommandException e) { Log.Error(e, li.Flatten());  throw; }
                        catch(InvalidCommandArgumentException e) { Log.Error(e, li.Flatten()); throw; }
                        catch(CommandProcessException e) { Log.Error(e, li.Flatten()); throw; }
                        catch(Exception e) 
                        {
                            var ex = new InvalidCommandException($"The command {li.Flatten()} threw an unspecified exception.", e);
                            Log.Fatal(e, li.Flatten());
                            throw ex;
                        }
                        CommandCalled?.Invoke(null, new(liarr));
                    }
                    return r;
                }
            );
        }

        /// <summary>
        /// Parses a yes/no input
        /// </summary>
        /// <param name="n">The string to be tested</param>
        /// <param name="boolstandard">Whether to try for true/false as well</param>
        /// <param name="forgiving">Whether to return false if it isn't true or Y</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Thrown when the argument cannot be parsed as a boolean</exception>
        public static bool ParseYesNo(string n, bool boolstandard = true, bool forgiving = false, bool caseSensitive = false)
        {
            if (!caseSensitive)
                n = n.ToLower();
            if (boolstandard && bool.TryParse(n, out bool res))
                return res;
            if (n == (caseSensitive ? "Y" : "y"))
                return true;
            if (forgiving || n == (caseSensitive ? "N" : "n"))
                return false;
            throw new ArgumentException($"The given string is unparsable: {n}");
        }
        /// <summary>
        /// Parses a yes/no input
        /// </summary>
        /// <param name="n">The string to be tested</param>
        /// <param name="boolstandard">Whether to try for true/false as well</param>
        /// <param name="forgiving">Whether to return false if it isn't true or Y</param>
        public static bool TryParseYesNo
            (string n, [MaybeNullWhen(false)] out bool result, bool boolstandard = true, bool forgiving = false, bool caseSensitive = false)
        {
            if (!caseSensitive)
                n = n.ToLower();
            if (boolstandard && bool.TryParse(n, out bool res))
            {
                result = res;
                return true;
            }
            if (n == (caseSensitive ? "Y" : "y"))
            {
                result = true;
                return true;
            }
            if (forgiving || n == (caseSensitive ? "N" : "n"))
            {
                result = false;
                return true;
            }
            result = default!;
            return false;
        }
    }
}