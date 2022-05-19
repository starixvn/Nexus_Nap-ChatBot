namespace Fanex.NCB.Criteria.Tests.SettingConfiguration
{
    using Fanex.NCB.Criteria.SettingConfiguration;
    using Xunit;

    public class UpdateJobInfoCommandTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new UpdateJobInfoCommand();
            var expected = "NCB_SettingConfiguration_JobInfo_Upd";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new UpdateJobInfoCommand
            {
                Id = 1,
                BatchSize = 100,
                TestingGroup = "test",
                ModifiedBy = 1
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
            var criteria = new UpdateJobInfoCommand
            {
                Id = 0,
                BatchSize = 0,
                TestingGroup = "",
                ModifiedBy = 0
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}
