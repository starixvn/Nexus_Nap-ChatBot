namespace Fanex.NCB.Criteria.Tests.SettlementInternalCSD;

using Fanex.NCB.AutoJobs.Core.Criteria.SettlementInternalCSD;
using Xunit;

public class ImportSettleInternalCSDCommandTests
{
    [Fact]
    public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
    {
        // Arrange
        var criteria = new ImportSettleInternalCSDCommand();
        var expected = "NCB_Settlement_InternalCSD_Import";

        // Act
        var actual = criteria.GetSettingKey();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void IsValid_WhenCalled_ReturnTrue()
    {
        // Arrange
        var criteria = new ImportSettleInternalCSDCommand();

        // Act
        var actual = criteria.IsValid();

        // Assert
        Assert.True(actual);
    }
}