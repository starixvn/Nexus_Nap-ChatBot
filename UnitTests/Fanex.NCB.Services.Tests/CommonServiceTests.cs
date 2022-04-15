namespace Fanex.NCB.Services.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NCB.Criteria.Common;
    using Fanex.NCB.Models.Common;
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
            var channels = fixture.Create<IEnumerable<ConversationChannel>>();
            var service = GetService();
            baseService.GetDataAsync<ConversationChannel>(Arg.Any<GetConversationChannelCriteria>()).Returns(channels);

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

        private CommonService GetService()
        {
            baseService = Substitute.For<IBaseService>();
            remotingRefService = Substitute.For<IRemotingRefService>();

            return new CommonService(baseService, remotingRefService);
        }
    }
}