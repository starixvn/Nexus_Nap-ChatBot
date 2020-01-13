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

    public class SpecialDeleteTicketService : ISpecialDeleteTicketService
    {
        private readonly IBaseService baseService;
        private readonly IChatbotService chatbotService;
        private readonly ISendNotificationService sendNotificationService;

        public SpecialDeleteTicketService(IBaseService baseService,
            IChatbotService chatbotService,
            ISendNotificationService sendNotificationService)
        {
            this.baseService = baseService;
            this.chatbotService = chatbotService;
            this.sendNotificationService = sendNotificationService;
        }

        public async Task PullTicketAndCreateNotification()
        {
            var systemSettings = await GetSystemSettings();

            if (systemSettings != null)
            {
                var messages = await GetMessageFromSpecialDelete(systemSettings);
                if (messages.Item1.Any() && messages.Item2.Any())
                {
                    var result = await ImportMessageToChatbotDB(messages.Item1, messages.Item2);

                    await OnAfterImportData(result.Item1, result.Item2);
                }
            }

            ExecuteJobScan();
        }

        private async Task OnAfterImportData(CommandResult commandResult, IEnumerable<NotificationMessage> notifications)
        {
            if (!commandResult.IsSuccessful)
            {
                await chatbotService.NotifyToGroup(commandResult.Message);
            }
            else
            {
                if (notifications.Any())
                {
                    await chatbotService.SendMessageToMultitpleAsync(notifications);
                }
            }
        }

        private void ExecuteJobScan()
        {
            Task.Run(() => sendNotificationService.ScanAndSendNotification()).ConfigureAwait(false);
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

        private async Task<ValueTuple<IEnumerable<Message>, IEnumerable<MessageDetail>>> GetMessageFromSpecialDelete(SystemSetting systemSettings)
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

        private async Task<SystemSetting> GetSystemSettings()
        {
            var criteria = new GetSystemSettingCriteria();

            return await baseService.GetDataEntityAsync<SystemSetting>(criteria);
        }
    }
}