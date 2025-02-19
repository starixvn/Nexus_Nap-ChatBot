namespace Fanex.NCB.Criteria.Tests.SettingConfiguration;

using Fanex.NCB.Criteria.SettingConfiguration;
using Xunit;

public class UpdateMappingCommandTests
{
    [Fact]
    public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
    {
        // Arrange
        var criteria = new UpdateMappingCommand();
        var expected = "NCB_SettingConfiguration_Mapping_Upd";

        // Act
        var actual = criteria.GetSettingKey();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void IsValid_WhenCalled_ReturnTrue()
    {
        // Arrange
        var criteria = new UpdateMappingCommand
        {
            JobId = 1,
            OldNotificationTypeId = 1,
            NewNotificationTypeId = 1,
            NewNotificationTemplateId = 1,
            OldNotificationTemplateId = 1,
            NewMsgContentsType = 1,
            NewRequiredParam = "test",
            NewIsTest = true,
            OldMsgContentsType = 1,
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
        var criteria = new UpdateMappingCommand
        {
            JobId = 0,
            OldNotificationTypeId = 1,
            NewNotificationTypeId = 0,
            NewNotificationTemplateId = 0,
            OldNotificationTemplateId = 1,
            NewMsgContentsType = 1,
            NewRequiredParam = "test",
            NewIsTest = true,
            OldMsgContentsType = 1,
            ModifiedBy = 1
        };

        // Act
        var actual = criteria.IsValid();

        // Assert
        Assert.False(actual);
    }
}