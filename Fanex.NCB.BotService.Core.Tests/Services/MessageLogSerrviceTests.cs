namespace Fanex.NCB.BotService.Core.Tests.Services
{
    using System.Threading.Tasks;
    using Fanex.Caching;
    using Fanex.NCB.BotService.Core.Enum;
    using Fanex.NCB.BotService.Core.Models;
    using Fanex.NCB.BotService.Core.Services;
    using Newtonsoft.Json;
    using NSubstitute;
    using Xunit;

    public class MessageLogSerrviceTests
    {
        private readonly CheckDuplicatedMessageLogSetting settings = new() { IsEnabled = true, TimeSpanInMinutes = 5 };
        private ICacheService cacheService;

        [Fact]
        public async Task IsExistedOnCacheOrExceptionList_OnExceptionList_ReturnTrueAndDidNotGetCache()
        {
            settings.ExceptionConversatiionIDs = new[] { "ABC" };
            var service = GetService();
            var actual = await service.IsExistedOnCacheOrExceptionList("ABC", "messsage");
            Assert.True(actual);
            await cacheService.DidNotReceive().GetAsync<string>(Arg.Any<string>());
        }

        [Fact]
        public async Task IsExistedOnCacheOrExceptionList_OnNotInExceptionList_ReturnTrueWhenNotInCache()
        {
            settings.ExceptionConversatiionIDs = new[] { "ADE" };
            var service = GetService();
            var logMsg = new ConversationMessageLog { ConversationId = "ABC", Message = "Msg", Status = SentMessageStatus.Success };
            cacheService.GetAsync<string>(Arg.Is("ABC")).Returns(JsonConvert.SerializeObject(logMsg));

            var actual = await service.IsExistedOnCacheOrExceptionList("ABC", "Msg");
            Assert.True(actual);
        }

        [Fact]
        public async Task InserMessageLogInRedisCache_WhenCalled_InsertOnCache()
        {
            var conversationLogMsg = new ConversationMessageLog { ConversationId = "A:1234", Message = "ABC" };
            var service = GetService();
            await service.InserMessageLogInRedisCache(conversationLogMsg);
            await cacheService.Received().SetAsync(Arg.Is("A-1234"),
                Arg.Is(JsonConvert.SerializeObject(conversationLogMsg)),
                Arg.Any<CacheItemOptions>());
        }

        private MessageLogService GetService()
        {
            cacheService = Substitute.For<ICacheService>();
            return new MessageLogService(settings, cacheService);
        }
    }
}