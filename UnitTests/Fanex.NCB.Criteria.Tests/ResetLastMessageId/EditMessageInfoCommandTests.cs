namespace Fanex.NCB.Criteria.Tests.ResetLastMessageId;

using System.Collections.Generic;
using Fanex.NCB.Criteria.ResetLastMessageId;
using Fanex.NCB.Models.ResetLastMessageId;
using Xunit;

public class EditMessageInfoCommandTests
{
    [Fact]
    public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
    {
        // Arrange
        var criteria = new EditMessageInfoCommand();
        var expected = "NCB_JobInfo_Upd";

        // Act
        var actual = criteria.GetSettingKey();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void IsValid_WhenCalled_ReturnTrue()
    {
        // Arrange
        var criteria = new EditMessageInfoCommand()
        {
            JobInfo = "[{JobIdentify: 1, MessageId: 2}]"
        };

        // Act
        var actual = criteria.IsValid();

        // Assert
        Assert.True(actual);
    }
}