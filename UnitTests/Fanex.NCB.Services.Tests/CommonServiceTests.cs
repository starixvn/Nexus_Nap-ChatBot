namespace Fanex.NCB.Services.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NCB.Criteria.Common;
    using Fanex.NCB.Models.Common;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Models.IntegrationMethod;
    using Fanex.NCB.Shared.Models.OIC;
    using Fanex.NCB.Shared.Services.Interfaces;
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
        public async Task GetNotificationTypesAsync_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var service = GetService();
            var types = fixture.Create<IEnumerable<NotificationType>>();
            baseService.GetDataAsync<NotificationType>(Arg.Any<GetNotificationTypesCriteria>()).Returns(types);

            // Act
            var result = await service.GetNotificationTypesAsync();

            // Assert
            Assert.Equal(types, result);
        }

        [Fact]
        public async Task GetProductsAsync_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var service = GetService();
            var products = fixture.Create<IEnumerable<Models.Common.Product>>();
            baseService.GetDataAsync<Models.Common.Product>(Arg.Any<GetProductsCriteria>()).Returns(products);

            // Act
            var result = await service.GetProductsAsync();

            // Assert
            Assert.Equal(products, result);
        }

        [Fact]
        public async Task GetNotificationStatusesAsync_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var service = GetService();
            var statuses = fixture.Create<IEnumerable<NotificationStatus>>();
            baseService.GetDataAsync<NotificationStatus>(Arg.Any<GetNotificationStatusCriteria>()).Returns(statuses);

            // act
            var result = await service.GetNotificationStatusesAsync();

            // Assert
            Assert.Equal(statuses, result);
        }

        [Fact]
        public async Task GetRecipientsAsync_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var service = GetService();
            var recipients = fixture.Create<IEnumerable<Recipient>>();
            baseService.GetDataAsync<Recipient>(Arg.Any<GetRecipientsCriteria>()).Returns(recipients);

            // Act
            var result = await service.GetRecipientsAsync();

            // Assert
            Assert.Equal(recipients, result);
        }

        [Fact]
        public async Task GetUsersAsync_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var service = GetService();
            var users = fixture.Create<IEnumerable<User>>();
            baseService.GetDataAsync<User>(Arg.Any<GetUsersCriteria>()).Returns(users);

            // Act
            var result = await service.GetUsersAsync();

            // Assert
            Assert.Equal(users, result);
        }

        [Fact]
        public async Task GetConversationChannelAsync_WhenCalled_ReturnExpectedChannels()
        {
            // Arrange
            var channels = fixture.CreateMany<ChannelInfo>();
            var service = GetService();
            baseService.GetDataAsync<ChannelInfo>(Arg.Any<GetConversationChannelCriteria>()).Returns(channels);

            // Act
            var result = await service.GetConversationChannelAsync();

            // Assert
            Assert.Equal(channels, result);
        }

        [Fact]
        public async Task GetPlatformAsync_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var service = GetService();
            var plaforms = fixture.Create<IEnumerable<Platform>>();
            baseService.GetDataAsync<Platform>(Arg.Any<GetPlatformCriteria>()).Returns(plaforms);

            // Act
            var result = await service.GetPlatformAsync(12);

            // Assert
            Assert.Equal(plaforms, result);
        }

        //[Fact]
        //public void GetCategogyName_WhenCalled_ReturnExpectedResult()
        //{
        //    // Arrange
        //    var service = GetService();
        //    remotingRefService.GetResourceDataByRefIdAndResourceId("Category", "1", "EN", string.Empty).Returns("categoryName");

        //    // Act
        //    var result = service.GetCategogyName("1", "EN");

        //    // Assert
        //    Assert.Equal("categoryName", result);
        //}

        //[Fact]
        //public void GetCustomerClassName_WhenCalled_ReturnExpectedResult()
        //{
        //    // Arrange
        //    var service = GetService();
        //    remotingRefService.GetResourceDataByRefIdAndResourceId("CustomerClass", "sa", "EN", string.Empty).Returns("customerClassName");

        //    // Act
        //    var result = service.GetCustomerClassName("sa", "EN");

        //    // Assert
        //    Assert.Equal("customerClassName", result);
        //}

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

        //[Fact]
        //public async Task GetServiceSetting_WhenCalled_ReturnExpectedResult()
        //{
        //    // Arrange
        //    var service = GetService();
        //    var serviceSetting = fixture.Create<ServiceSetting>();
        //    baseService.GetDataEntityAsync<ServiceSetting>(Arg.Any<GetServiceSettingCriteria>()).Returns(serviceSetting);

        //    // Act
        //    var result = await service.GetServiceSetting("tableName");

        //    // Assert
        //    Assert.Equal(serviceSetting, result);
        //}

        [Fact]
        public async Task GetIntegrationMethodAsync_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var service = GetService();
            var integrationMethodInfo = fixture.Create<IEnumerable<IntegrationMethodInfo>>();
            baseService.GetDataAsync<IntegrationMethodInfo>(Arg.Any<GetIntegrationMethodCriteria>()).Returns(integrationMethodInfo);

            // Act
            var result = await service.GetIntegrationMethodAsync(1);

            // Assert
            Assert.Equal(integrationMethodInfo, result);
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

        private CommonService GetService()
        {
            baseService = Substitute.For<IBaseService>();
            remotingRefService = Substitute.For<IRemotingRefService>();
            chatbotService = Substitute.For<IChatbotService>();
            pushSignalService = Substitute.For<IPushSignalRService>();
            logService = Substitute.For<ILogService>();

            return new CommonService(baseService, remotingRefService, chatbotService, pushSignalService, logService);
        }
    }
}