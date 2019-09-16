using Fanex.NCB.Criteria.Common;
using Xunit;

namespace Fanex.NCB.Criteria.Tests.Common
{
    public class GetNotificationTypeCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetNotificationTypesCriteria();
            var expected = "NCB_Common_NotificationType_Sel";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_ParamsArevalid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetNotificationTypesCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}