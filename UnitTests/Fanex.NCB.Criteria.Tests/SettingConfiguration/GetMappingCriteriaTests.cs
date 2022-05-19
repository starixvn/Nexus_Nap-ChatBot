namespace Fanex.NCB.Criteria.Tests.SettingConfiguration
{
    using Fanex.NCB.Criteria.SettingConfiguration;
    using Xunit;

    public class GetMappingCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetMappingCriteria();
            var expected = "NCB_SettingConfiguration_Mapping_Sel";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new GetMappingCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}
