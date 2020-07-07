namespace Fanex.NCB.Services.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NCB.Core.Constants;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Criteria.SpecialDeleteTicket;
    using Fanex.NCB.Models.NotifictaionSendMessage;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Models.SpecialDeleteTicket;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options;
    using NSubstitute;
    using Xunit;

    public class SpecialDeleteServiceTests
    {
        private IBaseService baseService;
        private IChatbotService chatbotService;
        private ISendNotificationService sendNotificationService;
        private IPushSignalService pushSignalService;
        private IOptionsSnapshot<List<HangfireJobSetting>> jobs;
        private IConfiguration configuration;

        [Fact]
        public async Task VoidingTicketToLicensee_WhenGetSettingIsNull_DoNothing()
        {
            var service = GetService();

            baseService.GetDataEntityAsync<SystemSetting>(Arg.Any<GetSystemSettingCriteria>()).Returns((SystemSetting)null);

            //Actual
            await service.VoidingTicketToLicensee();

            //Expected
            await baseService.DidNotReceiveWithAnyArgs().FetchMultipleAsync<Message, MessageDetail>(Arg.Any<GetMessageFromSpecialDeleteCriteria>());
            await baseService.DidNotReceive().FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(Arg.Any<ImportMessageFromSpecialDeleteCriteria>());

            //Assert
            await baseService.DidNotReceive().FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(Arg.Any<ImportMessageFromSpecialDeleteCriteria>());
            await chatbotService.DidNotReceive().SendMessageToMultitpleAsync(Arg.Any<List<NotificationMessage>>());
        }

        [Fact]
        public async Task VoidingTicketToLicensee_NoTicketToImport_DoNothing()
        {
            var service = GetService();
            baseService.GetDataEntityAsync<SystemSetting>(Arg.Any<GetSystemSettingCriteria>()).Returns(
                new SystemSetting { BatchSize = 100, MaxMessageID = 1989, MsgContentsTypeLst = "ABC" });

            baseService.FetchMultipleAsync<Message, MessageDetail>(Arg.Any<GetMessageFromSpecialDeleteCriteria>()).Returns(
              (new List<Message>(), new List<MessageDetail>())
                );
            //Actual
            await service.VoidingTicketToLicensee();

            //Expected
            await baseService.DidNotReceive().FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(Arg.Any<ImportMessageFromSpecialDeleteCriteria>());
        }

        [Fact]
        public async Task VoidingTicketToLicensee_HaveTicketToImport_ExecuteSpImportAndSendCarbonNotification()
        {
            var service = GetService();
            var fixture = new Fixture();
            baseService.GetDataEntityAsync<SystemSetting>(Arg.Any<GetSystemSettingCriteria>()).Returns(
                new SystemSetting { BatchSize = 100, MaxMessageID = 1989, MsgContentsTypeLst = "ABC" });

            baseService.FetchMultipleAsync<Message, MessageDetail>(Arg.Any<GetMessageFromSpecialDeleteCriteria>()).Returns(
              (fixture.Create<List<Message>>(), fixture.Create<List<MessageDetail>>()));

            baseService.FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(Arg.Any<ImportMessageFromSpecialDeleteCriteria>()).Returns(FakeData());

            //Actual
            await service.VoidingTicketToLicensee();

            //Expected
            await baseService.Received().FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(Arg.Any<ImportMessageFromSpecialDeleteCriteria>());
        }

        [Fact]
        public async Task VoidingAbnormalBetsToInternalDepartment_WhenGetSettingIsNull_DoNothing()
        {
            var service = GetService();

            baseService.GetDataEntityAsync<SystemSetting>(Arg.Any<GetAbnormalBetSystemSettingCriteria>()).Returns((SystemSetting)null);

            //Actual
            await service.VoidingAbnormalBetsToInternalDepartment();

            //Assert
            await baseService.DidNotReceiveWithAnyArgs().GetDataAsync<Message>(Arg.Any<GetAbnormalBetsMessageCriteria>());
            await baseService.DidNotReceive().FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(Arg.Any<ImportMessageFromSpecialDeleteCriteria>());
            await chatbotService.DidNotReceive().SendMessageToMultitpleAsync(Arg.Any<List<NotificationMessage>>());
        }

        [Fact]
        public async Task VoidingAbnormalBetsToInternalDepartment_HaveTicketToImport_CallSPImport()
        {
            var service = GetService();
            var fixture = new Fixture();
            baseService.GetDataEntityAsync<SystemSetting>(Arg.Any<GetAbnormalBetSystemSettingCriteria>())
                .Returns(new SystemSetting { BatchSize = 100, MaxMessageID = 1989, MsgContentsTypeLst = "103" });

            baseService.GetDataAsync<Message>(Arg.Any<GetAbnormalBetsMessageCriteria>()).Returns(fixture.Create<List<Message>>());
            baseService.GetDataAsync<string>(Arg.Any<GetListMessageAbnormalbetsFromYesterdayToCurrentCriteria>()).Returns(fixture.Create<List<string>>());
            baseService.GetDataAsync<AbnormalBetsMessage>(Arg.Any<GetAbnormalBetInfoCriteria>()).Returns(fixture.Create<IEnumerable<AbnormalBetsMessage>>());
            //Actual
            await service.VoidingAbnormalBetsToInternalDepartment();

            //Expected
            await baseService.DidNotReceive().FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(Arg.Any<ImportMessageFromSpecialDeleteCriteria>());
        }

        private ValueTuple<IEnumerable<NotificationRecipient>, IEnumerable<NotificationSendMessage>> FakeData()
        {
            var recipients = new List<NotificationRecipient>
            {
                new NotificationRecipient {
                    BotConversationId = "",
                    BotName = "",
                    ChatGroupConversationId = "ConversationId",
                    NotificationId = 1,
                    ChannelId = "ABC",
                    ServiceUrl = "test.com"
                }
            };
            var notifications = new List<NotificationSendMessage>
            {
                new NotificationSendMessage { NotificationId = 1, Content = "", Title = "" }
            };

            return new ValueTuple<IEnumerable<NotificationRecipient>, IEnumerable<NotificationSendMessage>>(recipients, notifications);
        }

        private SpecialDeleteService GetService()
        {
            baseService = Substitute.For<IBaseService>();
            chatbotService = Substitute.For<IChatbotService>();
            sendNotificationService = Substitute.For<ISendNotificationService>();
            jobs = Substitute.For<IOptionsSnapshot<List<HangfireJobSetting>>>();
            var hangfireJobs = new List<HangfireJobSetting>
            {
                new HangfireJobSetting { IsEnabled = true, JobName = HangfireJobName.VoidingTicketForLicensee},
                new HangfireJobSetting {IsEnabled = true, JobName = HangfireJobName.VoidingAbnormalBets}
            };
            jobs.Value.Returns(hangfireJobs);
            pushSignalService = Substitute.For<IPushSignalService>();
            configuration = Substitute.For<IConfiguration>();

            return new SpecialDeleteService(baseService, chatbotService, pushSignalService, configuration, jobs);
        }
    }
}