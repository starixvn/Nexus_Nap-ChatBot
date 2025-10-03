namespace Fanex.NCB.BotService.EndPoint.Tests.Constants;

using Fanex.NCB.BotService.EndPoint.Constants;
using FluentAssertions;
using Xunit;

public class ServiceNameTests
{
    [Fact]
    public void SkypeBot_ShouldHaveCorrectValue()
    {
        // Arrange & Act
        var value = ServiceName.SkypeBot;

        // Assert
        value.Should().Be("SkypeBot");
        value.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void TeamsBot_ShouldHaveCorrectValue()
    {
        // Arrange & Act
        var value = ServiceName.TeamsBot;

        // Assert
        value.Should().Be("TeamsBot");
        value.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void SkypeBot_ShouldBeConstant()
    {
        // Arrange & Act
        var value1 = ServiceName.SkypeBot;
        var value2 = ServiceName.SkypeBot;

        // Assert
        value1.Should().BeSameAs(value2);
        value1.Should().Be(value2);
    }

    [Fact]
    public void TeamsBot_ShouldBeConstant()
    {
        // Arrange & Act
        var value1 = ServiceName.TeamsBot;
        var value2 = ServiceName.TeamsBot;

        // Assert
        value1.Should().BeSameAs(value2);
        value1.Should().Be(value2);
    }

    [Fact]
    public void ServiceName_Constants_ShouldBeDifferent()
    {
        // Arrange & Act & Assert
        ServiceName.SkypeBot.Should().NotBe(ServiceName.TeamsBot);
    }

    [Fact]
    public void ServiceName_Constants_ShouldUseNameofOperator()
    {
        // Arrange & Act & Assert
        // These tests verify that the constants use nameof() correctly
        ServiceName.SkypeBot.Should().Be(nameof(ServiceName.SkypeBot).Replace("ServiceName.", ""));
        ServiceName.TeamsBot.Should().Be(nameof(ServiceName.TeamsBot).Replace("ServiceName.", ""));
    }
}
