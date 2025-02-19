using Fanex.NCB.Criteria.NotificationType;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fanex.NCB.Criteria.Tests.NotificationType;

public class GetNotificationTypeCriteriaTest
{
    [Fact]
    public void GetNotificationTypeCriteria_GetSettingKey_ReturnCorrectSettingKey()
    {
        // Arrange
        var command = new GetNotificationTypeCriteria();

        // Act
        var result = command.GetSettingKey();

        // Assert
        result.Should().Be("NCB_NotificationType_Sel");
    }

    [Theory]
    [InlineData(0, 0, false)]
    [InlineData(1, 1, true)]
    public void GetNotificationTypeCriteria_IsValid_CheckCorrectly(int pageIndex, int pageSize, bool expectedResult)
    {
        // Arrange
        var command = new GetNotificationTypeCriteria { PageIndex = pageIndex, PageSize = pageSize };

        // Act
        var result = command.IsValid();

        // Assert
        result.Should().Be(expectedResult);
    }
}