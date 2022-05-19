namespace Fanex.NCB.Criteria.Tests.SettingConfiguration
{
    using Fanex.NCB.Criteria.SettingConfiguration;
    using Xunit;

    public class GetMsgContentsTypesCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetMsgContentsTypesCriteria();
            var expected = "NCB_SettingConfiguration_MsgContentsTypeList_Sel";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new GetMsgContentsTypesCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}
