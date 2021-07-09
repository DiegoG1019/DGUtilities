using DiegoG.TelegramBot.Types;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DiegoG.TelegramBot.Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var bot = new TelegramBotClient("");
            var proc = new BotCommandProcessor(bot);

            while(true)
                await Task.Delay(500);
        }
    }

    [BotCommand]
    class DefaultTest : Help
    {
        public override Task<(string, bool)> Action(BotCommandArguments args)
        {
            throw new NotImplementedException();
        }

        public override Task<(string Result, bool Hold)> ActionReply(BotCommandArguments args)
        {
            throw new NotImplementedException();
        }

        public override void Cancel(User user)
        {
            throw new NotImplementedException();
        }
    }
}
