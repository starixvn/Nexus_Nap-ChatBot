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
    using Fanex.NCB.Shared.Models.SpecialDeleteTicket;
    using Fanex.NCB.Shared.Models.SpecialDeleteTicket.AbnormalBets;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
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
        private ISiteDbService siteDbService;

        #region PullVoidingMessageLicenseeAsync

        [Fact]
        public async Task PullVoidingMessageLicenseeAsync_SettingIsNull_DoNothing()
        {
            // Arrange
            SystemSetting setting = null;
            var service = GetService();
            baseService.GetDataEntityAsync<SystemSetting>(Arg.Any<GetSystemSettingCriteria>()).Returns(setting);

            // Act
            await service.PullVoidingMessageLicenseeAsync();

            // Assert
            service.DidNotReceive().Protected("GetMessageFromVoidingForLicenseeAsync", Arg.Any<SystemSetting>());
        }

        [Fact]
        public async Task PullVoidingMessageLicenseeAsync_NoDataToImport_DoNothing()
        {
            // Arrange
            var setting = fixture.Create<SystemSetting>();
            var messages = new List<Message>();
            var messageDetails = new List<MessageDetail>();
            var tuple = (messages, messageDetails);
            var service = GetService();
            baseService.GetDataEntityAsync<SystemSetting>(Arg.Any<GetSystemSettingCriteria>()).Returns(setting);
            service.Protected("GetMessageFromVoidingForLicenseeAsync", setting).Returns(Task.FromResult<(IEnumerable<Message>, IEnumerable<MessageDetail>)>(tuple));

            // Act
            await service.PullVoidingMessageLicenseeAsync();

            // Assert
            service.DidNotReceive().Protected("ImportMessageToChatbotDBAsync", messages, messageDetails);
        }

        [Fact]
        public async Task PullVoidingMessageLicenseeAsync_HasDataToImport_DoAsExpected()
        {
            // Arrange
            var setting = fixture.Create<SystemSetting>();
            var messages = fixture.Create<IEnumerable<Message>>();
            var messageDetails = fixture.Create<IEnumerable<MessageDetail>>();
            var command = new CommandResult { Code = 0 };
            var notifications = fixture.Create<IEnumerable<NotificationMessage>>();
            var service = GetService();
            baseService.GetDataEntityAsync<SystemSetting>(Arg.Any<GetSystemSettingCriteria>()).Returns(setting);
            service.Protected("GetMessageFromVoidingForLicenseeAsync", setting).Returns(Task.FromResult<(IEnumerable<Message>, IEnumerable<MessageDetail>)>((messages, messageDetails)));
            service.Protected("ImportMessageToChatbotDBAsync", messages, messageDetails).Returns(Task.FromResult<(CommandResult, IEnumerable<NotificationMessage>)>((command, notifications)));

            // Act
            await service.PullVoidingMessageLicenseeAsync();

            // Assert
            service.Received().Protected("UpdateMessageHistoryVoidingForLicenseeAsync", messageDetails);
            service.Received().Protected("OnAfterImportDataAsync", command, notifications);
        }

        #endregion PullVoidingMessageLicenseeAsync

        #region PullVoidingMessageInternalDepartmentAsync

        [Fact]
        public async Task PullVoidingMessageInternalDepartmentAsync_SettingIsNull_DoNothing()
        {
            // Arrange
            SystemSetting setting = null;
            var service = GetService();
            baseService.GetDataEntityAsync<SystemSetting>(Arg.Any<GetAbnormalBetSystemSettingCriteria>()).Returns(setting);

            // Act
            await service.PullVoidingMessageInternalDepartmentAsync();

            // Assert
            await baseService.DidNotReceive().GetDataAsync<MessageHistory>(Arg.Any<GetListMessageAbnormalbetsFromYesterdayToCurrentCriteria>());
        }

        [Fact]
        public async Task PullVoidingMessageInternalDepartmentAsync_NoDataVoidingHistory_StillGoOn()
        {
            // Arrange
            var setting = fixture.Create<SystemSetting>();
            var histories = new List<MessageHistory>();
            var service = GetService();
            baseService.GetDataEntityAsync<SystemSetting>(Arg.Any<GetAbnormalBetSystemSettingCriteria>()).Returns(setting);
            baseService.GetDataAsync<MessageHistory>(Arg.Any<GetListMessageAbnormalbetsFromYesterdayToCurrentCriteria>()).Returns(histories);

            // Act
            await service.PullVoidingMessageInternalDepartmentAsync();

            // Assert
            service.Received().Protected("GetMessageAndImportToChatbotAsync", histories, setting);
        }

        [Fact]
        public async Task PullVoidingMessageInternalDepartmentAsync_HasDataToImport_DoAsExpected()
        {
            // Arrange
            var setting = fixture.Create<SystemSetting>();
            var histories = fixture.Create<List<MessageHistory>>();
            var service = GetService();
            baseService.GetDataEntityAsync<SystemSetting>(Arg.Any<GetAbnormalBetSystemSettingCriteria>()).Returns(setting);
            baseService.GetDataAsync<MessageHistory>(Arg.Any<GetListMessageAbnormalbetsFromYesterdayToCurrentCriteria>()).Returns(histories);

            // Act
            await service.PullVoidingMessageInternalDepartmentAsync();

            // Assert
            service.Received().Protected("GetMessageAndImportToChatbotAsync", histories, setting);
        }

        #endregion PullVoidingMessageInternalDepartmentAsync

        #region UpdateMessageHistoryVoidingForLicenseeAsync

        [Fact]
        public async Task UpdateMessageHistoryVoidingForLicenseeAsync_IsSuccessful_DoAsExpected()
        {
            // Arrange
            var messageDetails = fixture.Create<IEnumerable<MessageDetail>>();
            var service = GetService();

            // Act
            await service.Public_UpdateMessageHistoryVoidingForLicenseeAsync(messageDetails);

            // Assert
            service.Received().Protected("InsertToMessageChatbotAsync", Arg.Any<string>(), 1);
        }

        #endregion UpdateMessageHistoryVoidingForLicenseeAsync

        #region OnAfterImportDataAsync

        [Fact]
        public async Task OnAfterImportDataAsync_IsSuccessful_DoAsExpected()
        {
            // Arrange
            var commandResult = new CommandResult { Code = 0 };
            var notifications = fixture.Create<IEnumerable<NotificationMessage>>();
            var notificationIds = string.Join(",", notifications.Select(x => x.NotificationId));
            var service = GetService();

            // Act
            await service.Public_OnAfterImportDataAsync(commandResult, notifications);

            // Assert
            await chatbotService.DidNotReceive().NotifyToChatGroupAsync(Arg.Any<string>());

            await pushSignalService.Received().OnInsertNotificationAsync(notificationIds);
            service.Received().Protected("SendMessageByChannelAsync", (byte)1, notifications);
            service.Received().Protected("SendMessageByChannelAsync", (byte)2, notifications);
        }

        [Fact]
        public async Task OnAfterImportDataAsync_IsUnsuccessful_WithMessage_DoAsExpected()
        {
            // Arrange
            var commandResult = new CommandResult { Code = 1, Message = "error" };
            var notifications = fixture.Create<IEnumerable<NotificationMessage>>();
            var notificationIds = string.Join(",", notifications.Select(x => x.NotificationId));
            var service = GetService();

            // Act
            await service.Public_OnAfterImportDataAsync(commandResult, notifications);

            // Assert
            await chatbotService.Received().NotifyToChatGroupAsync("error");

            await pushSignalService.DidNotReceive().OnInsertNotificationAsync(notificationIds);
            service.DidNotReceive().Protected("SendMessageByChannelAsync", (byte)1, notifications);
            service.DidNotReceive().Protected("SendMessageByChannelAsync", (byte)2, notifications);
        }

        #endregion OnAfterImportDataAsync

        [Fact]
        public async Task ImportMessageToChatbotDBAsync_WhenCalled_ReturnAsExpectedResult()
        {
            // Arrange
            var messages = fixture.Create<IEnumerable<Message>>();
            var messageDetails = fixture.Create<IEnumerable<MessageDetail>>();
            var recipients = fixture.Create<IEnumerable<NotificationRecipient>>();
            var notificationMessages = fixture.Create<IEnumerable<NotificationSendMessage>>();
            var notifications = fixture.Create<IEnumerable<NotificationMessage>>();
            var tuple = (recipients, notificationMessages);
            var criteria = new ImportMessageFromSpecialDeleteCriteria();
            var service = GetService();
            baseService.FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(Arg.Do<ImportMessageFromSpecialDeleteCriteria>(x => criteria = x)).Returns(tuple);
            chatbotService.GenerateNotifificationMessageForCarbonGroupAsync(notificationMessages, recipients).Returns(notifications);

            // Act
            var result = await service.Public_ImportMessageToChatbotDBAsync(messages, messageDetails);

            // Assert
            Assert.Equal(JsonConvert.SerializeObject(messages), criteria.MessageHeader);
            Assert.Equal(JsonConvert.SerializeObject(messageDetails), criteria.MessageDetail);
            Assert.Equal(criteria.ErrCode, result.Item1.Code);
            Assert.Equal(criteria.ErrMsg, result.Item1.Message);

            Assert.Equal(notifications, result.Item2);
        }

        [Fact]
        public async Task GetMessageFromVoidingForLicenseeAsync_WhenCalled_ReturnAsExpectedResult()
        {
            // Arrange
            var messages = new List<Message>
            {
                new Message { Content_en = "  content en  ", Content_cs = "  content cs  " }
            };
            var details = fixture.Create<IEnumerable<MessageDetail>>();
            var tuple = (messages, details);
            var setting = fixture.Create<SystemSetting>();
            var criteria = new GetMessageFromSpecialDeleteCriteria();
            var service = GetService();
            baseService.FetchMultipleAsync<Message, MessageDetail>(Arg.Do<GetMessageFromSpecialDeleteCriteria>(x => criteria = x)).Returns(tuple);

            // Act
            var result = await service.Public_GetMessageFromVoidingForLicenseeAsync(setting);

            // Assert
            Assert.Equal(setting.MaxMessageID, criteria.MessageID);
            Assert.Equal(setting.BatchSize, criteria.BatchSize);
            Assert.Equal(setting.MsgContentsTypeLst, criteria.MsgContentsTypeLst);
            Assert.Equal(setting.LastPostTime, criteria.LastPostTime);

            Assert.Equal(tuple, result);

            Assert.Collection(result.Item1,
                item =>
                {
                    Assert.Equal("content en", item.Content_en);
                    Assert.Equal("content cs", item.Content_cs);
                });
        }

        #region GetMessageAndImportToChatbotAsync

        [Fact]
        public async Task GetMessageAndImportToChatbotAsync_DoNotHaveAbnormalBetsMessage_DoNothing()
        {
            // Arrange
            var messageHistories = fixture.Create<IEnumerable<MessageHistory>>();
            var setting = fixture.Create<SystemSetting>();
            var messages = new List<Message>();
            var service = GetService();
            service.Protected("GetAbnormalBetsMessageContentAsync", setting, messageHistories).Returns(Task.FromResult<IEnumerable<Message>>(messages));

            // Act
            await service.Public_GetMessageAndImportToChatbotAsync(messageHistories, setting);

            // Asset
            service.DidNotReceive().Protected("ImportAbnormalBetNotificationToChatbotDBAsync",
                Arg.Any<IEnumerable<AbnormalMessage>>(),
                Arg.Any<IEnumerable<MessageHistory>>(),
                Arg.Any<int>());
        }

        [Fact]
        public async Task GetMessageAndImportToChatbotAsync_HaveAbnormalBetsMessage_DoAsExpected()
        {
            // Arrange
            var messageHistories = fixture.Create<IEnumerable<MessageHistory>>();
            var setting = fixture.Create<SystemSetting>();
            var messages = new List<Message>
            {
                new Message { RequestID = 1, MessageID = 1, Content_en = "content en", Content_cs = "content cs" },
                new Message { RequestID = 2, MessageID = 2, Content_en = "content en", Content_cs = "content cs" }
            };
            var abnormalMessages = new List<AbnormalMessage>
            {
                new AbnormalMessage { RequestID = 1, MessageID = 1 },
                new AbnormalMessage { RequestID = 2, MessageID = 2 }
            };
            var abnormalMessageHistories = fixture.Create<IEnumerable<AbnormalMessageHistory>>();
            var updatedUsernames = new List<VoidingHistoryUsername>
            {
                new VoidingHistoryUsername { MessageID = 1, UsernameLst = "username", CreditSiteLst = "creditsite", DepositSiteLst = "depositsite" },
                new VoidingHistoryUsername { MessageID = 2, UsernameLst = "username", CreditSiteLst = "creditsite", DepositSiteLst = "depositsite" }
            };
            var service = GetService();
            service.Protected("GetAbnormalBetsMessageContentAsync", setting, messageHistories).Returns(Task.FromResult<IEnumerable<Message>>(messages));
            service.Protected("GetVoidingHistoryAsync", Arg.Any<List<AbnormalTicketInfo>>(), Arg.Any<List<AbnormalUsernames>>(), Arg.Any<List<MessageHistory>>())
                .Returns(Task.FromResult<IEnumerable<AbnormalMessage>>(abnormalMessages));

            // Act
            await service.Public_GetMessageAndImportToChatbotAsync(messageHistories, setting);

            // Asset
            service.DidNotReceive().Protected("ImportAbnormalBetNotificationToChatbotDBAsync",
                abnormalMessages,
                Arg.Any<IEnumerable<MessageHistory>>(),
                2);
        }

        #endregion GetMessageAndImportToChatbotAsync

        [Fact]
        public async Task GetVoidingHistoryAsync_WhenCalled_ReturnAsExpectedResult()
        {
            // Arrange
            var messages = fixture.Create<List<MessageHistory>>();
            var xml = XmlHelper<MessageHistory>.Serialize(messages);
            var abnormalMsgs = fixture.Create<List<AbnormalMessage>>();
            var tickets = fixture.Create<List<AbnormalTicketInfo>>();
            var usernames = fixture.Create<List<AbnormalUsernames>>();
            var criteria = new GetAbnormalBetInfoCriteria();
            var service = GetService();
            baseService.GetDataAsync<AbnormalMessage>(Arg.Do<GetAbnormalBetInfoCriteria>(x => criteria = x)).Returns(abnormalMsgs);

            // Act
            var result = await service.Public_GetVoidingHistoryAsync(tickets, usernames, messages);

            // Assert
            Assert.Equal(abnormalMsgs, result);
            Assert.Equal(xml, criteria.MessageIdLst);
        }

        #region ImportAbnormalBetNotificationToChatbotDBAsync

        [Fact]
        public async Task ImportAbnormalBetNotificationToChatbotDBAsync_MessagesAreEmpty_DoNothing()
        {
            // Arrange
            var messages = new List<AbnormalMessage>();
            var histories = fixture.Create<IEnumerable<MessageHistory>>();
            var maxMessageId = fixture.Create<int>();
            var service = GetService();

            // Act
            await service.Public_ImportAbnormalBetNotificationToChatbotDBAsync(messages, histories, maxMessageId);

            // Assert
            await baseService.DidNotReceive().FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(Arg.Any<ImportAbnormalBetsMessageCriteria>());
        }

        [Fact]
        public async Task ImportAbnormalBetNotificationToChatbotDBAsync_ImportSuccessful_DoAsExpected()
        {
            // Arrange
            var messages = fixture.Create<IEnumerable<AbnormalMessage>>();
            var histories = fixture.Create<IEnumerable<MessageHistory>>();
            var xml = XmlHelper<MessageHistory>.Serialize(histories.ToList());
            var maxMessageId = fixture.Create<int>();
            var recipients = fixture.Create<IEnumerable<NotificationRecipient>>();
            var notifications = fixture.Create<IEnumerable<NotificationSendMessage>>();
            var notificationMessages = fixture.Create<IEnumerable<NotificationMessage>>();
            var criteria = new ImportAbnormalBetsMessageCriteria();
            var service = GetService();
            baseService.FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(Arg.Do<ImportAbnormalBetsMessageCriteria>(x => criteria = x))
                .Returns((recipients, notifications)).AndDoes(x => { criteria.ErrCode = 0; criteria.ErrMsg = ""; });
            chatbotService.GenerateNotifificationMessageForCarbonGroupAsync(notifications, recipients).Returns(notificationMessages);

            // Act
            await service.Public_ImportAbnormalBetNotificationToChatbotDBAsync(messages, histories, maxMessageId);

            // Assert
            Assert.Equal(maxMessageId, criteria.MaxMessageId);
            Assert.Equal(JsonConvert.SerializeObject(messages, new JsonSerializerSettings { DateFormatString = "yyyy-MM-dd HH:mm:ss.fff" }), criteria.MessageHeader);

            service.Received().Protected("InsertToMessageChatbotAsync", xml, 2);
            service.Received().Protected("OnAfterImportDataAsync", Arg.Is<CommandResult>(x => x.Code == 0 && x.Message == ""), notificationMessages);
        }

        [Fact]
        public async Task ImportAbnormalBetNotificationToChatbotDBAsync_ImportUnsuccessful_DoAsExpected()
        {
            // Arrange
            var messages = fixture.Create<IEnumerable<AbnormalMessage>>();
            var histories = fixture.Create<IEnumerable<MessageHistory>>();
            var xml = XmlHelper<MessageHistory>.Serialize(histories.ToList());
            var maxMessageId = fixture.Create<int>();
            var recipients = fixture.Create<IEnumerable<NotificationRecipient>>();
            var notifications = fixture.Create<IEnumerable<NotificationSendMessage>>();
            var notificationMessages = fixture.Create<IEnumerable<NotificationMessage>>();
            var criteria = new ImportAbnormalBetsMessageCriteria();
            var service = GetService();
            baseService.FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(Arg.Do<ImportAbnormalBetsMessageCriteria>(x => criteria = x))
                .Returns((recipients, notifications)).AndDoes(x => { criteria.ErrCode = 1; criteria.ErrMsg = "error message"; });
            chatbotService.GenerateNotifificationMessageForCarbonGroupAsync(notifications, recipients).Returns(notificationMessages);

            // Act
            await service.Public_ImportAbnormalBetNotificationToChatbotDBAsync(messages, histories, maxMessageId);

            // Assert
            Assert.Equal(maxMessageId, criteria.MaxMessageId);
            Assert.Equal(JsonConvert.SerializeObject(messages, new JsonSerializerSettings { DateFormatString = "yyyy-MM-dd HH:mm:ss.fff" }), criteria.MessageHeader);

            service.DidNotReceive().Protected("InsertToMessageChatbotAsync", xml, 2);
            service.Received().Protected("OnAfterImportDataAsync", Arg.Is<CommandResult>(x => x.Code == 1 && x.Message == "error message"), notificationMessages);
        }

        #endregion ImportAbnormalBetNotificationToChatbotDBAsync

        [Fact]
        public async Task InsertToMessageChatbotAsync_WhenCalled_DoAsExpected()
        {
            // Arrange
            var messages = fixture.Create<string>();
            var jobIdentity = fixture.Create<int>();
            var command = new InsertMessageHistoryCommand();
            var service = GetService();
            baseService.ExecuteNonQueryAsync(Arg.Do<InsertMessageHistoryCommand>(x => command = x)).Returns(Task.FromResult(new CommandResult()));

            // Act
            await service.Public_InsertToMessageChatbotAsync(messages, jobIdentity);

            // Assert
            await baseService.Received().ExecuteNonQueryAsync(command);

            Assert.Equal(jobIdentity, command.JobIdentify);
            Assert.Equal(messages, command.MessageIDLst);
        }

        #region SendMessageByChannelAsync

        [Fact]
        public async Task SendMessageByChannelAsync_WhenCalled_DoAsExpected()
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
        public async Task SendMessageByChannelAsync_Exception_DoAsExpected()
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
            siteDbService = Substitute.For<ISiteDbService>();
            configuration.GetSection("Environment").Value.Returns("UAT");
            return Substitute.For<SpecialDeleteServiceTestable>(baseService, chatbotService, pushSignalService, logService, siteDbService, configuration);
        }

        #endregion Helpers
    }
}