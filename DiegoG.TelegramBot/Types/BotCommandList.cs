using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiegoG.TelegramBot.Types
{
    public sealed class BotCommandList : IEnumerable<IBotCommand>
    {
        private readonly Dictionary<string, IBotCommand> dict = new();

        public int Count { get; private set; }
        public IBotCommand this[string commandName]
            => HasCommand(commandName)
               ? dict[commandName] ?? throw new InvalidBotCommandException(commandName, "does not exist")
               : throw new InvalidBotCommandException(commandName, "does not exist");

        internal void Add(IBotCommand cmd)
        {
            Count++;
            var trigger = cmd.Trigger.ToLower();
            ThrowIfDuplicateOrInvalid(trigger);
            dict.Add(trigger, cmd);
            if (cmd.Alias is not null)
            {
                var alias = cmd.Alias.ToLower();
                ThrowIfDuplicateOrInvalid(alias);
                dict.Add(alias, cmd);
            }
        }

        public bool HasCommand(string cmd) => dict.ContainsKey(cmd);

        public IEnumerator<IBotCommand> GetEnumerator()
        {
            foreach (var cmd in dict.Values)
                yield return cmd;
        }

        private void ThrowIfDuplicateOrInvalid(string cmd)
        {
            if (HasCommand(cmd))
                throw new InvalidOperationException($"Duplicate command detected: {cmd}. Commands, Command Aliases, or General Aliases must be unique from one another and themselves");
            if (cmd.Any(char.IsWhiteSpace))
                throw new InvalidBotCommandException(cmd, "Command triggers or aliases cannot contain whitespace");
        }

        internal BotCommandList() { }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

}
