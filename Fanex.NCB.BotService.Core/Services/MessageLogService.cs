namespace Fanex.NCB.BotService.Core.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Caching;
    using Fanex.NCB.BotService.Core.Models;
    using Fanex.NCB.BotService.Core.Services.Interfaces;
    using Newtonsoft.Json;

    public class MessageLogService : IMessageLogService
    {
        private readonly CheckDuplicatedMessageLogSetting settings;
        private readonly ICacheService cacheService;

        public MessageLogService(CheckDuplicatedMessageLogSetting settings, ICacheService cacheService)
        {
            this.settings = settings;
            this.cacheService = cacheService;
        }

        public async Task<bool> IsInvalidSendMessageToBotService(string conversationId, string message)
        {
            if (!settings.IsEnabled)
            {
                return false;
            }

            if (settings.ExceptionConversatiionIDs != null && settings.ExceptionConversatiionIDs.Contains(conversationId))
            {
                return false;
            }

            var messageLogJson = await cacheService.GetAsync<string>(conversationId.Replace(':', '-'));
            var messageLog = string.IsNullOrEmpty(messageLogJson) ? null : JsonConvert.DeserializeObject<ConversationMessageLog>(messageLogJson);

            return messageLog != null && messageLog.Message.Equals(message) && messageLog.IsSuccessful;
        }

        public async Task InserMessageLogIntoCache(ConversationMessageLog messageLog)
        {
            if (settings.IsEnabled)
            {
                await cacheService.SetAsync(
                messageLog.ConversationId.Replace(':', '-'),
                JsonConvert.SerializeObject(messageLog),
                new CacheItemOptions
                {
                    RelativeExpiration = TimeSpan.FromMinutes(settings.TimeSpanInMinutes)
                });
            }
        }
    }
}