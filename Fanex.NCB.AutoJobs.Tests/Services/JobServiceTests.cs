namespace Fanex.NCB.AutoJobs.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using Fanex.NCB.AutoJobs.App_Configs;
    using Fanex.NCB.AutoJobs.Services.Implements;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Microsoft.Extensions.Options;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using Xunit;

    public class JobServiceTests
    {
        private ISendNotificationService sendNotificationService;
        private ISyncDataService syncDataService;
        private IChatbotService chatbotService;
        private IOptionsSnapshot<MonitorChatbot> monitorOptions;
        private ILogger logger;

        [Fact]
        public async Task AutoScanAndSendNotifictaion_WhenCalled_ExecuteSuccessed()
        {
            var service = GetService();

            //Actual
            await service.AutoScanAndSendNotifictaion();

            //Assert
            await sendNotificationService.Received().ScanAndSendNotification();
        }

        [Fact]
        public async Task AutoScanAndSendNotifictaion_WhenCalled_ThrowException()
        {
            var service = GetService();
            sendNotificationService.ScanAndSendNotification().Throws(new Exception());

            //Assert
            await Assert.ThrowsAsync<Exception>(() => service.AutoScanAndSendNotifictaion());
            await logger.Received().ErrorAsync(Arg.Any<string>());
        }

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
            sendNotificationService = Substitute.For<ISendNotificationService>();
            syncDataService = Substitute.For<ISyncDataService>();
            chatbotService = Substitute.For<IChatbotService>();
            monitorOptions = Substitute.For<IOptionsSnapshot<MonitorChatbot>>();
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
            monitorOptions.Value.Returns(option);
            logger = Substitute.For<ILogger>();
            return new JobService(sendNotificationService, syncDataService, chatbotService, logger, monitorOptions);
        }

        #endregion Helper
    }
}