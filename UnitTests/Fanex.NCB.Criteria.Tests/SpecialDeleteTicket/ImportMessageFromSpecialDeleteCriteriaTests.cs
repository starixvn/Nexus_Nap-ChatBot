namespace Fanex.NCB.Criteria.Tests.SpecialDeleteTicket;

using Fanex.NCB.AutoJobs.Core.Criteria.SpecialDeleteTicket;
using Xunit;

public class ImportMessageFromSpecialDeleteCriteriaTests
{
    [Fact]
    public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
    {
        // Arrange
        var criteria = new ImportMessageFromSpecialDeleteCriteria();
        var expected = "NCB_SpecialDeleteMsg_Import";

        // Act
        var actual = criteria.GetSettingKey();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void IsValid_WhenCalled_ReturnTrue()
    {
        // Arrange
        var criteria = new ImportMessageFromSpecialDeleteCriteria
        {
            MessageHeader = "test",
            MessageDetail = "content",
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
        var criteria = new ImportMessageFromSpecialDeleteCriteria
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