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
        public async Task IsInvalidSendMessageToBotService_IsDisbaledSetting_ReturnFalse()
        {
            settings.IsEnabled = false;
            var service = GetService();
            var actual = await service.IsInvalidSendMessageToBotService("ABC", "messsage");
            Assert.False(actual);
            await cacheService.DidNotReceive().GetAsync<string>(Arg.Any<string>());
        }

        [Fact]
        public async Task IsInvalidSendMessageToBotService_OnExceptionList_ReturnFalse()
        {
            settings.ExceptionConversatiionIDs = new[] { "ABC" };
            var service = GetService();
            var actual = await service.IsInvalidSendMessageToBotService("ABC", "messsage");
            Assert.False(actual);
            await cacheService.DidNotReceive().GetAsync<string>(Arg.Any<string>());
        }

        [Fact]
        public async Task IsInvalidSendMessageToBotService_WhenExistedOnCache_ReturnFalse()
        {
            settings.ExceptionConversatiionIDs = new[] { "ABC" };
            var service = GetService();
            var logMsg = new ConversationMessageLog { ConversationId = "ABC", Message = "Msg", Status = SentMessageStatus.Success };
            cacheService.GetAsync<string>(Arg.Is("ABC")).Returns(JsonConvert.SerializeObject(logMsg));

            var actual = await service.IsInvalidSendMessageToBotService("ABC", "Msg");
            Assert.False(actual);
        }

        [Fact]
        public async Task IsInvalidSendMessageToBotService_WhenNotExistedOnCache_ReturnTrue()
        {
            var service = GetService();
            var logMsg = new ConversationMessageLog { ConversationId = "ABC", Message = "Msg", Status = SentMessageStatus.Success };
            cacheService.GetAsync<string>(Arg.Is("ABC")).Returns(JsonConvert.SerializeObject(logMsg));

            var actual = await service.IsInvalidSendMessageToBotService("ABC", "Msg");
            Assert.True(actual);
        }

        [Fact]
        public async Task InserMessageLogInRedisCache_WhenCalled_InsertOnCache()
        {
            var conversationLogMsg = new ConversationMessageLog { ConversationId = "A:1234", Message = "ABC" };
            var service = GetService();
            await service.InserMessageLogIntoCache(conversationLogMsg);
            await cacheService.Received().SetAsync(Arg.Do<string>(x => x.Contains("A-1234")),
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