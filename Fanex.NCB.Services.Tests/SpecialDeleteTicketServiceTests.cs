namespace Fanex.NCB.Services.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.Criteria.SpecialDeleteTicket;
    using Fanex.NCB.Models.NotifictaionSendMessage;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Models.SpecialDeleteTicket;
    using Fanex.NCB.Shared.Services.Interfaces;
    using NSubstitute;
    using Xunit;

    public class SpecialDeleteTicketServiceTests
    {
        private IBaseService baseService;
        private IChatbotService chatbotService;
        private ISendNotificationService sendNotificationService;

        [Fact]
        public async Task PullTicketAndCreateNotification_WhenGetSettingIsNull_DoNothing()
        {
            var service = GetService();
            baseService.GetDataEntityAsync<SystemSetting>(Arg.Any<GetSystemSettingCriteria>()).Returns((SystemSetting)null);

            //Actual
            await service.PullTicketAndCreateNotification();

            //Expected
            await baseService.DidNotReceiveWithAnyArgs().FetchMultipleAsync<Message, MessageDetail>(Arg.Any<GetMessageFromSpecialDeleteCriteria>());
            await baseService.DidNotReceive().FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(Arg.Any<ImportMessageFromSpecialDeleteCriteria>());
        }

        [Fact]
        public async Task PullTicketAndCreateNotification_NoTicketToImport_DoNothing()
        {
            var service = GetService();
            baseService.GetDataEntityAsync<SystemSetting>(Arg.Any<GetSystemSettingCriteria>()).Returns(
                new SystemSetting { BatchSize = 100, MaxMessageID = 1989, MsgContentsTypeLst = "ABC" });

            baseService.FetchMultipleAsync<Message, MessageDetail>(Arg.Any<GetMessageFromSpecialDeleteCriteria>()).Returns(
              (new List<Message>(), new List<MessageDetail>())
                );
            //Actual
            await service.PullTicketAndCreateNotification();

            //Expected
            await baseService.DidNotReceive().FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(Arg.Any<ImportMessageFromSpecialDeleteCriteria>());
        }

        [Fact]
        public async Task PullTicketAndCreateNotification_ImportFailed_NotifyChatGroup()
        {
            var service = GetService();
            baseService.GetDataEntityAsync<SystemSetting>(Arg.Any<GetSystemSettingCriteria>()).Returns(
                new SystemSetting { BatchSize = 100, MaxMessageID = 1989, MsgContentsTypeLst = "ABC" });

            baseService.FetchMultipleAsync<Message, MessageDetail>(Arg.Any<GetMessageFromSpecialDeleteCriteria>()).Returns(
              (new List<Message> { new Message { MessageID = 1 } }, new List<MessageDetail> { new MessageDetail { MessageID = 1 } })
                );
            var fakeData = FakeData();
            baseService.FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(Arg.Any<ImportMessageFromSpecialDeleteCriteria>()).Returns(fakeData);

            //Actual
            await service.PullTicketAndCreateNotification();

            //Expected
            await chatbotService.Received(1).SendMessageToMultitpleAsync(Arg.Any<IEnumerable<NotificationMessage>>());
        }

        private ValueTuple<IEnumerable<NotificationRecipient>, IEnumerable<NotificationSendMessage>> FakeData()
        {
            var recipients = new List<NotificationRecipient>
            {
                new NotificationRecipient { BotConversationId = "", BotName = "", ChatGroupConversationId = "adsa", NotificationId = 1}
            };
            var notifications = new List<NotificationSendMessage>
            {
                new NotificationSendMessage { NotificationId = 1, Content = "ABC", Title = "" }
            };

            return new ValueTuple<IEnumerable<NotificationRecipient>, IEnumerable<NotificationSendMessage>>(recipients, notifications);
        }

        private SpecialDeleteTicketService GetService()
        {
            baseService = Substitute.For<IBaseService>();
            chatbotService = Substitute.For<IChatbotService>();
            sendNotificationService = Substitute.For<ISendNotificationService>();
            return new SpecialDeleteTicketService(baseService, chatbotService, sendNotificationService);
        }
    }
}