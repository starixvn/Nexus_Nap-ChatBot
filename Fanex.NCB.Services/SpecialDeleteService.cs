namespace Fanex.NCB.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NCB.Criteria.SiteMessage;
    using Fanex.NCB.Criteria.SpecialDeleteTicket;
    using Fanex.NCB.Models.NotifictaionSendMessage;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Extensions;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Models.SiteMessage;
    using Fanex.NCB.Shared.Models.SpecialDeleteTicket;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;

    public class SpecialDeleteService : ISpecialDeleteService
    {
        private readonly IBaseService baseService;
        private readonly IPushSignalService pushSignalService;
        private readonly IChatbotService chatbotService;
        private readonly string Environment;
        private const int AbnormalAccountType = 2;
        private const int VoidTicketForLicenseeJobIdentity = 1;
        private const int VoidAbnormalbetsJobIdentity = 2;
        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";

        public SpecialDeleteService(IBaseService baseService,
            IChatbotService chatbotService,
            IPushSignalService pushSignalService,
            IConfiguration configuration
            )
        {
            this.baseService = baseService;
            this.pushSignalService = pushSignalService;
            this.chatbotService = chatbotService;
            Environment = configuration.GetSection("Environment")?.Value;
        }

        #region Pull voiding ticket send to Licensee

        public async Task PullVoidingMessageLicensee()
        {
            var systemSettings = await GetVoidingForLicenseeSystemSettings();

            if (systemSettings != null)
            {
                var messages = await GetMessageFromVoidingForLicensee(systemSettings);
                if (messages.Item1.Any() && messages.Item2.Any())
                {
                    var result = await ImportMessageToChatbotDB(messages.Item1, messages.Item2);

                    await UpdateMessageHistoryVoidingForLicensee(result.Item1, messages.Item2);

                    await OnAfterImportData(result.Item1, result.Item2);
                }
            }
        }

        private async Task UpdateMessageHistoryVoidingForLicensee(CommandResult commandResult, IEnumerable<MessageDetail> msgDetails)
        {
            if (commandResult.IsSuccessful)
            {
                var msgHistories = msgDetails.Select(x => new MessageHistory { MessageID = x.MessageID, TargetID = x.TargetID }).ToList();
                await InsertToMessageChatbot(msgHistories, VoidTicketForLicenseeJobIdentity);
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
                    await PushInsertedNotifications(string.Join(",", notifications.Select(item => item.NotificationId)));
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

            var notification = chatbotService.GenerateNotifificationMessage(data.Item2, data.Item1);
            var command = new CommandResult
            {
                Code = criteria.ErrCode,
                Message = criteria.ErrMsg
            };

            return new ValueTuple<CommandResult, IEnumerable<NotificationMessage>>(command, notification);
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
                MsgContentsTypeLst = systemSettings.MsgContentsTypeLst,
                LastPostTime = systemSettings.LastPostTime,
                Environment = Environment
            };

            return await baseService.FetchMultipleAsync<Message, MessageDetail>(criteria);
        }

        private async Task<SystemSetting> GetVoidingForLicenseeSystemSettings()
        {
            var criteria = new GetSystemSettingCriteria();

            return await baseService.GetDataEntityAsync<SystemSetting>(criteria);
        }

        #endregion Pull voiding ticket send to Licensee

        #region Pull voiding ticket and send to internal department

        public async Task PullVoidingMessageInternalDepartment()
        {
            var systemSettings = await GetAbnormalBetSystemSettings();

            if (systemSettings != null)
            {
                var listMsgYesterdayToCurrent = await GetMessageListFromYesterdayToCurrent();

                if (listMsgYesterdayToCurrent.Any())
                {
                    await GetMessageAndImportToChatbot(listMsgYesterdayToCurrent, systemSettings);
                }
            }
        }

        private async Task GetMessageAndImportToChatbot(List<MessageHistory> listMsgYesterdayToCurrent, SystemSetting settings)
        {
            var messages = await GetAbnormalBetsMessages(settings, listMsgYesterdayToCurrent);

            if (messages.Any())
            {
                var listMsg = messages.Select(m => new MessageHistory { MessageID = m.MessageID, RequestID = m.RequestID }).ToList();

                var voidingHistories = await GetVoidingHistory(listMsg);
                if (voidingHistories.Item1.Any(m => m.Type == AbnormalAccountType))
                {
                    var abnormalMessges = voidingHistories.Item2
                        .Where(i2 => voidingHistories.Item1.Where(i1 => i1.Type == AbnormalAccountType)
                                                           .Any(m => m.MessageID == i2.MessageId));
                    var updatedUsernames = await GetAbnormalAccountUpdatedUsernames(abnormalMessges);
                    UpdateAbnormalAccountUpdatedUsernames(voidingHistories.Item1, updatedUsernames);
                }

                MappingAbnormalBetsData(messages, voidingHistories.Item1);

                int maxMsgId = messages.Max(m => m.MessageID);
                await ImportAbnormalBetNotificationToChatbotDB(voidingHistories.Item1, listMsg, maxMsgId);
            }
        }

        private async Task<IEnumerable<VoidingHistoryUsername>> GetAbnormalAccountUpdatedUsernames(IEnumerable<AbnormalMessageHistory> messages)
        {
            var criteria = new GetAbnormalAccountUsernameLstCriteria
            {
                MessageIDLst = XmlHelper<AbnormalMessageHistory>.Serialize(messages.ToList())
            };

            var result = await baseService.GetDataAsync<VoidingHistoryUsername>(criteria);
            return result;
        }

        private static void UpdateAbnormalAccountUpdatedUsernames(IEnumerable<AbnormalMessage> origMessages, IEnumerable<VoidingHistoryUsername> updatedUsernames)
        {
            if (updatedUsernames.Any())
            {
                foreach (var message in origMessages)
                {
                    message.UsernameLst = string.Join(",", updatedUsernames.Where(m => m.MessageID == message.MessageID)?.Select(x => x.UsernameLst));
                    message.CreditSiteLst = string.Join(",", updatedUsernames.Where(m => m.MessageID == message.MessageID)?.Select(x => x.CreditSiteLst));
                    message.DepositSiteLst = string.Join(",", updatedUsernames.Where(m => m.MessageID == message.MessageID)?.Select(x => x.DepositSiteLst));
                }
            }
        }

        private async Task<List<MessageHistory>> GetMessageListFromYesterdayToCurrent()
        {
            var result = await baseService.GetDataAsync<MessageHistory>(new GetListMessageAbnormalbetsFromYesterdayToCurrentCriteria());
            return result.ToList();
        }

        private static void MappingAbnormalBetsData(IEnumerable<Message> messages, IEnumerable<AbnormalMessage> voidingHistories)
        {
            if (voidingHistories.Any())
            {
                foreach (var msg in voidingHistories)
                {
                    msg.Content_en = messages.FirstOrDefault(m => m.MessageID == msg.MessageID)?.Content_en.Trim().Replace("@@", string.Empty);
                    msg.Content_cs = messages.FirstOrDefault(m => m.MessageID == msg.MessageID)?.Content_cs.Trim().Replace("@@", string.Empty);
                    msg.MsgContentsType = messages.FirstOrDefault(m => m.MessageID == msg.MessageID) != null ? messages.FirstOrDefault(m => m.MessageID == msg.MessageID).MsgContentsType : 0;
                }
            }
        }

        private async Task<SystemSetting> GetAbnormalBetSystemSettings()
        {
            var criteria = new GetAbnormalBetSystemSettingCriteria();

            return await baseService.GetDataEntityAsync<SystemSetting>(criteria);
        }

        private async Task<IEnumerable<Message>> GetAbnormalBetsMessages(SystemSetting setting, List<MessageHistory> listMsgs)
        {
            var criteria = new GetAbnormalBetsMessageCriteria
            {
                MessageIDLst = XmlHelper<MessageHistory>.Serialize(listMsgs),
                BatchSize = setting.BatchSize,
                MsgContentsTypeLst = setting.MsgContentsTypeLst,
                Environment = Environment
            };

            return await baseService.GetDataAsync<Message>(criteria);
        }

        private async Task<(IEnumerable<AbnormalMessage>, IEnumerable<AbnormalMessageHistory>)> GetVoidingHistory(List<MessageHistory> messages)
        {
            string messageIdLst = XmlHelper<MessageHistory>.Serialize(messages);

            if (!string.IsNullOrWhiteSpace(messageIdLst))
            {
                var criteria = new GetAbnormalBetInfoCriteria
                {
                    MessageIdLst = messageIdLst
                };

                return await baseService.FetchMultipleAsync<AbnormalMessage, AbnormalMessageHistory>(criteria);
            }

            return (new List<AbnormalMessage>(), new List<AbnormalMessageHistory>());
        }

        private async Task ImportAbnormalBetNotificationToChatbotDB(IEnumerable<AbnormalMessage> messages,
            IEnumerable<MessageHistory> msgHistories, int maxMessageId)
        {
            if (messages.Any())
            {
                var criteria = new ImportAbnormalBetsMessageCriteria
                {
                    MessageHeader = JsonConvert.SerializeObject(messages, new JsonSerializerSettings { DateFormatString = DateTimeFormat }),
                    MaxMessageId = maxMessageId
                };

                var data = await baseService.FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(criteria);

                var notification = chatbotService.GenerateNotifificationMessage(data.Item2, data.Item1);
                var command = new CommandResult
                {
                    Code = criteria.ErrCode,
                    Message = criteria.ErrMsg
                };

                if (command.IsSuccessful)
                {
                    await InsertToMessageChatbot(msgHistories, VoidAbnormalbetsJobIdentity);
                }

                await OnAfterImportData(command, notification);
            }
        }

        private async Task InsertToMessageChatbot(IEnumerable<MessageHistory> messages, int jobIdentity)
        {
            var command = new InsertMessageHistoryCommand
            {
                JobIdentify = jobIdentity,
                MessageIDLst = XmlHelper<MessageHistory>.Serialize(messages.ToList()),
                Environment = Environment
            };

            await baseService.ExecuteNonQueryAsync(command);
        }

        private async Task PushInsertedNotifications(string notificationIds)
        {
            await pushSignalService.OnInsertNotification(notificationIds);
        }

        #endregion Pull voiding ticket and send to internal department
    }
}