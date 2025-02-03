namespace Fanex.NCB.Criteria.Tests.SettingConfiguration;

using Fanex.NCB.Criteria.SettingConfiguration;
using FluentAssertions;
using Xunit;

public class GetJobInfoCriteriaTests
{
    [Fact]
    public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
    {
        // Arrange
        var criteria = new GetJobInfoCriteria();
        var expected = "NCB_SettingConfiguration_JobInfo_Sel";

        // Act
        var actual = criteria.GetSettingKey();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(0, false)]
    public void IsValid_WhenCalled_ReturnExpected(int jobId, bool expected)
    {
        // Arrange
        var criteria = new GetJobInfoCriteria
        {
            JobId = jobId
        };

        // Act
        var actual = criteria.IsValid();

        // Assert
        actual.Should().Be(expected);
    }
}