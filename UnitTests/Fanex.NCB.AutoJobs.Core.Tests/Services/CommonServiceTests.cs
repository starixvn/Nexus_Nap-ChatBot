namespace Fanex.NCB.AutoJobs.Core.Tests.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NCB.AutoJobs.Core.Criteria.Common;
    using Fanex.NCB.AutoJobs.Core.Criteria.SpecialDeleteTicket;
    using Fanex.NCB.AutoJobs.Core.Models.Common;
    using Fanex.NCB.AutoJobs.Core.Models.Core;
    using Fanex.NCB.AutoJobs.Core.Models.NotifictaionSendMessage;
    using Fanex.NCB.AutoJobs.Core.Services;
    using Fanex.NCB.AutoJobs.Core.Services.Interfaces;
    using Fanex.RemotingRef;
    using NSubstitute;
    using Xunit;

    public class CommonServiceTests
    {
        private readonly IFixture fixture = new Fixture();
        private IBaseService baseService;
        private IRemotingRefService remotingRefService;
        private IChatbotService chatbotService;
        private IPushSignalRService pushSignalService;
        private ILogService logService;

        [Fact]
        public async Task DeleteOldLog_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var service = GetService();

            // Act
            await service.DeleteOldLog("currentDate");

            // Assert
            await baseService.Received().ExecuteNonQueryAsync(Arg.Any<DeleteOldConversationMessageLogCommand>());
        }

        [Fact]
        public void GetCategogyName_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var service = GetService();
            remotingRefService.GetResourceDataByRefIdAndResourceId("Category", "1", "EN", string.Empty).Returns("categoryName");

            // Act
            var result = service.GetCategogyName("1", "EN");

            // Assert
            Assert.Equal("categoryName", result);
        }

        [Fact]
        public void GetCustomerClassName_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var service = GetService();
            remotingRefService.GetResourceDataByRefIdAndResourceId("CustomerClass", "sa", "EN", string.Empty).Returns("customerClassName");

            // Act
            var result = service.GetCustomerClassName("sa", "EN");

            // Assert
            Assert.Equal("customerClassName", result);
        }

        [Fact]
        public async Task GetJsonDataFromDbACCAsync_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var service = GetService();
            baseService.GetDataEntityAsync<string>(Arg.Any<GetJsonDataOnDbACCCriteria>()).Returns("jsonData");

            // Act
            var result = await service.GetJsonDataFromDbACCAsync("tableName");

            // Assert
            Assert.Equal("jsonData", result);
        }

        [Fact]
        public async Task GetServiceSetting_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var service = GetService();
            var serviceSetting = fixture.Create<ServiceSetting>();
            baseService.GetDataEntityAsync<ServiceSetting>(Arg.Any<GetServiceSettingCriteria>()).Returns(serviceSetting);

            // Act
            var result = await service.GetServiceSetting("tableName");

            // Assert
            Assert.Equal(serviceSetting, result);
        }

        [Fact]
        public async Task OnAfterImportDataAsync_IsSuccessful_DoAsExpected()
        {
            // Arrange
            var commandResult = new CommandResult { Code = 0 };
            var notifications = fixture.Create<IEnumerable<NotificationMessage>>();
            var notificationIds = string.Join(",", notifications.Select(x => x.NotificationId));
            var service = GetService();

            // Act
            await service.OnAfterImportDataAsync(commandResult, notifications);

            // Assert
            await chatbotService.DidNotReceive().NotifyToChatGroupAsync(Arg.Any<string>());
            await pushSignalService.Received().PushNewNotification(notificationIds);
        }

        [Fact]
        public async Task OnAfterImportDataAsync_IsUnsuccessful_DoAsExpected()
        {
            // Arrange
            var commandResult = new CommandResult { Code = 1, Message = "error" };
            var notifications = fixture.Create<IEnumerable<NotificationMessage>>();
            var notificationIds = string.Join(",", notifications.Select(x => x.NotificationId));
            var service = GetService();

            // Act
            await service.OnAfterImportDataAsync(commandResult, notifications);

            // Assert
            await chatbotService.Received().NotifyToChatGroupAsync("error");
            await pushSignalService.DidNotReceive().PushNewNotification(notificationIds);
        }

        [Fact]
        public async Task SendMessageByChannelAsync_WhenCalled_CallExpectedFunction()
        {
            // Arrange
            var service = GetService();
            var notifications = new List<NotificationMessage>
            {
                new() { ChannelId = 1, Message = "", NotificationId = 1 },
                new() { ChannelId = 2, Message = "", NotificationId = 1 }
            };

            // Act
            await service.SendMessageByChannelAsync(1, notifications);

            // Assert
            await chatbotService.Received(1).SendMultiMessageAsync(1, Arg.Any<IEnumerable<NotificationMessage>>());
        }


        private CommonService GetService()
        {
            baseService = Substitute.For<IBaseService>();
            remotingRefService = Substitute.For<IRemotingRefService>();
            chatbotService = Substitute.For<IChatbotService>();
            pushSignalService = Substitute.For<IPushSignalRService>();
            logService = Substitute.For<ILogService>();

            return new CommonService(baseService, remotingRefService, chatbotService, logService, pushSignalService);
        }
    }
}
