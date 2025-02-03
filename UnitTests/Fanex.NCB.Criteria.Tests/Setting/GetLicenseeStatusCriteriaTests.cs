namespace Fanex.NCB.Criteria.Tests.Setting;

using Fanex.NCB.Criteria.Setting;
using Xunit;

public class GetLicenseeStatusCriteriaTests
{
    [Fact]
    public void GetSettingKey_ReturnCorrectSettingKey()
    {
        // Arrange
        var criteria = new GetLicenseeStatusCriteria();
        var expected = "NCB_Setting_LicenseeStatus_Sel";

        // Act
        var actual = criteria.GetSettingKey();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void IsValid_ParamsArevalid_ReturnTrue()
    {
        // Arrange
        var criteria = new GetLicenseeStatusCriteria();

        // Act
        var actual = criteria.IsValid();

        // Assert
        Assert.True(actual);
    }
}