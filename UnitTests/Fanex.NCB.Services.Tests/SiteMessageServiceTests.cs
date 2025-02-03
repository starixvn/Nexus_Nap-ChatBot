namespace Fanex.NCB.Services.Tests;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Models.NotifictaionSendMessage;
using Interfaces;
using TestableClasses;
using Shared.Extensions;
using Fanex.NCB.Shared.Models;
using Fanex.NCB.Shared.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NSubstitute;
using Xunit;

//public class SiteMessageServiceTests
//{
//    private readonly IFixture fixture = new Fixture();
//    private IBaseService baseService;
//    private IChatbotService chatbotService;
//    private IPushSignalRService pushSignalService;
//    private IConfiguration configuration;
//    private ICommonService commonService;

//    #region PullMessageFromSiteMessageAsync

//    [Fact]
//    public async Task PullMessageFromSiteMessageAsync_SettingIsNull_DoNothing()
//    {
//        // Arrange
//        SystemSetting setting = null;
//        var service = GetService();
//        baseService.GetDataEntityAsync<SystemSetting>(Arg.Any<GetSiteMessageSettingCriteria>()).Returns(setting);

//        // Act
//        await service.PullMessageFromSiteMessageAsync();

//        // Assert
//        service.DidNotReceive().Protected("GetDataFromSiteMessageAsync", Arg.Any<SystemSetting>());
//    }

//    [Fact]
//    public async Task PullMessageFromSiteMessageAsync_NoDataToImport_DoNothing()
//    {
//        // Arrange
//        var setting = fixture.Create<SystemSetting>();
//        var messages = new List<SiteMessageModel>();
//        var messageDetails = new List<SiteMessageDetail>();
//        var tuple = (messages, messageDetails);
//        var service = GetService();
//        baseService.GetDataEntityAsync<SystemSetting>(Arg.Any<GetSiteMessageSettingCriteria>()).Returns(setting);
//        service.Protected("GetDataFromSiteMessageAsync", setting)
//            .Returns(Task.FromResult<(IEnumerable<SiteMessageModel>, IEnumerable<SiteMessageDetail>)>(tuple));

//        // Act
//        await service.PullMessageFromSiteMessageAsync();

//        // Assert
//        service.DidNotReceive().Protected("ImportMessageToChatbotDBAsync", messages, messageDetails);
//    }

//    [Fact]
//    public async Task PullMessageFromSiteMessageAsync_HasDataToImport_DoAsExpected()
//    {
//        // Arrange
//        var setting = fixture.Create<SystemSetting>();
//        var messages = new List<SiteMessageModel>
//        {
//            new SiteMessageModel { MessageID = 1, MatchID = 1 },
//            new SiteMessageModel { MessageID = 2, MatchID = 2 }
//        };
//        var messageDetails = fixture.Create<IEnumerable<SiteMessageDetail>>();
//        var matchInfos = new List<SiteMessageMatchInfoModel>
//        {
//            new SiteMessageMatchInfoModel { MatchID = 1 },
//            new SiteMessageMatchInfoModel { MatchID = 2 }
//        };
//        var command = fixture.Create<CommandResult>();
//        var notifications = fixture.Create<IEnumerable<NotificationMessage>>();
//        var service = GetService();
//        baseService.GetDataEntityAsync<SystemSetting>(Arg.Any<GetSiteMessageSettingCriteria>()).Returns(setting);
//        service.Protected("GetDataFromSiteMessageAsync", setting)
//            .Returns(Task.FromResult<(IEnumerable<SiteMessageModel>, IEnumerable<SiteMessageDetail>)>((messages, messageDetails)));
//        service.Protected("GetMatchInfoAsync", "1,2").Returns(Task.FromResult<IEnumerable<SiteMessageMatchInfoModel>>(matchInfos));
//        service.Protected("ImportMessageToChatbotDBAsync", messages, messageDetails)
//            .Returns(Task.FromResult<(CommandResult, IEnumerable<NotificationMessage>)>((command, notifications)));

