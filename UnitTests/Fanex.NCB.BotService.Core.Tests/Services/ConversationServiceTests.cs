namespace Fanex.NCB.BotService.Core.Tests.Services;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Fanex.NCB.BotService.Core.Criteria;
using Fanex.NCB.BotService.Core.Models;
using Fanex.NCB.BotService.Core.Services;
using Fanex.NCB.Shared.Models;
using Fanex.NCB.Shared.Services.Interfaces;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class ConversationServiceTests
{
    private readonly IFixture fixture = new Fixture();
    private readonly IBaseService baseService;
    private readonly CheckDuplicatedMessageLogSetting settings;
    private readonly ConversationService conversationService;

    public ConversationServiceTests()
    {
        baseService = Substitute.For<IBaseService>();
        settings = fixture.Create<CheckDuplicatedMessageLogSetting>();
        conversationService = new ConversationService(baseService, settings);
    }

    [Fact]
    public async Task AddBotToGroup_WithValidCommand_CallsBaseService()
    {
        // Arrange
        var command = fixture.Create<AddBotToGroupCommand>();
        var expectedResult = fixture.Create<CommandResult>();
        baseService.ExecuteNonQueryAsync(command).Returns(expectedResult);

        // Act
        var result = await conversationService.AddBotToGroup(command);

        // Assert
        result.Should().Be(expectedResult);
        await baseService.Received(1).ExecuteNonQueryAsync(command);
    }

    [Fact]
    public async Task RemoveBotOutGroup_WithValidCommand_CallsBaseService()
    {
        // Arrange
        var command = fixture.Create<RemoveBotOutGroupCommand>();
        var expectedResult = fixture.Create<CommandResult>();
        baseService.ExecuteNonQueryAsync(command).Returns(expectedResult);

        // Act
        var result = await conversationService.RemoveBotOutGroup(command);

        // Assert
        result.Should().Be(expectedResult);
        await baseService.Received(1).ExecuteNonQueryAsync(command);
    }

    [Fact]
    public async Task UpdateGroupTopic_WithValidCommand_CallsBaseService()
    {
        // Arrange
        var command = fixture.Create<UpdateGroupTopicCommand>();
        var expectedResult = fixture.Create<CommandResult>();
        baseService.ExecuteNonQueryAsync(command).Returns(expectedResult);

        // Act
        var result = await conversationService.UpdateGroupTopic(command);

        // Assert
        result.Should().Be(expectedResult);
        await baseService.Received(1).ExecuteNonQueryAsync(command);
    }

    [Fact]
    public async Task MigratedToSupergroupAsync_WithValidCommand_CallsBaseService()
    {
        // Arrange
        var command = fixture.Create<MigratedToSupergroupCommand>();
        var expectedResult = fixture.Create<CommandResult>();
        baseService.ExecuteNonQueryAsync(command).Returns(expectedResult);

        // Act
        var result = await conversationService.MigratedToSupergroupAsync(command);

        // Assert
        result.Should().Be(expectedResult);
        await baseService.Received(1).ExecuteNonQueryAsync(command);
    }

    [Fact]
    public async Task AddMessageLog_WithValidCommand_CallsBaseService()
    {
        // Arrange
        var command = fixture.Create<AddMessageLogCommand>();
        var expectedResult = fixture.Create<CommandResult>();
        baseService.ExecuteNonQueryAsync(command).Returns(expectedResult);

        // Act
        var result = await conversationService.AddMessageLog(command);

        // Assert
        result.Should().Be(expectedResult);
        await baseService.Received(1).ExecuteNonQueryAsync(command);
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange & Act
        var service = new ConversationService(baseService, settings);

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithNullBaseService_CreatesInstance()
    {
        // Arrange & Act
        var service = new ConversationService(null, settings);

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithNullSettings_CreatesInstance()
    {
        // Arrange & Act
        var service = new ConversationService(baseService, null);

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public void GetDistinctNotificationConversation_WithEmptyList_ReturnsEmptyList()
    {
        // Arrange
        var notifications = new List<NotificationMessage>();

        // Act
        var result = conversationService.GetDistinctNotificationConversation(notifications);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetDistinctNotificationConversation_WithSingleNotification_ReturnsDistinctConversations()
    {
        // Arrange
        var notification = new NotificationMessage
        {
            NotificationId = 1,
            Message = "Test message",
            Recipients = new List<ConversationInfo>
            {
                new ConversationInfo
                {
                    BotName = "TestBot",
                    BotConversationId = "bot123",
                    ChannelId = "channel1",
                    ConversationId = "conv1",
                    ServiceUrl = "https://test.com"
                }
            }
        };
        var notifications = new List<NotificationMessage> { notification };

        // Act
        var result = conversationService.GetDistinctNotificationConversation(notifications);

        // Assert
        result.Should().HaveCount(1);
        result.First().NotificationId.Should().Be(1);
        result.First().Message.Should().Be("Test message");
    }

    [Fact]
    public void GetDistinctNotificationConversation_WithDuplicateConversations_RemovesDuplicates()
    {
        // Arrange
        var notification = new NotificationMessage
        {
            NotificationId = 1,
            Message = "Test message",
            Recipients = new List<ConversationInfo>
            {
                new ConversationInfo
                {
                    BotName = "TestBot",
                    BotConversationId = "bot123",
                    ChannelId = "channel1",
                    ConversationId = "conv1",
                    ServiceUrl = "https://test.com"
                },
                new ConversationInfo
                {
                    BotName = "TestBot",
                    BotConversationId = "bot123",
                    ChannelId = "channel1",
                    ConversationId = "conv1",
                    ServiceUrl = "https://test.com"
                }
            }
        };
        var notifications = new List<NotificationMessage> { notification };

        // Act
        var result = conversationService.GetDistinctNotificationConversation(notifications);

        // Assert
        result.Should().HaveCount(1);
        result.First().Recipients.Should().HaveCount(1);
    }

    [Fact]
    public void GetDistinctNotificationConversation_WithExceptionGroup_DoesNotMarkAsDuplicated()
    {
        // Arrange
        settings.ExceptionConversatiionIDs = new[] { "conv1" };
        var notification = new NotificationMessage
        {
            NotificationId = 1,
            Message = "Test message",
            Recipients = new List<ConversationInfo>
            {
                new ConversationInfo
                {
                    BotName = "TestBot",
                    BotConversationId = "bot123",
                    ChannelId = "channel1",
                    ConversationId = "conv1",
                    ServiceUrl = "https://test.com"
                }
            }
        };
        var notifications = new List<NotificationMessage> { notification };

        // Act
        var result = conversationService.GetDistinctNotificationConversation(notifications);

        // Assert
        result.Should().HaveCount(1);
        result.First().Recipients.First().IsDuplicated.Should().BeFalse();
    }

    [Fact]
    public void GetDistinctNotificationConversation_WithMultipleNotifications_ProcessesAll()
    {
        // Arrange
        var notifications = new List<NotificationMessage>
        {
            new NotificationMessage
            {
                NotificationId = 1,
                Message = "Message 1",
                Recipients = new List<ConversationInfo>
                {
                    new ConversationInfo
                    {
                        BotName = "Bot1",
                        BotConversationId = "bot1",
                        ChannelId = "channel1",
                        ConversationId = "conv1",
                        ServiceUrl = "https://test1.com"
                    }
                }
            },
            new NotificationMessage
            {
                NotificationId = 2,
                Message = "Message 2",
                Recipients = new List<ConversationInfo>
                {
                    new ConversationInfo
                    {
                        BotName = "Bot2",
                        BotConversationId = "bot2",
                        ChannelId = "channel2",
                        ConversationId = "conv2",
                        ServiceUrl = "https://test2.com"
                    }
                }
            }
        };

        // Act
        var result = conversationService.GetDistinctNotificationConversation(notifications);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(n => n.NotificationId == 1);
        result.Should().Contain(n => n.NotificationId == 2);
    }
}
