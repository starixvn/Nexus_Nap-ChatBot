using Fanex.NCB.Criteria.NotificationType;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fanex.NCB.Criteria.Tests.NotificationType
{
    public class InsertNotificationTypeCommandTest
    {
        [Fact]
        public void InsertNotificationTypeCommand_GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var command = new InsertNotificationTypeCommand();

            // Act
            var result = command.GetSettingKey();

            // Assert
            result.Should().Be("NCB_NotificationType_Ins");
        }

        [Theory]
        [InlineData("", 1, "1", "1", false)]
        [InlineData("1", 0, "1", "1", false)]
        [InlineData("1", 1, "", "1", false)]
        [InlineData("1", 1, "1", "", false)]
        [InlineData("1", 1, "1", "1", true)]
        public void InsertNotificationTypeCommand_IsValid_CheckCorrectly(
            string notificationTypeName, 
            int systemId, 
            string recipientTypeIds, 
            string chatGroupTypeIds, 
            bool expectedResult)
        {
            // Arrange
            var command = new InsertNotificationTypeCommand {
                NotificationTypeName = notificationTypeName, 
                SystemId = systemId, 
                RecipientTypeIds = recipientTypeIds,
                ChatGroupTypeIds = chatGroupTypeIds
            };

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().Be(expectedResult);
        }
    }
}
