using Fanex.NCB.SkypeBot.Controllers;
using Fanex.NCB.SkypeBot.Services.NotificationType;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Fanex.NCB.SkypeBot.UnitTests.Controller.NotificationTypeControllerTest
{
    public class NotificationTypeController_IndexPageAsyncTest
    {
        [Fact]
        public void IndexPageAsync_WhenCalled_ReturnViewCorrectly()
        {
            // Arrange
            var notificationTypeService = new Mock<INotificationTypeService>();
            var controller = new NotificationTypeController(notificationTypeService.Object);

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            result.ViewName.Should().BeEquivalentTo("~/Views/SkypeBot/NotificationType/Index.cshtml");
        }
    }
}