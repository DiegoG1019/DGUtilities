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
    (string[] Arguments, User User)
    {
        public override string ToString()
            => $"Command {Arguments.Flatten()} sent by User {User}";
    }
}
