﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace DiegoG.TelegramBot.Types
{
    public sealed class BotCommandList : IEnumerable<IBotCommand>
    {
        private readonly Dictionary<string, IBotCommand> dict = new();
        private readonly static List<BotCommand> BotCommands = new();

        public IEnumerable<BotCommand> AvailableCommands => BotCommands;

        public int Count { get; private set; }
        public IBotCommand this[string commandName]
            => HasCommand(commandName)
               ? dict[commandName] ?? throw new InvalidBotCommandException(commandName, "does not exist")
               : throw new InvalidBotCommandException(commandName, "does not exist");

        internal void Add(IBotCommand cmd)
        {
            if (!cmd.Validate(out var msg))
                throw new InvalidOperationException($"Unable to load command {cmd.Trigger}: {msg}");
            Count++;
            var trigger = cmd.Trigger.ToLower();
            ThrowIfDuplicateOrInvalid(trigger);
            dict.Add(trigger, cmd);

            var tn = trigger.StartsWith('/') ? trigger[1..] : trigger;

            BotCommands.Add(new()
            {
                Command = tn,
                Description = @$"{cmd.HelpUsage} - {cmd.HelpExplanation}"
            });

            if (cmd.Alias is not null)
            {
                var alias = cmd.Alias.ToLower();
                ThrowIfDuplicateOrInvalid(alias);
                dict.Add(alias, cmd); 
                BotCommands.Add(new()
                {
                    Command = alias.StartsWith('/') ? alias[1..] : alias,
                    Description = $"Alias of {tn}"
                });
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
