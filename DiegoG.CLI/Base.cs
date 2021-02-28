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
using System.Reflection;

namespace DiegoG.CLI
{
    public sealed record CommandProcessorSettings
    (bool EnableHelpCmd = true, bool EnablePrintCmd = true, bool EnableHelloWorldCmd = true, bool EnableChainTestCmd = true, bool EnableConcatenator = true, bool SingleCharFlags = true)
    { }

    public sealed record CommandArguments
    (string[] Arguments, string[] Flags, string[] Options, string[] Original)
    { }

    public interface ICommand
    {
        /// <summary>
        /// The action to be taken when the command is invoked
        /// </summary>
        /// <param name="args">The argument list given, index 0 is NOT the executable's name</param>
        /// <returns>The return value of the command. Consider returning strings that can be used as arguments by other commands. If the method cannot be made async, consider returning Task.FromResult(YourResult)</returns>
        Task<string> Action(CommandArguments args);
        /// <summary>
        /// Explains the purpose and effects of the command
        /// </summary>
        string HelpExplanation { get; }
        /// <summary>
        /// Explains the usage and syntax of the command
        /// </summary>
        string HelpUsage { get; }
        /// <summary>
        /// Provides detailed information of each option setting. Set to null to ignore
        /// </summary>
        IEnumerable<(string Option, string Explanation)> HelpOptions { get; }
        /// <summary>
        /// Defines the trigger of the command (Case Insensitive)
        /// </summary>
        string Trigger { get; }
        /// <summary>
        /// An alternate, usually shortened way to call the command. Set to null to ignore, can not be duplicate with any of the aliases or triggers
        /// </summary>
        string Alias { get; }
        /// <summary>
        /// Used to clear any data that might be stored in the command, be it instance or static
        /// </summary>
        protected void ClearData();
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class CLICommandAttribute : Attribute
    { }

    public sealed class CommandList : IEnumerable<ICommand>
    {
        private readonly Dictionary<string, ICommand> dict = new Dictionary<string, ICommand>();

        public int Count { get; private set; }
        public ICommand this[string commandName] 
            => HasCommand(commandName)
                    ? dict[commandName] ?? throw new InvalidCommandException($"Command '{commandName}' does not exist.")
                    : throw new InvalidCommandException($"Command '{commandName}' does not exist.");
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
        public static bool EnableConcatenator { get; private set; } = true;
        /// <summary>
        /// If true, -dg will be taken as -d and -g
        /// </summary>
        public static bool SingleCharFlags { get; private set; } = true;
        public static Version Version { get; } = new("DiegoG.CLI", 1, 0, 0, 0);

        public static IEnumerable<string> SplitCommandLine(string commandLine)
        {
            bool inQuotes = false;
            commandLine = commandLine.Trim();
            if (commandLine.StartsWith(ModuleFileName))
                commandLine = commandLine.Remove(0, ModuleFileName.Length - 1);
            return commandLine.Split(c =>
            {
                if (c == '\"')
                    inQuotes = !inQuotes;
                return !inQuotes && c == ' ';
            }).Select(arg => arg.Trim().TrimMatchingQuotes('\"')).Where(arg => !string.IsNullOrEmpty(arg));
        }

        public static CommandArguments FullSplit(string args) => FullSplit(SplitCommandLine(args).ToArray());
        public static CommandArguments FullSplit(string[] args)
            => new(CleanFromOptions(args), GetFlags(args), GetOptions(args), args);

        public static string[] CleanFromOptions(string[] args)
            => args.Where(s => !s.StartsWith("-")).ToArray();

        public static string[] GetFlags_SingleChar(string[] args)
            => ((IEnumerable<IEnumerable<char>>)args.Where(s => s.StartsWith('-') && !s.StartsWith("--"))).SelectMany(c => c).Where(ch => ch != '-').Select(c => new string(c, 1)).ToArray();

        public static string[] GetFlags_String(string[] args)
            => args.Where(s => s.StartsWith('-') && !s.StartsWith("--")).Select(s => s[1..]).ToArray();

        public static string[] GetFlags(string[] args)
            => SingleCharFlags ? GetFlags_SingleChar(args) : GetFlags_String(args).ToArray();

        public static string[] GetOptions(string[] args)
            => args.Where(s => s.StartsWith("--") && !s.StartsWith("---") && s.Length > 2).Select(s => s[2..]).ToArray();

        public const string CommandConcatenator = "|";
        public static readonly string ModuleFileName = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;

        public class CommandCallEventArgs : EventArgs
        {
            public CommandArguments Args { get; init; }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="Args">This array will be COPIED, not reference passed</param>
            public CommandCallEventArgs(CommandArguments args) => Args = args;
        }

        public static event EventHandler<CommandCallEventArgs> CommandCalled;

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
        public static async Task<string> Call(string args) => await Call(SplitCommandLine(args).ToArray());
        
