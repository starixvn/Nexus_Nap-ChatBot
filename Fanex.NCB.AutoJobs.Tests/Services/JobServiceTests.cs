namespace Fanex.NCB.AutoJobs.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using Fanex.NCB.AutoJobs.Constants;
    using Fanex.NCB.AutoJobs.Services.Implements;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Microsoft.Extensions.Options;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using Xunit;

    public class JobServiceTests
    {
        private ISyncDataService syncDataService;
        private IChatbotService chatbotService;
        private ISpecialDeleteTicketService specialDeleteTicketService;
        private IOptionsMonitor<List<HangfireJobSetting>> jobSettings;
        private IOptionsSnapshot<MonitorChatbot> monitorOptions;
        private ILogger logger;

        [Fact]
        public async Task SyncDataFromNapUserControl_WhenCalled_ExecuteSuccessed()
        {
            var service = GetService();

            //Actual
            await service.SyncDataFromNapUserControl();

            //Assert
            await syncDataService.Received().NapUserControlSyncDataAsync();
        }

        [Fact]
        public async Task SyncDataFromNapUserControl_WhenCalled_ThrowException()
        {
            var service = GetService();
            syncDataService.NapUserControlSyncDataAsync().Throws(new Exception());

            //Assert
            await Assert.ThrowsAsync<Exception>(() => service.SyncDataFromNapUserControl());
            await logger.Received().ErrorAsync(Arg.Any<string>());
        }

        [Fact]
        public async Task SyncDataFromOIC_WhenCalled_ExecuteSuccessed()
        {
            var service = GetService();

            //Actual
            await service.SyncDataFromOIC();

            //Assert
            await syncDataService.Received().OicSyncDataAsync();
        }

        [Fact]
        public async Task SyncDataFromOIC_WhenCalled_ThrowException()
        {
            var service = GetService();
            syncDataService.OicSyncDataAsync().Throws(new Exception());

            //Assert
            await Assert.ThrowsAsync<Exception>(() => service.SyncDataFromOIC());
            await logger.Received().ErrorAsync(Arg.Any<string>());
        }

        [Fact]
        public async Task PullTicketFromSpecialDeleteAndCreateNotification_WhenCalled_ExecuteSuccessed()
        {
            var service = GetService();

            //Actual
            await service.PullTicketCreateNotificationAndScanNotification();

            //Assert
            await specialDeleteTicketService.Received().PullTicketAndCreateNotification();
        }

        [Fact]
        public async Task PullTicketFromSpecialDeleteAndCreateNotification_WhenCalled_ThrowException()
        {
            var service = GetService();
            specialDeleteTicketService.PullTicketAndCreateNotification().Throws(new Exception());

            //Assert
            await Assert.ThrowsAsync<Exception>(() => service.PullTicketCreateNotificationAndScanNotification());
            await chatbotService.Received().NotifyToGroup(Arg.Any<string>());
            await logger.Received().ErrorAsync(Arg.Any<string>());
        }

        [Fact]
        public async Task MonitorChatbot_WhenCalled_ExecuteSuccessed()
        {
            var service = GetService();

            //Actual
            await service.MonitorChatbot();

            //Assert
            await chatbotService.Received().SendMessageToMultitpleAsync(Arg.Any<List<NotificationMessage>>());
        }

        [Fact]
        public async Task MonitorChatbot_WhenCalled_ThrowException()
        {
            var service = GetService();
            chatbotService.SendMessageToMultitpleAsync(Arg.Any<List<NotificationMessage>>()).Throws(new Exception());

            //Assert
            await Assert.ThrowsAsync<Exception>(() => service.MonitorChatbot());
            await logger.Received().ErrorAsync(Arg.Any<string>());
        }

        #region Helper

        private JobService GetService()
        {
            syncDataService = Substitute.For<ISyncDataService>();
            chatbotService = Substitute.For<IChatbotService>();
            specialDeleteTicketService = Substitute.For<ISpecialDeleteTicketService>();
            monitorOptions = Substitute.For<IOptionsSnapshot<MonitorChatbot>>();
            jobSettings = Substitute.For<IOptionsMonitor<List<HangfireJobSetting>>>();
            var option = new MonitorChatbot
            {
                IsMonitoring = true,
                BotConversationId = "A",
                BotName = "A",
                ChannelId = "A",
                ConversationId = "",
                Message = "",
                ServiceUrl = "dsa"
            };
            var settings = new List<HangfireJobSetting>
            {
                new HangfireJobSetting {JobName = HangfireJob.SendNotification, CronExpression = "* * * * *", IsEnabled = true},
                new HangfireJobSetting {JobName = HangfireJob.SyncNapUserControlData, CronExpression = "* * * * *", IsEnabled = true},
                new HangfireJobSetting {JobName = HangfireJob.SyncOICData, CronExpression = "* * * * *", IsEnabled = true},
                new HangfireJobSetting {JobName = HangfireJob.PullTicketFromSpecialDeleteAndScanNoti, CronExpression = "* * * * *", IsEnabled = true},
                new HangfireJobSetting {JobName = HangfireJob.MonitorSkypebot, CronExpression = "* * * * *", IsEnabled = true}
            };
            jobSettings.CurrentValue.Returns(settings);
            monitorOptions.Value.Returns(option);
            logger = Substitute.For<ILogger>();
            return new JobService(syncDataService, chatbotService, specialDeleteTicketService,
                logger, jobSettings, monitorOptions);
        }

        #endregion Helper
    }
}