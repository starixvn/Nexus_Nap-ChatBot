namespace Fanex.NCB.Criteria.Tests.SpecialDeleteTicket;

using Fanex.NCB.AutoJobs.Core.Criteria.SpecialDeleteTicket;
using Xunit;

public class GetSystemSettingCriteriaTests
{
    [Fact]
    public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
    {
        // Arrange
        var criteria = new GetSystemSettingCriteria();
        var expected = "NCB_MsgSetting_SpecialDelete_Sel";

        // Act
        var actual = criteria.GetSettingKey();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void IsValid_WhenCalled_ReturnTrue()
    {
        // Arrange
        var criteria = new GetSystemSettingCriteria();

        // Act
        var actual = criteria.IsValid();

        // Assert
        Assert.True(actual);
    }
}