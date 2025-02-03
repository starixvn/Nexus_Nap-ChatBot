namespace Fanex.NCB.BotService.EndPoint.Tests.TestableClasses.Telegram;

using Core.Services;
using Core.Services.Interfaces;
using Fanex.NCB.BotService.EndPoint.Controllers.Telegram;
using Shared.Models;
    using Fanex.NCB.Shared.Services.Interfaces;
using global::Telegram.Bot.Types;
    using System.Threading.Tasks;

public class UpdateControllerTestable : UpdateController
{
    public UpdateControllerTestable(
            ICommonLogService logger,
        ITelegramBotService botService,
        IConversationService conversationService) : base(logger, botService, conversationService)
    {
    }

    public async Task<CommandResult> Public_EditChatTitledAsync(Message message)
    {
        return await base.EditChatTitledAsync(message);
    }

    public async Task<CommandResult> Public_AddBotToGroupAsync(Message message, User bot)
    {
        return await base.AddBotToGroupAsync(message, bot);
    }

    public async Task<CommandResult> Public_RemoveBotToGroupAsync(Message message)
    {
        return await base.RemoveBotToGroupAsync(message);
    }

    public async Task<CommandResult> Public_MigratedToSupergroupAsync(Message message)
    {
        return await base.MigratedToSupergroupAsync(message);
    }
}