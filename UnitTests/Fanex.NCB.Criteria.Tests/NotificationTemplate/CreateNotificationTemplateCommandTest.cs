using AutoFixture;
using Fanex.NCB.Criteria.NotificationTemplate;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fanex.NCB.Criteria.Tests.NotificationTemplate;

public class CreateNotificationTemplateCommandTest
{
    [Fact]
    public void CreateNotificationTemplateCommand_GetSettingKey_ReturnCorrectSettingKey()
    {
        // Arrange
        var command = new CreateNotificationTemplateCommand();

        // Act
        var result = command.GetSettingKey();

        // Assert
        result.Should().Be("NCB_NotificationTemplate_Ins");
    }

    [Theory]
    [InlineData(0, 1, "1", "1", "1", false)]
    [InlineData(1, 0, "1", "1", "1", false)]
    [InlineData(1, 1, "", "1", "1", false)]
    [InlineData(1, 1, "1", "", "1", false)]
    [InlineData(1, 1, "1", "1", "", false)]
    [InlineData(1, 1, "1", "1", "1", true)]
    public void CreateNotificationTemplateCommand_IsValid_CheckCorrectly(
        int notificationTypeId,
        int creatorId,
        string notificationTemplateName,
        string title,
        string content,
        bool expectedResult)
    {
        // Arrange
        var fixture = new Fixture();
        var criteria = fixture.Create<CreateNotificationTemplateCommand>();
        criteria.NotificationTypeId = notificationTypeId;
        criteria.CreatorId = creatorId;
        criteria.NotificationTemplateName = notificationTemplateName;
        criteria.Title = title;
        criteria.Content = content;

        // Act
        var result = criteria.IsValid();

        // Assert
        result.Should().Be(expectedResult);
    }
}