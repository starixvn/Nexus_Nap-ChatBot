namespace Fanex.NCB.Criteria.Tests.ResultAmendmentLicensee;

using Fanex.NCB.AutoJobs.Core.Criteria.ResultAmendmentLicensee;
using Xunit;

public class GetMessageResultAmendmentLicenseeCriteriaTests
{
    [Fact]
    public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
    {
        // Arrange
        var criteria = new GetMessageResultAmendmentLicenseeCriteria();
        var expected = "NAP_Chatbot_ResultAmendment_Licensee_MessageSel";

        // Act
        var actual = criteria.GetSettingKey();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void IsValid_WhenCalled_ReturnTrue()
    {
        // Arrange
        var criteria = new GetMessageResultAmendmentLicenseeCriteria();

        // Act
        var actual = criteria.IsValid();

        // Assert
        Assert.True(actual);
    }
}