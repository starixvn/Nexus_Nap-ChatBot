using AutoFixture;
using Fanex.NCB.Criteria.LocalNap.User;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fanex.NCB.Criteria.Tests.LocalNap
{
    public class GetUserByUsernameCriteriaTest
    {
        [Fact]
        public void GetUserByUsernameCriteria_GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var command = new GetUserByUsernameCriteria();

            // Act
            var result = command.GetSettingKey();

            // Assert
            result.Should().Be("NAP_User_GetByUsername");
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("1", true)]
        public void GetUserByUsernameCriteria_IsValid_CheckCorrectly(string userName, bool expectedResult)
        {
            // Arrange
            var fixture = new Fixture();
            var criteria = fixture.Create<GetUserByUsernameCriteria>();
            criteria.Username = userName;

            // Act
            var result = criteria.IsValid();

            // Assert
            result.Should().Be(expectedResult);
        }
    }
}
