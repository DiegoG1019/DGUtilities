using DiegoG.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace DiegoG.TelegramBot.Types
{
    public sealed record BotCommandArguments
    {
        public string ArgString { get; init; }
        public User User { get; init; }
        public string[] Arguments { get; init; }

        public BotCommandArguments(string argString, User user)
        {
            ArgString = argString;
            User = user;
            Arguments = BotCommandProcessor.SeparateArgs(argString);
        }

        public override string ToString()
            => $"Command {ArgString} sent by User {User}";
    }
}
