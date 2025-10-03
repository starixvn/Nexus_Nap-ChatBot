namespace Fanex.NCB.Main.Controller.Tests.Hubs;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Fanex.NCB.Main.Hubs;
using Fanex.NCB.Models.Notification;
using Fanex.NCB.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using NSubstitute;
using Xunit;

public class ChatbotHubTests
{
    private readonly IFixture fixture = new Fixture();
    private readonly INotificationService notificationService;
    private readonly IHubCallerClients<IChatbotClient> clients;
    private readonly IChatbotClient allClients;
    private readonly HubCallerContext context;

    public ChatbotHubTests()
    {
        notificationService = Substitute.For<INotificationService>();
        clients = Substitute.For<IHubCallerClients<IChatbotClient>>();
        allClients = Substitute.For<IChatbotClient>();
        context = Substitute.For<HubCallerContext>();

        clients.All.Returns(allClients);
    }

    private ChatbotHub GetHub()
    {
        var hub = new ChatbotHub(notificationService)
        {
            Clients = clients,
            Context = context
        };
        return hub;
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidNotificationService_CreatesInstance()
    {
        // Arrange & Act
        var hub = new ChatbotHub(notificationService);

        // Assert
        hub.Should().NotBeNull();
        hub.Should().BeOfType<ChatbotHub>();
    }

    [Fact]
    public void Constructor_WithNullNotificationService_CreatesInstance()
    {
        // Arrange, Act & Assert - Constructor doesn't validate parameters
        var hub = new ChatbotHub(null);
        hub.Should().NotBeNull();
    }

    #endregion

    #region UpdateNewNotification Tests

    [Fact]
    public async Task UpdateNewNotification_WithValidNotificationIds_CallsClientsAll()
    {
        // Arrange
        var hub = GetHub();
        var notificationIds = fixture.Create<string>();

        // Act
        await hub.UpdateNewNotification(notificationIds);

        // Assert
        await allClients.Received(1).InsertNewNotifications(notificationIds);
    }

    [Fact]
    public async Task UpdateNewNotification_WithEmptyString_CallsClientsAll()
    {
        // Arrange
        var hub = GetHub();
        var notificationIds = string.Empty;

        // Act
        await hub.UpdateNewNotification(notificationIds);

        // Assert
        await allClients.Received(1).InsertNewNotifications(notificationIds);
    }

    [Fact]
    public async Task UpdateNewNotification_WithNullString_CallsClientsAll()
    {
        // Arrange
        var hub = GetHub();
        string notificationIds = null;

        // Act
        await hub.UpdateNewNotification(notificationIds);

        // Assert
        await allClients.Received(1).InsertNewNotifications(notificationIds);
    }

    #endregion

    #region UpdateNotificationStatus Tests

    [Fact]
    public async Task UpdateNotificationStatus_WithValidNotificationIds_CallsClientsAll()
    {
        // Arrange
        var hub = GetHub();
        var notificationIds = fixture.Create<string>();

        // Act
        await hub.UpdateNotificationStatus(notificationIds);

        // Assert
        await allClients.Received(1).UpdateNotificationStatus(notificationIds);
    }

    [Fact]
    public async Task UpdateNotificationStatus_WithEmptyString_CallsClientsAll()
    {
        // Arrange
        var hub = GetHub();
        var notificationIds = string.Empty;

        // Act
        await hub.UpdateNotificationStatus(notificationIds);

        // Assert
        await allClients.Received(1).UpdateNotificationStatus(notificationIds);
    }

    #endregion

    #region UpdateErrorNotification Tests

    [Fact]
    public async Task UpdateErrorNotification_CallsNotificationServiceAndClientsAll()
    {
        // Arrange
        var hub = GetHub();
        var errNotifications = fixture.CreateMany<NotificationPaging>(3).ToList();
        notificationService.GetErrNotificationsAsync().Returns(errNotifications);

        // Act
        await hub.UpdateErrorNotification();

        // Assert
        await notificationService.Received(1).GetErrNotificationsAsync();
        await allClients.Received(1).UpdateErrNotification(Arg.Any<IEnumerable<Notification>>());
    }

    [Fact]
    public async Task UpdateErrorNotification_WithEmptyNotifications_CallsClientsAll()
    {
        // Arrange
        var hub = GetHub();
        var errNotifications = new List<NotificationPaging>();
        notificationService.GetErrNotificationsAsync().Returns(errNotifications);

        // Act
        await hub.UpdateErrorNotification();

        // Assert
        await notificationService.Received(1).GetErrNotificationsAsync();
        await allClients.Received(1).UpdateErrNotification(Arg.Any<IEnumerable<Notification>>());
    }

    [Fact]
    public async Task UpdateErrorNotification_WithNullNotifications_CallsClientsAll()
    {
        // Arrange
        var hub = GetHub();
        IEnumerable<NotificationPaging> errNotifications = null;
        notificationService.GetErrNotificationsAsync().Returns(errNotifications);

        // Act
        await hub.UpdateErrorNotification();

        // Assert
        await notificationService.Received(1).GetErrNotificationsAsync();
        await allClients.Received(1).UpdateErrNotification(Arg.Any<IEnumerable<Notification>>());
    }

    #endregion
}
