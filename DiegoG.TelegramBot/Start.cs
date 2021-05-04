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

        public Task<(string, bool)> Action(BotCommandArguments args) => Task.FromResult(("Hello! Welcome! Please type /help", false));

        public Task<(string Result, bool Hold)> ActionReply(BotCommandArguments args)
        {
            throw new NotImplementedException();
        }

        public void Cancel(User user)
        {
            throw new NotImplementedException();
        }
    }
}
