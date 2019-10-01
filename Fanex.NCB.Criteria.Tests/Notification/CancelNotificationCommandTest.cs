using AutoFixture;
using Fanex.NCB.Criteria.Notification;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fanex.NCB.Criteria.Tests.Notification
{
    public class CancelNotificationCommandTest
    {
        [Fact]
        public void CancelNotificationCommand_GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var command = new CancelNotificationCommand();

            // Act
            var result = command.GetSettingKey();

            // Assert
            result.Should().Be("NCB_NotificationList_Cancel_Upd");
        }

        [Theory]
        [InlineData(1, 0, false)]
        [InlineData(0, 1, false)]
        [InlineData(1, 1, true)]
        public void CancelNotificationCommand_IsValid_CheckCorrectly(int notificationId, int userId, bool expectedResult)
        {
            // Arrange
            var fixture = new Fixture();
            var criteria = fixture.Create<CancelNotificationCommand>();
            criteria.NotificationId = notificationId;
            criteria.UserId = userId;

            // Act
            var result = criteria.IsValid();

            // Assert
            result.Should().Be(expectedResult);
        }
    }
}
