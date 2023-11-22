namespace Fanex.NCB.BotService.Core.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Caching;
    using Fanex.NCB.BotService.Core.Models;
    using Fanex.NCB.BotService.Core.Services.Interfaces;
    using ILogger = Logging.ILogger;

    public class MessageLogService : IMessageLogService
    {
        private readonly CheckDuplicatedMessageLogSetting settings;
        private readonly ICacheService cacheService;
        private readonly IRedisCacheExtendService redisCacheExtend;
        private readonly ILogger logger;

        public MessageLogService(CheckDuplicatedMessageLogSetting settings,
            ICacheService cacheService,
            IRedisCacheExtendService redisCacheExtend,
            ILogger logger)
        {
            this.settings = settings;
            this.cacheService = cacheService;
            this.redisCacheExtend = redisCacheExtend;
            this.logger = logger;
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
                logger.Error(ex.Message);
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
                    await logger.ErrorAsync(ex.Message);
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
                logger.Error(ex.Message);
            }
        }

        private async Task<bool> IsExistedOnCache(string conversationId, string guid)
            => await cacheService.ContainAsync(ModifyKey(conversationId, guid));

        private static string ModifyKey(string key, string guiid) => $"{key}:{guiid}";

        private static string ModifyConversationId(string conversationId) => conversationId.Replace(':', '-');
    }
}