namespace Fanex.NCB.BotService.EndPoint.Tests.TestableClasses.Telegram
{
    using Core;
    using Core.Services;
    using Core.Services.Interfaces;
    using Fanex.NCB.BotService.EndPoint.Controllers.Telegram;
    using Shared.Models;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Microsoft.Extensions.Options;
    using System;
    using System.Threading.Tasks;

    public class MessageControllerTestable : MessageController
    {
        public MessageControllerTestable(
            IOptions<AppSettings> appSettings,
            ICommonLogService commonLogService,
            IConversationService conversationService,
            ITelegramBotService botService,
            IMessageLogService messageLogService) : base(appSettings, commonLogService, conversationService, botService, messageLogService)
        {
        }

        public async Task<SendMessageResult> Public_SendMessageAsync(ConversationInfo conversationInfo)
        {
            return await base.SendMessageAsync(conversationInfo);
        }

        public async Task Public_AddMessageLogToDatabaseAsync(ConversationInfo conversationInfo, SendMessageResult result)
        {
            await base.AddMessageLogToDatabaseAsync(conversationInfo, result);
        }

        public async Task Public_WriteLogAsync(Exception exception)
        {
            await base.WriteLogAsync(exception);
        }
    }
}