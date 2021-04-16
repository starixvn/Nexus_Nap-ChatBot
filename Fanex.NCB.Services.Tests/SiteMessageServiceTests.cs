namespace Fanex.NCB.Services.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NCB.Criteria.SiteMessage;
    using Fanex.NCB.Criteria.SpecialDeleteTicket;
    using Fanex.NCB.Models.NotifictaionSendMessage;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Models.SiteMessage;
    using Fanex.NCB.Shared.Models.SpecialDeleteTicket;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Microsoft.Extensions.Configuration;
    using NSubstitute;
    using Xunit;

    public class SiteMessageServiceTests
    {
        private IBaseService baseService;
        private IChatbotService chatbotService;
        private IPushSignalService pushSignalService;
        private IConfiguration configuration;

        [Fact]
        public async Task PullMessageFromSiteMessage_NoSettings_DoNothing()
        {
            var service = GetService();

            baseService.GetDataEntityAsync<SystemSetting>(Arg.Any<GetSiteMessageSettingCriteria>()).Returns((SystemSetting)null);

            //Actual
            await service.PullMessageFromSiteMessage();

            //Expected
            await baseService.DidNotReceive().FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(Arg.Any<ImportMessageFromSpecialDeleteCriteria>());
        }

        [Fact]
        public async Task PullMessageFromSiteMessage_NoMessageToImport_DoNothing()
        {
            var service = GetService();
            baseService.GetDataEntityAsync<SystemSetting>(Arg.Any<GetSiteMessageSettingCriteria>()).Returns(
                new SystemSetting { BatchSize = 100, MaxMessageID = 1989, MsgContentsTypeLst = "ABC" });

            baseService.FetchMultipleAsync<SiteMessageModel, SiteMessageDetail>(Arg.Any<GetMessageInfoFromAdminSite>()).Returns(
              (new List<SiteMessageModel>(), new List<SiteMessageDetail>())
                );
            //Actual
            await service.PullMessageFromSiteMessage();

            //Expected
            await baseService.DidNotReceive().FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(Arg.Any<ImportMessageFromSpecialDeleteCriteria>());
        }

        [Fact]
        public async Task PullMessageFromSiteMessage_HaveMessagetToImport_ExecuteSpImportAndSendCarbonNotification()
        {
            var service = GetService();
            var fixture = new Fixture();
            baseService.GetDataEntityAsync<SystemSetting>(Arg.Any<GetSiteMessageSettingCriteria>()).Returns(fixture.Create<SystemSetting>());

            var siteMsgs = new List<SiteMessageModel>
            {
                new SiteMessageModel { MessageID = 1000, MatchID = 10, Content_en = "EN", Content_cs = "CN" },
                fixture.Create<SiteMessageModel>()
            };
            var details = new List<SiteMessageDetail> { new SiteMessageDetail { MessageID = 1000, SiteName = "12bet" }, fixture.Create<SiteMessageDetail>() };

            var matchInfo = new List<SiteMessageMatchInfoModel>
            {
                new SiteMessageMatchInfoModel { MatchID = 10, IsTestMatch = false },
                fixture.Create<SiteMessageMatchInfoModel>()
            };
            baseService.FetchMultipleAsync<SiteMessageModel, SiteMessageDetail>(Arg.Any<GetMessageInfoFromAdminSite>()).Returns(
              (siteMsgs, details));

            baseService.GetDataAsync<SiteMessageMatchInfoModel>(Arg.Any<GetEventInfoCriteria>()).Returns(matchInfo);

            //Actual
            await service.PullMessageFromSiteMessage();

            //Expected
            await baseService.Received().FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(Arg.Any<ImportDataSiteMessageAndGetGroupTestCriteria>());
        }

        [Fact]
        public async Task PullMessageFromSiteMessage_ImportSucess_SendMessageToCarbonGroup()
        {
            var service = GetService();
            var fixture = new Fixture();
            baseService.GetDataEntityAsync<SystemSetting>(Arg.Any<GetSiteMessageSettingCriteria>()).Returns(fixture.Create<SystemSetting>());

            var siteMsgs = new List<SiteMessageModel>
            {
                new SiteMessageModel { MessageID = 1000, MatchID = 10, Content_en = "EN", Content_cs = "CN" },
                fixture.Create<SiteMessageModel>()
            };
            var details = new List<SiteMessageDetail> { new SiteMessageDetail { MessageID = 1000, SiteName = "12bet" },
                fixture.Create<SiteMessageDetail>() };

            var matchInfo = new List<SiteMessageMatchInfoModel>
            {
                new SiteMessageMatchInfoModel { MatchID = 10, IsTestMatch = false },
                fixture.Create<SiteMessageMatchInfoModel>()
            };
            baseService.FetchMultipleAsync<SiteMessageModel, SiteMessageDetail>(Arg.Any<GetMessageInfoFromAdminSite>()).Returns(
              (siteMsgs, details));

            baseService.GetDataAsync<SiteMessageMatchInfoModel>(Arg.Any<GetEventInfoCriteria>()).Returns(matchInfo);
            baseService.FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(Arg.Any<ImportDataSiteMessageAndGetGroupTestCriteria>()).Returns(FakeData());
            chatbotService.GenerateNotifificationMessage(Arg.Any<List<NotificationSendMessage>>(), Arg.Any<List<NotificationRecipient>>())
                .Returns(fixture.Create<List<NotificationMessage>>());

            //Actual
            await service.PullMessageFromSiteMessage();

            //Expected
            await chatbotService.Received().SendMessageToMultitpleAsync(Arg.Any<List<NotificationMessage>>());
            await PushSignalRReceiveCall();
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

        private async Task PushSignalRReceiveCall()
        {
            await pushSignalService.Received().OnInsertNotification(Arg.Any<string>());
        }

        private SiteMessageService GetService()
        {
            baseService = Substitute.For<IBaseService>();
            chatbotService = Substitute.For<IChatbotService>();
            configuration = Substitute.For<IConfiguration>();
            pushSignalService = Substitute.For<IPushSignalService>();

            return new SiteMessageService(baseService, chatbotService, pushSignalService, configuration);
        }
    }
}