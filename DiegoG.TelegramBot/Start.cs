using DiegoG.TelegramBot.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace DiegoG.TelegramBot
{
    public class Start : IBotCommand
    {
        public string HelpExplanation => "Starts the bot";

        public string HelpUsage => "/start";

        public IEnumerable<(string Option, string Explanation)>? HelpOptions => null;

        public string Trigger => "/start";

        public string? Alias => null;

        public BotCommandProcessor Processor { get; set; }

        public virtual Task<(string, bool)> Action(BotCommandArguments args) => Task.FromResult(("Hello! Welcome! Please type /help", false));

        public virtual Task<(string Result, bool Hold)> ActionReply(BotCommandArguments args) => Task.FromResult(("", false));

        public virtual void Cancel(User user)
        {
            return;
        }
    }
}
