namespace Fanex.NCB.BotService.Core.Services
{
    using Fanex.NCB.Shared.Models;
    using System.Threading.Tasks;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    public interface ITelegramBotService
    {
        Task SetWebhookAsync();

        Task<User> GetBotInfoAsync(long botId);

        Task<Message> SendTextMessageAsync(long botId, long chatId, string messageContent, ParseMode parseMode = ParseMode.MarkdownV2);

        Task<SendMessageResult> SendTextMessageAsync(ConversationInfo conversationInfo);

        Task<SendMessageResult> SendMessageWithImagesAsync(ConversationInfo conversationInfo);

        Task<SendMessageResult> ResendImageOnlyAsync(ConversationInfo conversationInfo);
    }
}