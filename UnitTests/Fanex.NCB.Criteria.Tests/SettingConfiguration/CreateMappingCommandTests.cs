namespace Fanex.NCB.Criteria.Tests.SettingConfiguration
{
    using Fanex.NCB.Criteria.SettingConfiguration;
    using Xunit;

    public class CreateMappingCommandTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new CreateMappingCommand();
            var expected = "NCB_SettingConfiguration_Mapping_Ins";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_WhenCalled_ReturnTrue()
        {
            // Arrange
            var criteria = new CreateMappingCommand
            {
                JobId = 1,
                NotificationTypeId = 2,
                NotificationTemplateId = 1,
                MsgContentsType = 1,
                RequiredParam = "test",
                IsTest = true,
                CreatorId = 1
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
            var criteria = new CreateMappingCommand
            {
                JobId = 0,
                NotificationTypeId = 0,
                NotificationTemplateId = 0,
                MsgContentsType = 1,
                RequiredParam = "test",
                IsTest = true,
                CreatorId = 1
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}
