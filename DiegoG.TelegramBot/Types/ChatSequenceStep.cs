using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiegoG.TelegramBot.Types
{
    public class ChatSequence<TContext>
    {
        public TContext Context { get; init; }
        public ChatSequenceStep<TContext> FirstStep { get; init; } = new();
        public ChatSequenceStep<TContext> CurrentStep { get; private set; } = null;

        public async Task<string> Advance()
        {

        }

        public ChatSequence(TContext context, ChatSequenceStep<TContext> firstStep) : this(context)
            => FirstStep = firstStep;

        public ChatSequence(TContext context)
            => Context = context;
    }

    public class ChatSequenceStep<TContext>
    {
        public string Name { get; init; } = string.Empty;
        public Func<TContext, bool> Condition { get; init; } = t => throw new InvalidOperationException("The Condition for this Step was not defined");
        public Func<TContext, Task<string>> Response { get; init; } = t => throw new InvalidOperationException("The Response for this Step was not defined");
        public IEnumerable<ChatSequenceStep<TContext>> Children { get; init; } = Array.Empty<ChatSequenceStep<TContext>>();

        public ChatSequenceStep() { }

        public ChatSequenceStep(string name, Func<TContext, bool> condition, Func<TContext, Task<string>> response, IOrderedEnumerable<ChatSequenceStep<TContext>> children)
        {
            Name = name;
            Condition = condition;
            Response = response;
            Children = children;
        }
    }
}
