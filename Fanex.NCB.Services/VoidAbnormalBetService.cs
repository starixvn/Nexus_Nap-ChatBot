namespace Fanex.NCB.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NCB.Criteria.SpecialDeleteTicket;
    using Fanex.NCB.Models.NotifictaionSendMessage;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Models.SpecialDeleteTicket;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Newtonsoft.Json;
    using ONEbook.UM.Services;

    public class VoidAbnormalBetService : IVoidAbnormalBetService
    {
        private readonly IBaseService baseService;
        private readonly IChatbotService chatbotService;
        private readonly IMaintenanceService maintenanceService;

        public VoidAbnormalBetService(IBaseService baseService,
            IChatbotService chatbotService,
            IMaintenanceService maintenanceService)
        {
            this.baseService = baseService;
            this.chatbotService = chatbotService;
            this.maintenanceService = maintenanceService;
        }

        public async Task GetAbnormalBetAndInsertNotification()
        {
            var maintanceTime = await maintenanceService.GetUnderMaintenanceTimeAsync();
            if (!maintanceTime.IsUnderMaintenanceTime)
            {
                var systemSettings = await GetSystemSettings();

                if (systemSettings != null)
                {
                    var abnormalBetMsgs = await GetAbnormalBetMessage(systemSettings);

                    await ImportToChatbotDB(abnormalBetMsgs.Item1, abnormalBetMsgs.Item2);
                }
            }
        }

        private async Task ImportToChatbotDB(IEnumerable<Message> messages, IEnumerable<AbnormalBetMessage> details)
        {
            if (messages.Any())
            {
                var criteria = new ImportAbnormalBetMessageCriteria
                {
                    MessageHeader = JsonConvert.SerializeObject(messages),
                    MessageDetail = JsonConvert.SerializeObject(details)
                };

                var data = await baseService.FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(criteria);

                var notification = GenerateNotifificationMessage(data.Item2, data.Item1);
                var command = new CommandResult
                {
                    Code = criteria.ErrCode,
                    Message = criteria.ErrMsg
                };

                await OnAfterImportData(command, notification);
            }
        }

        private async Task OnAfterImportData(CommandResult commandResult, IEnumerable<NotificationMessage> notifications)
        {
            if (!commandResult.IsSuccessful)
            {
                await chatbotService.NotifyToChatGroup(commandResult.Message);
            }
            else
            {
                if (notifications.Any())
                {
                    await chatbotService.SendMessageToMultitpleAsync(notifications);
                }
            }
        }

        private IEnumerable<NotificationMessage> GenerateNotifificationMessage(IEnumerable<NotificationSendMessage> notifications, IEnumerable<NotificationRecipient> recipients)
        {
            var messages = new List<NotificationMessage>();
            if (notifications != null && notifications.Any())
            {
                foreach (var noti in notifications)
                {
                    var notification = new NotificationMessage
                    {
                        NotifictaionId = noti.NotificationId,
                        Message = noti.Content,
                        Recipients = recipients.Select(r =>
                            new ConversationInfo
                            {
                                BotConversationId = r.BotConversationId,
                                BotName = r.BotName,
                                ChannelId = r.ChannelId,
                                ConversationId = r.ChatGroupConversationId,
                                ServiceUrl = r.ServiceUrl
                            }
                        )
                    };
                    messages.Add(notification);
                }
            }

            return messages;
        }

        private async Task<ValueTuple<IEnumerable<Message>, IEnumerable<AbnormalBetMessage>>>
            GetAbnormalBetMessage(SystemSetting systemSettings)
        {
            var criteria = new GetAbnormalBetMessageCriteria
            {
                MaxMessageId = systemSettings.MaxMessageID,
                BatchSize = systemSettings.BatchSize
            };

            return await baseService.FetchMultipleAsync<Message, AbnormalBetMessage>(criteria);
        }

        private async Task<SystemSetting> GetSystemSettings()
        {
            var criteria = new GetSystemSettingCriteria();

            return await baseService.GetDataEntityAsync<SystemSetting>(criteria);
        }
    }
}