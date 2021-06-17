using AutoFixture;
using Fanex.NCB.Criteria.LocalNap.Security;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fanex.NCB.Criteria.Tests.LocalNap
{
    public class UserReCheckInUserCriteriaTest
    {
        [Fact]
        public void UserReCheckInUserCriteria_GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var command = new UserReCheckInUserCriteria();

            // Act
            var result = command.GetSettingKey();

            // Assert
            result.Should().Be("NAP_User_ReCheckIn");
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(1, true)]
        public void UserReCheckInUserCriteria_IsValid_CheckCorrectly(int onUserId, bool expectedResult)
        {
            // Arrange
            var fixture = new Fixture();
            var criteria = fixture.Create<UserReCheckInUserCriteria>();
            criteria.OnUserId = onUserId;

            // Act
            var result = criteria.IsValid();

            // Assert
            result.Should().Be(expectedResult);
        }
    }
}
