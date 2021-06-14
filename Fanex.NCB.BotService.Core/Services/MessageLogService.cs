namespace Fanex.NCB.BotService.Core.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Caching;
    using Fanex.NCB.BotService.Core.Models;
    using Fanex.NCB.BotService.Core.Services.Interfaces;

    public class MessageLogService : IMessageLogService
    {
        private readonly CheckDuplicatedMessageLogSetting settings;
        private readonly ICacheService cacheService;
        private readonly IRedisCacheExtendService redisCacheExtend;

        public MessageLogService(CheckDuplicatedMessageLogSetting settings,
            ICacheService cacheService,
            IRedisCacheExtendService redisCacheExtend)
        {
            this.settings = settings;
            this.cacheService = cacheService;
            this.redisCacheExtend = redisCacheExtend;
        }

        public bool IsInvalidSendMessageToBotService(string conversationId, string message)
        {
            if (!settings.IsEnabled)
            {
                return false;
            }

            if (settings.ExceptionConversatiionIDs != null && settings.ExceptionConversatiionIDs.Contains(conversationId))
            {
                return false;
            }

            var messageContentList = redisCacheExtend.GetValueByKeyContains<string>(conversationId.Replace(':', '-'));

            bool isInvalid = false;
            foreach (string logContent in messageContentList)
            {
                if (logContent != null && logContent.Equals(message))
                {
                    isInvalid = true;

                    return isInvalid;
                }
            }

            return isInvalid;
        }

        public async Task InserMessageLogIntoCache(ConversationMessageLog messageLog)
        {
            if (settings.IsEnabled && messageLog.IsSuccessful && !IsExistedOnCache(messageLog))
            {
                await cacheService.SetAsync(
                ModifyKey(messageLog.ConversationId.Replace(':', '-')),
                messageLog.Message,
                new CacheItemOptions
                {
                    RelativeExpiration = TimeSpan.FromMinutes(settings.TimeSpanInMinutes)
                });
            }
        }

        private bool IsExistedOnCache(ConversationMessageLog messageLog)
        {
            var jsonLogList = redisCacheExtend.GetValueByKeyContains<string>(messageLog.ConversationId.Replace(':', '-'));

            return jsonLogList.Any(content => content.Equals(messageLog.Message));
        }

        private static string ModifyKey(string key) => $"{key}:{Guid.NewGuid()}";
    }
}