namespace Fanex.NCB.Criteria.Tests.ChatGroupMapping;

using Fanex.NCB.Criteria.ChatGroupMapping;
using Xunit;

public class GetChatGroupMappingCriteriaTests
{
    [Fact]
    public void GetSettingKey_ReturnCorrectSettingKey()
    {
        // Arrange
        var criteria = new GetChatGroupMappingCriteria();
        var expected = "NCB_ChatGroupMapping_Sel";

        // Act
        var actual = criteria.GetSettingKey();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 0)]
    [InlineData(0, 1)]
    public void IsValid_ParamsAreInvalid_ReturnFalse(int pageIndex, int pageSize)
    {
        // Arrange
        var criteria = new GetChatGroupMappingCriteria
        {
            PageIndex = pageIndex,
            PageSize = pageSize
        };
        // Act
        var actual = criteria.IsValid();

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public void IsValid_ParamsArevalid_ReturnTrue()
    {
        // Arrange
        var criteria = new GetChatGroupMappingCriteria
        {
            PageIndex = 1,
            PageSize = 1
        };
        // Act
        var actual = criteria.IsValid();

        // Assert
        Assert.True(actual);
    }
}