using AutoFixture;
using Fanex.NCB.Criteria.LocalNap.User;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fanex.NCB.Criteria.Tests.LocalNap
{
    public class GetSiteLevelCriteriaTest
    {
        [Fact]
        public void GetSiteLevelCriteria_GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var command = new GetSiteLevelCriteria();

            // Act
            var result = command.GetSettingKey();

            // Assert
            result.Should().Be("NAP_Site_GetByUser");
        }

        [Theory]
        [InlineData(0, "1", false)]
        [InlineData(1, "", false)]
        [InlineData(1, "1", true)]
        public void GetSiteLevelCriteria_IsValid_CheckCorrectly(int userId, string environment, bool expectedResult)
        {
            // Arrange
            var fixture = new Fixture();
            var criteria = fixture.Create<GetSiteLevelCriteria>();
            criteria.UserId = userId;
            criteria.Environment = environment;

            // Act
            var result = criteria.IsValid();

            // Assert
            result.Should().Be(expectedResult);
        }
    }
}
