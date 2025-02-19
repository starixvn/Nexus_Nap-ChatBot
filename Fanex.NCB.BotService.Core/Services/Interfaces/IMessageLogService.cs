namespace Fanex.NCB.BotService.Core.Services.Interfaces;

using System.Threading.Tasks;

public interface IMessageLogService
{
    bool IsInvalidSendMessageToBotService(string conversationId, string message);

    Task InsertMessageLogIntoCache(string conversationId, string messageContent, string guiid);

    Task RemoveMessageLogOnCache(string conversationId, string guid);
}