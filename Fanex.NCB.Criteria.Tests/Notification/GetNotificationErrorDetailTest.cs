using Fanex.NCB.Criteria.Notification;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fanex.NCB.Criteria.Tests.Notification
{
    public class GetNotificationErrorDetailTest
    {
        [Fact]
        public void GetNotificationErrorDetailCriteria_GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var command = new GetNotificationErrorDetailCriteria();

            // Act
            var result = command.GetSettingKey();

            // Assert
            result.Should().Be("NCB_NotificationList_ErrDetail_Sel");
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(1, true)]
        public void GetNotificationErrorDetailCriteria_IsValid_CheckCorrectly(int notificationId, bool expectedResult)
        {
            // Arrange
            var command = new GetNotificationErrorDetailCriteria { NotificationId = notificationId };

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().Be(expectedResult);
        }
    }
}
