using AutoFixture;
using Fanex.NCB.Criteria.Notification;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fanex.NCB.Criteria.Tests.Notification
{
    public class GetNotificationsCriteriaTest
    {
        [Fact]
        public void GetNotificationsCriteria_GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var command = new GetNotificationsCriteria();

            // Act
            var result = command.GetSettingKey();

            // Assert
            result.Should().Be("NCB_NotificationList_Sel");
        }

        [Theory]
        [InlineData(0, 1, false)]
        [InlineData(1, 0, false)]
        [InlineData(1, 1, true)]
        public void GetNotificationsCriteria_IsValid_CheckCorrectly(int pageIndex, int pageSize, bool expectedResult)
        {
            // Arrange
            var fixture = new Fixture();
            var command = fixture.Create<GetNotificationsCriteria>();
            command.PageIndex = pageIndex;
            command.PageSize = pageSize;

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().Be(expectedResult);
        }
    }
}
