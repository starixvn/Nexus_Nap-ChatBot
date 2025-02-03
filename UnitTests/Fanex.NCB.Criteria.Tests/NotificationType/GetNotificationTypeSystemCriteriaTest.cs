using Fanex.NCB.Criteria.NotificationType;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fanex.NCB.Criteria.Tests.NotificationType;

public class GetNotificationTypeSystemCriteriaTest
{
    [Fact]
    public void GetNotificationTypeSystemCriteria_GetSettingKey_ReturnCorrectSettingKey()
    {
        // Arrange
        var command = new GetNotificationTypeSystemCriteria();

        // Act
        var result = command.GetSettingKey();

        // Assert
        result.Should().Be("NCB_Setting_System_Sel");
    }

    [Fact]
    public void GetNotificationTypeSystemCriteria_IsValid_CheckCorrectly()
    {
        // Arrange
        var command = new GetNotificationTypeSystemCriteria();

        // Act
        var result = command.IsValid();

        // Assert
        result.Should().Be(true);
    }
}