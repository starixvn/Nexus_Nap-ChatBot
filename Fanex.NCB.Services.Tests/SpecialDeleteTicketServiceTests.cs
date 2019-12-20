namespace Fanex.NCB.Services.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.Criteria.SpecialDeleteTicket;
    using Fanex.NCB.Models.NotifictaionSendMessage;
    using Fanex.NCB.Repository.Interfaces;
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
        private ISpecialDeleteRepository specialDeleteRepository;

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
        public async Task PullTicketAndCreateNotification_HaveTicket_ImportSucess()
        {
            var service = GetService();
            baseService.GetDataEntityAsync<SystemSetting>(Arg.Any<GetSystemSettingCriteria>()).Returns(
                new SystemSetting { BatchSize = 100, MaxMessageID = 1989, MsgContentsTypeLst = "ABC" });

            baseService.FetchMultipleAsync<Message, MessageDetail>(Arg.Any<GetMessageFromSpecialDeleteCriteria>()).Returns(
              (new List<Message> { new Message { MessageID = 1 } }, new List<MessageDetail> { new MessageDetail { MessageID = 1 } })
                );
            var fakeData = FakeData(true);
            specialDeleteRepository.ImportMessageAndGetNotification(Arg.Any<ImportMessageFromSpecialDeleteCriteria>()).Returns(fakeData);

            //Actual
            await service.PullTicketAndCreateNotification();

            //Expected
            await chatbotService.DidNotReceive().NotifyToGroup(Arg.Any<string>());
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
            var fakeData = FakeData(false);
            specialDeleteRepository.ImportMessageAndGetNotification(Arg.Any<ImportMessageFromSpecialDeleteCriteria>()).Returns(fakeData);

            //Actual
            await service.PullTicketAndCreateNotification();

            //Expected
            await chatbotService.Received(1).NotifyToGroup(Arg.Any<string>());
        }

        private ValueTuple<CommandResult, IEnumerable<NotificationMessage>> FakeData(bool isSuccessful)
        {
            var command = new CommandResult { Code = isSuccessful ? 0 : 101, Message = "" };
            var msg = new List<NotificationMessage>
            {
                new NotificationMessage
                {
                    NotifictaionId = 1,
                    Message = "ABC",
                    Recipients = new List<ConversationInfo>()
                }
            };
            return new ValueTuple<CommandResult, IEnumerable<NotificationMessage>>(command, msg);
        }

        private SpecialDeleteTicketService GetService()
        {
            baseService = Substitute.For<IBaseService>();
            chatbotService = Substitute.For<IChatbotService>();
            specialDeleteRepository = Substitute.For<ISpecialDeleteRepository>();
            return new SpecialDeleteTicketService(baseService, chatbotService, specialDeleteRepository);
        }
    }
}