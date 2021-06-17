using Fanex.NCB.Criteria.NotificationTemplate;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fanex.NCB.Criteria.Tests.NotificationTemplate
{
    public class GetTemplateByIdCriteriaTest
    {
        [Fact]
        public void GetTemplateByIdCriteria_GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var command = new GetTemplateByIdCriteria();

            // Act
            var result = command.GetSettingKey();

            // Assert
            result.Should().Be("NCB_NotificationTemplate_Sel");
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(1, true)]
        public void GetTemplateByIdCriteria_IsValid_CheckCorrectly(int notificationTemplateId, bool expectedResult)
        {
            // Arrange
            var command = new GetTemplateByIdCriteria { NotificationTemplateId = notificationTemplateId };

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().Be(expectedResult);
        }
    }
}
