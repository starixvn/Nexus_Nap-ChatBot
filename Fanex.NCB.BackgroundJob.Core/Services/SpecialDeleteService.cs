namespace Fanex.NCB.AutoJobs.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Constants;
    using Criteria.SiteMessage;
    using Criteria.SpecialDeleteTicket;
    using Helpers;
    using Models.Common;
    using Fanex.NCB.AutoJobs.Core.Models.Core;
    using Models.NotifictaionSendMessage;
    using Models.SpecialDeleteTicket;
    using Models.SpecialDeleteTicket.AbnormalBets;
    using Interfaces;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;

    public class SpecialDeleteService : ISpecialDeleteService
    {
        private readonly IBaseService baseService;
        private readonly IChatbotService chatbotService;
        private readonly ICommonService commonService;
        private readonly IRefNoTicketService refNoTicketService;
        private readonly string Environment;
        private const int VoidTicketForLicenseeJobIdentity = 1;
        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";

#pragma warning disable S107 // Methods should not have too many parameters

        public SpecialDeleteService(IBaseService baseService,
            IChatbotService chatbotService,
            IConfiguration configuration,
            ICommonService commonService,
            IRefNoTicketService refNoTicketService)
#pragma warning restore S107 // Methods should not have too many parameters
        {
            this.baseService = baseService;
            this.chatbotService = chatbotService;
            this.commonService = commonService;
            this.refNoTicketService = refNoTicketService;
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
                    var (issueTypes, messageDetails) = await GetIssueTypeAsync(messages, details);

                    var (command, notifications) = await ImportMessageToChatbotDBAsync(issueTypes, messageDetails);

                    if (command.IsSuccessful && notifications.Any())
                    {
                        await UpdateMessageHistoryVoidingForLicenseeAsync(details);
                    }

                    await commonService.OnAfterImportDataAsync(command, notifications);
                }
            }
        }

        private async Task<ValueTuple<IEnumerable<SpecialDeleteIssueType>, IEnumerable<MessageDetail>>> GetIssueTypeAsync(IEnumerable<Message> messages, IEnumerable<MessageDetail> messageDetails)
        {
            var criteria = new GetMessageIssueTypeCriteria
            {
                Message = XmlHelper<Message>.Serialize(messages.ToList()),
                MessageDetail = XmlHelper<MessageDetail>.Serialize(messageDetails.ToList())
            };
            return await baseService.FetchMultipleAsync<SpecialDeleteIssueType, MessageDetail>(criteria);
        }

        protected internal virtual async Task UpdateMessageHistoryVoidingForLicenseeAsync(IEnumerable<MessageDetail> msgDetails)
        {
            var msgHistories = msgDetails.Select(x => new MessageHistory { MessageId = x.MessageId, TargetId = x.TargetId }).ToList();
            var xmlMsg = XmlHelper<MessageHistory>.Serialize(msgHistories);
            await InsertToMessageChatbotAsync(xmlMsg, VoidTicketForLicenseeJobIdentity);
        }

        protected internal virtual async Task<(CommandResult, IEnumerable<NotificationMessage>)> ImportMessageToChatbotDBAsync(
            IEnumerable<SpecialDeleteIssueType> messages,
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
            var systemSetting = await commonService.GetServiceSetting(Constants.ServiceName_SpecialDeleteInternalCSD);
            if (systemSetting != null)
            {
                var criteria = new GetVoidingHistoryCollectionIdCriteria
                {
                    LastVoidingHistoryCollectionId = systemSetting.LastVoidingHistoryCollectionId,
                    BatchSize = systemSetting.BatchSize
                };
                var messageIdLst = await baseService.GetDataAsync<AbnormalMessage>(criteria);
                await GetMessageAndImportToChatbotAsync(systemSetting, messageIdLst);
            }
        }

        protected internal virtual async Task<ValueTuple<IEnumerable<AbnormalMessageContent>, IEnumerable<MessageMsgDetail>, int>> GetAbnormalMessageDetail(
            IEnumerable<AbnormalMessage> messageIdLst,
            int batchSize,
            int lastMsgTargetId
        )
        {
            var criteria = new GetAbnormalBetMessageDetailCriteria
            {
                MessageIdLst = XmlHelper<AbnormalMessage>.Serialize(messageIdLst.ToList()),
                BatchSize = batchSize,
                LastMsgTargetId = lastMsgTargetId
            };
            var data = await baseService.FetchMultipleAsync<AbnormalMessageContent, MessageMsgDetail>(criteria);
            return new ValueTuple<IEnumerable<AbnormalMessageContent>, IEnumerable<MessageMsgDetail>, int>(data.Item1, data.Item2, criteria.MaxMsgtargetId);
        }

        protected internal virtual async Task<(IEnumerable<AbnormalMessageDetail>, IEnumerable<AbnormalMessage>, IEnumerable<AbnormalAccountInfo>, IEnumerable<DuplicateTicketDetail>)>
        GetVoidingHistoryInternalCSDInfoAsync(
            IEnumerable<AbnormalMessageContent> messageInfo,
            IEnumerable<MessageMsgDetail> messageDetail)
        {
            var criteria = new GetVoidingHistoryInternalCSDInfoCriteria
            {
                MessageInfo = XmlHelper<AbnormalMessageContent>.Serialize(messageInfo.ToList()),
                MessageDetail = XmlHelper<MessageMsgDetail>.Serialize(messageDetail.ToList()),
            };

            return await FetchVoidingHistoryInfo(criteria);
        }

        private async Task<(IEnumerable<AbnormalMessageDetail>, IEnumerable<AbnormalMessage>, IEnumerable<AbnormalAccountInfo>, IEnumerable<DuplicateTicketDetail>)>
        FetchVoidingHistoryInfo(ICriteria criteria)
            => await baseService.FetchMultipleAsync<AbnormalMessageDetail, AbnormalMessage, AbnormalAccountInfo, DuplicateTicketDetail>(criteria);

        protected internal virtual async Task GetMessageAndImportToChatbotAsync(ServiceSetting setting, IEnumerable<AbnormalMessage> messageIdLst)
        {
            var (messageInfo, messageDetail, maxMsgTargetId) = await GetAbnormalMessageDetail(messageIdLst, setting.BatchSize, setting.LastMsgTargetId);
            var (historyDetail, msg, abnormalAccountInfos, dupTicketDetails) = await GetVoidingHistoryInternalCSDInfoAsync(messageInfo, messageDetail);
            if (dupTicketDetails.Any())
            {
                await GetTransInfoAsync(dupTicketDetails);
            }
            FormatTelegramData(historyDetail);
            await ImportAbnormalBetNotificationToChatbotDBAsync(historyDetail, msg, dupTicketDetails, abnormalAccountInfos, maxMsgTargetId);
        }

        protected internal virtual async Task GetTransInfoAsync(IEnumerable<DuplicateTicketDetail> dupTicketDetails)
        {
            var distinctVoidedTransIds = dupTicketDetails.Select(i => i.VoidedTransId).Distinct();
            var tasks = new List<Task<(long, string)>>();
            foreach (var transid in distinctVoidedTransIds)
            {
                tasks.Add(Task.Run(() => refNoTicketService.GetTransInfoAsync(transid)));
            }

            var results = await Task.WhenAll(tasks);
            foreach (var (tranid, transinfo) in results)
            {
                var tickets = dupTicketDetails.Where(x => x.VoidedTransId == tranid);
                foreach (var item in tickets)
                {
                    item.TransInfo = transinfo;
                }
            }
        }

        protected internal static void FormatTelegramData(IEnumerable<AbnormalMessageDetail> historyDetail)
        {
            foreach (var msg in historyDetail)
            {
                msg.LeagueName = TelegramFormatExtensions.EscapeSpecialSyntax(msg.LeagueName);
                msg.HomeName = TelegramFormatExtensions.EscapeSpecialSyntax(msg.HomeName);
                msg.AwayName = TelegramFormatExtensions.EscapeSpecialSyntax(msg.AwayName);
            }
        }

        protected internal virtual async Task ImportAbnormalBetNotificationToChatbotDBAsync(
            IEnumerable<AbnormalMessageDetail> historyDetails,
            IEnumerable<AbnormalMessage> msgs,
            IEnumerable<DuplicateTicketDetail> duplicateTicketDetails,
            IEnumerable<AbnormalAccountInfo> abnormalAccountInfos,
            int maxMsgTargetId
        )
        {
            if (historyDetails.Any())
            {
                var criteria = new ImportAbnormalBetsMessageCriteria
                {
                    MessageHeader = JsonConvert.SerializeObject(historyDetails, new JsonSerializerSettings { DateFormatString = DateTimeFormat }),
                    MessageDetail = JsonConvert.SerializeObject(msgs, new JsonSerializerSettings { DateFormatString = DateTimeFormat }),
                    DuplicatedInfo = JsonConvert.SerializeObject(duplicateTicketDetails, new JsonSerializerSettings { DateFormatString = DateTimeFormat }),
                    AbnormalAccountInfo = JsonConvert.SerializeObject(abnormalAccountInfos, new JsonSerializerSettings { DateFormatString = DateTimeFormat }),
                    InMsgTargetId = maxMsgTargetId
                };
                var (recipients, notifications) = await baseService.FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(criteria);
                var notificationMessages = chatbotService.GenerateNotifificationMessageForCarbonGroupAsync(notifications, recipients);
                var command = new CommandResult
                {
                    Code = criteria.ErrCode,
                    Message = criteria.ErrMsg
                };

                await commonService.OnAfterImportDataAsync(command, notificationMessages);
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
        #endregion Pull voiding ticket and send to internal department
    }
}