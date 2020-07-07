namespace Fanex.NCB.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    using Fanex.NCB.Core.Constants;
    using Fanex.NCB.Core.Extensions;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Criteria.SiteMessage;
    using Fanex.NCB.Criteria.SpecialDeleteTicket;
    using Fanex.NCB.Models.NotifictaionSendMessage;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Models.SiteMessage;
    using Fanex.NCB.Shared.Models.SpecialDeleteTicket;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

    public class SpecialDeleteService : ISpecialDeleteService
    {
        private readonly IBaseService baseService;
        private readonly IPushSignalService pushSignalService;
        private readonly IChatbotService chatbotService;
        private readonly string Environment;
        private const int VoidAbnormalbetsJobIdentify = 2;

        private readonly IOptionsSnapshot<List<HangfireJobSetting>> jobSettings;
        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";

        public SpecialDeleteService(IBaseService baseService,
            IChatbotService chatbotService,
            IPushSignalService pushSignalService,
            IConfiguration configuration,
            IOptionsSnapshot<List<HangfireJobSetting>> jobSettings)
        {
            this.baseService = baseService;
            this.pushSignalService = pushSignalService;
            this.chatbotService = chatbotService;
            Environment = configuration.GetSection("Environment")?.Value;
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
                    PushInsertedNotifications(string.Join(",", notifications.Select(item => item.NotificationId)));
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
                    var listMsgYesterdayToCurrent = await GetMessageListFromYesterdayToCurrent();
                    var messages = await GetAbnormalBetsMessages(systemSettings, string.Join(',', listMsgYesterdayToCurrent));
                    if (messages.Any())
                    {
                        var voidingHistories = await GetVoidingHistory(string.Join(",", messages.Select(m => m.MessageID)));

                        MappingAbnormalBetsData(messages, voidingHistories);

                        int maxMsgId = messages.Max(m => m.MessageID);
                        await ImportAbnormalBetNotificationToChatbotDB(voidingHistories, maxMsgId);
                    }
                }
            }
        }

        private async Task<IEnumerable<string>> GetMessageListFromYesterdayToCurrent()
        {
            return await baseService.GetDataAsync<string>(new GetListMessageAbnormalbetsFromYesterdayToCurrentCriteria());
        }

        private static void MappingAbnormalBetsData(IEnumerable<Message> messages, IEnumerable<AbnormalBetsMessage> voidingHistories)
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

        private async Task<IEnumerable<Message>> GetAbnormalBetsMessages(SystemSetting setting, string listMessages)
        {
            var criteria = new GetAbnormalBetsMessageCriteria
            {
                MessageIDLst = listMessages,
                BatchSize = setting.BatchSize,
                MsgContentsTypeLst = setting.MsgContentsTypeLst,
                Environment = Environment
            };

            return await baseService.GetDataAsync<Message>(criteria);
        }

        private async Task<IEnumerable<AbnormalBetsMessage>> GetVoidingHistory(string msgIds)
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

        private async Task ImportAbnormalBetNotificationToChatbotDB(IEnumerable<AbnormalBetsMessage> messages, int maxMessageId)
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
                    await InsertToMessageChatbot(messages);
                }

                await OnAfterImportData(command, notification);
            }
        }

        private async Task InsertToMessageChatbot(IEnumerable<AbnormalBetsMessage> messages)
        {
            var msgHistory = messages.Select(x => new MessageHistory { MessageID = x.MessageID, TargetID = 0 }).ToList();
            var serializer = new XmlSerializer(typeof(List<MessageHistory>),
                                   new XmlRootAttribute("root"));

            var stream = new StringWriter();
            serializer.Serialize(stream, msgHistory);

            var command = new InsertMessageHistoryCommand
            {
                JobIdentify = VoidAbnormalbetsJobIdentify,
                MessageIDLst = stream.ToString(),
                Environment = Environment
            };

            await baseService.ExecuteNonQueryAsync(command);
        }

        private void PushInsertedNotifications(string notificationIds)
        {
            Task.Run(() => pushSignalService.OnInsertNotification(notificationIds));
        }
    }
}