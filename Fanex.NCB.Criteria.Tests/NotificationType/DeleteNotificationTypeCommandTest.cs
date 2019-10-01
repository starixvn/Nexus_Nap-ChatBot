using Fanex.NCB.Criteria.NotificationType;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fanex.NCB.Criteria.Tests.NotificationType
{
    public class DeleteNotificationTypeCommandTest
    {
        [Fact]
        public void DeleteNotificationTypeCommand_GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var command = new DeleteNotificationTypeCommand();

            // Act
            var result = command.GetSettingKey();

            // Assert
            result.Should().Be("NCB_NotificationType_Del");
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(1, true)]
        public void DeleteNotificationTypeCommand_IsValid_CheckCorrectly(int notificationTypeId, bool expectedResult)
        {
            // Arrange
            var command = new DeleteNotificationTypeCommand { NotificationTypeId = notificationTypeId };

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().Be(expectedResult);
        }
    }
}
