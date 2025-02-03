using Fanex.NCB.Criteria.Notification;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fanex.NCB.Criteria.Tests.Notification;

public class GetEditNotificationInfoCriteriaTest
{
    [Fact]
    public void GetEditNotificationInfoCriteria_GetSettingKey_ReturnCorrectSettingKey()
    {
        // Arrange
        var command = new GetEditNotificationInfoCriteria();

        // Act
        var result = command.GetSettingKey();

        // Assert
        result.Should().Be("NCB_EditNotification_Sel");
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(1, true)]
    public void GetEditNotificationInfoCriteria_IsValid_CheckCorrectly(int notificationId, bool expectedResult)
    {
        // Arrange
        var command = new GetEditNotificationInfoCriteria { InNotificationId = notificationId };

        // Act
        var result = command.IsValid();

        // Assert
        result.Should().Be(expectedResult);
    }
}