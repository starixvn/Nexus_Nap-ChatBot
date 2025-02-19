using AutoFixture;
using Fanex.NCB.Criteria.LocalNap.Security;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fanex.NCB.Criteria.Tests.LocalNap;

public class UserSsoGetTimeUserCriteriaTest
{
    [Fact]
    public void UserSsoGetTimeUserCriteria_GetSettingKey_ReturnCorrectSettingKey()
    {
        // Arrange
        var command = new UserSsoGetTimeUserCriteria();

        // Act
        var result = command.GetSettingKey();

        // Assert
        result.Should().Be("NAP_User_SSOGetTime");
    }

    [Fact]
    public void UserSsoGetTimeUserCriteria_IsValid_CheckCorrectly()
    {
        // Arrange
        var fixture = new Fixture();
        var criteria = fixture.Create<UserSsoGetTimeUserCriteria>();

        // Act
        var result = criteria.IsValid();

        // Assert
        result.Should().Be(true);
    }
}