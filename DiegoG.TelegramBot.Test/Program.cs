using DiegoG.TelegramBot.Types;
using DiegoG.Utilities;
using Serilog;
using System;
using System.Collections.Generic;
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
    class AsyncGetTest : IBotCommand
    {
        public BotCommandProcessor Processor { get; set; }

        public string HelpExplanation => "Tests the Get queued requests";

        public string HelpUsage => Trigger;

        public IEnumerable<(string Option, string Explanation)> HelpOptions => null;

        public string Trigger => "/testget";

        public string Alias => null;

        public Task<(string Result, bool Hold)> Action(BotCommandArguments args)
        {
            Processor.MessageQueue.ApiSaturationLimit = 3;
            AsyncTaskManager tasks = new();
            Log.Verbose("Start");
            tasks.Add(Processor.MessageQueue.EnqueueFunc(async b => { Log.Verbose("A1"); await Task.Delay(600); Log.Verbose("A2"); return 0; }));
            tasks.Add(Processor.MessageQueue.EnqueueFunc(async b => { Log.Verbose("B1"); await Task.Delay(600); Log.Verbose("B2"); return 0; }));
            tasks.Add(Processor.MessageQueue.EnqueueFunc(async b => { Log.Verbose("C1"); await Task.Delay(600); Log.Verbose("C2"); return 0; }));
            tasks.Add(Processor.MessageQueue.EnqueueFunc(async b => { Log.Verbose("D1"); await Task.Delay(600); Log.Verbose("D2"); return 0; }));
            tasks.Add(Processor.MessageQueue.EnqueueFunc(async b => { Log.Verbose("E1"); await Task.Delay(600); Log.Verbose("E2"); return 0; }));
            Log.Verbose("End");
            return Task.FromResult(("Done, check the console", false));
        }

        public Task<(string Result, bool Hold)> ActionReply(BotCommandArguments args)
        {
            throw new NotImplementedException();
        }

        public void Cancel(User user)
        {
            throw new NotImplementedException();
        }
    }

    [BotCommand]
    class DefaultTest : Default
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
