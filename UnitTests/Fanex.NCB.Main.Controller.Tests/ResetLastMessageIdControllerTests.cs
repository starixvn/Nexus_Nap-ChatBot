namespace Fanex.NCB.Main.Controllers.Tests;

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using NAP.Client.Configuration;
using NAP.Client.Helpers.Web;
using Fanex.NAP.Client.Services;
using Core.Constants;
using Criteria.ResetLastMessageId;
using Models.ResetLastMessageId;
using Services.Interfaces;
using Fanex.NCB.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;
using static Fanex.NAP.Core.Business.Constant;

public class ResetLastMessageIdControllerTests
{
    private readonly IFixture fixture = new Fixture();
    private INapSettings napSettings;
    private INapSession napSession;
    private IUserService userService;
    private IResetLastMessageIdService resetLastMessageIdService;

    [Fact]
    public void Index_WhenCalled_ReturnExpectedView()
    {
        // Arrange
        var controller = GetController();
        userService.IsHavingPermission(napSession.UserId, napSettings.SiteId, FunctionKey.ResetLastMessageId,
                Policy.Host)
            .Returns(true);

        // Action
        var actual = controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(actual);
        var haveHostPermission = Assert.IsType<bool>(viewResult.ViewData["HaveHostPermission"]);
        Assert.True(haveHostPermission);
    }

    [Fact]
    public async Task GetCurrentPreviousMaxMessage_WhenCalled_ReturnExpectedResult()
    {
        // Arrange
        var currentMaxMessageInfos = fixture.Create<IEnumerable<CurrentMaxMessageInfo>>();
        var messageTypeInfos = fixture.Create<IEnumerable<MessageTypeInfo>>();
        var (msgReqIds, messageIds) = fixture.Create<(IEnumerable<MessageRequestId>, IEnumerable<int>)>();
        var missedInfo = fixture.Create<IEnumerable<MissingMesssageInfo>>();
        var controller = GetController();
        resetLastMessageIdService.GetCurrentPreviousMaxMessageAsync()
            .Returns((currentMaxMessageInfos, messageTypeInfos));
        resetLastMessageIdService.GetAbnormalBetsMissingMessageAsync().Returns((msgReqIds, messageIds));
        resetLastMessageIdService.GetMissingMessageInfoAsync(Arg.Is(currentMaxMessageInfos), Arg.Is(messageTypeInfos),
            Arg.Is(msgReqIds), Arg.Is(messageIds)).Returns(missedInfo);

        // Act
        var result = await controller.GetCurrentPreviousMaxMessage();

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var currentMaxMessage =
            (IEnumerable<CurrentMaxMessageInfo>)jsonResult.Value.GetType().GetProperty("data")
                .GetValue(jsonResult.Value);
        var internalMissingMsg = (IEnumerable<MessageRequestId>)jsonResult.Value.GetType()
            .GetProperty("internalMissingMsg").GetValue(jsonResult.Value);
        var missingMsgInfo = (IEnumerable<MissingMesssageInfo>)jsonResult.Value.GetType().GetProperty("missingMsgInfo")
            .GetValue(jsonResult.Value);
        Assert.Equal(currentMaxMessageInfos, currentMaxMessage);
        Assert.Equal(msgReqIds, internalMissingMsg);
        Assert.Equal(missedInfo, missingMsgInfo);
    }

    [Fact]
    public async Task CreateMissingMessage_WhenCalled_ReturnExpectedResult()
    {
        // Arrange
        var command = fixture.Create<CreateMissingMessageCommand>();
        var commandResult = fixture.Create<CommandResult>();
        var listMsgs = fixture.Create<List<MissingInfoIns>>();
        var controller = GetController();
        resetLastMessageIdService.CreateMissingMessageAsync(listMsgs).Returns(commandResult);

        // Act
        var result = await controller.CreateMissingMessage(listMsgs);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var actual = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
        Assert.Equal(commandResult, actual);
    }

    private ResetLastMessageIdController GetController()
    {
        napSettings = Substitute.For<INapSettings>();
        napSession = Substitute.For<INapSession>();
        userService = Substitute.For<IUserService>();
        resetLastMessageIdService = Substitute.For<IResetLastMessageIdService>();

        return new ResetLastMessageIdController(napSettings, napSession, userService, resetLastMessageIdService);
    }
}