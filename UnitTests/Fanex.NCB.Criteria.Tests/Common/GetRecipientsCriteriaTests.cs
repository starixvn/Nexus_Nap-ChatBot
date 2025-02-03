namespace Fanex.NCB.Criteria.Tests.Common;

using Fanex.NCB.Criteria.Common;
using Xunit;

public class GetRecipientsCriteriaTests
{
    [Fact]
    public void GetSettingKey_ReturnCorrectSettingKey()
    {
        // Arrange
        var criteria = new GetRecipientsCriteria();
        var expected = "NCB_Common_Recipient_Sel";

        // Act
        var actual = criteria.GetSettingKey();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void IsValid_AlwaysReturnTrue()
    {
        // Arrange
        var criteria = new GetRecipientsCriteria();
        // Act
        var actual = criteria.IsValid();

        // Assert
        Assert.True(actual);
    }
}