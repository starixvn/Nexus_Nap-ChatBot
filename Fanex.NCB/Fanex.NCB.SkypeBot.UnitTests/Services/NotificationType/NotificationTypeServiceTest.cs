using Fanex.NCB.SkypeBot.BaseStorage;
using Fanex.NCB.SkypeBot.Model.DTO.NotificationType;
using Fanex.NCB.SkypeBot.Services.NotificationType;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fanex.NCB.SkypeBot.UnitTests.Services.NotificationType
{
    public class NotificationTypeServiceTest
    {
        [Fact]
        public void GetAll_WhenCalled_CallStorageGetAll()
        {
            // Arrange
            var storage = new Mock<INotificationTypeStorage>();
            var service = new NotificationTypeService(storage.Object);

            var fakeData = new List<NotificationTypeItem>() {
                new NotificationTypeItem { }
            };
            storage.Setup(r => r.GetAll()).Returns(fakeData);

            // Act
            var result = service.GetAll();

            // Assert
            result.Count.Should().Be(1);
        }

    }
}
