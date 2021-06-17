using AutoFixture;
using Fanex.NCB.Criteria.Notification;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fanex.NCB.Criteria.Tests.Notification
{
    public class GetTemplatesCriteriaTest
    {
        [Fact]
        public void GetTemplatesCriteria_GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var command = new GetTemplatesCriteria();

            // Act
            var result = command.GetSettingKey();

            // Assert
            result.Should().Be("NCB_NewNotification_GetTemplates");
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(1, true)]
        public void GetTemplatesCriteria_IsValid_CheckCorrectly(int notificationTypeId, bool expectedResult)
        {
            // Arrange
            var fixture = new Fixture();
            var command = fixture.Create<GetTemplatesCriteria>();
            command.NotificationTypeId = notificationTypeId;

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().Be(expectedResult);
        }
    }
}
