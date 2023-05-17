namespace Fanex.NCB.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NCB.Core.Enum;
    using Fanex.NCB.Criteria.SiteMessage;
    using Fanex.NCB.Criteria.SpecialDeleteTicket;
    using Fanex.NCB.Models.NotifictaionSendMessage;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Extensions;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Models.SpecialDeleteTicket;
    using Fanex.NCB.Shared.Models.SpecialDeleteTicket.AbnormalBets;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;

    public class SpecialDeleteService : ISpecialDeleteService
    {
        private readonly IBaseService baseService;
        private readonly IPushSignalRService pushSignalService;
        private readonly IChatbotService chatbotService;
        private readonly ILogService logService;
        private readonly ISiteDbService siteDbService;
        private readonly ICommonService commonService;
        private readonly string Environment;
        private const int VoidTicketForLicenseeJobIdentity = 1;
        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";

        public SpecialDeleteService(IBaseService baseService,
            IChatbotService chatbotService,
            IPushSignalRService pushSignalService,
            ILogService logService,
            IConfiguration configuration,
            ISiteDbService siteDbService,
            ICommonService commonService)
        {
            this.baseService = baseService;
            this.pushSignalService = pushSignalService;
            this.chatbotService = chatbotService;
            this.logService = logService;
            this.siteDbService = siteDbService;
            this.commonService = commonService;
            Environment = configuration.GetSection("Environment")?.Value;
        }

        #region Pull voiding ticket send to Licensee

        public async Task PullVoidingMessageLicenseeAsync()
        {
            var systemSetting = await baseService.GetDataEntityAsync<SystemSetting>(new GetSystemSettingCriteria());
            if (systemSetting != null)
            {
                var (messages, details) = await GetMessageFromVoidingForLicenseeAsync(systemSetting);
                if (messages.Any() && details.Any())
                {
                    var (command, notifications) = await ImportMessageToChatbotDBAsync(messages, details);

                    if (command.IsSuccessful && notifications.Any())
                    {
                        await UpdateMessageHistoryVoidingForLicenseeAsync(details);
                    }

                    await OnAfterImportDataAsync(command, notifications);
                }
            }
        }

        protected internal virtual async Task UpdateMessageHistoryVoidingForLicenseeAsync(IEnumerable<MessageDetail> msgDetails)
        {
            var msgHistories = msgDetails.Select(x => new MessageHistory { MessageId = x.MessageID, TargetId = x.TargetID }).ToList();
            var xmlMsg = XmlHelper<MessageHistory>.Serialize(msgHistories);
            await InsertToMessageChatbotAsync(xmlMsg, VoidTicketForLicenseeJobIdentity);
        }

        protected internal virtual async Task OnAfterImportDataAsync(
            CommandResult commandResult,
            IEnumerable<NotificationMessage> notifications)
        {
            if (!commandResult.IsSuccessful)
            {
                await chatbotService.NotifyToChatGroupAsync(commandResult.Message);
            }
            else
            {
                if (notifications.Any())
                {
                    await Task.WhenAll(
                        SendMessageByChannelAsync((byte)ConversationChannel.Skype, notifications),
                        SendMessageByChannelAsync((byte)ConversationChannel.Telegram, notifications)
                    );

                    var notificationIds = string.Join(",", notifications.Select(item => item.NotificationId));
                    await pushSignalService.PushNewNotification(notificationIds);
                }
            }
        }

        protected internal virtual async Task<(CommandResult, IEnumerable<NotificationMessage>)> ImportMessageToChatbotDBAsync(
            IEnumerable<Message> messages,
            IEnumerable<MessageDetail> messageDetails)
        {
            var criteria = new ImportMessageFromSpecialDeleteCriteria
            {
                MessageHeader = JsonConvert.SerializeObject(messages),
                MessageDetail = JsonConvert.SerializeObject(messageDetails)
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

        protected internal virtual async Task<(IEnumerable<Message>, IEnumerable<MessageDetail>)> GetMessageFromVoidingForLicenseeAsync(SystemSetting systemSetting)
        {
            var criteria = new GetMessageFromSpecialDeleteCriteria
            {
                SiteAsJson = await commonService.GetJsonDataFromDbACCAsync("Site"),
                DepCustSAsJson = await commonService.GetJsonDataFromDbACCAsync("Dep_CustSuper"),
                MessageID = systemSetting.MaxMessageID,
                BatchSize = systemSetting.BatchSize,
                MsgContentsTypeLst = systemSetting.MsgContentsTypeLst,
                LastPostTime = systemSetting.LastPostTime,
                Environment = Environment
            };
            var (messages, details) = await baseService.FetchMultipleAsync<Message, MessageDetail>(criteria);
            foreach (Message msg in messages)
            {
                msg.Content_en = !string.IsNullOrWhiteSpace(msg.Content_en) ? msg.Content_en.Trim() : string.Empty;
                msg.Content_cs = !string.IsNullOrWhiteSpace(msg.Content_cs) ? msg.Content_cs.Trim() : string.Empty;
            }

            return (messages, details);
        }

        #endregion Pull voiding ticket send to Licensee

        #region Pull voiding ticket and send to internal department

        public async Task PullVoidingMessageInternalDepartmentAsync()
        {
            var systemSetting = await baseService.GetDataEntityAsync<SystemSetting>(new GetAbnormalBetSystemSettingCriteria());
            if (systemSetting != null)
            {
                var criteria = new GetVoidingHIstoryCollectionIdCriteria
                {
                    LastVoidingHistoryCollectionId = systemSetting.LastVoidingHistoryCollectionId,
                    BatchSize = systemSetting.BatchSize
                };
                var (tranIds, usernames) = await baseService.FetchMultipleAsync<CollectionTransId, CollectionUsername>(criteria);

                await GetMessageAndImportToChatbotAsync(tranIds, usernames, systemSetting);
            }
        }

        protected internal virtual async Task GetMessageAndImportToChatbotAsync(
            IEnumerable<CollectionTransId> tranIds,
            IEnumerable<CollectionUsername> usernameList,
            SystemSetting setting)
        {
            var (tickets, usernames) = await siteDbService.GetTicketUsernamesAynnc(tranIds, usernameList);

            var (historyDetail, msg) = await GetVoidingHistoryDetailAsync(tickets, usernames);
            var msgContents = await GetAbnormalBetsMessageContentAsync(msg);
            MappingData(historyDetail, msgContents);
            await ImportAbnormalBetNotificationToChatbotDBAsync(historyDetail, msg);
        }

        private static void MappingData(IEnumerable<AbnormalMessageDetail> historyDetail, IEnumerable<AbnormalMessageContent> msgContents)
        {
            foreach (var msg in historyDetail)
            {
                var message = msgContents.FirstOrDefault(m => m.VoidingHistoryCollectionId == msg.VoidingHistoryCollectionId);

                msg.Content_en = message?.Content_en?.Trim().Replace("@@", string.Empty);
                msg.Content_cs = message?.Content_cs?.Trim().Replace("@@", string.Empty);
                msg.LeagueName = TelegramFormatExtensions.EscapeSpecialSyntax(msg.LeagueName);
                msg.HomeName = TelegramFormatExtensions.EscapeSpecialSyntax(msg.HomeName);
                msg.AwayName = TelegramFormatExtensions.EscapeSpecialSyntax(msg.AwayName);
            }
        }

        internal virtual async Task<IEnumerable<AbnormalMessageContent>> GetAbnormalBetsMessageContentAsync(IEnumerable<AbnormalMessage> listMsgs)
        {
            var criteria = new GetAbnormalBetsMessageCriteria
            {
                MessageIDLst = XmlHelper<AbnormalMessage>.Serialize(listMsgs.ToList()),
            };

            return await baseService.GetDataAsync<AbnormalMessageContent>(criteria);
        }

        protected internal virtual async Task<(IEnumerable<AbnormalMessageDetail>, IEnumerable<AbnormalMessage>)> GetVoidingHistoryDetailAsync(
            IEnumerable<AbnormalTicketInfo> tickets,
            IEnumerable<AbnormalUsernames> usernames)
        {
            var criteria = new GetAbnormalBetInfoCriteria
            {
                TicketInfo = XmlHelper<AbnormalTicketInfo>.Serialize(tickets.ToList()),
                UsernameLst = XmlHelper<AbnormalUsernames>.Serialize(usernames.ToList())
            };

            return await baseService.FetchMultipleAsync<AbnormalMessageDetail, AbnormalMessage>(criteria);
        }

        protected internal virtual async Task ImportAbnormalBetNotificationToChatbotDBAsync(
            IEnumerable<AbnormalMessageDetail> historyDetails,
            IEnumerable<AbnormalMessage> msgs)
        {
            if (historyDetails.Any())
            {
                var criteria = new ImportAbnormalBetsMessageCriteria
                {
                    MessageHeader = JsonConvert.SerializeObject(historyDetails, new JsonSerializerSettings { DateFormatString = DateTimeFormat }),
                    MessageDetail = JsonConvert.SerializeObject(msgs, new JsonSerializerSettings { DateFormatString = DateTimeFormat })
                };
                var (recipients, notifications) = await baseService.FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(criteria);
                var notificationMessages = chatbotService.GenerateNotifificationMessageForCarbonGroupAsync(notifications, recipients);
                var command = new CommandResult
                {
                    Code = criteria.ErrCode,
                    Message = criteria.ErrMsg
                };

                await OnAfterImportDataAsync(command, notificationMessages);
            }
        }

        protected internal virtual async Task InsertToMessageChatbotAsync(string messageHistory, int jobIdentity)
        {
            var command = new InsertMessageHistoryCommand
            {
                JobIdentify = jobIdentity,
                MessageIDLst = messageHistory,
                Environment = Environment
            };

            await baseService.ExecuteNonQueryAsync(command);
        }

        protected internal virtual async Task SendMessageByChannelAsync(byte channelId, IEnumerable<NotificationMessage> notifications)
        {
            try
            {
                var filterNotifications = notifications.Where(x => x.ChannelId == channelId).ToList();
                await chatbotService.SendMultiMessageAsync(channelId, filterNotifications);
            }
            catch (Exception ex)
            {
                logService.WriteLogFileError(ex.Message);
            }
        }

        #endregion Pull voiding ticket and send to internal department
    }
}