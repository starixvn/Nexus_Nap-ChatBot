using System.Threading.Tasks;
using AutoFixture;
using Fanex.NCB.Core.Models;
using Fanex.NCB.Services.Interfaces;
using Fanex.NCB.SkypeBot.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
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
            var resultModel = fixture.Create<CommandResult>();
            notificationTypeService.Setup(s => s.DeleteAsync(notificationTypeId)).Returns(Task.FromResult(resultModel));

            // Act
            var result = await controller.Delete(notificationTypeId) as JsonResult;

            // Assert
            result.Should().BeEquivalentTo(new JsonResult(resultModel));
        }
    }
}