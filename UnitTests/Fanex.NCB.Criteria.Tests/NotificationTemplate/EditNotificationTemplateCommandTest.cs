using AutoFixture;
using Fanex.NCB.Criteria.NotificationTemplate;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fanex.NCB.Criteria.Tests.NotificationTemplate;

public class EditNotificationTemplateCommandTest
{
    [Fact]
    public void EditNotificationTemplateCommand_GetSettingKey_ReturnCorrectSettingKey()
    {
        // Arrange
        var command = new EditNotificationTemplateCommand();

        // Act
        var result = command.GetSettingKey();

        // Assert
        result.Should().Be("NCB_NotificationTemplate_Upd");
    }

    [Theory]
    [InlineData(0, 1, 1, "1", "1", "1", false)]
    [InlineData(1, 0, 1, "1", "1", "1", false)]
    [InlineData(1, 1, 0, "1", "1", "1", false)]
    [InlineData(1, 1, 1, "", "1", "1", false)]
    [InlineData(1, 1, 1, "1", "", "1", false)]
    [InlineData(1, 1, 1, "1", "1", "", false)]
    [InlineData(1, 1, 1, "1", "1", "1", true)]
    public void EditNotificationTemplateCommand_IsValid_CheckCorrectly(
        int notificationTemplateId,
        int notificationTypeId,
        int modifierId,
        string notificationTemplateName,
        string title,
        string content,
        bool expectedResult)
    {
        // Arrange
        var fixture = new Fixture();
        var criteria = fixture.Create<EditNotificationTemplateCommand>();
        criteria.NotificationTypeId = notificationTypeId;
        criteria.NotificationTemplateId = notificationTemplateId;
        criteria.NotificationTemplateName = notificationTemplateName;
        criteria.ModifierId = modifierId;
        criteria.Title = title;
        criteria.Content = content;

        // Act
        var result = criteria.IsValid();

        // Assert
        result.Should().Be(expectedResult);
    }
}