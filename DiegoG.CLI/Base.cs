using System;
using System.Linq;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using DiegoG.Utilities.Collections;
using System.Collections.Concurrent;
using DiegoG.Utilities;
using System.IO;
using System.Runtime.InteropServices;

namespace DiegoG.CLI
{
    public class InvalidCommandException : Exception
    {
        public InvalidCommandException(string message) : base(message) { }
    }

    public interface ICommand
    {
        string Action(string[] args);
        string HelpExplanation { get; }
        string HelpUsage { get; }
        string Trigger { get; }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class CLICommandAttribute : Attribute
    { }

    public sealed class CommandList : IEnumerable<ICommand>
    {
        private readonly Dictionary<string, ICommand> dict = new Dictionary<string, ICommand>();
        public ICommand this[string commandName] => dict[commandName];
        public void Add(ICommand cmd) => dict.Add(cmd.Trigger, cmd);
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

        public const string CommandSeparator = "||";
        public static readonly string ExeName = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;

        static Commands()
        {
            try
            {
                foreach (var ty in ReflectionCollectionMethods.GetAllTypesWithAttribute(typeof(CLICommandAttribute), false))
                    CommandList.Add(Activator.CreateInstance(ty) as ICommand);
            }catch(Exception e)
            {
                throw new TypeLoadException($"All classes attributed with CLICommandAttribute must not be generic, abstract, or static, must have a parameterless constructor, and must implement ICommand directly or indirectly. CLICommandAttribute is not inheritable. Check inner exception for more details.", e);
            }
        }
        public static CommandList CommandList { get; } = new();

        private static readonly ConcurrentDataType<int> commandsleft = new ConcurrentDataType<int>();
        private static readonly ConcurrentQueue<string[]> commandbuffer = new ConcurrentQueue<string[]>();
        private static readonly List<string> commandresults = new List<string>();

        private static void ClearCommandBuffer()
        {
            while (true)
            {
                if(commandbuffer.TryDequeue(out string[] args))
                {
                    if (CommandList.HasCommand(args[0]))
                        commandresults.Add(CommandList[args[0]].Action(args));
                    else
                        throw new InvalidCommandException($"Unrecognized command: {args[0]}");
                    commandsleft.Data--;
                    continue;
                }
                if (commandsleft.Data <= 0)
                    return;
            }
        }

        public static async Task<List<string>> CallFromFile(string filepath) => await Call(File.ReadAllText(filepath));
        public static async Task<List<string>> Call(string args) => await Call(ExeName + CommandLineToArgs(args));
        public static async Task<List<string>> Call(string[] args)
        {
            commandresults.Clear();
            if (args.Length <= 0)
                return commandresults;
            commandsleft.Data = args.Select(e => e == CommandSeparator).Count();

            var cmdbuffer = Task.Run(ClearCommandBuffer);

            RestartFor:;
            for (int i = 0; i < args.Length; i++) 
            {
                args[i] = args[i].ToLower();
                if (args[i] == CommandSeparator)
                {
                    var (oe1, e2) = args.SplitAtIndex(i);
                    var e1 = oe1.ToList();
                    e1.RemoveAt(e1.Count - 1);
                    commandbuffer.Enqueue(e1.ToArray());

                    args = e2.ToArray();
                    goto RestartFor;
                }
            }
            await cmdbuffer;
            if (cmdbuffer.IsFaulted)
                throw cmdbuffer.Exception;
            return commandresults;
        }
        public static string GetHelp(string cmd)
        {
            var c = CommandList[cmd];
            return $" > {c.Trigger} {c.HelpUsage} | {c.HelpExplanation}";
        }
    }

}