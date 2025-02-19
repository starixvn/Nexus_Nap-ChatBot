namespace Fanex.NCB.Criteria.Tests.SettingConfiguration;

using Fanex.NCB.Criteria.SettingConfiguration;
using Xunit;

public class GetAllSettingTypeCriteriaTests
{
    [Fact]
    public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
    {
        // Arrange
        var criteria = new GetAllSettingTypeCriteria();
        var expected = "NCB_SettingConfiguration_MappingSettingTypeList_Sel";

        // Act
        var actual = criteria.GetSettingKey();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void IsValid_WhenCalled_ReturnTrue()
    {
        // Arrange
        var criteria = new GetAllSettingTypeCriteria();

        // Act
        var actual = criteria.IsValid();

        // Assert
        Assert.True(actual);
    }
}