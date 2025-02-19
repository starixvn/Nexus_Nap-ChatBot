namespace Fanex.NCB.Criteria.Tests.SiteMessage;

using Fanex.NCB.AutoJobs.Core.Criteria.SiteMessage;
using FluentAssertions;
using Xunit;

public class GetEventInfoCriteriaTests
{
    [Fact]
    public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
    {
        // Arrange
        var criteria = new GetEventInfoCriteria();
        var expected = "NAP_Chatbot_SiteMesssage_EventInfo_Sel";

        // Act
        var actual = criteria.GetSettingKey();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("test", true)]
    [InlineData("", false)]
    public void IsValid_WhenCalled_ReturnExpected(string matchIDLst, bool expected)
    {
        // Arrange
        var criteria = new GetEventInfoCriteria
        {
            MatchIDLst = matchIDLst
        };

        // Act
        var actual = criteria.IsValid();

        // Assert
        actual.Should().Be(expected);
    }
}