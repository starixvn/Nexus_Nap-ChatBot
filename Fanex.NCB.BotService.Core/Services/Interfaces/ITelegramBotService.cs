namespace Fanex.NCB.BotService.Core.Services
{
    using System.Threading.Tasks;
    using Fanex.NCB.Shared.Models;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    public interface ITelegramBotService
    {
        Task SetWebhookAsync();

        Task<User> GetBotInfoAsync(long botId);

        Task<Message> SendTextMessageAsync(long botId, long chatId, string messageContent, ParseMode parseMode = ParseMode.MarkdownV2);

        Task<SendMessageResult> SendTextMessageAsync(ConversationInfo conversationInfo);
    }
}