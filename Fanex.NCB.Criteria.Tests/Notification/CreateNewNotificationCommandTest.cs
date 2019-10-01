using AutoFixture;
using Fanex.NCB.Criteria.Notification;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fanex.NCB.Criteria.Tests.Notification
{
    public class CreateNewNotificationCommandTest
    {
        [Fact]
        public void CreateNewNotificationCommand_GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var command = new CreateNewNotificationCommand();

            // Act
            var result = command.GetSettingKey();

            // Assert
            result.Should().Be("NCB_NewNotification_Ins");
        }

        [Theory]
        [InlineData(0, 1, "1", "1", "1", false)]
        [InlineData(1, 0, "1", "1", "1", false)]
        [InlineData(1, 1, "", "1", "1", false)]
        [InlineData(1, 1, "1", "", "1", false)]
        [InlineData(1, 1, "1", "1", "", false)]
        [InlineData(1, 1, "1", "1", "1", true)]
        public void CreateNewNotificationCommand_IsValid_CheckCorrectly(
            int notificationTypeId, 
            int creatorId,
            string recipientIds,
            string title,
            string content,
            bool expectedResult)
        {
            // Arrange
            var fixture = new Fixture();
            var criteria = fixture.Create<CreateNewNotificationCommand>();
            criteria.NotificationTypeId = notificationTypeId;
            criteria.CreatorId = creatorId;
            criteria.RecipientIDLst = recipientIds;
            criteria.Title = title;
            criteria.Content = content;

            // Act
            var result = criteria.IsValid();

            // Assert
            result.Should().Be(expectedResult);
        }
    }
}
