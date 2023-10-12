using AutoFixture;
using Fanex.NCB.Criteria.Notification;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fanex.NCB.Criteria.Tests.Notification
{
    public class EditNotificationCommandTest
    {
        [Fact]
        public void EditNotificationCommand_GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var command = new EditNotificationCommand();

            // Act
            var result = command.GetSettingKey();

            // Assert
            result.Should().Be("NCB_EditNotification_Upd");
        }

        [Theory]
        [InlineData(0, 1, "1", "1", "1", false)]
        [InlineData(1, 0, "1", "1", "1", false)]
        [InlineData(1, 1, "", "1", "1", false)]
        [InlineData(1, 1, "1", "", "1", false)]
        [InlineData(1, 1, "1", "1", "", false)]
        [InlineData(1, 1, "1", "1", "1", true)]
        public void EditNotificationCommand_IsValid_CheckCorrectly(
            int notificationId,
            int userId,
            string recipientIds,
            string title,
            string content,
            bool expectedResult)
        {
            // Arrange
            var fixture = new Fixture();
            var criteria = fixture.Create<EditNotificationCommand>();
            criteria.InNotificationId = notificationId;
            criteria.InUserId = userId;
            criteria.InRecipientIds = recipientIds;
            criteria.InTitle = title;
            criteria.InContent = content;

            // Act
            var result = criteria.IsValid();

            // Assert
            result.Should().Be(expectedResult);
        }
    }
}
