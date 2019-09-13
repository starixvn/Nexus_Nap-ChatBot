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
    using RestSharp;
    using Xunit;

    public class SendNotificationServiceTests
    {
        private IChatbotService chatbotService;
        private IBaseService baseService;
        private IOptionsSnapshot<AppSettings> appSettings;

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
            await chatbotService.DidNotReceive().SendMessageToMultitpleAsync(Arg.Any<IEnumerable<NotificationMessage>>());
            await baseService.DidNotReceive().ExecuteNonQueryAsync(Arg.Any<INonQueryCommand>());
        }

        [Fact]
        public async Task ScanAndSendNotification_ScanHasNoti_UpdateStatusAndSendNoti_UpdateWhenSendFailed()
        {
            var notifications = FakeData();
            var service = GetService();
            baseService.FetchMultipleAsync<NotificationSendMessage, NotificationRecipient>(Arg.Any<GetNotificationsToSendCriteria>()).Returns(notifications);

            //Actual
            await service.ScanAndSendNotification();

            //Assert
            await chatbotService.Received().SendMessageToMultitpleAsync(Arg.Any<IEnumerable<NotificationMessage>>());
            await baseService.Received().ExecuteNonQueryAsync(Arg.Any<INonQueryCommand>());
        }

        [Fact]
        public async Task ScanAndSendNotification_ScanHasNoti_UpdateStatusAndSendNoti_UpdateWhenSendSuccessed()
        {
            var notifications = FakeData();
            var response = FakeResponse();
            var service = GetService();
            baseService.FetchMultipleAsync<NotificationSendMessage, NotificationRecipient>(Arg.Any<GetNotificationsToSendCriteria>()).Returns(notifications);
            chatbotService.SendMessageToMultitpleAsync(Arg.Any<IEnumerable<NotificationMessage>>()).Returns(response);
            //Actual
            await service.ScanAndSendNotification();

            //Assert
            await chatbotService.Received().SendMessageToMultitpleAsync(Arg.Any<IEnumerable<NotificationMessage>>());
            await baseService.Received(2).ExecuteNonQueryAsync(Arg.Any<INonQueryCommand>());
        }

        [Fact]
        public async Task SendNotificationImmediately_UpdateStatusAndSendNoti_UpdateWhenSendSuccessed()
        {
            var notifications = FakeData();
            var response = FakeResponse();
            var service = GetService();
            baseService.FetchMultipleAsync<NotificationSendMessage, NotificationRecipient>(Arg.Any<GetNotificationByIdCriteria>()).Returns(notifications);
            chatbotService.SendMessageToMultitpleAsync(Arg.Any<IEnumerable<NotificationMessage>>()).Returns(response);
            //Actual
            await service.SendNotificationImmediately(1);

            //Assert
            await chatbotService.Received().SendMessageToMultitpleAsync(Arg.Any<IEnumerable<NotificationMessage>>());
            await baseService.Received(2).ExecuteNonQueryAsync(Arg.Any<INonQueryCommand>());
        }

        #region Helper

        private ValueTuple<IEnumerable<NotificationSendMessage>, IEnumerable<NotificationRecipient>> FakeData()
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

        private IRestResponse FakeResponse()
        {
            return new RestResponse
            {
                StatusCode = HttpStatusCode.OK,
                ResponseStatus = ResponseStatus.Completed,
                ContentType = "application/json",
                Content = "[{\"NotificationId\": 1, \"ConversationId\": \"ABC\", \"ErrCode\": 0,\"ErrorMessage\": \"\", \"SentTime\": \"2019-01-01\"},{\"NotificationId\": 2, \"ConversationId\": \"DEF\", \"ErrCode\": 1,\"ErrorMessage\": \"\", \"SentTime\": \"2019-01-01\"}]"
            };
        }

        private SendNotificationService GetService()
        {
            chatbotService = Substitute.For<IChatbotService>();
            baseService = Substitute.For<IBaseService>();

            var settings = new AppSettings
            {
                SiteId = 69,
                ItemEachBatchSendNotification = 10
            };
            appSettings = Substitute.For<IOptionsSnapshot<AppSettings>>();
            appSettings.Value.Returns(settings);

            return new SendNotificationService(chatbotService, baseService, appSettings);
        }

        #endregion Helper
    }
}