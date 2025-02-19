using AutoFixture;
using Fanex.NCB.Criteria.LocalNap.User;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fanex.NCB.Criteria.Tests.LocalNap;

public class GetUserFunctionPolicyCriteriaTest
{
    [Fact]
    public void GetUserFunctionPolicyCriteria_GetSettingKey_ReturnCorrectSettingKey()
    {
        // Arrange
        var command = new GetUserFunctionPolicyCriteria();

        // Act
        var result = command.GetSettingKey();

        // Assert
        result.Should().Be("NAP_User_GetUserSiteFunctionKeyPolicy");
    }

    [Theory]
    [InlineData(0, 1, "1", false)]
    [InlineData(1, 0, "1", false)]
    [InlineData(1, 1, "", false)]
    [InlineData(1, 1, "1", true)]
    public void GetUserFunctionPolicyCriteria_IsValid_CheckCorrectly(
        int userId,
        int siteId,
        string functionKey,
        bool expectedResult)
    {
        // Arrange
        var fixture = new Fixture();
        var criteria = fixture.Create<GetUserFunctionPolicyCriteria>();
        criteria.UserId = userId;
        criteria.SiteId = siteId;
        criteria.FunctionKey = functionKey;

        // Act
        var result = criteria.IsValid();

        // Assert
        result.Should().Be(expectedResult);
    }
}