using AutoFixture;
using Fanex.NCB.Criteria.Notification;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fanex.NCB.Criteria.Tests.Notification;

public class GetRecipientsCriteriaTest
{
    [Fact]
    public void GetRecipientsCriteria_GetSettingKey_ReturnCorrectSettingKey()
    {
        // Arrange
        var command = new GetRecipientsCriteria();

        // Act
        var result = command.GetSettingKey();

        // Assert
        result.Should().Be("NCB_NewNotification_Recipient_Sel");
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(1, true)]
    public void GetRecipientsCriteria_IsValid_CheckCorrectly(int notificationTypeId, bool expectedResult)
    {
        // Arrange
        var fixture = new Fixture();
        var command = fixture.Create<GetRecipientsCriteria>();
        command.InNotificationTypeId = notificationTypeId;

        // Act
        var result = command.IsValid();

        // Assert
        result.Should().Be(expectedResult);
    }
}