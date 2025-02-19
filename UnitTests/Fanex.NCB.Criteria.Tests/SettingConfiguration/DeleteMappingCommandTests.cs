namespace Fanex.NCB.Criteria.Tests.SettingConfiguration;

using Fanex.NCB.Criteria.SettingConfiguration;
using Xunit;

public class DeleteMappingCommandTests
{
    [Fact]
    public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
    {
        // Arrange
        var criteria = new DeleteMappingCommand();
        var expected = "NCB_SettingConfiguration_Mapping_Del";

        // Act
        var actual = criteria.GetSettingKey();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void IsValid_WhenCalled_ReturnTrue()
    {
        // Arrange
        var criteria = new DeleteMappingCommand
        {
            JobId = 1,
            NotificationTemplateId = 1,
            MsgContentsType = 1,
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
        var criteria = new DeleteMappingCommand
        {
            JobId = 0,
            NotificationTemplateId = 0,
            MsgContentsType = 0,
            ModifiedBy = 0
        };

        // Act
        var actual = criteria.IsValid();

        // Assert
        Assert.False(actual);
    }
}