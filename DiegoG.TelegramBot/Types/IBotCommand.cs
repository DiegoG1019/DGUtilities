using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace DiegoG.TelegramBot.Types
{
    
    public interface IBotCommand
    {
        /// <summary>
        /// The action to be taken when the command is invoked. Please be aware that the engine will try to remove the slash
        /// </summary>
        /// <returns>The return value of the command. If the method cannot be made async, consider returning Task.FromResult(YourResult)</returns>
        Task<(string Result, bool Hold)> Action(BotCommandArguments args);

        /// <summary>
        /// While Hold is set to not null, the command processor will call ActionReply whenever another message is sent by the users
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        Task<(string Result, bool Hold)> ActionReply(BotCommandArguments args);

        /// <summary>
        /// Cancels the currently ongoing ActionReply executed by the command
        /// </summary>
        void Cancel(User user);

        /// <summary>
        /// Explains the purpose and effects of the command
        /// </summary>
        string HelpExplanation { get; }
        /// <summary>
        /// Explains the usage and syntax of the command - CommandName [Argument] (OptionalArgument)
        /// </summary>
        string HelpUsage { get; }
        /// <summary>
        /// Provides detailed information of each option setting. Set to null to ignore
        /// </summary>
        IEnumerable<(string Option, string Explanation)>? HelpOptions { get; }
        /// <summary>
        /// Defines the trigger of the command (Case Insensitive)
        /// </summary>
        string Trigger { get; }
        /// <summary>
        /// An alternate, usually shortened way to call the command. Set to null to ignore, can not be duplicate with any of the aliases or triggers
        /// </summary>
        string? Alias { get; }
    }
}
