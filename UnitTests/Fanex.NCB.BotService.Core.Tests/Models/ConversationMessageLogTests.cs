namespace Fanex.NCB.BotService.Core.Tests.Models;

using System;
using Fanex.NCB.BotService.Core.Enum;
using Fanex.NCB.BotService.Core.Models;
using FluentAssertions;
using Xunit;

public class ConversationMessageLogTests
{
    [Fact]
    public void IsSuccessful_WhenStatusIsSuccess_ReturnsTrue()
    {
        // Arrange
        var log = new ConversationMessageLog
        {
            Status = SentMessageStatus.Success
        };

        // Act
        var result = log.IsSuccessful;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsSuccessful_WhenStatusIsNotSuccess_ReturnsFalse()
    {
        // Arrange
        var log = new ConversationMessageLog
        {
            Status = SentMessageStatus.Fail
        };

        // Act
        var result = log.IsSuccessful;

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Properties_CanBeSetAndRetrieved()
    {
        // Arrange
        var conversationId = "test-conversation-id";
        var message = "Test message";
        var sentDate = DateTime.UtcNow;
        var status = SentMessageStatus.Success;

        // Act
        var log = new ConversationMessageLog
        {
            ConversationId = conversationId,
            Message = message,
            SentDate = sentDate,
            Status = status
        };

        // Assert
        log.ConversationId.Should().Be(conversationId);
        log.Message.Should().Be(message);
        log.SentDate.Should().Be(sentDate);
        log.Status.Should().Be(status);
    }

    [Fact]
    public void SentDate_CanBeNull()
    {
        // Arrange & Act
        var log = new ConversationMessageLog
        {
            SentDate = null
        };

        // Assert
        log.SentDate.Should().BeNull();
    }

    [Fact]
    public void DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var log = new ConversationMessageLog();

        // Assert
        log.ConversationId.Should().BeNull();
        log.Message.Should().BeNull();
        log.SentDate.Should().BeNull();
        log.Status.Should().Be(default(SentMessageStatus));
        log.IsSuccessful.Should().BeTrue(); // Since default status (0) is Success
    }
}
