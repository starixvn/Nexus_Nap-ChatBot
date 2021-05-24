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

        public async Task<bool> IsExistedOnCacheOrExceptionList(string conversationId, string message)
        {
            bool isExistedOnExceptionList = settings.ExceptionConversatiionIDs != null && settings.ExceptionConversatiionIDs.Contains(conversationId);

            if (isExistedOnExceptionList)
            {
                return isExistedOnExceptionList;
            }

            var messageLogJson = await cacheService.GetAsync<string>(conversationId.Replace(':', '-'));
            var messageLog = string.IsNullOrEmpty(messageLogJson) ? null : JsonConvert.DeserializeObject<ConversationMessageLog>(messageLogJson);

            return messageLog != null && messageLog.Message.Equals(message) && messageLog.IsSuccessful;
        }

        public async Task InserMessageLogInRedisCache(ConversationMessageLog messageLog)
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