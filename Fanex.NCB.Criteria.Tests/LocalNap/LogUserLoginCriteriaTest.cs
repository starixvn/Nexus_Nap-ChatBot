using AutoFixture;
using Fanex.NCB.Criteria.LocalNap.Security;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fanex.NCB.Criteria.Tests.LocalNap
{
    public class LogUserLoginCriteriaTest
    {
        [Fact]
        public void LogUserLoginCriteria_GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var command = new LogUserLoginCriteria();

            // Act
            var result = command.GetSettingKey();

            // Assert
            result.Should().Be("NAP_LogUserLogin_Insert");
        }

        [Fact]
        public void LogUserLoginCriteria_IsValid_CheckCorrectly()
        {
            // Arrange
            var fixture = new Fixture();
            var command = fixture.Create<LogUserLoginCriteria>();

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().Be(true);
        }
    }
}
