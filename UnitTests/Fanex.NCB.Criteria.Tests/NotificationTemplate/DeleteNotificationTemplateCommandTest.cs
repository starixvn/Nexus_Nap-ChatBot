using Fanex.NCB.Criteria.NotificationTemplate;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fanex.NCB.Criteria.Tests.NotificationTemplate;

public class DeleteNotificationTemplateCommandTest
{
    [Fact]
    public void DeleteNotificationTemplateCommand_GetSettingKey_ReturnCorrectSettingKey()
    {
        // Arrange
        var command = new DeleteNotificationTemplateCommand();

        // Act
        var result = command.GetSettingKey();

        // Assert
        result.Should().Be("NCB_NotificationTemplate_Del");
    }

    [Theory]
    [InlineData(0, 1, false)]
    [InlineData(1, 0, false)]
    [InlineData(1, 1, true)]
    public void DeleteNotificationTemplateCommand_IsValid_CheckCorrectly(int notificationTemplateId, int userId,
        bool expectedResult)
    {
        // Arrange
        var command = new DeleteNotificationTemplateCommand
            { NotificationTemplateId = notificationTemplateId, UserId = userId };

        // Act
        var result = command.IsValid();

        // Assert
        result.Should().Be(expectedResult);
    }
}