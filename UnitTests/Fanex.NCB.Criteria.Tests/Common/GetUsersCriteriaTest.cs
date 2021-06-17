using Fanex.NCB.Criteria.Common;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fanex.NCB.Criteria.Tests.Common
{
    public class GetUsersCriteriaTest
    {
        [Fact]
        public void GetUsersCriteria_GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var command = new GetUsersCriteria();

            // Act
            var result = command.GetSettingKey();

            // Assert
            result.Should().Be("NCB_Common_User_Sel");
        }

        [Fact]
        public void GetUsersCriteria_IsValid_CheckCorrectly()
        {
            // Arrange
            var command = new GetUsersCriteria();

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().Be(true);
        }
    }
}
