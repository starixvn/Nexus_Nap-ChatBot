using Fanex.NCB.Criteria.LocalNap.Group;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fanex.NCB.Criteria.Tests.LocalNap
{
    public class GetGroupByIdCriteriaTest
    {
        [Fact]
        public void GetGroupByIdCriteria_GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var command = new GetGroupByIdCriteria();

            // Act
            var result = command.GetSettingKey();

            // Assert
            result.Should().Be("NAP_Group_GetById");
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(1, true)]
        public void GetGroupByIdCriteria_IsValid_CheckCorrectly(int groupId, bool expectedResult)
        {
            // Arrange
            var command = new GetGroupByIdCriteria { GroupId = groupId };

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().Be(expectedResult);
        }
    }
}
