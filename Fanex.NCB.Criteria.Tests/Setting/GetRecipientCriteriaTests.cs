namespace Fanex.NCB.Criteria.Tests.Setting
{
    using Fanex.NCB.Criteria.Setting;
    using Xunit;

    public class GetRecipientCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetRecipientCriteria();
            var expected = "NCB_Setting_Recipient_Sel";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_ParamsArevalid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetRecipientCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}