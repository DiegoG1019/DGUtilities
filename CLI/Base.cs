using System;
using System.Linq;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using DiegoG.Utilities.Collections;
using System.Collections.Concurrent;
using DiegoG.Utilities;
using System.IO;

namespace DiegoG.CLI
{
    public class InvalidCommandException : Exception
    {
        public InvalidCommandException(string message) : base(message) { }
    }

    public interface Cmd
    {
        string Action(string[] args);
        string HelpExplanation { get; }
        string HelpUsage { get; }
        string Trigger { get; }
    }

    public sealed class CommandList : IEnumerable<Cmd>
    {
        private Dictionary<string, Cmd> dict = new Dictionary<string, Cmd>();
        public Cmd this[string commandName] => dict[commandName];
        public void Add(Cmd cmd) => dict.Add(cmd.Trigger, cmd);
        public bool HasCommand(string cmd) => dict.ContainsKey(cmd);

        public IEnumerator<Cmd> GetEnumerator()
        {
            foreach (var cmd in dict.Values)
                yield return cmd;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public static class Commands
    {
        public const string CommandSeparator = "||";

        static Commands()
        {
            CommandList = new CommandList()
            {
                //Pon tus comandos aqui
                new HelloWorld(),
                new Help(),
                new Print()
            };
        }
        public static CommandList CommandList { get; }

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

        public static async Task<List<string>> Call(string filepath) => await Call(File.ReadAllText(filepath).Split(' '));
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