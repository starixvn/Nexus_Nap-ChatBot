namespace Fanex.NCB.Criteria.Tests.Common
{
    using Fanex.NCB.Criteria.Common;
    using Xunit;

    public class GetConversationChannelCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetConversationChannelCriteria();
            var expected = "NCB_Channel_Sel";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new GetConversationChannelCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}