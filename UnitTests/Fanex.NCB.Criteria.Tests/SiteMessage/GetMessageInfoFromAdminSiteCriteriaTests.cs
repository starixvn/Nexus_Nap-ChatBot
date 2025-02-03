namespace Fanex.NCB.Criteria.Tests.SiteMessage;

using System;
using Fanex.NCB.AutoJobs.Core.Criteria.SiteMessage;
using Xunit;

public class GetMessageInfoFromAdminSiteCriteriaTests
{
    [Fact]
    public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
    {
        // Arrange
        var criteria = new GetMessageInfoFromAdminSiteCriteria();
        var expected = "NAP_Chatbot_SiteMesssage_Sel";

        // Act
        var actual = criteria.GetSettingKey();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void IsValid_WhenCalled_ReturnTrue()
    {
        // Arrange
        var criteria = new GetMessageInfoFromAdminSiteCriteria
        {
            MessageID = 1,
            BatchSize = 100,
            MsgContentsTypeLst = "test",
            LastPostTime = new DateTime(2022, 5, 19),
            Environment = "UAT"
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
        var criteria = new GetMessageInfoFromAdminSiteCriteria
        {
            MessageID = 0,
            BatchSize = 0,
            MsgContentsTypeLst = "",
            LastPostTime = new DateTime(2022, 5, 19),
            Environment = "UAT"
        };

        // Act
        var actual = criteria.IsValid();

        // Assert
        Assert.False(actual);
    }
}