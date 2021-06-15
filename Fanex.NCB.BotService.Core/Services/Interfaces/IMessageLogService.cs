namespace Fanex.NCB.BotService.Core.Services.Interfaces
{
    using System.Threading.Tasks;

    public interface IMessageLogService
    {
        Task<bool> IsInvalidSendMessageToBotService(string conversationId, string message);

        Task InserMessageLogIntoCache(string conversationId, string messageContent, string guiid);

        Task RemoveMessageLogOnCache(string conversationId, string guid);
    }
}