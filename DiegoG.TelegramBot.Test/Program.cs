using DiegoG.TelegramBot.Types;
using Serilog;
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
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .CreateLogger();

            var bot = new TelegramBotClient("1787830577:AAENYWvgMaEERt1VFO8SgPxDLcR35d8q5kw");
            var proc = new BotCommandProcessor(bot);

            bot.StartReceiving();

            Log.Information($"Connected to {await bot.GetMeAsync()}");

            while(true)
                await Task.Delay(500);
        }
    }

    [BotCommand]
    class DefaultTest : Help
    {
        public override Task<(string, bool)> Action(BotCommandArguments args)
        {
            return Task.FromResult(("a", false));
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
