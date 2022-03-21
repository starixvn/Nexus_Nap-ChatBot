namespace Fanex.NCB.Criteria.Tests.Common
{
    using Fanex.NCB.Criteria.Common;
    using Xunit;
    public class GetPlatformCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetPlatformCriteria();
            var expected = "NCB_Common_Platform_Sel";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetPlatformCriteria()
            {
                NotificationTypeId = 1
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void IsValid_ParamsAreInValid_ReturnFalse()
        {
            // Arrange
            var criteria = new GetPlatformCriteria()
            {
                NotificationTypeId = 0
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}
