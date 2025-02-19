namespace Fanex.NCB.Criteria.Tests.ChatGroupMapping;

using Fanex.NCB.Criteria.ChatGroupMapping;
using Xunit;

public class UpdateActiveConverstionGroupCommandTests
{
    [Fact]
    public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
    {
        // Arrange
        var criteria = new UpdateActiveConverstionGroupCommand();
        var expected = "NCB_ChatGroupMapping_ActiveGroup_Upd";

        // Act
        var actual = criteria.GetSettingKey();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void IsValid_WhenCalled_ReturnTrue()
    {
        // Arrange
        var criteria = new UpdateActiveConverstionGroupCommand
        {
            GroupID = 1,
            UserID = 1,
            IsActive = true
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
        var criteria = new UpdateActiveConverstionGroupCommand
        {
            GroupID = 0,
            UserID = 1,
            IsActive = true
        };

        // Act
        var actual = criteria.IsValid();

        // Assert
        Assert.False(actual);
    }
}