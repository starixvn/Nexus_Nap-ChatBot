namespace Fanex.NCB.Main.Controllers.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NCB.Main.ViewModel.TreeSelect;
    using Fanex.NCB.Models.Common;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.SkypeBot.Controllers;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using Xunit;

    public class CommonControllerTests
    {
        private ICommonService commonService;

        [Fact]
        public async Task GetNotificationTypes_WhenCalled_ReturnJsonResult()
        {
            var controller = GetController();
            var fixture = new Fixture();
            var expectedTypes = fixture.Create<IEnumerable<NotificationType>>();

            commonService.GetNotificationTypesAsync().Returns(expectedTypes);

            var result = await controller.GetNotificationTypes().ConfigureAwait(false);

            var json = Assert.IsType<JsonResult>(result);
            var returnedTypes = Assert.IsAssignableFrom<IEnumerable<NotificationType>>(json.Value);
            Assert.Equal(expectedTypes, returnedTypes);
        }

        [Fact]
        public async Task GetProducts_WhenCalled_ReturnJsonResult()
        {
            var controller = GetController();
            var fixture = new Fixture();
            var expectedProducts = fixture.Create<IEnumerable<Product>>();

            commonService.GetProductsAsync().Returns(expectedProducts);

            var result = await controller.GetProducts().ConfigureAwait(false);

            var json = Assert.IsType<JsonResult>(result);
            var returnedProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(json.Value);
            Assert.Equal(expectedProducts, returnedProducts);
        }

        [Fact]
        public async Task GetNotificationStatuses_WhenCalled_ReturnJsonResult()
        {
            var controller = GetController();
            var fixture = new Fixture();
            var expectedStatuses = fixture.Create<IEnumerable<NotificationStatus>>();

            commonService.GetNotificationStatusesAsync().Returns(expectedStatuses);

            var result = await controller.GetNotificationStatuses().ConfigureAwait(false);

            var json = Assert.IsType<JsonResult>(result);
            var returnedStatuses = Assert.IsAssignableFrom<IEnumerable<NotificationStatus>>(json.Value);
            Assert.Equal(expectedStatuses, returnedStatuses);
        }

        [Fact]
        public async Task GetRecipients_WhenCalled_ReturnJsonResult()
        {
            var controller = GetController();
            var fixture = new Fixture();
            var expectedRecipients = fixture.Create<IEnumerable<Recipient>>();

            commonService.GetRecipientsAsync().Returns(expectedRecipients);

            var result = await controller.GetRecipients().ConfigureAwait(false);

            var json = Assert.IsType<JsonResult>(result);
            Assert.IsAssignableFrom<IEnumerable<TreeNodeItem>>(json.Value);
        }

        [Fact]
        public async Task GetUsers_WhenCalled_ReturnJsonResult()
        {
            var controller = GetController();
            var fixture = new Fixture();
            var expectedUsers = fixture.Create<IEnumerable<User>>();

            commonService.GetUsersAsync().Returns(expectedUsers);

            var result = await controller.GetUsers().ConfigureAwait(false);

            var json = Assert.IsType<JsonResult>(result);
            var returnedUsers = Assert.IsAssignableFrom<IEnumerable<User>>(json.Value);
            Assert.Equal(expectedUsers, returnedUsers);
        }

        private CommonController GetController()
        {
            commonService = Substitute.For<ICommonService>();

            return new CommonController(commonService);
        }
    }
}