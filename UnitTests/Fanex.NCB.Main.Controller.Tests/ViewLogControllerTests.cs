namespace Fanex.NCB.Main.Controllers.Tests;

using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Models.ViewLog;
using Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

public class ViewLogControllerTests
{
    private IViewLogService viewLogService;

    [Fact]
    public void Index_WhenCalled_ReturnExpectedView()
    {
        // Arrange
        var controller = GetController();

        var actual = controller.Index();

        // Assert
        Assert.IsType<ViewResult>(actual);
    }

    [Fact]
    public async Task GetNotificationSystemMessage_WhenCalled_ReturnJsonResult()
    {
        // Arrange
        var controller = GetController();

        var fixture = new Fixture();
        var notificationTypeId = fixture.Create<int?>();
        var pageIndex = fixture.Create<int>();
        var pageSize = fixture.Create<int>();
        var total = fixture.Create<int>();
        var templates = fixture.Create<IEnumerable<ViewLogNotificationSystemMessage>>();

        // Act
        viewLogService.GetNotificationSystemMessageAsync(notificationTypeId, pageIndex, pageSize)
            .Returns((total, templates));

        var result = await controller.GetNotificationSystemMessage(notificationTypeId, pageIndex, pageSize);

        // Assert
        Assert.IsType<JsonResult>(result);
    }

    [Fact]
    public async Task GetRecipientList_WhenCalled_ReturnJsonResult()
    {
        // Arrange
        var controller = GetController();

        var fixture = new Fixture();
        var notificationTypeId = fixture.Create<int?>();
        var pageIndex = fixture.Create<int>();
        var pageSize = fixture.Create<int>();
        var total = fixture.Create<int>();
        var templates = fixture.Create<IEnumerable<ViewLogNotificationRecipientList>>();

        // Act
        viewLogService.GetNotificationRecipientListAsync(notificationTypeId, pageIndex, pageSize)
            .Returns((total, templates));

        var result = await controller.GetNotificationRecipientList(notificationTypeId, pageIndex, pageSize);

        // Assert
        Assert.IsType<JsonResult>(result);
    }

    [Fact]
    public async Task GetConversationMessageLog_WhenCalled_ReturnJsonResult()
    {
        // Arrange
        var controller = GetController();

        var fixture = new Fixture();
        var notificationTypeId = fixture.Create<int?>();
        var pageIndex = fixture.Create<int>();
        var pageSize = fixture.Create<int>();
        var total = fixture.Create<int>();
        var templates = fixture.Create<IEnumerable<ViewLogConversationMessageLog>>();
        var templateMap = fixture.Create<IEnumerable<ViewLogConversationMessageLogRecipient>>();

        // Act
        viewLogService.GetConversationMessageLogAsync(notificationTypeId, pageIndex, pageSize)
            .Returns((total, templates, templateMap));

        var result = await controller.GetConversationMessageLog(notificationTypeId, pageIndex, pageSize);

        // Assert
        Assert.IsType<JsonResult>(result);
    }

    private ViewLogController GetController()
    {
        viewLogService = Substitute.For<IViewLogService>();
        return new ViewLogController(viewLogService);
    }
}