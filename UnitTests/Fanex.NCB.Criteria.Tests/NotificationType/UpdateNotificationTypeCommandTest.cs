using Fanex.NCB.Criteria.NotificationType;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fanex.NCB.Criteria.Tests.NotificationType;

public class UpdateNotificationTypeCommandTest
{
    [Fact]
    public void UpdateNotificationTypeCommand_GetSettingKey_ReturnCorrectSettingKey()
    {
        // Arrange
        var command = new UpdateNotificationTypeCommand();

        // Act
        var result = command.GetSettingKey();

        // Assert
        result.Should().Be("NCB_NotificationType_Upd");
    }

    [Theory]
    [InlineData(0, 1, "1", "1", "1", false)]
    [InlineData(1, 0, "1", "1", "1", false)]
    [InlineData(1, 1, "", "1", "1", false)]
    [InlineData(1, 1, "1", "", "1", false)]
    [InlineData(1, 1, "1", "1", "", false)]
    [InlineData(1, 1, "1", "1", "1", true)]
    public void UpdateNotificationTypeCommand_IsValid_CheckCorrectly(
        int notificationTypeId,
        int systemId,
        string notificationTypeName,
        string recipientTypeIds,
        string chatGroupTypeIds,
        bool expectedResult)
    {
        // Arrange
        var command = new UpdateNotificationTypeCommand
        {
            NotificationTypeId = notificationTypeId,
            SystemId = systemId,
            NotificationTypeName = notificationTypeName,
            RecipientTypeIds = recipientTypeIds,
            ChatGroupTypeIds = chatGroupTypeIds
        };

        // Act
        var result = command.IsValid();

        // Assert
        result.Should().Be(expectedResult);
    }
}