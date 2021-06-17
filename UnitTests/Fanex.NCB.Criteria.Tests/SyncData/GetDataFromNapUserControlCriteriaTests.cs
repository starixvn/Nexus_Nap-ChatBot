namespace Fanex.NCB.Criteria.Tests.SyncData
{
    using Fanex.NCB.Criteria.SyncData;
    using Xunit;

    public class GetDataFromNapUserControlCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetDataFromNapUserControlCriteria();
            var expected = "NAP_ChatBot_GetDataBySiteId";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void IsValid_ParamsAreInvalid_ReturnFalse(int siteId)
        {
            // Arrange
            var criteria = new GetDataFromNapUserControlCriteria
            {
                SiteId = siteId
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetDataFromNapUserControlCriteria
            {
                SiteId = 69
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}