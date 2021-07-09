using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading;
using DiegoG.TelegramBot.Types;
using DiegoG.Utilities;
using System.Collections.Concurrent;
using Telegram.Bot;
using Serilog;

namespace DiegoG.TelegramBot
{
    public class MessageQueue
    {
        public enum MessageSinkStatus
        {
            Inactive,
            Active,
            Stopping,
            Stopped,
            ForceStopping,
            ForceStopped
        }

        public delegate Task BotAction(TelegramBotClient bot);

        public MessageSinkStatus QueueStatus { get; private set; } = MessageSinkStatus.Inactive;

        private ConcurrentQueue<BotAction> BotActionQueue { get; set; }

        private Thread SenderThread;

        public void EnqueueAction(BotAction action)
            => BotActionQueue.Enqueue(action);

        public void Stop()
            => QueueStatus = MessageSinkStatus.Stopping;

        public void ForceStop()
            => QueueStatus = MessageSinkStatus.ForceStopping;

        public TelegramBotClient BotClient { get; private set; }
        public MessageQueue(TelegramBotClient client)
        {
            BotClient = client;
            BotActionQueue = new();
            SenderThread = new(Sender);
            SenderThread.Start();
            QueueStatus = MessageSinkStatus.Active;
        }

        const int StandardWait = 1000;
        const int FailureWait = 60_000;
        int Wait___ = StandardWait;
        int Wait
        {
            get
            {
                var t = Wait___;
                Wait___ = StandardWait;
                return t;
            }
            set => Wait___ = value;
        }

        private readonly ConcurrentQueue<DateTime> Requests = new();

        private readonly TimeSpan TM_ = TimeSpan.FromMinutes(1);
        private ref readonly TimeSpan OneMinute => ref TM_;

        private async void Sender()
        {
            AsyncTaskManager tasks = new();

            while(QueueStatus is MessageSinkStatus.Active)
            {
                if (CheckForceStopping())
                    return;

                Thread.Sleep(Wait);

                while(!Requests.IsEmpty)
                {
                    if (CheckForceStopping())
                        return;

                    var start = Requests.Count;

                    if (Requests.TryPeek(out var x) && DateTime.Now - x >= OneMinute)
                    {
                        Requests.TryDequeue(out _);
                        continue;
                    }

                    var now = Requests.Count;
                    if (start != now)
                        Log.Verbose($"{start - now} requests cooled down, {now} still hot");

                    break;
                }

                try
                {
                    while (Requests.Count < 20 && BotActionQueue.TryDequeue(out var action))
                    {
                        if (CheckForceStopping())
                            return;

                        Requests.Enqueue(DateTime.Now);
                        tasks.Run(() => action(BotClient));
                    }
                    if(tasks.Count > 0)
                        Log.Verbose($"Fired {tasks.Count} new requests");
                    await tasks;
                }
                catch
                {
                    Log.Error("An error ocurred while executing the queued actions, some data may have been lost");
                    Wait = FailureWait;
                }
            }

            if (QueueStatus is not MessageSinkStatus.ForceStopped)
                QueueStatus = MessageSinkStatus.Stopped;

            bool CheckForceStopping()
            {
                if (QueueStatus is MessageSinkStatus.ForceStopping)
                {
                    Log.Information("BotActionQueue was forcefully stopped");
                    QueueStatus = MessageSinkStatus.ForceStopped;
                    return true;
                }
                return false;
            }
        }
    }
}