//        // Act
//        await service.PullMessageFromSiteMessageAsync();

//        // Assert
//        service.Received().Protected("OnAfterImportDataAsync", command, notifications, messageDetails);
//    }

//    #endregion PullMessageFromSiteMessageAsync

//    [Fact]
//    public async Task ImportMessageToChatbotDBAsync_WhenCalled_ReturnAsExpectedResult()
//    {
//        // Arrange
//        var messages = fixture.Create<IEnumerable<SiteMessageModel>>();
//        var messageDetails = fixture.Create<IEnumerable<SiteMessageDetail>>();
//        var recipients = fixture.Create<IEnumerable<NotificationRecipient>>();
//        var notificationMessages = fixture.Create<IEnumerable<NotificationSendMessage>>();
//        var notifications = fixture.Create<IEnumerable<NotificationMessage>>();
//        var tuple = (recipients, notificationMessages);
//        var criteria = new ImportDataSiteMessageAndGetGroupTestCriteria();
//        var service = GetService();
//        baseService.FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(Arg.Do<ImportDataSiteMessageAndGetGroupTestCriteria>(x => criteria = x)).Returns(tuple);
//        chatbotService.GenerateNotifificationMessageForCarbonGroupAsync(notificationMessages, recipients).Returns(notifications);

//        // Act
//        var result = await service.Public_ImportMessageToChatbotDBAsync(messages, messageDetails);

//        // Assert
//        Assert.Equal(JsonConvert.SerializeObject(messages), criteria.MessageHeader);
//        Assert.Equal(JsonConvert.SerializeObject(messageDetails), criteria.MessageDetail);
//        Assert.Equal(criteria.ErrCode, result.Item1.Code);
//        Assert.Equal(criteria.ErrMsg, result.Item1.Message);

//        Assert.Equal(notifications, result.Item2);
//    }

//    #region OnAfterImportDataAsync

//    [Fact]
//    public async Task OnAfterImportDataAsync_IsSuccessful_DoAsExpected()
//    {
//        // Arrange
//        var commandResult = new CommandResult { Code = 0 };
//        var notifications = fixture.Create<IEnumerable<NotificationMessage>>();
//        var messageDetails = fixture.Create<IEnumerable<SiteMessageDetail>>();
//        var notificationIds = string.Join(",", notifications.Select(x => x.NotificationId));
//        var service = GetService();

//        // Act
//        await service.Public_OnAfterImportDataAsync(commandResult, notifications, messageDetails);

//        // Assert
//        await chatbotService.DidNotReceive().NotifyToChatGroupAsync(Arg.Any<string>());

//        await pushSignalService.Received().PushNewNotification(notificationIds);
//        await commonService.Received(1).SendMessageByChannelAsync((byte)1, notifications);
//        await commonService.Received(1).SendMessageByChannelAsync((byte)2, notifications);
//    }

//    [Fact]
//    public async Task OnAfterImportDataAsync_IsUnsuccessful_DoAsExpected()
//    {
//        // Arrange
//        var commandResult = new CommandResult { Code = 1, Message = "error" };
//        var notifications = fixture.Create<IEnumerable<NotificationMessage>>();
//        var messageDetails = fixture.Create<IEnumerable<SiteMessageDetail>>();
//        var notificationIds = string.Join(",", notifications.Select(x => x.NotificationId));
//        var service = GetService();

//        // Act
//        await service.Public_OnAfterImportDataAsync(commandResult, notifications, messageDetails);

//        // Assert
//        await chatbotService.Received().NotifyToChatGroupAsync("error");

//        await pushSignalService.DidNotReceive().PushNewNotification(notificationIds);
//        await commonService.DidNotReceive().SendMessageByChannelAsync((byte)1, notifications);
//        await commonService.DidNotReceive().SendMessageByChannelAsync((byte)2, notifications);
//    }

