namespace Fanex.NCB.BotService.Core.Tests;

using Fanex.NCB.BotService.Core;
using FluentAssertions;
using Xunit;

public class BotHelperTests
{
    [Fact]
    public void GetMessageInGroupByChannel_WithAtMentionAndCommand_ReturnsCommandOnly()
    {
        // Arrange
        var sourceMessage = "@TestBot help me";
        var botName = "TestBot";

        // Act
        var result = BotHelper.GetMessageInGroupByChannel(sourceMessage, botName);

        // Assert
        result.Should().Be("help me");
    }

    [Fact]
    public void GetMessageInGroupByChannel_WithBotNameAndCommand_ReturnsCommandOnly()
    {
        // Arrange
        var sourceMessage = "TestBot help me";
        var botName = "TestBot";

        // Act
        var result = BotHelper.GetMessageInGroupByChannel(sourceMessage, botName);

        // Assert
        result.Should().Be("help me");
    }

    [Fact]
    public void GetMessageInGroupByChannel_WithAtMentionOnly_ReturnsOriginalMessage()
    {
        // Arrange
        var sourceMessage = "@TestBot";
        var botName = "TestBot";

        // Act
        var result = BotHelper.GetMessageInGroupByChannel(sourceMessage, botName);

        // Assert
        result.Should().Be("@TestBot");
    }

    [Fact]
    public void GetMessageInGroupByChannel_WithBotNameOnly_ReturnsOriginalMessage()
    {
        // Arrange
        var sourceMessage = "TestBot";
        var botName = "TestBot";

        // Act
        var result = BotHelper.GetMessageInGroupByChannel(sourceMessage, botName);

        // Assert
        result.Should().Be("TestBot");
    }

    [Fact]
    public void GetMessageInGroupByChannel_WithoutBotMention_ReturnsOriginalMessage()
    {
        // Arrange
        var sourceMessage = "Hello everyone";
        var botName = "TestBot";

        // Act
        var result = BotHelper.GetMessageInGroupByChannel(sourceMessage, botName);

        // Assert
        result.Should().Be("Hello everyone");
    }

    [Fact]
    public void GetMessageInGroupByChannel_WithDifferentBotName_ReturnsOriginalMessage()
    {
        // Arrange
        var sourceMessage = "@OtherBot help me";
        var botName = "TestBot";

        // Act
        var result = BotHelper.GetMessageInGroupByChannel(sourceMessage, botName);

        // Assert
        result.Should().Be("@OtherBot help me");
    }

    [Fact]
    public void GetMessageInGroupByChannel_WithPartialBotNameMatch_ReturnsOriginalMessage()
    {
        // Arrange
        var sourceMessage = "Test help me";
        var botName = "TestBot";

        // Act
        var result = BotHelper.GetMessageInGroupByChannel(sourceMessage, botName);

        // Assert
        result.Should().Be("Test help me");
    }

    [Fact]
    public void GetMessageInGroupByChannel_WithAtMentionAndMultipleSpaces_ReturnsCommandTrimmed()
    {
        // Arrange
        var sourceMessage = "@TestBot    help me";
        var botName = "TestBot";

        // Act
        var result = BotHelper.GetMessageInGroupByChannel(sourceMessage, botName);

        // Assert
        result.Should().Be("help me");
    }

    [Fact]
    public void GetMessageInGroupByChannel_WithEmptyMessage_ReturnsEmptyMessage()
    {
        // Arrange
        var sourceMessage = "";
        var botName = "TestBot";

        // Act
        var result = BotHelper.GetMessageInGroupByChannel(sourceMessage, botName);

        // Assert
        result.Should().Be("");
    }

    [Fact]
    public void GetMessageInGroupByChannel_WithNullMessage_ThrowsException()
    {
        // Arrange
        string sourceMessage = null;
        var botName = "TestBot";

        // Act & Assert
        Assert.Throws<System.NullReferenceException>(() =>
            BotHelper.GetMessageInGroupByChannel(sourceMessage, botName));
    }

    [Fact]
    public void GetMessageInGroupByChannel_WithBotNameInMiddle_ReturnsOriginalMessage()
    {
        // Arrange
        var sourceMessage = "Hello TestBot how are you";
        var botName = "TestBot";

        // Act
        var result = BotHelper.GetMessageInGroupByChannel(sourceMessage, botName);

        // Assert
        result.Should().Be("Hello TestBot how are you");
    }

    [Fact]
    public void GetMessageInGroupByChannel_WithCaseSensitiveBotName_ReturnsOriginalMessage()
    {
        // Arrange
        var sourceMessage = "@testbot help me";
        var botName = "TestBot";

        // Act
        var result = BotHelper.GetMessageInGroupByChannel(sourceMessage, botName);

        // Assert
        result.Should().Be("@testbot help me");
    }
}