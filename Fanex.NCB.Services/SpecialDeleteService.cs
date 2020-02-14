namespace Fanex.NCB.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NCB.Core.Constants;
    using Fanex.NCB.Core.Extensions;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Criteria.SpecialDeleteTicket;
    using Fanex.NCB.Models.NotifictaionSendMessage;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Models.SpecialDeleteTicket;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using ONEbook.UM.Services;

    public class SpecialDeleteService : ISpecialDeleteService
    {
        private readonly IBaseService baseService;
        private readonly IChatbotService chatbotService;
        private readonly IMaintenanceService maintenanceService;
        private readonly IOptionsSnapshot<List<HangfireJobSetting>> jobSettings;

        public SpecialDeleteService(IBaseService baseService,
            IChatbotService chatbotService,
            IMaintenanceService maintenanceService,
            IOptionsSnapshot<List<HangfireJobSetting>> jobSettings)
        {
            this.baseService = baseService;
            this.chatbotService = chatbotService;
            this.maintenanceService = maintenanceService;
            this.jobSettings = jobSettings;
        }

        public async Task VoidingTicketToLicensee()
        {
            var maintanceTime = await maintenanceService.GetUnderMaintenanceTimeAsync();

            if (HangfireHelper.IsEnabledJob(jobSettings.Value, HangfireJobName.VoidingTicketForLicensee) &&
                   !maintanceTime.IsUnderMaintenanceTime)
            {
                var systemSettings = await GetVoidingForLicenseeSystemSettings();

                if (systemSettings != null)
                {
                    var messages = await GetMessageFromVoidingForLicensee(systemSettings);
                    if (messages.Item1.Any() && messages.Item2.Any())
                    {
                        var result = await ImportMessageToChatbotDB(messages.Item1, messages.Item2);

                        await OnAfterImportData(result.Item1, result.Item2);
                    }
                }
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

        private async Task<ValueTuple<CommandResult, IEnumerable<NotificationMessage>>> ImportMessageToChatbotDB(
            IEnumerable<Message> messages,
            IEnumerable<MessageDetail> messageDetails)
        {
            var criteria = new ImportMessageFromSpecialDeleteCriteria
            {
                MessageHeader = JsonConvert.SerializeObject(messages),
                MessageDetail = JsonConvert.SerializeObject(messageDetails)
            };

            var data = await baseService.FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(criteria);
            var notification = GenerateNotifificationMessage(data.Item2, data.Item1);
            var command = new CommandResult
            {
                Code = criteria.ErrCode,
                Message = criteria.ErrMsg
            };

            return new ValueTuple<CommandResult, IEnumerable<NotificationMessage>>(command, notification);
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

        private async Task<ValueTuple<IEnumerable<Message>, IEnumerable<MessageDetail>>> GetMessageFromVoidingForLicensee(SystemSetting systemSettings)
        {
            var criteria = new GetMessageFromSpecialDeleteCriteria
            {
                MessageID = systemSettings.MaxMessageID,
                BatchSize = systemSettings.BatchSize,
                MsgContentsTypeLst = systemSettings.MsgContentsTypeLst
            };

            var result = await baseService.FetchMultipleAsync<Message, MessageDetail>(criteria);

            if (result.Item1.Any())
            {
                foreach (var msg in result.Item1)
                {
                    var contentEn = !string.IsNullOrWhiteSpace(msg.Content_en) ? msg.Content_en.Trim() : string.Empty;
                    var contentCs = !string.IsNullOrWhiteSpace(msg.Content_cs) ? msg.Content_cs.Trim() : string.Empty;
                    msg.Content_en = contentEn;
                    msg.Content_cs = contentCs;
                }
            }

            return result;
        }

        private async Task<SystemSetting> GetVoidingForLicenseeSystemSettings()
        {
            var criteria = new GetSystemSettingCriteria();

            return await baseService.GetDataEntityAsync<SystemSetting>(criteria);
        }

        private async Task<SystemSetting> GetAbnormalBetSystemSettings()
        {
            var criteria = new GetAbnormalBetSystemSettingCriteria();

            return await baseService.GetDataEntityAsync<SystemSetting>(criteria);
        }

        public async Task VoidingAbnormalBetToInternalDepartment()
        {
            var maintanceTime = await maintenanceService.GetUnderMaintenanceTimeAsync();

            if (HangfireHelper.IsEnabledJob(jobSettings.Value, HangfireJobName.VoidingAbnormalBets) &&
                   !maintanceTime.IsUnderMaintenanceTime)
            {
                var systemSettings = await GetAbnormalBetSystemSettings();

                if (systemSettings != null)
                {
                    var abnormalBetMsgs = await GetAbnormalBetMessage(systemSettings);

                    await ImportAbnormalBetNotificationToChatbotDB(abnormalBetMsgs.Item1, abnormalBetMsgs.Item2);
                }
            }
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

        private async Task ImportAbnormalBetNotificationToChatbotDB(IEnumerable<Message> messages, IEnumerable<AbnormalBetMessage> details)
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
    }
}