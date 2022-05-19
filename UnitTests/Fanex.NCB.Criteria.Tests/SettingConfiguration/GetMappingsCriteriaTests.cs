namespace Fanex.NCB.Criteria.Tests.SettingConfiguration
{
    using Fanex.NCB.Criteria.SettingConfiguration;
    using Xunit;

    public class GetMappingsCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetMappingsCriteria();
            var expected = "NCB_SettingConfiguration_MappingList_Sel";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new GetMappingsCriteria
            {
                JobId = 1,
                PageIndex = 1,
                PageSize = 100,
                Total = 100
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnFalse()
        {
            // Arrange
            var criteria = new GetMappingsCriteria
            {
                JobId = 0,
                PageIndex = 1,
                PageSize = 100,
                Total = 100
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}
