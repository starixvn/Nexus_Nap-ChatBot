namespace Fanex.NCB.Services.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Criteria.SendNotification;
    using Fanex.NCB.Models.NotifictaionSendMessage;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Microsoft.Extensions.Options;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using Xunit;

    public class SendNotificationServiceTests
    {
        private IChatbotService chatbotService;
        private IBaseService baseService;
        private IPushSignalService pushSignalService;
        private IOptionsSnapshot<AppSettings> appSettings;
        private IOptionsSnapshot<MonitorChatbot> monitor;
        private ILogService logger;

        [Fact]
        public async Task ScanAndSendNotification_ScanEmptyNoti_NotUpdateStatusAndSendNoti()
        {
            var notifications = new List<NotificationSendMessage>();
            var recipients = new List<NotificationRecipient>();
            var emptyListNoti = new ValueTuple<IEnumerable<NotificationSendMessage>, IEnumerable<NotificationRecipient>>(notifications, recipients);
            var service = GetService();
            baseService.FetchMultipleAsync<NotificationSendMessage, NotificationRecipient>(Arg.Any<GetNotificationsToSendCriteria>()).Returns(emptyListNoti);

            //Actual
            await service.ScanAndSendNotification();

            //Assert
            await chatbotService.DidNotReceive().SendMessageToMultitpleAsync(Arg.Any<List<NotificationMessage>>());
            await baseService.DidNotReceive().ExecuteNonQueryAsync(Arg.Any<INonQueryCommand>());
        }

        [Fact]
        public async Task ScanAndSendNotification_ScanHasNoti_UpdateStatusAndSendNoti_UpdateWhenSendFailed()
        {
            var notifications = FakeData();
            var service = GetService();
            var response = FakeResponse(false);
            baseService.FetchMultipleAsync<NotificationSendMessage, NotificationRecipient>(Arg.Any<GetNotificationsToSendCriteria>()).Returns(notifications);
            chatbotService.SendMessageToMultitpleAsync(Arg.Any<List<NotificationMessage>>()).Returns(response);

            //Actual
            await service.ScanAndSendNotification();

            //Assert
            await chatbotService.Received().SendMessageToMultitpleAsync(Arg.Any<List<NotificationMessage>>());
            await baseService.Received().ExecuteNonQueryAsync(Arg.Any<INonQueryCommand>());
        }

        [Fact]
        public async Task ScanAndSendNotification_ScanHasNoti_UpdateStatusAndSendNoti_UpdateWhenSendSuccessed()
        {
            var notifications = FakeData();
            var response = FakeResponse(true);
            var service = GetService();
            baseService.FetchMultipleAsync<NotificationSendMessage, NotificationRecipient>(Arg.Any<GetNotificationsToSendCriteria>()).Returns(notifications);
            chatbotService.SendMessageToMultitpleAsync(Arg.Any<List<NotificationMessage>>()).Returns(response);
            //Actual
            await service.ScanAndSendNotification();

            //Assert
            await chatbotService.Received().SendMessageToMultitpleAsync(Arg.Any<List<NotificationMessage>>());
        }

        [Fact]
        public async Task ScanAndSendNotification_ScanHasNoti_ChatbotServiceThrowExceptions()
        {
            var notifications = FakeData();
            var service = GetService();
            baseService.FetchMultipleAsync<NotificationSendMessage, NotificationRecipient>(Arg.Any<GetNotificationsToSendCriteria>()).Returns(notifications);

            chatbotService.SendMessageToMultitpleAsync(Arg.Any<List<NotificationMessage>>()).Throws(new Exception());
            await service.ScanAndSendNotification();
            //Assert
            await baseService.Received(2).ExecuteNonQueryAsync(Arg.Any<INonQueryCommand>());
        }

        [Fact]
        public async Task SendNotificationImmediately_IsCreating_UpdateStatusAndSendNoti_UpdateWhenSendSuccessed()
        {
            var notifications = FakeData();
            var response = FakeResponse(true);
            var service = GetService();
            baseService.FetchMultipleAsync<NotificationSendMessage, NotificationRecipient>(Arg.Any<GetNotificationByIdCriteria>()).Returns(notifications);
            chatbotService.SendMessageToMultitpleAsync(Arg.Any<List<NotificationMessage>>()).Returns(response);
            //Actual
            await service.SendNotificationImmediately(1);
            PushSignalNewNotification("1");
            //Assert
            await chatbotService.Received().SendMessageToMultitpleAsync(Arg.Any<List<NotificationMessage>>());
            await baseService.Received(2).ExecuteNonQueryAsync(Arg.Any<INonQueryCommand>());
        }

        [Fact]
        public async Task SendNotificationImmediately_NotIsCreating_UpdateStatusAndSendNoti_UpdateWhenSendSuccessed()
        {
            var notifications = FakeData();
            var response = FakeResponse(true);
            var service = GetService();
            baseService.FetchMultipleAsync<NotificationSendMessage, NotificationRecipient>(Arg.Any<GetNotificationByIdCriteria>()).Returns(notifications);
            chatbotService.SendMessageToMultitpleAsync(Arg.Any<List<NotificationMessage>>()).Returns(response);
            //Actual
            await service.SendNotificationImmediately(1);

            PushSignalUpdateNotiStatus("1");
            //Assert
            await chatbotService.Received().SendMessageToMultitpleAsync(Arg.Any<List<NotificationMessage>>());
            await baseService.Received(2).ExecuteNonQueryAsync(Arg.Any<INonQueryCommand>());
        }

        private void PushSignalNewNotification(string notificationIds)
        {
            Task.Run(() => pushSignalService.Received().OnInsertNotification(notificationIds));
        }

        private void PushSignalUpdateNotiStatus(string notificationIds)
        {
            Task.Run(() => pushSignalService.Received().OnUpdateNotificationStatus(notificationIds));
        }

        #region Helper

        private static ValueTuple<IEnumerable<NotificationSendMessage>, IEnumerable<NotificationRecipient>> FakeData()
        {
            var notifications = new List<NotificationSendMessage>
            {
                new NotificationSendMessage { NotificationId = 1, Content = "Text"} ,
                new NotificationSendMessage { NotificationId = 2, Content = "Text 2" }
            };

            var recipients = new List<NotificationRecipient>
            {
                new NotificationRecipient {NotificationId = 1, ErrCode = 0, ChatGroupConversationId = "ABC"},
                new NotificationRecipient {NotificationId = 2, ErrCode = 0, ChatGroupConversationId = "CDE"},
                new NotificationRecipient {NotificationId = 2, ErrCode = 1, ChatGroupConversationId = "FGH"}
            };
            return new ValueTuple<IEnumerable<NotificationSendMessage>, IEnumerable<NotificationRecipient>>(notifications, recipients);
        }

        private static NotificationMessageResponse FakeResponse(bool isSuccess)
        {
            var result = new List<SendMessageResult>
            {
                new SendMessageResult { NotificationId = 1, ConversationId = "1", ErrorCode = 0, ErrorMessage = "", SentTime = DateTime.Now }
            };
            return new NotificationMessageResponse
            {
                IsSuccessStatusCode = isSuccess,
                StatusCode = isSuccess ? HttpStatusCode.OK : HttpStatusCode.BadRequest,
                NotificationMessageResult = result
            };
        }

        private SendNotificationService GetService()
        {
            chatbotService = Substitute.For<IChatbotService>();
            baseService = Substitute.For<IBaseService>();
            logger = Substitute.For<ILogService>();
            monitor = Substitute.For<IOptionsSnapshot<MonitorChatbot>>();

            var monitorChatbot = new MonitorChatbot
            {
                IsMonitoring = true,
                BotConversationId = "",
                ServiceUrl = "",
            };
            var settings = new AppSettings
            {
                SiteId = 69,
                ItemEachBatchSendNotification = 10
            };
            appSettings = Substitute.For<IOptionsSnapshot<AppSettings>>();
            appSettings.Value.Returns(settings);
            monitor.Value.Returns(monitorChatbot);
            pushSignalService = Substitute.For<IPushSignalService>();

            return new SendNotificationService(chatbotService, baseService, pushSignalService, appSettings, monitor, logger);
        }

        #endregion Helper
    }
}