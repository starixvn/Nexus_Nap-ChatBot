namespace Fanex.NCB.Criteria.Tests.Setting
{
    using Fanex.NCB.Criteria.Setting;
    using Xunit;

    public class SearchRecipientCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new SearchRecipientCriteria();
            var expected = "NCB_Setting_SearchRecipient_Sel";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void IsValid_ParamsAreInvalid_ReturnFalse(string query)
        {
            // Arrange
            var criteria = new SearchRecipientCriteria
            {
                Keyword = query
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
            var criteria = new SearchRecipientCriteria
            {
                Keyword = "A"
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}