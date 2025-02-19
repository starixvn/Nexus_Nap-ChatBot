using AutoFixture;
using Fanex.NCB.Criteria.LocalNap.Site;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fanex.NCB.Criteria.Tests.LocalNap;

public class GetByIdSiteCriteriaTest
{
    [Fact]
    public void GetByIdSiteCriteria_GetSettingKey_ReturnCorrectSettingKey()
    {
        // Arrange
        var command = new GetByIdSiteCriteria();

        // Act
        var result = command.GetSettingKey();

        // Assert
        result.Should().Be("NAP_Site_GetById");
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(1, true)]
    public void GetByIdSiteCriteria_IsValid_CheckCorrectly(int siteId, bool expectedResult)
    {
        // Arrange
        var fixture = new Fixture();
        var criteria = fixture.Create<GetByIdSiteCriteria>();
        criteria.SiteId = siteId;

        // Act
        var result = criteria.IsValid();

        // Assert
        result.Should().Be(expectedResult);
    }
}