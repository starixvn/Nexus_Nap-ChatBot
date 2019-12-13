namespace Fanex.NCB.Services.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.Criteria.SpecialDeleteTicket;
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

        [Fact]
        public async Task PullTicketAndCreateNotification_WhenGetSettingIsNull_DoNothing()
        {
            var service = GetService();
            baseService.GetDataEntityAsync<SystemSetting>(Arg.Any<GetSystemSettingCriteria>()).Returns((SystemSetting)null);

            //Actual
            await service.PullTicketAndCreateNotification();

            //Expected
            await baseService.DidNotReceiveWithAnyArgs().FetchMultipleAsync<Message, MessageDetail>(Arg.Any<GetMessageFromSpecialDeleteCriteria>());
            await baseService.DidNotReceive().ExecuteNonQueryAsync(Arg.Any<ImportMessageFromSpecialDeleteCommand>());
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
            await baseService.DidNotReceive().ExecuteNonQueryAsync(Arg.Any<ImportMessageFromSpecialDeleteCommand>());
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
            baseService.ExecuteNonQueryAsync(Arg.Any<ImportMessageFromSpecialDeleteCommand>()).Returns(new CommandResult { Code = 0 });

            //Actual
            await service.PullTicketAndCreateNotification();

            //Expected
            await chatbotService.DidNotReceive().NotifySponDemoChatGroup(Arg.Any<string>());
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
            baseService.ExecuteNonQueryAsync(Arg.Any<ImportMessageFromSpecialDeleteCommand>()).Returns(new CommandResult { Code = 1 });

            //Actual
            await service.PullTicketAndCreateNotification();

            //Expected
            await chatbotService.Received(1).NotifySponDemoChatGroup(Arg.Any<string>());
        }

        private SpecialDeleteTicketService GetService()
        {
            baseService = Substitute.For<IBaseService>();
            chatbotService = Substitute.For<IChatbotService>();
            return new SpecialDeleteTicketService(baseService, chatbotService);
        }
    }
}