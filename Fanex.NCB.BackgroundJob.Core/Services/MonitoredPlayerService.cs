namespace Fanex.NCB.AutoJobs.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NCB.AutoJobs.Core.Criteria.MonitoredPlayer;
    using Fanex.NCB.AutoJobs.Core.Enum;
    using Fanex.NCB.AutoJobs.Core.Models.Common;
    using Fanex.NCB.AutoJobs.Core.Models.Core;
    using Fanex.NCB.AutoJobs.Core.Models.MonitoredPlayer;
    using Fanex.NCB.AutoJobs.Core.Models.NotifictaionSendMessage;
    using Fanex.NCB.AutoJobs.Core.Services.Interfaces;
    using Newtonsoft.Json;

    public class MonitoredPlayerService : IMonitoredPlayerService
    {
        private readonly IBaseService baseService;
        private readonly IChatbotService chatbotService;
        private readonly IPushSignalRService pushSignalService;
        private readonly ILogService logService;

        public MonitoredPlayerService(
            IBaseService baseService,
            IChatbotService chatbotService,
            IPushSignalRService pushSignalService,
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
                var (messages, messageSel) = await GetMonitoredPlayerAsync(messageSetting);

                if (messages.Any())
                {
                    var (command, notifications) = await ImportMessageFromMonitoredPlayerAsync(messages, messageSel.ServerName, messageSel.LastDataRow, messageSel.MaxLogId);
                    await OnAfterImportDataAsync(command, notifications);
                }
            }
        }

        public async Task<MessageSetting> GetMessageSettingAsync()
        {
            var criteria = new GetMsgSettingMonitoredPlayerCriteria();
            return await baseService.GetDataEntityAsync<MessageSetting>(criteria);
        }

        public async Task<(IEnumerable<MonitoredPlayerInfo>, MessageSetting)> GetMonitoredPlayerAsync(MessageSetting messageSetting)
        {
            var criteria = new GetMonitoredPlayerInfoCriteria
            {
                BatchSize = messageSetting.BatchSize,
                LastLogId = messageSetting.LastLogId,
                LastDataRow = messageSetting.LastDataRow,
                ServerName = messageSetting.ServerName
            };
            var messageInfo = await baseService.GetDataAsync<MonitoredPlayerInfo>(criteria);
            var messageSel = new MessageSetting
            {
                LastDataRow = criteria.LastDataRow,
                ServerName = criteria.ServerName,
                MaxLogId = criteria.MaxLogId
            };
            return (messageInfo, messageSel);
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
                    await pushSignalService.PushNewNotification(notificationIds);
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