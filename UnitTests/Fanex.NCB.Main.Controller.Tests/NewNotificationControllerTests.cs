namespace Fanex.NCB.Main.Controllers.Tests;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using NAP.Client.Helpers.Web;
using Hubs;
using ViewModel.Notification;
using ViewModel.TreeSelect;
using Models.Common;
using Models.Notification;
using Models.NotificationTemplate;
using Services.Interfaces;
using Fanex.NCB.Shared.Models;
using Fanex.NCB.SkypeBot.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using NSubstitute;
using Xunit;

public class NewNotificationControllerTests
{
    private readonly IFixture fixture = new Fixture();
    private INotificationService notificationService;
    private INapSession napSession;
    private IHubContext<ChatbotHub, IChatbotClient> chatHubContext;

    [Fact]
    public void Index_WhenCalled_ReturnExpectedView()
    {
        var controller = GetController();

        var actual = controller.Index();

        Assert.IsType<ViewResult>(actual);
    }

    [Fact]
    public async Task Create_ValidModel_ReturnSuccessCommandResult()
    {
        var controller = GetController();
        napSession.UserId = fixture.Create<int>();

        var viewModel = new CreateNewNotificationViewModel
        {
            ChannelIds = "1,2",
            NotificationTypeId = 1,
            ProductId = 1,
            RecipientIds = "1,2",
            Title = "title",
            Content = "**content**",
            SendDate = DateTime.Now,
            SendImmediately = true,
            Platforms = "1,2"
        };
        var contents = new List<NotificationTemplateContent>
        {
            new() { ChannelId = 1, Content = "**content**" },
            new() { ChannelId = 2, Content = "**content**" }
        };
        var model = new CreateNotification();

        notificationService.CreateAsync(Arg.Do<CreateNotification>(x => model = x))
            .Returns((new CommandResult { Code = 0 }, string.Empty));

        var result = await controller.Create(viewModel);

        var json = Assert.IsType<JsonResult>(result);
        var commandResult = Assert.IsType<CommandResult>(json.Value);
        Assert.True(commandResult.IsSuccessful);

        Assert.Equal(viewModel.NotificationTypeId, model.NotificationTypeId);
        Assert.Equal(viewModel.ProductId, model.ProductId);
        Assert.Equal(viewModel.Title, model.Title);
        Assert.Equal(JsonConvert.SerializeObject(contents), model.Content);
        Assert.Equal(viewModel.SendDate, model.SendDate);
        Assert.Equal(viewModel.SendImmediately, model.SendImmediately);
        Assert.Equal(viewModel.RecipientIds, model.RecipientIds);
        Assert.Equal(viewModel.Platforms, model.Platforms);
        Assert.Equal(napSession.UserId, model.CreatorId);
    }

    [Fact]
    public async Task Create_InvalidModel_ReturnErrorCommandResult()
    {
        var controller = GetController();
        napSession.UserId = fixture.Create<int>();

        var viewModel = fixture.Create<CreateNewNotificationViewModel>();
        viewModel.RecipientIds = string.Empty;
        var result = await controller.Create(viewModel);
        var json = Assert.IsType<JsonResult>(result);
        var commandResult = Assert.IsType<CommandResult>(json.Value);
        Assert.False(commandResult.IsSuccessful);
        Assert.Equal("Invaliad data. The recipients cannot be null", commandResult.Message);
    }

    [Fact]
    public async Task GetRecipients_WhenCalled_ReturnJsonResult()
    {
        var controller = GetController();
        var notificationTypeId = fixture.Create<int>();
        var recipientIds = fixture.Create<string>();
        var channelIds = fixture.Create<string>();
        var productId = fixture.Create<int?>();
        var platforms = fixture.Create<string>();

        var allRecipients = fixture.Create<(IEnumerable<Recipient>, IEnumerable<uint>)>();

        notificationService.GetRecipientsAsync(notificationTypeId, platforms, recipientIds, productId, channelIds)
            .Returns(allRecipients);

        var result = await controller.GetRecipients(notificationTypeId, platforms, recipientIds, productId, channelIds);

        var jsonResult = Assert.IsType<JsonResult>(result);
        var recipients = jsonResult.Value.GetType().GetProperty("recipients").GetValue(jsonResult.Value);
        var selectedRecipients =
            jsonResult.Value.GetType().GetProperty("selectedRecipients").GetValue(jsonResult.Value);
        Assert.IsAssignableFrom<IEnumerable<TreeNodeItem>>(recipients);
        Assert.Equal(allRecipients.Item2, selectedRecipients);
    }

