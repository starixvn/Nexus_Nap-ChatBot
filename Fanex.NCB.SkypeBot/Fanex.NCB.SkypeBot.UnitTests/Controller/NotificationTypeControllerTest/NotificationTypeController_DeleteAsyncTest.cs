using AutoFixture;
using Fanex.NCB.Common.DTO;
using Fanex.NCB.SkypeBot.Controllers;
using Fanex.NCB.SkypeBot.Services.NotificationType;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Fanex.NCB.SkypeBot.UnitTests.Controller.NotificationTypeControllerTest
{
    public class NotificationTypeController_DeleteAsyncTest
    {
        [Fact]
        public async Task DeleteAsync_WhenCalled_ReturnJsonCorrectly()
        {
            // Arrange
            var fixture = new Fixture();
            var notificationTypeService = new Mock<INotificationTypeService>();
            var controller = new NotificationTypeController(notificationTypeService.Object);

            var notificationTypeId = fixture.Create<int>();
            var resultModel = fixture.Create<ResultModel>();
            notificationTypeService.Setup(s => s.DeleteNotificationTypeAsync(notificationTypeId)).Returns(Task.FromResult(resultModel));

            // Act
            var result = await controller.Delete(notificationTypeId) as JsonResult;

            // Assert
            result.Should().BeEquivalentTo(new JsonResult(resultModel));
        }
    }
}