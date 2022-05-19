namespace Fanex.NCB.Criteria.Tests.Notification
{
    using Fanex.NCB.Criteria.Notification;
    using Xunit;

    public class GetUpdatedNotificationCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetUpdatedNotificationCriteria();
            var expected = "NCB_NotificationList_UpdatedNotification_Sel";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new GetUpdatedNotificationCriteria
            {
                NotificationIds = "1",
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}
