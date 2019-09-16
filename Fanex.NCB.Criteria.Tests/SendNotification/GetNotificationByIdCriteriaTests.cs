namespace Fanex.NCB.Criteria.Tests.SendNotification
{
    using Fanex.NCB.Criteria.SendNotification;
    using Xunit;

    public class GetNotificationByIdCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetNotificationByIdCriteria();
            var expected = "NCB_SendNotification_GetImmedNotifUpdStatus";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void IsValid_ParamsAreInvalid_ReturnFalse(int notificationId)
        {
            // Arrange
            var criteria = new GetNotificationByIdCriteria
            {
                NotificationId = notificationId
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_ParamsArevalid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetNotificationByIdCriteria
            {
                NotificationId = 1
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}