namespace Fanex.NCB.Services.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NCB.Criteria.SiteMessage;
    using Fanex.NCB.Criteria.SpecialDeleteTicket;
    using Fanex.NCB.Models.NotifictaionSendMessage;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Services.Tests.TestableClasses;
    using Fanex.NCB.Shared.Extensions;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Models.SiteMessage;
    using Fanex.NCB.Shared.Models.SpecialDeleteTicket;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Microsoft.Extensions.Configuration;
    using NSubstitute;
    using Xunit;

    public class SpecialDeleteServiceTests
    {
        private readonly IFixture fixture = new Fixture();
        private IBaseService baseService;
        private IChatbotService chatbotService;
        private IPushSignalService pushSignalService;
        private ILogService logService;
        private IConfiguration configuration;

        ////[Fact]
        ////public async Task PullVoidingMessageLicensee_WhenGetSettingIsNull_DoNothing()
        ////{
        ////    var service = GetService();

        ////    baseService.GetDataEntityAsync<SystemSetting>(Arg.Any<GetSystemSettingCriteria>()).Returns((SystemSetting)null);

        ////    //Actual
        ////    await service.PullVoidingMessageLicensee();

        ////    //Expected
        ////    await baseService.DidNotReceiveWithAnyArgs().FetchMultipleAsync<Message, MessageDetail>(Arg.Any<GetMessageFromSpecialDeleteCriteria>());
        ////    await baseService.DidNotReceive().FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(Arg.Any<ImportMessageFromSpecialDeleteCriteria>());

        ////    //Assert
        ////    await baseService.DidNotReceive().FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(Arg.Any<ImportMessageFromSpecialDeleteCriteria>());
        ////    await chatbotService.DidNotReceive().SendMessageToMultitpleAsync(Arg.Any<List<NotificationMessage>>());
        ////}

        [Fact]
        public async Task PullVoidingMessageLicensee_NoTicketToImport_DoNothing()
        {
            var service = GetService();
            baseService.GetDataEntityAsync<SystemSetting>(Arg.Any<GetSystemSettingCriteria>()).Returns(
                new SystemSetting { BatchSize = 100, MaxMessageID = 1989, MsgContentsTypeLst = "ABC" });

            baseService.FetchMultipleAsync<Message, MessageDetail>(Arg.Any<GetMessageFromSpecialDeleteCriteria>()).Returns(
              (new List<Message>(), new List<MessageDetail>()));
            //Actual
            await service.PullVoidingMessageLicensee();

            //Expected
            await baseService.DidNotReceive().FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(Arg.Any<ImportMessageFromSpecialDeleteCriteria>());
        }

        [Fact]
        public async Task PullVoidingMessageLicensee_HaveTicketToImport_ExecuteSpImportAndSendCarbonNotification()
        {
            var service = GetService();
            baseService.GetDataEntityAsync<SystemSetting>(Arg.Any<GetSystemSettingCriteria>()).Returns(
                new SystemSetting { BatchSize = 100, MaxMessageID = 1989, MsgContentsTypeLst = "ABC" });

            baseService.FetchMultipleAsync<Message, MessageDetail>(Arg.Any<GetMessageFromSpecialDeleteCriteria>()).Returns(
              (fixture.Create<List<Message>>(), fixture.Create<List<MessageDetail>>()));

            baseService.FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(Arg.Any<ImportMessageFromSpecialDeleteCriteria>()).Returns(FakeData());

            //Actual
            await service.PullVoidingMessageLicensee();

            //Expected
            await baseService.Received().FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(Arg.Any<ImportMessageFromSpecialDeleteCriteria>());
        }

        ////[Fact]
        ////public async Task PullVoidingMessageInternalDepartment_WhenGetSettingIsNull_DoNothing()
        ////{
        ////    var service = GetService();

        ////    baseService.GetDataEntityAsync<SystemSetting>(Arg.Any<GetAbnormalBetSystemSettingCriteria>()).Returns((SystemSetting)null);

        ////    //Actual
        ////    await service.PullVoidingMessageInternalDepartment();

        ////    //Assert
        ////    await baseService.DidNotReceiveWithAnyArgs().GetDataAsync<Message>(Arg.Any<GetAbnormalBetsMessageCriteria>());
        ////    await baseService.DidNotReceive().FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(Arg.Any<ImportMessageFromSpecialDeleteCriteria>());
        ////    await chatbotService.DidNotReceive().SendMessageToMultitpleAsync(Arg.Any<List<NotificationMessage>>());
        ////}

        ////[Fact]
        ////public async Task PullVoidingMessageInternalDepartment_HaveTicketToImport_CallSPImport()
        ////{
        ////    var service = GetService();
        ////    baseService.GetDataEntityAsync<SystemSetting>(Arg.Any<GetAbnormalBetSystemSettingCriteria>())
        ////        .Returns(new SystemSetting { BatchSize = 100, MaxMessageID = 1989, MsgContentsTypeLst = "103" });

        ////    baseService.GetDataAsync<Message>(Arg.Any<GetAbnormalBetsMessageCriteria>()).Returns(fixture.Create<List<Message>>());
        ////    baseService.GetDataAsync<MessageHistory>(Arg.Any<GetListMessageAbnormalbetsFromYesterdayToCurrentCriteria>()).Returns(fixture.Create<List<MessageHistory>>());
        ////    baseService.GetDataAsync<AbnormalMessage>(Arg.Any<GetAbnormalBetInfoCriteria>()).Returns(fixture.Create<IEnumerable<AbnormalMessage>>());

        ////    //Actual
        ////    await service.PullVoidingMessageInternalDepartment();

        ////    //Expected
        ////    await baseService.DidNotReceive().FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(Arg.Any<ImportMessageFromSpecialDeleteCriteria>());
        ////}

        private static ValueTuple<IEnumerable<NotificationRecipient>, IEnumerable<NotificationSendMessage>> FakeData()
        {
            var recipients = new List<NotificationRecipient>
            {
                new NotificationRecipient {
                    BotConversationId = "",
                    BotName = "",
                    ChatGroupConversationId = "ConversationId",
                    NotificationId = 1,
                    ServiceUrl = "test.com"
                }
            };
            var notifications = new List<NotificationSendMessage>
            {
                new NotificationSendMessage { NotificationId = 1, Content = "", Title = "" }
            };

            return new ValueTuple<IEnumerable<NotificationRecipient>, IEnumerable<NotificationSendMessage>>(recipients, notifications);
        }

        [Fact]
        public async Task GetAbnormalAccountUpdatedUsernameAsync_WhenCalled_ReturnAsExpectedResult()
        {
            // Arrange
            var messages = fixture.Create<IEnumerable<AbnormalMessageHistory>>();
            var xml = XmlHelper<AbnormalMessageHistory>.Serialize(messages.ToList());
            var histories = fixture.Create<IEnumerable<VoidingHistoryUsername>>();
            var criteria = new GetAbnormalAccountUsernameLstCriteria();
            var service = GetService();
            baseService.GetDataAsync<VoidingHistoryUsername>(Arg.Do<GetAbnormalAccountUsernameLstCriteria>(x => criteria = x)).Returns(histories);

            // Act
            var result = await service.Public_GetAbnormalAccountUpdatedUsernameAsync(messages);

            // Assert
            Assert.Equal(histories, result);

            Assert.Equal(xml, criteria.MessageIDLst);
        }

        [Fact]
        public async Task GetMessageHistoryAsync_WhenCalled_ReturnAsExpectedResult()
        {
            // Arrange
            var messageHistories = fixture.Create<List<MessageHistory>>();
            var service = GetService();
            baseService.GetDataAsync<MessageHistory>(Arg.Any<GetListMessageAbnormalbetsFromYesterdayToCurrentCriteria>()).Returns(messageHistories);

            // Act
            var result = await service.Public_GetMessageHistoryAsync();

            // Assert
            Assert.Equal(messageHistories, result);
        }

        [Fact]
        public async Task GetAbnormalBetSystemSettingAsync_WhenCalled_ReturnAsExpectedResult()
        {
            // Arrange
            var setting = fixture.Create<SystemSetting>();
            var service = GetService();
            baseService.GetDataEntityAsync<SystemSetting>(Arg.Any<GetAbnormalBetSystemSettingCriteria>()).Returns(setting);

            // Act
            var result = await service.Public_GetAbnormalBetSystemSettingAsync();

            // Assert
            Assert.Equal(setting, result);
        }

        [Fact]
        public async Task GetAbnormalBetsMessageAsync_WhenCalled_ReturnAsExpectedResult()
        {
            // Arrange
            var setting = fixture.Create<SystemSetting>();
            var messageHistories = fixture.Create<List<MessageHistory>>();
            var messages = fixture.Create<IEnumerable<Message>>();
            var criteria = new GetAbnormalBetsMessageCriteria();
            var service = GetService();
            baseService.GetDataAsync<Message>(Arg.Do<GetAbnormalBetsMessageCriteria>(x => criteria = x)).Returns(messages);

            // Act
            var result = await service.Public_GetAbnormalBetsMessageAsync(setting, messageHistories);

            // Assert
            Assert.Equal(messages, result);

            Assert.Equal(XmlHelper<MessageHistory>.Serialize(messageHistories), criteria.MessageIDLst);
            Assert.Equal(setting.BatchSize, criteria.BatchSize);
            Assert.Equal(setting.MsgContentsTypeLst, criteria.MsgContentsTypeLst);
        }

        [Fact]
        public async Task GetVoidingHistoryAsync_WhenCalled_ReturnAsExpectedResult()
        {
            // Arrange
            var messageHistories = fixture.Create<List<MessageHistory>>();
            var xml = XmlHelper<MessageHistory>.Serialize(messageHistories);
            var messages = fixture.Create<List<AbnormalMessage>>();
            var histories = fixture.Create<List<AbnormalMessageHistory>>();
            var tuple = (messages, histories);
            var criteria = new GetAbnormalBetInfoCriteria();
            var service = GetService();
            baseService.FetchMultipleAsync<AbnormalMessage, AbnormalMessageHistory>(Arg.Do<GetAbnormalBetInfoCriteria>(x => criteria = x)).Returns(tuple);

            // Act
            var result = await service.Public_GetVoidingHistoryAsync(messageHistories);

            // Assert
            Assert.Equal(messages, result.Item1);
            Assert.Equal(histories, result.Item2);

            Assert.Equal(xml, criteria.MessageIdLst);
        }

        [Fact]
        public async Task InsertToMessageChatbotAsync_WhenCalled_ExpectedCallCorrectMethod()
        {
            // Arrange
            var messages = fixture.Create<IEnumerable<MessageHistory>>();
            var xml = XmlHelper<MessageHistory>.Serialize(messages.ToList());
            var jobIdentity = fixture.Create<int>();
            var command = new InsertMessageHistoryCommand();
            var service = GetService();
            baseService.ExecuteNonQueryAsync(Arg.Do<InsertMessageHistoryCommand>(x => command = x)).Returns(Task.FromResult(new CommandResult()));

            // Act
            await service.Public_InsertToMessageChatbotAsync(messages, jobIdentity);

            // Assert
            await baseService.Received().ExecuteNonQueryAsync(command);

            Assert.Equal(jobIdentity, command.JobIdentify);
            Assert.Equal(xml, command.MessageIDLst);
        }

        #region SendMessageByChannelAsync

        [Fact]
        public async Task SendMessageByChannelAsync_WhenCalled_ExpectedCallCorrectMethod()
        {
            // Arrange
            var channelId = fixture.Create<byte>();
            var notifications = fixture.Create<IEnumerable<NotificationMessage>>();
            var service = GetService();

            // Act
            await service.Public_SendMessageByChannelAsync(channelId, notifications);

            // Assert
            await chatbotService.Received().SendMultiMessageAsync(channelId, Arg.Any<IEnumerable<NotificationMessage>>());
        }

        [Fact]
        public async Task SendMessageByChannelAsync_Exception_ExpectedWriteLog()
        {
            // Arrange
            var channelId = fixture.Create<byte>();
            var notifications = fixture.Create<IEnumerable<NotificationMessage>>();
            var exception = fixture.Create<Exception>();
            var service = GetService();
            chatbotService.SendMultiMessageAsync(channelId, Arg.Any<IEnumerable<NotificationMessage>>())
                .Returns(Task.FromException<NotificationMessageResponse>(exception));

            // Act
            await service.Public_SendMessageByChannelAsync(channelId, notifications);

            // Assert
            logService.Received().WriteLogFileError(exception.Message);
        }

        #endregion SendMessageByChannelAsync

        #region Helpers

        private SpecialDeleteServiceTestable GetService()
        {
            baseService = Substitute.For<IBaseService>();
            chatbotService = Substitute.For<IChatbotService>();
            pushSignalService = Substitute.For<IPushSignalService>();
            logService = Substitute.For<ILogService>();
            configuration = Substitute.For<IConfiguration>();

            return Substitute.For<SpecialDeleteServiceTestable>(baseService, chatbotService, pushSignalService, logService, configuration);
        }

        #endregion Helpers
    }
}