        /// <summary>
        /// Calls the first element of Arguments and passes args as is
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task<string> Call(CommandArguments args)
            => IsInit ? args is null ?
                throw new ArgumentNullException(nameof(args)) :
                await Task.Run(
                async () =>
                {
                    Log.Debug($"Command.Call({args.Original.Flatten()})");
                    return await ProcessCommand(args.Arguments.FirstOrDefault(), args);
                }
            ) : throw new InvalidOperationException("Cannot utilize this class without calling Commands.Initialize(CommandProcessorSettings settings) first");

        /// <summary>
        /// Calls the first element of Arguments and passes args as is, appending the return value of each command as the last element of Arguments. Ignores EnableConcatenator. It is recommended to leave Arguments empty (but not null).
        /// </summary>
        /// <param name="argslist"></param>
        /// <returns></returns>
        public static async Task<string> Call(List<CommandArguments> argslist)
            => IsInit ? argslist is null ?
                throw new ArgumentNullException(nameof(argslist)) :
                await Task.Run(
                async () =>
                {
                    string tail = null;
                    argslist.Reverse();
                    foreach (var args in argslist)
                    {
                        if (!string.IsNullOrEmpty(tail))
                            args.Arguments.ToList().Add(tail);
                        Log.Debug($"Command.Call({args.Original.Flatten()})");
                        tail = await ProcessCommand(args.Arguments.FirstOrDefault(), args);
                    }
                    return tail;
                }
            ) : throw new InvalidOperationException("Cannot utilize this class without calling Commands.Initialize(CommandProcessorSettings settings) first");

        /// <summary>
        /// Calls the first element as a command, and all other elements as its arguments. Commands can be concatenated using ">>". If the first element equal's the program's executable name, it is omitted. This method will, invariably queue into the Threadpool, even if the specific command doesn't. If the command, does, in fact, call Task.Run, they'll become nested Tasks
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <exception cref="InvalidCommandException">Throws when a given command is invalid, or an unspecified Exception is thrown.</exception>
        /// <exception cref="InvalidCommandArgumentException">Throws when the arguments to a given command are invalid</exception>
        public static async Task<string> Call(params string[] args)
            => IsInit ? args is null ?
                throw new ArgumentNullException(nameof(args)) :
                await Task.Run(
                async () =>
                {
                    if (args[0] == ModuleFileName)
                        args = args[1..^0];
                    args = args.ToLower().ToArray();
                    Log.Debug($"Command.Call({args.Flatten()})");
                    if (!EnableConcatenator)
                        return await ProcessCommand(args.FirstOrDefault(), FullSplit(args));
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
                        r = await ProcessCommand(s, FullSplit(liarr));
                    }
                    return r;
                }
            ) : throw new InvalidOperationException("Cannot utilize this class without calling Commands.Initialize(CommandProcessorSettings settings) first");

        private static async Task<string> ProcessCommand(string s, CommandArguments liarr)
        {
            string r = "";
            try
            {
                r = await CommandList[s].Action(liarr);
            }
            catch (InvalidCommandException e) { Log.Error(e, liarr.Original.Flatten()); throw; }
            catch (InvalidCommandArgumentException e) { Log.Error(e, liarr.Original.Flatten()); throw; }
            catch (CommandProcessException e) { Log.Error(e, liarr.Original.Flatten()); throw; }
            catch (Exception e)
            {
                var ex = new InvalidCommandException($"The command {liarr.Original.Flatten()} threw an unspecified exception.", e);
                Log.Fatal(e, liarr.Original.Flatten());
                throw ex;
            }
            CommandCalled?.Invoke(null, new(liarr));
            return r;
        }

        private static bool IsInit = false;
        public static void Initialize(CommandProcessorSettings settings)
        {
            if (IsInit)
                throw new InvalidOperationException("Cannot Initialize Twice");
            Log.Verbose("DiegoG.CLI Sweeping through current assembly in a quest for commands");
            LoadCommands();

            Log.Verbose($"DiegoG.CLI Enabling default commands: Help {settings.EnableHelpCmd} | Print {settings.EnablePrintCmd} | HelloWorld {settings.EnableHelloWorldCmd} | ChainTest {settings.EnableChainTestCmd}");
            var (help, print, hw, chain, concat, scf) = settings;
            if (help) CommandList.Add(new CLICommands.Help());
            if (print) CommandList.Add(new CLICommands.Print());
            if (hw) CommandList.Add(new CLICommands.HelloWorld());
            if (chain)
            {
                CommandList.Add(new CLICommands.ChainLinkACommand());
                CommandList.Add(new CLICommands.ChainLinkBCommand());
                CommandList.Add(new CLICommands.ChainLinkCCommand());
            }
            EnableConcatenator = concat;
            SingleCharFlags = scf;
            IsInit = true;
        }

        /// <summary>
        /// USE ONLY FOR LOADING COMMANDS FROM DYNAMICALLY LOADED ASSEMBLIES (i.e. Assembly.Load()). Meant only for extensions
        /// It's also preferrable to NOT call this, and instead load all extensions before loading commands
        /// </summary>
        /// <param name="assemblies"></param>
        public static void LoadCommands(Assembly[] assemblies)
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
        private static void LoadCommands()
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