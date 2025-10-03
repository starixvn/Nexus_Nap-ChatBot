namespace Fanex.NCB.AutoJobs.Core.Tests.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Fanex.NCB.AutoJobs.Core.Criteria.Common;
using Fanex.NCB.AutoJobs.Core.Criteria.MonitorMatch;
using Fanex.NCB.AutoJobs.Core.Models.Common;
using Fanex.NCB.AutoJobs.Core.Models.Core;
using Fanex.NCB.AutoJobs.Core.Models.MonitorMatch;
using Fanex.NCB.AutoJobs.Core.Models.NotifictaionSendMessage;
using Fanex.NCB.AutoJobs.Core.Services;
using Fanex.NCB.AutoJobs.Core.Services.Interfaces;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class MonitorMatchServiceTests
{
    private readonly IFixture fixture = new Fixture();
    private readonly IBaseService baseService;
    private readonly IChatbotService chatbotService;
    private readonly ICommonService commonService;
    private readonly ILogService logService;

    public MonitorMatchServiceTests()
    {
        baseService = Substitute.For<IBaseService>();
        chatbotService = Substitute.For<IChatbotService>();
        commonService = Substitute.For<ICommonService>();
        logService = Substitute.For<ILogService>();
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange & Act
        var service = GetService();

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public async Task SendMessageMonitoredMatchAsync_SystemSettingIsNull_LogsError()
    {
        // Arrange
        baseService.GetDataEntityAsync<MonitorMatchSetting>(Arg.Any<GetServiceSettingCriteria>())
            .Returns((MonitorMatchSetting)null);
        var service = GetService();

        // Act
        await service.SendMessageMonitoredMatchAsync();

        // Assert
        await logService.Received(1).WriteMessageSentryErrorAsync("Match Monitoring service setting is null.");
    }

    [Fact]
    public async Task SendMessageMonitoredMatchAsync_SystemSettingExists_CallsGetMonitorMatchMessages()
    {
        // Arrange
        var systemSetting = fixture.Create<MonitorMatchSetting>();
        baseService.GetDataEntityAsync<MonitorMatchSetting>(Arg.Any<GetServiceSettingCriteria>())
            .Returns(systemSetting);
        baseService.GetDataAsync<MonitorMatchInfo>(Arg.Any<GetMsgSettingMonitorMatchCriteria>())
            .Returns(new List<MonitorMatchInfo>());
        var service = GetService();

        // Act
        await service.SendMessageMonitoredMatchAsync();

        // Assert
        await baseService.Received(1).GetDataEntityAsync<MonitorMatchSetting>(
            Arg.Is<GetServiceSettingCriteria>(x => x.InServiceName == "Match Monitoring"));
        await baseService.Received(1).GetDataAsync<MonitorMatchInfo>(Arg.Any<GetMsgSettingMonitorMatchCriteria>());
    }

    [Fact]
    public async Task SendMessageMonitoredMatchAsync_NoMessages_DoesNotImportMessages()
    {
        // Arrange
        var systemSetting = fixture.Create<MonitorMatchSetting>();
        baseService.GetDataEntityAsync<MonitorMatchSetting>(Arg.Any<GetServiceSettingCriteria>())
            .Returns(systemSetting);
        baseService.GetDataAsync<MonitorMatchInfo>(Arg.Any<GetMsgSettingMonitorMatchCriteria>())
            .Returns(new List<MonitorMatchInfo>());
        var service = GetService();

        // Act
        await service.SendMessageMonitoredMatchAsync();

        // Assert
        await baseService.DidNotReceive().FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(
            Arg.Any<ImportMsgFromMonitorMatchCriteria>());
        await commonService.DidNotReceive().OnAfterImportDataAsync(
            Arg.Any<CommandResult>(), Arg.Any<IEnumerable<NotificationMessage>>());
    }

    [Fact]
    public async Task SendMessageMonitoredMatchAsync_HasMessages_ImportsMessagesAndCallsAfterImport()
    {
        // Arrange
        var systemSetting = fixture.Create<MonitorMatchSetting>();
        var messages = new List<MonitorMatchInfo>
        {
            new MonitorMatchInfo { LogId = 1, HomeTeam = "Team1", AwayTeam = "Team2", SettingInfo = "[]" },
            new MonitorMatchInfo { LogId = 2, HomeTeam = "Team3", AwayTeam = "Team4", SettingInfo = "[]" },
            new MonitorMatchInfo { LogId = 3, HomeTeam = "Team5", AwayTeam = "Team6", SettingInfo = "[]" }
        };
        var recipients = fixture.CreateMany<NotificationRecipient>(2).ToList();
        var notificationSendMessages = fixture.CreateMany<NotificationSendMessage>(2).ToList();
        var notificationMessages = fixture.CreateMany<NotificationMessage>(2).ToList();

        baseService.GetDataEntityAsync<MonitorMatchSetting>(Arg.Any<GetServiceSettingCriteria>())
            .Returns(systemSetting);
        baseService.GetDataAsync<MonitorMatchInfo>(Arg.Any<GetMsgSettingMonitorMatchCriteria>())
            .Returns(messages);
        baseService.FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(
            Arg.Any<ImportMsgFromMonitorMatchCriteria>())
            .Returns((recipients, notificationSendMessages));
        chatbotService.GenerateNotifificationMessageForCarbonGroupAsync(
            notificationSendMessages, recipients)
            .Returns(notificationMessages);

        var service = GetService();

        // Act
        await service.SendMessageMonitoredMatchAsync();

        // Assert
        await baseService.Received(1).FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(
            Arg.Any<ImportMsgFromMonitorMatchCriteria>());
        chatbotService.Received(1).GenerateNotifificationMessageForCarbonGroupAsync(
            notificationSendMessages, recipients);
        await commonService.Received(1).OnAfterImportDataAsync(
            Arg.Any<CommandResult>(), notificationMessages);
    }

    [Fact]
    public async Task SendMessageMonitoredMatchAsync_GetMsgSettingMonitorMatchCriteria_SetsCorrectProperties()
    {
        // Arrange
        var systemSetting = new MonitorMatchSetting
        {
            LastLogId = 123,
            BatchSize = 50,
            LastDataRow = "100",
            ServerName = "TestServer"
        };

        baseService.GetDataEntityAsync<MonitorMatchSetting>(Arg.Any<GetServiceSettingCriteria>())
            .Returns(systemSetting);
        baseService.GetDataAsync<MonitorMatchInfo>(Arg.Any<GetMsgSettingMonitorMatchCriteria>())
            .Returns(new List<MonitorMatchInfo>());
        var service = GetService();

        // Act
        await service.SendMessageMonitoredMatchAsync();

        // Assert
        await baseService.Received(1).GetDataAsync<MonitorMatchInfo>(
            Arg.Is<GetMsgSettingMonitorMatchCriteria>(x =>
                x.LastLogId == 123 &&
                x.BatchSize == 50 &&
                x.LastDataRow == "100" &&
                x.ServerName == "TestServer"));
    }

    [Fact]
    public async Task SendMessageMonitoredMatchAsync_ImportMsgFromMonitorMatchCriteria_SetsCorrectProperties()
    {
        // Arrange
        var systemSetting = fixture.Create<MonitorMatchSetting>();
        var messages = new List<MonitorMatchInfo>
        {
            new MonitorMatchInfo { LogId = 1, HomeTeam = "Team1", AwayTeam = "Team2", SettingInfo = "[]" },
            new MonitorMatchInfo { LogId = 2, HomeTeam = "Team3", AwayTeam = "Team4", SettingInfo = "[]" }
        };
        var recipients = fixture.CreateMany<NotificationRecipient>(1).ToList();
        var notificationSendMessages = fixture.CreateMany<NotificationSendMessage>(1).ToList();

        var criteria = new GetMsgSettingMonitorMatchCriteria
        {
            LastDataRow = "200",
            ServerName = "TestServer2",
            MaxLogId = 456
        };

        baseService.GetDataEntityAsync<MonitorMatchSetting>(Arg.Any<GetServiceSettingCriteria>())
            .Returns(systemSetting);
        baseService.GetDataAsync<MonitorMatchInfo>(Arg.Do<GetMsgSettingMonitorMatchCriteria>(x =>
        {
            x.LastDataRow = criteria.LastDataRow;
            x.ServerName = criteria.ServerName;
            x.MaxLogId = criteria.MaxLogId;
        }))
            .Returns(messages);
        baseService.FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(
            Arg.Any<ImportMsgFromMonitorMatchCriteria>())
            .Returns((recipients, notificationSendMessages));
        chatbotService.GenerateNotifificationMessageForCarbonGroupAsync(
            Arg.Any<IEnumerable<NotificationSendMessage>>(), Arg.Any<IEnumerable<NotificationRecipient>>())
            .Returns(new List<NotificationMessage>());

        var service = GetService();

        // Act
        await service.SendMessageMonitoredMatchAsync();

        // Assert
        await baseService.Received(1).FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(
            Arg.Is<ImportMsgFromMonitorMatchCriteria>(x =>
                x.ServerName == criteria.ServerName &&
                x.LastDataRow == criteria.LastDataRow &&
                x.PLastLogId == criteria.MaxLogId &&
                !string.IsNullOrEmpty(x.Message)));
    }

    [Fact]
    public async Task SendMessageMonitoredMatchAsync_CommandResult_SetsCorrectCodeAndMessage()
    {
        // Arrange
        var systemSetting = fixture.Create<MonitorMatchSetting>();
        var messages = new List<MonitorMatchInfo>
        {
            new MonitorMatchInfo { LogId = 1, HomeTeam = "Team1", AwayTeam = "Team2", SettingInfo = "[]" }
        };
        var recipients = fixture.CreateMany<NotificationRecipient>(1).ToList();
        var notificationSendMessages = fixture.CreateMany<NotificationSendMessage>(1).ToList();
        var notificationMessages = fixture.CreateMany<NotificationMessage>(1).ToList();

        var expectedErrCode = 1;
        var expectedErrMsg = "Test error message";

        baseService.GetDataEntityAsync<MonitorMatchSetting>(Arg.Any<GetServiceSettingCriteria>())
            .Returns(systemSetting);
        baseService.GetDataAsync<MonitorMatchInfo>(Arg.Any<GetMsgSettingMonitorMatchCriteria>())
            .Returns(messages);
        baseService.FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(
            Arg.Do<ImportMsgFromMonitorMatchCriteria>(x =>
            {
                x.ErrCode = expectedErrCode;
                x.ErrMsg = expectedErrMsg;
            }))
            .Returns((recipients, notificationSendMessages));
        chatbotService.GenerateNotifificationMessageForCarbonGroupAsync(
            notificationSendMessages, recipients)
            .Returns(notificationMessages);

        var service = GetService();

        // Act
        await service.SendMessageMonitoredMatchAsync();

        // Assert
        await commonService.Received(1).OnAfterImportDataAsync(
            Arg.Is<CommandResult>(x => x.Code == expectedErrCode && x.Message == expectedErrMsg),
            notificationMessages);
    }

    [Fact]
    public async Task SendMessageMonitoredMatchAsync_SerializesMessagesToJson()
    {
        // Arrange
        var systemSetting = fixture.Create<MonitorMatchSetting>();
        var messages = new List<MonitorMatchInfo>
        {
            new MonitorMatchInfo { LogId = 1, HomeTeam = "Team1", AwayTeam = "Team2" },
            new MonitorMatchInfo { LogId = 2, HomeTeam = "Team3", AwayTeam = "Team4" }
        };
        var recipients = fixture.CreateMany<NotificationRecipient>(1).ToList();
        var notificationSendMessages = fixture.CreateMany<NotificationSendMessage>(1).ToList();

        baseService.GetDataEntityAsync<MonitorMatchSetting>(Arg.Any<GetServiceSettingCriteria>())
            .Returns(systemSetting);
        baseService.GetDataAsync<MonitorMatchInfo>(Arg.Any<GetMsgSettingMonitorMatchCriteria>())
            .Returns(messages);
        baseService.FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(
            Arg.Any<ImportMsgFromMonitorMatchCriteria>())
            .Returns((recipients, notificationSendMessages));
        chatbotService.GenerateNotifificationMessageForCarbonGroupAsync(
            Arg.Any<IEnumerable<NotificationSendMessage>>(), Arg.Any<IEnumerable<NotificationRecipient>>())
            .Returns(new List<NotificationMessage>());

        var service = GetService();

        // Act
        await service.SendMessageMonitoredMatchAsync();

        // Assert
        await baseService.Received(1).FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(
            Arg.Is<ImportMsgFromMonitorMatchCriteria>(x =>
                x.Message.Contains("\"LogId\":1") &&
                x.Message.Contains("\"HomeTeam\":\"Team1\"") &&
                x.Message.Contains("\"LogId\":2") &&
                x.Message.Contains("\"HomeTeam\":\"Team3\"")));
    }

    private MonitorMatchService GetService()
    {
        return new MonitorMatchService(baseService, chatbotService, commonService, logService);
    }
}