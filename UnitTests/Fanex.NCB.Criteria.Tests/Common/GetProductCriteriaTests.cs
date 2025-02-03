namespace Fanex.NCB.Criteria.Tests.Common;

using Fanex.NCB.Criteria.Common;
using Xunit;

public class GetProductCriteriaTests
{
    [Fact]
    public void GetSettingKey_ReturnCorrectSettingKey()
    {
        // Arrange
        var criteria = new GetProductsCriteria();
        var expected = "NCB_Common_Product_Sel";

        // Act
        var actual = criteria.GetSettingKey();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void IsValid_AlwaysReturnTrue()
    {
        // Arrange
        var criteria = new GetProductsCriteria();
        // Act
        var actual = criteria.IsValid();

        // Assert
        Assert.True(actual);
    }
}