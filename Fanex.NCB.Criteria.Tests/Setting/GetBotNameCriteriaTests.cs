namespace Fanex.NCB.Criteria.Tests.Setting
{
    using Fanex.NCB.Criteria.Setting;
    using Xunit;

    public class GetBotNameCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetBotNameCriteria();
            var expected = "NCB_Setting_BotName_Sel";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_ParamsArevalid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetBotNameCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}