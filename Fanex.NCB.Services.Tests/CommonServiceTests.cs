namespace Fanex.NCB.Services.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NCB.Criteria.Common;
    using Fanex.NCB.Models.Common;
    using Fanex.NCB.Shared.Services.Interfaces;
    using NSubstitute;
    using Xunit;

    public class CommonServiceTests
    {
        private IBaseService baseService;

        [Fact]
        public async Task GetNotificationTypesAsync_WhenCalled_ReturnExpectedResult()
        {
            var service = GetService();

            var fixture = new Fixture();

            var types = fixture.Create<IEnumerable<NotificationType>>();

            baseService.GetDataAsync<NotificationType>(Arg.Any<GetNotificationTypesCriteria>()).Returns(types);

            var result = await service.GetNotificationTypesAsync().ConfigureAwait(false);

            Assert.Equal(types, result);
        }

        [Fact]
        public async Task GetProductsAsync_WhenCalled_ReturnExpectedResult()
        {
            var service = GetService();

            var fixture = new Fixture();

            var products = fixture.Create<IEnumerable<Product>>();

            baseService.GetDataAsync<Product>(Arg.Any<GetProductsCriteria>()).Returns(products);

            var result = await service.GetProductsAsync().ConfigureAwait(false);

            Assert.Equal(products, result);
        }

        [Fact]
        public async Task GetNotificationStatusesAsync_WhenCalled_ReturnExpectedResult()
        {
            var service = GetService();

            var fixture = new Fixture();

            var statuses = fixture.Create<IEnumerable<NotificationStatus>>();

            baseService.GetDataAsync<NotificationStatus>(Arg.Any<GetNotificationStatusCriteria>()).Returns(statuses);

            var result = await service.GetNotificationStatusesAsync().ConfigureAwait(false);

            Assert.Equal(statuses, result);
        }

        [Fact]
        public async Task GetRecipientsAsync_WhenCalled_ReturnExpectedResult()
        {
            var service = GetService();

            var fixture = new Fixture();

            var recipients = fixture.Create<IEnumerable<Recipient>>();

            baseService.GetDataAsync<Recipient>(Arg.Any<GetRecipientsCriteria>()).Returns(recipients);

            var result = await service.GetRecipientsAsync().ConfigureAwait(false);

            Assert.Equal(recipients, result);
        }

        [Fact]
        public async Task GetUsersAsync_WhenCalled_ReturnExpectedResult()
        {
            var service = GetService();

            var fixture = new Fixture();

            var users = fixture.Create<IEnumerable<User>>();

            baseService.GetDataAsync<User>(Arg.Any<GetUsersCriteria>()).Returns(users);

            var result = await service.GetUsersAsync().ConfigureAwait(false);

            Assert.Equal(users, result);
        }

        private CommonService GetService()
        {
            baseService = Substitute.For<IBaseService>();

            return new CommonService(baseService);
        }
    }
}