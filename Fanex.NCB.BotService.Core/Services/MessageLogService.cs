namespace Fanex.NCB.BotService.Core.Services
{
    using Caching;
    using Models;
    using Interfaces;
    using Fanex.NCB.Shared.Services.Interfaces;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class MessageLogService : IMessageLogService
    {
        private readonly CheckDuplicatedMessageLogSetting settings;
        private readonly ICacheService cacheService;
        private readonly IRedisCacheExtendService redisCacheExtend;
        private readonly ICommonLogService commonLogService;

        public MessageLogService(CheckDuplicatedMessageLogSetting settings,
            ICacheService cacheService,
            IRedisCacheExtendService redisCacheExtend,
            ICommonLogService commonLogService)
        {
            this.settings = settings;
            this.cacheService = cacheService;
            this.redisCacheExtend = redisCacheExtend;
            this.commonLogService = commonLogService;
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
            bool isInvalid = false;

            try
            {
                var messageContentList = redisCacheExtend.GetValueByKeyContains<string>(ModifyConversationId(conversationId));

                foreach (string logContent in messageContentList)
                {
                    if (logContent != null && logContent.Equals(message))
                    {
                        isInvalid = true;

                        return isInvalid;
                    }
                }
            }
            catch (Exception ex)
            {
                commonLogService.LogFileError(ex);
            }

            return isInvalid;
        }

        public async Task InsertMessageLogIntoCache(string conversationId, string messageContent, string guiid)
        {
            if (settings.IsEnabled && !await IsExistedOnCache(conversationId, messageContent))
            {
                try
                {
                    await cacheService.SetAsync(
                        ModifyKey(ModifyConversationId(conversationId), guiid),
                        messageContent,
                        new CacheItemOptions
                        {
                            RelativeExpiration = TimeSpan.FromMinutes(settings.TimeSpanInMinutes)
                        });
                }
                catch (Exception ex)
                {
                    await commonLogService.SentryErrorAsync(ex.Message, ex);
                }
            }
        }

        public async Task RemoveMessageLogOnCache(string conversationId, string guid)
        {
            try
            {
                string key = ModifyKey(ModifyConversationId(conversationId), guid);
                await cacheService.RemoveAsync(key);
            }
            catch (Exception ex)
            {
                await commonLogService.SentryErrorAsync(ex.Message, ex);
            }
        }

        private async Task<bool> IsExistedOnCache(string conversationId, string guid)
            => await cacheService.ContainAsync(ModifyKey(conversationId, guid));

        private static string ModifyKey(string key, string guiid) => $"{key}:{guiid}";

        private static string ModifyConversationId(string conversationId) => conversationId.Replace(':', '-');
    }
}