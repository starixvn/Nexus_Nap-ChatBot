using System.Threading.Tasks;
using AutoFixture;
using Fanex.NCB.Core.Models;
using Fanex.NCB.Models.NotificationType;
using Fanex.NCB.Services.Interfaces;
using Fanex.NCB.SkypeBot.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Fanex.NCB.SkypeBot.UnitTests.Controller.NotificationTypeControllerTest
{
    public class NotificationTypeController_EditAsyncTest
    {
        [Fact]
        public async Task EditAsync_WhenCalled_ReturnJsonCorrectly()
        {
            // Arrange
            var fixture = new Fixture();
            var notificationTypeService = new Mock<INotificationTypeService>();
            var controller = new NotificationTypeController(notificationTypeService.Object);

            var criteria = fixture.Create<NotificationType>();
            var resultModel = fixture.Create<CommandResult>();
            notificationTypeService.Setup(s => s.UpdateAsync(criteria)).Returns(Task.FromResult(resultModel));

            // Act
            var result = await controller.Edit(criteria) as JsonResult;

            // Assert
            result.Should().BeEquivalentTo(new JsonResult(resultModel));
        }
    }
}