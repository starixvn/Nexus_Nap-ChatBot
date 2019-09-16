namespace Fanex.NCB.Criteria.Tests.Common
{
    using Fanex.NCB.Criteria.Common;
    using Xunit;

    public class GetNotificationStatusCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetNotificationStatusCriteria();
            var expected = "NCB_Common_NotificationStatus_Sel";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_AlwaysReturnTrue()
        {
            // Arrange
            var criteria = new GetNotificationStatusCriteria();
            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}