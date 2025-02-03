namespace Fanex.NCB.Criteria.Tests.ResultAmendmentInternalCSD;

using Fanex.NCB.AutoJobs.Core.Criteria.ResultAmendmentInternalCSD;
using Xunit;

public class ImportResultAmendmentCSDCommandTests
{
    [Fact]
    public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
    {
        // Arrange
        var criteria = new ImportResultAmendmentCSDCommand();
        var expected = "NCB_ResultAmendment_InternalCSD_Import";

        // Act
        var actual = criteria.GetSettingKey();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void IsValid_WhenCalled_ReturnTrue()
    {
        // Arrange
        var criteria = new ImportResultAmendmentCSDCommand();

        // Act
        var actual = criteria.IsValid();

        // Assert
        Assert.True(actual);
    }
}