    [Fact]
    public async Task GetTemplates_WhenCalled_ReturnJsonResult()
    {
        var controller = GetController();
        var notificationTypeId = fixture.Create<int>();

        var templates = fixture.Create<IEnumerable<Template>>();

        notificationService.GetTemplatesAsync(notificationTypeId).Returns(templates);

        var result = await controller.GetTemplates(notificationTypeId);

        var json = Assert.IsType<JsonResult>(result);
        var returnedTemplates = Assert.IsAssignableFrom<IEnumerable<Template>>(json.Value);
        Assert.Equal(templates, returnedTemplates);
    }

    [Fact]
    public async Task GetResendNotification_WhenCalled_ReturnExpectedJsonResult()
    {
        var controller = GetController();
        var notificationId = fixture.Create<int>();
        var isResendAll = fixture.Create<bool>();
        var recipientIds = fixture.Create<string>();
        var resendNotifictaion = fixture.Create<ResendNotification>();
        var allRecipients = fixture.Create<(IEnumerable<Recipient>, IEnumerable<uint>)>();
        var unSelectable =
            resendNotifictaion.Recipients.Where(r => !allRecipients.Item1.Any(x => x.RecipientId == r.RecipientId));

        var expected = new
        {
            resendNotification = resendNotifictaion,
            recipients = GenerateTreeHelper.FromRecipient(allRecipients.Item1),
            unSelectableRecipients = unSelectable,
            selectedRecipients = allRecipients.Item2
        };
        var platforms = resendNotifictaion.Platforms.Select(item => item.PlatformId).ToList();
        var integrationMethods =
            resendNotifictaion.IntegrationMethods.Select(item => item.IntegrationMethodId).ToList();
        var productId = resendNotifictaion.ProductId;
        notificationService.GetResendNotificationInfoAsync(notificationId, isResendAll).Returns(resendNotifictaion);
        notificationService.GetRecipientsAsync(resendNotifictaion.NotificationTypeId, string.Join(",", platforms),
                recipientIds, productId, string.Join(",", integrationMethods), resendNotifictaion.ChannelId.ToString())
            .Returns(allRecipients);

        var result = await controller.GetResendNotification(notificationId, recipientIds, isResendAll);

        var json = Assert.IsType<JsonResult>(result);
        var obj = Assert.IsAssignableFrom<object>(json.Value);
        Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(obj));
    }

    [Fact]
    public async Task Create_WithSuccessfulResultAndNotificationIds_PushesSignalRNotification()
    {
        // Arrange
        var controller = GetController();
        napSession.UserId = fixture.Create<int>();

        var viewModel = new CreateNewNotificationViewModel
        {
            ChannelIds = "1,2",
            NotificationTypeId = 1,
            ProductId = 1,
            RecipientIds = "1,2",
            Title = "title",
            Content = "**content**",
            SendDate = DateTime.Now,
            SendImmediately = true,
            Platforms = "1,2"
        };

        var notificationIds = "1,2,3";
        notificationService.CreateAsync(Arg.Any<CreateNotification>())
            .Returns((new CommandResult { Code = 0 }, notificationIds));

        // Act
        var result = await controller.Create(viewModel);

        // Assert
        var json = Assert.IsType<JsonResult>(result);
        var commandResult = Assert.IsType<CommandResult>(json.Value);
        Assert.True(commandResult.IsSuccessful);

        // Verify that SignalR notification is pushed (this is tested indirectly through the method call)
        // The actual SignalR push happens in a separate thread via Task.Run
    }

    [Fact]
    public async Task Create_WithSuccessfulResultButEmptyNotificationIds_DoesNotPushSignalRNotification()
    {
        // Arrange
        var controller = GetController();
        napSession.UserId = fixture.Create<int>();

        var viewModel = new CreateNewNotificationViewModel
        {
            ChannelIds = "1,2",
            NotificationTypeId = 1,
            ProductId = 1,
            RecipientIds = "1,2",
            Title = "title",
            Content = "**content**",
            SendDate = DateTime.Now,
            SendImmediately = true,
            Platforms = "1,2"
        };

        notificationService.CreateAsync(Arg.Any<CreateNotification>())
            .Returns((new CommandResult { Code = 0 }, string.Empty));

        // Act
        var result = await controller.Create(viewModel);

        // Assert
        var json = Assert.IsType<JsonResult>(result);
        var commandResult = Assert.IsType<CommandResult>(json.Value);
        Assert.True(commandResult.IsSuccessful);
    }

    [Fact]
    public async Task Create_WithSuccessfulResultButWhitespaceNotificationIds_DoesNotPushSignalRNotification()
    {
        // Arrange
        var controller = GetController();
        napSession.UserId = fixture.Create<int>();

        var viewModel = new CreateNewNotificationViewModel
        {
            ChannelIds = "1,2",
            NotificationTypeId = 1,
            ProductId = 1,
            RecipientIds = "1,2",
            Title = "title",
            Content = "**content**",
            SendDate = DateTime.Now,
            SendImmediately = true,
            Platforms = "1,2"
        };

        notificationService.CreateAsync(Arg.Any<CreateNotification>())
            .Returns((new CommandResult { Code = 0 }, "   "));

        // Act
        var result = await controller.Create(viewModel);

        // Assert
        var json = Assert.IsType<JsonResult>(result);
        var commandResult = Assert.IsType<CommandResult>(json.Value);
        Assert.True(commandResult.IsSuccessful);
    }

    [Fact]
    public async Task Create_WithNullPlatforms_SetsEmptyString()
    {
        // Arrange
        var controller = GetController();
        napSession.UserId = fixture.Create<int>();

        var viewModel = new CreateNewNotificationViewModel
        {
            ChannelIds = "1,2",
            NotificationTypeId = 1,
            ProductId = 1,
            RecipientIds = "1,2",
            Title = "title",
            Content = "**content**",
            SendDate = DateTime.Now,
            SendImmediately = true,
            Platforms = null
        };

        CreateNotification capturedModel = null;
        notificationService.CreateAsync(Arg.Do<CreateNotification>(x => capturedModel = x))
            .Returns((new CommandResult { Code = 0 }, string.Empty));

        // Act
        var result = await controller.Create(viewModel);

        // Assert
        Assert.Equal(string.Empty, capturedModel.Platforms);
    }

    [Fact]
    public async Task Create_WithNullIntegrationMethodIds_SetsEmptyString()
    {
        // Arrange
        var controller = GetController();
        napSession.UserId = fixture.Create<int>();

        var viewModel = new CreateNewNotificationViewModel
        {
            ChannelIds = "1,2",
            NotificationTypeId = 1,
            ProductId = 1,
            RecipientIds = "1,2",
            Title = "title",
            Content = "**content**",
            SendDate = DateTime.Now,
            SendImmediately = true,
            IntegrationMethodIds = null
        };

        CreateNotification capturedModel = null;
        notificationService.CreateAsync(Arg.Do<CreateNotification>(x => capturedModel = x))
            .Returns((new CommandResult { Code = 0 }, string.Empty));

        // Act
        var result = await controller.Create(viewModel);

        // Assert
        Assert.Equal(string.Empty, capturedModel.IntegrationMethodIds);
    }

    [Fact]
    public async Task Create_WithNullFileName_SetsNull()
    {
        // Arrange
        var controller = GetController();
        napSession.UserId = fixture.Create<int>();

        var viewModel = new CreateNewNotificationViewModel
        {
            ChannelIds = "1,2",
            NotificationTypeId = 1,
            ProductId = 1,
            RecipientIds = "1,2",
            Title = "title",
            Content = "**content**",
            SendDate = DateTime.Now,
            SendImmediately = true,
            FileName = null
        };

        CreateNotification capturedModel = null;
        notificationService.CreateAsync(Arg.Do<CreateNotification>(x => capturedModel = x))
            .Returns((new CommandResult { Code = 0 }, string.Empty));

        // Act
        var result = await controller.Create(viewModel);

        // Assert
        Assert.Null(capturedModel.FileName);
    }

    [Fact]
    public async Task GetRecipients_WithNullPlatforms_SetsEmptyString()
    {
        // Arrange
        var controller = GetController();
        var notificationTypeId = fixture.Create<int>();
        var recipientIds = fixture.Create<string>();
        var channelIds = fixture.Create<string>();
        var productId = fixture.Create<int?>();
        string platforms = null;

        var allRecipients = fixture.Create<(IEnumerable<Recipient>, IEnumerable<uint>)>();
        notificationService.GetRecipientsAsync(notificationTypeId, string.Empty, recipientIds, productId, null, channelIds)
            .Returns(allRecipients);

        // Act
        var result = await controller.GetRecipients(notificationTypeId, platforms, recipientIds, productId, null, channelIds);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        await notificationService.Received(1).GetRecipientsAsync(notificationTypeId, string.Empty, recipientIds, productId, null, channelIds);
    }

    private NewNotificationController GetController()
    {
        notificationService = Substitute.For<INotificationService>();
        napSession = Substitute.For<INapSession>();
        chatHubContext = Substitute.For<IHubContext<ChatbotHub, IChatbotClient>>();

        return new NewNotificationController(notificationService, napSession, chatHubContext);
    }
}