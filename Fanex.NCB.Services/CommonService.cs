namespace Fanex.NCB.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NCB.Core.Enum;
    using Fanex.NCB.Criteria.Common;
    using Fanex.NCB.Criteria.SpecialDeleteTicket;
    using Fanex.NCB.Models.Common;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Models.IntegrationMethod;
    using Fanex.NCB.Shared.Models.OIC;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Fanex.RemotingRef;

    public class CommonService : ICommonService
    {
        private readonly IBaseService baseService;
        private readonly IRemotingRefService remotingRefService;
        private readonly IChatbotService chatbotService;
        private readonly IPushSignalRService pushSignalService;
        private readonly ILogService logService;

        public CommonService(
            IBaseService baseService,
            IRemotingRefService remotingRefService,
            IChatbotService chatbotService,
            IPushSignalRService pushSignalService,
            ILogService logService
        )
        {
            this.baseService = baseService;
            this.remotingRefService = remotingRefService;
            this.chatbotService = chatbotService;
            this.pushSignalService = pushSignalService;
            this.logService = logService;
        }

        public async Task<IEnumerable<NotificationType>> GetNotificationTypesAsync()
        {
            return await baseService.GetDataAsync<NotificationType>(new GetNotificationTypesCriteria());
        }

        public async Task<IEnumerable<Models.Common.Product>> GetProductsAsync()
        {
            return await baseService.GetDataAsync<Models.Common.Product>(new GetProductsCriteria());
        }

        public async Task<IEnumerable<NotificationStatus>> GetNotificationStatusesAsync()
        {
            return await baseService.GetDataAsync<NotificationStatus>(new GetNotificationStatusCriteria());
        }

        public async Task<IEnumerable<Recipient>> GetRecipientsAsync()
        {
            return await baseService.GetDataAsync<Recipient>(new GetRecipientsCriteria());
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await baseService.GetDataAsync<User>(new GetUsersCriteria());
        }

        public async Task<IEnumerable<ChannelInfo>> GetConversationChannelAsync()
        {
            return await baseService.GetDataAsync<ChannelInfo>(new GetConversationChannelCriteria());
        }

        public Task<IEnumerable<Platform>> GetPlatformAsync(int notificationTypeId)
        {
            var criteria = new GetPlatformCriteria
            {
                NotificationTypeId = notificationTypeId
            };
            return baseService.GetDataAsync<Platform>(criteria);
        }

        public string GetCategogyName(string categoryId, string languageCode)
        {
            var categoryName = remotingRefService.GetResourceDataByRefIdAndResourceId("Category", categoryId, languageCode, string.Empty);
            return categoryName;
        }

        public string GetCustomerClassName(string customerClass, string languageCode)
        {
            var customerClassName = remotingRefService.GetResourceDataByRefIdAndResourceId("CustomerClass", customerClass, languageCode, string.Empty);
            return customerClassName;
        }

        public async Task<string> GetJsonDataFromDbACCAsync(string tableName)
        {
            string jsonData = await baseService.GetDataEntityAsync<string>(new GetJsonDataOnDbACCCriteria { TableName = tableName });
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

        public async Task<IEnumerable<IntegrationMethodInfo>> GetIntegrationMethodAsync(int notificationTypeId)
        {
            var criteria = new GetIntegrationMethodCriteria
            {
                InNotificationTypeId = notificationTypeId,
                InIntegrationMethodId = null,
                InIntegrationMethodName = null
            };
            return await baseService.GetDataAsync<IntegrationMethodInfo>(criteria);
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

        public async Task OnAfterImportDataAsync(CommandResult commandResult, IEnumerable<NotificationMessage> notificationMessages)
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
                    await pushSignalService.PushNewNotification(notificationIds);
                }
            }
        }
    }
}