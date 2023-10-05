using AutoFixture;
using Fanex.NCB.Criteria.Notification;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fanex.NCB.Criteria.Tests.Notification
{
    public class GetEditNotificationRecipientsCriteriaTest
    {
        [Fact]
        public void GetEditNotificationRecipientsCriteria_GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var command = new GetEditNotificationRecipientsCriteria();

            // Act
            var result = command.GetSettingKey();

            // Assert
            result.Should().Be("NCB_EditNotification_Recipient_Sel");
        }

        [Theory]
        [InlineData(0, 1, false)]
        [InlineData(1, 0, false)]
        [InlineData(1, 1, true)]
        public void GetEditNotificationRecipientsCriteria_IsValid_CheckCorrectly(int notificationTypeId, int notificationId, bool expectedResult)
        {
            // Arrange
            var fixture = new Fixture();
            var command = fixture.Create<GetEditNotificationRecipientsCriteria>();
            command.InNotificationTypeId = notificationTypeId;
            command.InNotificationId = notificationId;

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().Be(expectedResult);
        }
    }
}
