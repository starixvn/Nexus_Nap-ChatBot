namespace Fanex.NCB.BotService.Core.Services.Interfaces
{
    using System.Threading.Tasks;
    using Fanex.NCB.BotService.Core.Models;

    public interface IMessageLogService
    {
        Task<bool> IsExistedOnCacheOrExceptionList(string conversationId, string message);

        Task InserMessageLogInRedisCache(ConversationMessageLog messageLog);
    }
}