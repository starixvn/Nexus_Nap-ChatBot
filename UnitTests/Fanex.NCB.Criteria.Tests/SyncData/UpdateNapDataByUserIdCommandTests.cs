namespace Fanex.NCB.Criteria.Tests.SyncData;

using Fanex.NCB.Criteria.SyncData;
using Xunit;

public class UpdateNapDataByUserIdCommandTests
{
    [Fact]
    public void GetSettingKey_ReturnCorrectSettingKey()
    {
        // Arrange
        var criteria = new UpdateNapDataByUserIdCommand();
        var expected = "NAP_UserControl_SyncData_ByUserId";

        // Act
        var actual = criteria.GetSettingKey();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void IsValid_ParamsAreInvalid_ReturnFalse(int userId)
    {
        // Arrange
        var criteria = new UpdateNapDataByUserIdCommand
        {
            UserId = userId
        };

        // Act
        var actual = criteria.IsValid();

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public void IsValid_ParamsAreValid_ReturnTrue()
    {
        // Arrange
        var criteria = new UpdateNapDataByUserIdCommand
        {
            UserId = 8
        };

        // Act
        var actual = criteria.IsValid();

        // Assert
        Assert.True(actual);
    }
}