using DiegoG.TelegramBot.Types;
using DiegoG.TelegramBot.Types.ChatSequenceTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace DiegoG.TelegramBot
{
    public abstract class ChatBot<TContext> : Default where TContext : IChatSequenceContext, new()
    {
        /// <summary>
        /// The first step to follow. A new Context and ChatSequence will be created for each User, but only one step tree will be shared
        /// </summary>
        public abstract ChatSequenceStep<TContext> FirstStep { get; }
        protected readonly Dictionary<long, ChatSequence<TContext>> ActiveContexts = new();

        public override async Task<(string, bool)> Action(BotCommandArguments args)
        {
            ActiveContexts.Add(args.User.Id, new(FirstStep));
            return (await ActiveContexts[args.User.Id].EnterFirst(), true);
        }

        public override async Task<(string Result, bool Hold)> ActionReply(BotCommandArguments args)
        {
            try
            {
                var seq = ActiveContexts[args.User.Id];
                var r = await seq.Respond(args);

                if (r.Action is Response.ResponseAction.Advance)
                {
                    Processor.MessageQueue.EnqueueAction(b => b.SendTextMessageAsync(args.Message.Chat.Id, r.ResponseValue));
                    var ad = await seq.Advance();
                    return ad.Success is Advancement.SuccessCode.EndOfSequence
                        ? throw new InvalidOperationException("Unexpected End of Sequence")
                        : ad.Success is Advancement.SuccessCode.Failure
                        ? throw new InvalidOperationException("Unable to advance, none of the next possible steps have favorable conditions")
                        : (ad.EnterValue!, true);
                }

                var act = r.Action is Response.ResponseAction.Continue;

                if (!act)
                    Cancel(args.User);

                return (r.ResponseValue, act);
            }
            catch
            {
                Cancel(args.User);
                throw;
            }
        }

        public override void Cancel(User user)
        {
            ActiveContexts.Remove(user.Id);
        }
    }
}
