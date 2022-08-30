namespace Fanex.NCB.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NCB.Core.Enum;
    using Fanex.NCB.Criteria.MonitoredPlayer;
    using Fanex.NCB.Models.NotifictaionSendMessage;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Models.MonitoredPlayer;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Newtonsoft.Json;

    public class MonitoredPlayerService : IMonitoredPlayerService
    {
        private readonly IBaseService baseService;
        private readonly IChatbotService chatbotService;
        private readonly IPushSignalService pushSignalService;
        private readonly ILogService logService;

        public MonitoredPlayerService(
            IBaseService baseService,
            IChatbotService chatbotService,
            IPushSignalService pushSignalService,
            ILogService logService
        )
        {
            this.baseService = baseService;
            this.chatbotService = chatbotService;
            this.pushSignalService = pushSignalService;
            this.logService = logService;
        }

        public async Task SendMessageMonitoredPlayerAsync()
        {
            var messageSetting = await GetMessageSettingAsync();
            if (messageSetting != null)
            {
                var messages = await GetMonitoredPlayerAsync(messageSetting);

                if (messages.Any())
                {
                    var (command, notifications) = await ImportMessageFromMonitoredPlayerAsync(messages, messageSetting.ServiceName, messageSetting.LastDataRow, messageSetting.LastLogId);
                    await OnAfterImportDataAsync(command, notifications);
                }
            }
        }

        public async Task<MessageSetting> GetMessageSettingAsync()
        {
            var criteria = new GetMsgSettingMonitoredPlayerCriteria();
            return await baseService.GetDataEntityAsync<MessageSetting>(criteria);
        }

        public async Task<IEnumerable<MonitoredPlayerInfo>> GetMonitoredPlayerAsync(MessageSetting messageSetting)
        {
            var criteria = new GetMonitoredPlayerInfoCriteria
            {
                BatchSize = messageSetting.BatchSize,
                LastLogId = messageSetting.LastLogId,
                LastDataRow = messageSetting.LastDataRow,
                ServerName = messageSetting.ServiceName
            };
            return await baseService.GetDataAsync<MonitoredPlayerInfo>(criteria);
        }

        public async Task<(CommandResult, IEnumerable<NotificationMessage>)> ImportMessageFromMonitoredPlayerAsync(
            IEnumerable<MonitoredPlayerInfo> messages,
            string serverName,
            string lastDataRow,
            int pLastLogId
        )
        {
            var criteria = new ImportFromMonitoredPlayerCriteria
            {
                Message = JsonConvert.SerializeObject(messages),
                ServerName = serverName,
                LastDataRow = lastDataRow,
                PLastLogId = pLastLogId
            };

            var (recipients, notificationMessages) = await baseService.FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(criteria);
            var notifications = chatbotService.GenerateNotifificationMessageForCarbonGroupAsync(notificationMessages, recipients);
            var command = new CommandResult
            {
                Code = criteria.ErrCode,
                Message = criteria.ErrMsg
            };

            return (command, notifications);
        }

        protected internal virtual async Task OnAfterImportDataAsync(CommandResult commandResult, IEnumerable<NotificationMessage> notificationMessages)
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
                    await pushSignalService.OnInsertNotificationAsync(notificationIds);
                }
            }
        }

        protected internal virtual async Task SendMessageByChannelAsync(byte channelId, IEnumerable<NotificationMessage> notifications)
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
}
