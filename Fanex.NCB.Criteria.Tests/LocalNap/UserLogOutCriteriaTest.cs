using Fanex.NCB.Criteria.LocalNap.Security;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fanex.NCB.Criteria.Tests.LocalNap
{
    public class UserLogOutCriteriaTest
    {
        [Fact]
        public void UserLogOutCriteria_GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var command = new UserLogOutCriteria();

            // Act
            var result = command.GetSettingKey();

            // Assert
            result.Should().Be("NAP_User_Logout");
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(1, true)]
        public void UserLogOutCriteria_IsValid_CheckCorrectly(int onUserId, bool expectedResult)
        {
            // Arrange
            var command = new UserLogOutCriteria { OnUserId = onUserId };

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().Be(expectedResult);
        }
    }
}
