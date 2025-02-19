using AutoFixture;
using Fanex.NCB.Criteria.Notification;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fanex.NCB.Criteria.Tests.Notification;

public class ResolveNotificationCommandTest
{
    [Fact]
    public void ResolveNotificationCommand_GetSettingKey_ReturnCorrectSettingKey()
    {
        // Arrange
        var command = new ResolveNotificationCommand();

        // Act
        var result = command.GetSettingKey();

        // Assert
        result.Should().Be("NCB_NotificationList_Resolve_Upd");
    }

    [Theory]
    [InlineData(0, 1, "1", false)]
    [InlineData(1, 0, "1", false)]
    [InlineData(1, 1, "", false)]
    [InlineData(1, 1, "1", true)]
    public void ResolveNotificationCommand_IsValid_CheckCorrectly(
        int notificationId,
        int userId,
        string resolvedRemark,
        bool expectedResult)
    {
        // Arrange
        var fixture = new Fixture();
        var command = fixture.Create<ResolveNotificationCommand>();
        command.NotificationId = notificationId;
        command.UserId = userId;
        command.ResolvedRemark = resolvedRemark;

        // Act
        var result = command.IsValid();

        // Assert
        result.Should().Be(expectedResult);
    }
}