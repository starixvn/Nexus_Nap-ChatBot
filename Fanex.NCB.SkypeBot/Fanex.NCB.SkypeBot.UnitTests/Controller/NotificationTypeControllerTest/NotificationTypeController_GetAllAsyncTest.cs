using AutoFixture;
using Fanex.NCB.SkypeBot.Controllers;
using Fanex.NCB.SkypeBot.Model.DTO.NotificationType;
using Fanex.NCB.SkypeBot.Services.NotificationType;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Fanex.NCB.SkypeBot.UnitTests.Controller.NotificationTypeControllerTest
{
    public class NotificationTypeController_GetAllAsyncTest
    {
        [Fact]
        public async Task GetAllAsync_WhenCalled_ReturnJsonCorrectly()
        {
            // Arrange
            var fixture = new Fixture();
            var notificationTypeService = new Mock<INotificationTypeService>();
            var controller = new NotificationTypeController(notificationTypeService.Object);

            var pageIndex = fixture.Create<int>();
            var pageSize = fixture.Create<int>();
            var resultModel = fixture.Create<NotificationTypeListResultModel>();
            notificationTypeService.Setup(s => s.GetNotificationTypeListAsync(pageIndex, pageSize)).Returns(Task.FromResult(resultModel));

            // Act
            var result = await controller.GetAll(pageIndex, pageSize) as JsonResult;

            // Assert
            result.Should().BeEquivalentTo(new JsonResult(resultModel));
        }
    }
}