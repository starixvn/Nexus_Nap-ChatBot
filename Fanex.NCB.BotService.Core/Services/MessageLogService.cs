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

            var messageContentList = await redisCacheExtend.GetValueByKeyContains<string>(conversationId.Replace(':', '-'));

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

        public async Task InserMessageLogIntoCache(string conversationId, string messageContent, string guiid)
        {
            if (settings.IsEnabled && !await IsExistedOnCache(conversationId, messageContent))
            {
                await cacheService.SetAsync(
                ModifyKey(conversationId.Replace(':', '-'), guiid),
                messageContent,
                new CacheItemOptions
                {
                    RelativeExpiration = TimeSpan.FromMinutes(settings.TimeSpanInMinutes)
                });
            }
        }

        public async Task RemoveMessageLogInCache(string conversationId, string guid)
        {
            string key = ModifyKey(conversationId, guid);
            await cacheService.RemoveAsync(key);
        }

        private async Task<bool> IsExistedOnCache(string conversationId, string guid)
        {
            return await cacheService.ContainAsync(ModifyKey(conversationId, guid));
        }

        private static string ModifyKey(string key, string guiid) => $"{key}:{guiid}";
    }
}