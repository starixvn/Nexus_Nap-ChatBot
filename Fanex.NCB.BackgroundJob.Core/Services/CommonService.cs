namespace Fanex.NCB.AutoJobs.Core.Services;

using System.Threading.Tasks;
using Criteria.Common;
using Criteria.SpecialDeleteTicket;
using Enum;
using Models.Common;
using Fanex.NCB.AutoJobs.Core.Models.Core;
using Models.NotifictaionSendMessage;
using Interfaces;
using RemotingRef;

public class CommonService : ICommonService
{
    private readonly IBaseService baseService;
    private readonly IRemotingRefService remotingRefService;
    private readonly IChatbotService chatbotService;
    private readonly ILogService logService;
    private readonly IPushSignalRService pushSignalRService;

    public CommonService(IBaseService baseService,
        IRemotingRefService remotingRefService,
        IChatbotService chatbotService,
        ILogService logService,
        IPushSignalRService pushSignalRService)
    {
        this.baseService = baseService;
        this.remotingRefService = remotingRefService;
        this.chatbotService = chatbotService;
        this.logService = logService;
        this.pushSignalRService = pushSignalRService;
    }

    public async Task DeleteOldLog(string currentDate)
    {
        var command = new DeleteOldConversationMessageLogCommand
        {
            CurrentDate = currentDate
        };

        await baseService.ExecuteNonQueryAsync(command);
    }

    public string GetCategogyName(string categoryId, string languageCode)
    {
        var categoryName =
            remotingRefService.GetResourceDataByRefIdAndResourceId("Category", categoryId, languageCode, string.Empty);
        return categoryName;
    }

    public string GetCustomerClassName(string customerClass, string languageCode)
    {
        var customerClassName =
            remotingRefService.GetResourceDataByRefIdAndResourceId("CustomerClass", customerClass, languageCode,
                string.Empty);
        return customerClassName;
    }

    public async Task<string> GetJsonDataFromDbACCAsync(string tableName)
    {
        var jsonData =
            await baseService.GetDataEntityAsync<string>(new GetJsonDataOnDbACCCriteria { TableName = tableName });
        return jsonData;
    }

    public async Task<ServiceSetting> GetServiceSetting(string inServiceName)
    {
        var criteria = new GetServiceSettingCriteria
        {
            InServiceName = inServiceName
        };
        return await baseService.GetDataEntityAsync<ServiceSetting>(criteria);
    }

    public async Task OnAfterImportDataAsync(CommandResult commandResult,
        IEnumerable<NotificationMessage> notificationMessages)
    {
        if (!commandResult.IsSuccessful)
        {
            await chatbotService.NotifyToChatGroupAsync(commandResult.Message);
        }
        else
        {
            if (notificationMessages.Any())
            {
                await Task.WhenAll(
                    SendMessageByChannelAsync((byte)ConversationChannel.Skype, notificationMessages),
                    SendMessageByChannelAsync((byte)ConversationChannel.Telegram, notificationMessages)
                );

                var notificationIds = string.Join(",", notificationMessages.Select(item => item.NotificationId));
                await pushSignalRService.PushNewNotification(notificationIds);
            }
        }
    }

    public async Task SendMessageByChannelAsync(byte channelId, IEnumerable<NotificationMessage> notifications)
    {
        try
        {
            var selectedNotifications = notifications.Where(x => x.ChannelId == channelId).ToList();
            await chatbotService.SendMultiMessageAsync(channelId, selectedNotifications);
        }
        catch (Exception ex)
        {
            logService.WriteLogFileError(ex.Message);
        }
    }
}