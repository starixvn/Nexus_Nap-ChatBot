using AutoFixture;
using Fanex.NCB.Criteria.LocalNap.User;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fanex.NCB.Criteria.Tests.LocalNap
{
    public class GetMenuItemCriteriaTest
    {
        [Fact]
        public void GetMenuItemCriteria_GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var command = new GetMenuItemCriteria();

            // Act
            var result = command.GetSettingKey();

            // Assert
            result.Should().Be("NAP_MenuItem_GetByUserAndSiteIncludeGroup");
        }

        [Theory]
        [InlineData(0, 1, false)]
        [InlineData(1, 0, false)]
        [InlineData(1, 1, true)]
        public void GetMenuItemCriteria_IsValid_CheckCorrectly(int userId, int siteId, bool expectedResult)
        {
            // Arrange
            var fixture = new Fixture();
            var criteria = fixture.Create<GetMenuItemCriteria>();
            criteria.UserId = userId;
            criteria.SiteId = siteId;

            // Act
            var result = criteria.IsValid();

            // Assert
            result.Should().Be(expectedResult);
        }
    }
}
