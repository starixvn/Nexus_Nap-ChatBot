using AutoFixture;
using Fanex.NCB.Criteria.LocalNap.Security;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fanex.NCB.Criteria.Tests.LocalNap
{
    public class UserCheckInCriteriaTest
    {
        [Fact]
        public void UserCheckInCriteria_GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var command = new UserCheckInCriteria();

            // Act
            var result = command.GetSettingKey();

            // Assert
            result.Should().Be("NAP_User_CheckIn");
        }

        [Theory]
        [InlineData(0, 1, false)]
        [InlineData(1, 0, false)]
        [InlineData(1, 1, true)]
        public void UserCheckInCriteria_IsValid_CheckCorrectly(int userId, int siteId, bool expectedResult)
        {
            // Arrange
            var fixture = new Fixture();
            var criteria = fixture.Create<UserCheckInCriteria>();
            criteria.UserId = userId;
            criteria.SiteId = siteId;

            // Act
            var result = criteria.IsValid();

            // Assert
            result.Should().Be(expectedResult);
        }
    }
}
