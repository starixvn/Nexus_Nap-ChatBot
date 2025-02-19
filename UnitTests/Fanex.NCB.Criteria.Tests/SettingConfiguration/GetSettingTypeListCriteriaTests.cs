namespace Fanex.NCB.Criteria.Tests.SettingConfiguration;

using Fanex.NCB.Criteria.SettingConfiguration;
using Xunit;

public class GetSettingTypeListCriteriaTests
{
    [Fact]
    public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
    {
        // Arrange
        var criteria = new GetSettingTypeListCriteria();
        var expected = "NCB_SettingConfiguration_SettingTypeList_Sel";

        // Act
        var actual = criteria.GetSettingKey();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void IsValid_WhenCalled_ReturnTrue()
    {
        // Arrange
        var criteria = new GetSettingTypeListCriteria
        {
            PageIndex = 1,
            PageSize = 100,
            SettingType = 100
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
        var criteria = new GetSettingTypeListCriteria
        {
            PageIndex = 0,
            PageSize = 0,
            SettingType = 100
        };

        // Act
        var actual = criteria.IsValid();

        // Assert
        Assert.False(actual);
    }
}