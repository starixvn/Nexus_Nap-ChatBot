namespace Fanex.NCB.Criteria.Tests.Setting
{
    using Fanex.NCB.Criteria.Setting;
    using Xunit;

    public class GetRecipientTypeCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetRecipientTypeCriteria();
            var expected = "NCB_Common_RecipientType_Sel";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_ParamsArevalid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetRecipientTypeCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}