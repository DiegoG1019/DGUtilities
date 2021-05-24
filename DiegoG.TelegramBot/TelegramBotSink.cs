using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DiegoG.Utilities;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;
using DiegoG.TelegramBot;
using DiegoG.Utilities.Settings;

namespace DiegoG.TelegramBot
{
    public static class TelegramBotSinkMethods
    {
        /// <summary>
        /// Due to Telegram's Rate Limiting, it is not recommended to sink anything below `Information` events
        /// </summary>
        public static LoggerConfiguration TelegramBot(this LoggerSinkConfiguration loggerConfiguration, ChatId chatid, TelegramBotClient botclient, LogEventLevel level = LogEventLevel.Information, string? hashtag = null)
            => loggerConfiguration.Sink(new TelegramBotSink(chatid, botclient, level, hashtag));
    }

    /// <summary>
    /// Due to Telegram's Rate Limiting, it is not recommended to sink anything below `Information` events
    /// </summary>
    public class TelegramBotSink : ILogEventSink
    {
        private readonly TelegramBotClient Client;
        private readonly ChatId Id;
        private readonly ConcurrentQueue<string> MessageQueue = new();
        private readonly Thread DequeueThread;
        private readonly CancellationTokenSource TokenSource = new();
        private readonly LogEventLevel Level;
        private readonly string? Hashtag;
        private bool AllowNewMessages = true;

        public TelegramBotSink(ChatId id, TelegramBotClient client, LogEventLevel level, string? hashtag)
        {
            Id = id;
            Client = client;
            Level = level;
            if(hashtag is not null)
                Hashtag = string.Join("", hashtag.Split(' '));
            DequeueThread = new(async o =>
            {
                CancellationToken token = (CancellationToken)o!;
                AsyncTaskManager tasks = new();
                while (!token.IsCancellationRequested)
                {
                    Thread.Sleep(60000);
                    for (int i = 0; i < 10 && !MessageQueue.IsEmpty; i++)
                    {
                        if(MessageQueue.TryDequeue(out var msg))
                            tasks.Run(async () =>
                            {
                                try
                                {
                                    await Client.SendTextMessageAsync(Id, msg, Telegram.Bot.Types.Enums.ParseMode.MarkdownV2, false, true);
                                }
                                catch (Exception e)
                                {
                                    const string Delayed = " !!delayed";
                                    int d = 1;
                                    if (msg.Contains(Delayed))
                                    {
                                        var match = Regex.Match(msg, Delayed + @"\d+");
                                        if (match.Success && int.TryParse(match.Value.Replace(Delayed, ""), out d))
                                        {
                                            MessageQueue.Enqueue(Regex.Replace(msg, Delayed + @"\d+", Delayed + (d + 1)));
                                            return;
                                        }
                                        MessageQueue.Enqueue(msg + d);
                                        return;
                                    }
                                    MessageQueue.Enqueue(msg + Delayed);
                                    return;
                                }
                            });
                    }
                    await tasks;
                }
            });
            DequeueThread.Start(TokenSource.Token);
            AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                TokenSource.Cancel();

                AllowNewMessages = false;
                AsyncTaskManager tasks = new();
                try
                {
                    while (!MessageQueue.IsEmpty)
                    {
                        Thread.Sleep(60000);
                        for (int i = 0; i < 15 && !MessageQueue.IsEmpty; i++)
                            if(MessageQueue.TryDequeue(out var msg))
                                tasks.Add(Client.SendTextMessageAsync(Id, msg, Telegram.Bot.Types.Enums.ParseMode.MarkdownV2, false, true));
                        tasks.WaitAll();
                    }
                }catch(Exception)
                {
                    return;
                }
            };
        }

        public void Emit(LogEvent logEvent)
		{
            if (AllowNewMessages && logEvent.Level >= Level)
                MessageQueue.Enqueue($"{(Hashtag is not null ? $"#{Hashtag}\n" : "")}*[{logEvent.Level}]:* {logEvent.RenderMessage()}\n\\{{{DateTime.Now:MM/dd/yyyy hh:mm:ss tt zzz}\\}}".TelegramLegalizeMarkupV2());
	    }
    }
}
