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

    public class SpecialDeleteService : ISpecialDeleteService
    {
        private readonly IBaseService baseService;
        private readonly IPushSignalService pushSignalService;
        private readonly IChatbotService chatbotService;
        private readonly IOptionsSnapshot<List<HangfireJobSetting>> jobSettings;
        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";

        public SpecialDeleteService(IBaseService baseService,
               IChatbotService chatbotService,
            IPushSignalService pushSignalService,
            IChatbotService chatbotService,
            IOptionsSnapshot<List<HangfireJobSetting>> jobSettings)
        {
            this.baseService = baseService;
            this.pushSignalService = pushSignalService;
            this.chatbotService = chatbotService;
            this.jobSettings = jobSettings;
        }

        public async Task VoidingTicketToLicensee()
        {
            if (HangfireHelper.IsEnabledJob(jobSettings.Value, HangfireJobName.VoidingTicketForLicensee))
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

            if (!string.IsNullOrWhiteSpace(criteria.InsertedNotificationIds))
            {
                PushInsertedNotifications(criteria.InsertedNotificationIds);
            }
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
            var result = await GetMessageFromSpecialDelete(systemSettings);

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

        private async Task<ValueTuple<IEnumerable<Message>, IEnumerable<MessageDetail>>> GetMessageFromSpecialDelete(SystemSetting systemSettings)
        {
            var criteria = new GetMessageFromSpecialDeleteCriteria
            {
                MessageID = systemSettings.MaxMessageID,
                BatchSize = systemSettings.BatchSize,
                MsgContentsTypeLst = systemSettings.MsgContentsTypeLst
            };

            return await baseService.FetchMultipleAsync<Message, MessageDetail>(criteria);
        }

        private async Task<SystemSetting> GetVoidingForLicenseeSystemSettings()
        {
            var criteria = new GetSystemSettingCriteria();

            return await baseService.GetDataEntityAsync<SystemSetting>(criteria);
        }

        public async Task VoidingAbnormalBetsToInternalDepartment()
        {
            if (HangfireHelper.IsEnabledJob(jobSettings.Value, HangfireJobName.VoidingAbnormalBets))
            {
                var systemSettings = await GetAbnormalBetSystemSettings();

                if (systemSettings != null)
                {
                    var result = await GetAbnormalBetsMessages(systemSettings);
                    var abnormalBetMsgs = await GetAbnormalBetMessage(string.Join(",", result.Select(m => m.MessageID)));

                    foreach (var msg in abnormalBetMsgs)
                    {
                        msg.Content_en = result.FirstOrDefault(m => m.MessageID == msg.MessageID)?.Content_en.Trim().Replace("@@", string.Empty);
                        msg.Content_cs = result.FirstOrDefault(m => m.MessageID == msg.MessageID)?.Content_cs.Trim().Replace("@@", string.Empty);
                        msg.MsgContentsType = result.FirstOrDefault(m => m.MessageID == msg.MessageID) != null ? result.FirstOrDefault(m => m.MessageID == msg.MessageID).MsgContentsType : 0;
                    }

                    await ImportAbnormalBetNotificationToChatbotDB(abnormalBetMsgs);
                }
            }
        }

        private async Task<SystemSetting> GetAbnormalBetSystemSettings()
        {
            var criteria = new GetAbnormalBetSystemSettingCriteria();

            return await baseService.GetDataEntityAsync<SystemSetting>(criteria);
        }

        private async Task<IEnumerable<Message>> GetAbnormalBetsMessages(SystemSetting setting)
        {
            var criteria = new GetAbnormalBetsMessageCriteria
            {
                MessageID = setting.MaxMessageID,
                BatchSize = setting.BatchSize,
                MsgContentsTypeLst = setting.MsgContentsTypeLst
            };

            return await baseService.GetDataAsync<Message>(criteria);
        }

        private async Task<IEnumerable<AbnormalBetsMessage>> GetAbnormalBetMessage(string msgIds)
        {
            if (!string.IsNullOrWhiteSpace(msgIds))
            {
                var criteria = new GetAbnormalBetInfoCriteria
                {
                    MessageIdLst = msgIds
                };

                return await baseService.GetDataAsync<AbnormalBetsMessage>(criteria);
            }

            return new List<AbnormalBetsMessage>();
        }

        private async Task ImportAbnormalBetNotificationToChatbotDB(IEnumerable<AbnormalBetsMessage> messages)
        {
            if (messages.Any())
            {
                var criteria = new ImportAbnormalBetsMessageCriteria
                {
                    MessageHeader = JsonConvert.SerializeObject(messages,
                    new JsonSerializerSettings { DateFormatString = DateTimeFormat })
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
        
         private void PushInsertedNotifications(string notificationIds)
        {
            Task.Run(() => pushSignalService.OnInsertNotification(notificationIds));
	}
    }
}