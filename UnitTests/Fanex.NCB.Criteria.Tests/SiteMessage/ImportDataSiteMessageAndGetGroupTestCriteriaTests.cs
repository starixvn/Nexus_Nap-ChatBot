namespace Fanex.NCB.Criteria.Tests.SiteMessage;

using Fanex.NCB.AutoJobs.Core.Criteria.SiteMessage;
using Xunit;

public class ImportDataSiteMessageAndGetGroupTestCriteriaTests
{
    [Fact]
    public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
    {
        // Arrange
        var criteria = new ImportDataSiteMessageAndGetGroupTestCriteria();
        var expected = "NCB_SiteMessage_Import";

        // Act
        var actual = criteria.GetSettingKey();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void IsValid_WhenCalled_ReturnTrue()
    {
        // Arrange
        var criteria = new ImportDataSiteMessageAndGetGroupTestCriteria
        {
            MessageHeader = "test",
            MessageDetail = "test",
            ErrCode = 0,
            ErrMsg = ""
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
        var criteria = new ImportDataSiteMessageAndGetGroupTestCriteria
        {
            MessageHeader = "",
            MessageDetail = "",
            ErrCode = 0,
            ErrMsg = ""
        };

        // Act
        var actual = criteria.IsValid();

        // Assert
        Assert.False(actual);
    }
}