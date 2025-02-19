namespace Fanex.NCB.Criteria.Tests.SpecialDeleteTicket;

using Fanex.NCB.AutoJobs.Core.Criteria.SpecialDeleteTicket;
using Xunit;

public class GetMessageFromSpecialDeleteCriteriaTests
{
    [Fact]
    public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
    {
        // Arrange
        var criteria = new GetMessageFromSpecialDeleteCriteria();
        var expected = "NAP_Chatbot_SpecialDeleteMsg_Sel";

        // Act
        var actual = criteria.GetSettingKey();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void IsValid_WhenCalled_ReturnTrue()
    {
        // Arrange
        var criteria = new GetMessageFromSpecialDeleteCriteria
        {
            MessageID = 1,
            BatchSize = 2,
            Environment = "UAT",
            MsgContentsTypeLst = "test",
            LastPostTime = new System.DateTime(2022, 5, 19)
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
        var criteria = new GetMessageFromSpecialDeleteCriteria
        {
            MessageID = 0,
            BatchSize = 0,
            Environment = "UAT",
            MsgContentsTypeLst = "",
            LastPostTime = new System.DateTime(2022, 5, 19)
        };

        // Act
        var actual = criteria.IsValid();

        // Assert
        Assert.False(actual);
    }
}