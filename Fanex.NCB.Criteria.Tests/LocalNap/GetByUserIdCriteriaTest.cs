using AutoFixture;
using Fanex.NCB.Criteria.LocalNap.User;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fanex.NCB.Criteria.Tests.LocalNap
{
    public class GetByUserIdCriteriaTest
    {
        [Fact]
        public void GetByUserIdCriteria_GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var command = new GetByUserIdCriteria();

            // Act
            var result = command.GetSettingKey();

            // Assert
            result.Should().Be("NAP_User_GetById");
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(1, true)]
        public void GetByUserIdCriteria_IsValid_CheckCorrectly(int userId, bool expectedResult)
        {
            // Arrange
            var fixture = new Fixture();
            var criteria = fixture.Create<GetByUserIdCriteria>();
            criteria.UserId = userId;

            // Act
            var result = criteria.IsValid();

            // Assert
            result.Should().Be(expectedResult);
        }
    }
}
