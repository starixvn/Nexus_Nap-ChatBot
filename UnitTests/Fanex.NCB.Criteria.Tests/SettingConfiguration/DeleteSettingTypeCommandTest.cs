namespace Fanex.NCB.Criteria.Tests.SettingConfiguration
{
    using Fanex.NCB.Criteria.SettingConfiguration;
    using FluentAssertions;
    using Xunit;

    public class DeleteSettingTypeCommandTest
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new DeleteSettingTypeCommand();
            var expected = "NCB_SettingConfiguration_SettingType_Del";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 1, true)]
        [InlineData(0, 0, false)]
        public void IsValid_WhenCalled_ReturnExpected(int type, int modifiedBy, bool expected)
        {
            // Arrange
            var criteria = new DeleteSettingTypeCommand
            {
                Type = type,
                ModifiedBy = modifiedBy        
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            actual.Should().Be(expected);
        }
    }
}