//    #endregion OnAfterImportDataAsync

//    [Fact]
//    public async Task InsertMessageHistoryAsync_WhenCalled_DoAsExpected()
//    {
//        // Arrange
//        var messageDetails = fixture.Create<IEnumerable<SiteMessageDetail>>();
//        var histories = messageDetails.Select(x => new MessageHistory { MessageId = x.MessageID, TargetId = x.TargetID }).ToList();
//        var xml = XmlHelper<MessageHistory>.Serialize(histories);
//        var command = new InsertMessageHistoryCommand();
//        var service = GetService();
//        baseService.ExecuteNonQueryAsync(Arg.Do<InsertMessageHistoryCommand>(x => command = x)).Returns(new CommandResult());

//        // Act
//        await service.Public_InsertMessageHistoryAsync(messageDetails);

//        // Assert
//        await baseService.Received().ExecuteNonQueryAsync(command);

//        Assert.Equal(3, command.JobIdentify);
//        Assert.Equal(xml, command.MessageIDLst);
//    }

//    [Fact]
//    public async Task GetMatchInfoAsync_WhenCalled_ReturnAsExpected()
//    {
//        // Arrange
//        var matchIds = fixture.Create<string>();
//        var matchInfos = fixture.Create<IEnumerable<SiteMessageMatchInfoModel>>();
//        var criteria = new GetEventInfoCriteria();
//        var service = GetService();
//        baseService.GetDataAsync<SiteMessageMatchInfoModel>(Arg.Do<GetEventInfoCriteria>(x => criteria = x)).Returns(matchInfos);

//        // Act
//        var result = await service.Public_GetMatchInfoAsync(matchIds);

//        // Assert
//        Assert.Equal(matchInfos, result);
//    }

//    [Fact]
//    public async Task GetDataFromSiteMessageAsync_WhenCalled_ReturnAsExpectedResult()
//    {
//        // Arrange
//        var messages = new List<SiteMessageModel>
//        {
//            new SiteMessageModel { Content_en = "  content en  ", Content_cs = "  content cs  " }
//        };
//        var details = fixture.Create<IEnumerable<SiteMessageDetail>>();
//        var tuple = (messages, details);
//        var setting = fixture.Create<SystemSetting>();
//        var criteria = new GetMessageInfoFromAdminSiteCriteria();
//        var service = GetService();
//        baseService.FetchMultipleAsync<SiteMessageModel, SiteMessageDetail>(Arg.Do<GetMessageInfoFromAdminSiteCriteria>(x => criteria = x)).Returns(tuple);

//        // Act
//        var result = await service.Public_GetDataFromSiteMessageAsync(setting);

//        // Assert
//        Assert.Equal(setting.MaxMessageID, criteria.MessageID);
//        Assert.Equal(setting.BatchSize, criteria.BatchSize);
//        Assert.Equal(setting.MsgContentsTypeLst, criteria.MsgContentsTypeLst);
//        Assert.Equal(setting.LastPostTime, criteria.LastPostTime);

//        Assert.Equal(tuple, result);

//        Assert.Collection(result.Item1,
//            item =>
//            {
//                Assert.Equal("content en", item.Content_en);
//                Assert.Equal("content cs", item.Content_cs);
//            });
//    }

//    #region Helpers

//    private SiteMessageServiceTestable GetService()
//    {
//        baseService = Substitute.For<IBaseService>();
//        chatbotService = Substitute.For<IChatbotService>();
//        configuration = Substitute.For<IConfiguration>();
//        pushSignalService = Substitute.For<IPushSignalRService>();
//        commonService = Substitute.For<ICommonService>();

//        return Substitute.For<SiteMessageServiceTestable>(
//            baseService,
//            chatbotService,
//            pushSignalService,
//            configuration,
//            commonService);
//    }

//    #endregion Helpers
//}