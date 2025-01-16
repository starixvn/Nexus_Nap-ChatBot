namespace Fanex.NCB.Main.Controllers.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoFixture;
    using ViewModel.TreeSelect;
    using Models.Common;
    using Services.Interfaces;
    using Fanex.NCB.Shared.Models.OIC;
    using Fanex.NCB.SkypeBot.Controllers;
    using FluentAssertions;
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

            var result = await controller.GetNotificationTypes();

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

            var result = await controller.GetProducts();

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

            var result = await controller.GetNotificationStatuses();

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

            var result = await controller.GetRecipients();

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

            var result = await controller.GetUsers();

            var json = Assert.IsType<JsonResult>(result);
            var returnedUsers = Assert.IsAssignableFrom<IEnumerable<User>>(json.Value);
            Assert.Equal(expectedUsers, returnedUsers);
        }

        [Fact]
        public void GetSettingTypes_WhenCalled_ReturnsSettingTypes()
        {
            // Arrange
            var controller = GetController();
            var expected = new List<SettingType>
            {
                new SettingType
                {
                    Type = "Msg Contents Type",
                    Value = 1
                }
            };

            // Action
            var actual = controller.GetSettingTypes() as JsonResult;

            // Assert
            actual.Value.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetPlatforms_WhenCalled_ReturnJsonResult()
        {
            var controller = GetController();
            var fixture = new Fixture();
            var expectedPlatform = fixture.Create<IEnumerable<Platform>>();

            commonService.GetPlatformAsync(12).Returns(expectedPlatform);

            var result = await controller.GetPlatforms(12);

            var json = Assert.IsType<JsonResult>(result);
            var platforms = Assert.IsAssignableFrom<IEnumerable<Platform>>(json.Value);
            Assert.Equal(expectedPlatform, platforms);
        }

        private CommonController GetController()
        {
            commonService = Substitute.For<ICommonService>();

            return new CommonController(commonService);
        }
    }
}