namespace Fanex.NCB.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NCB.Core.Enum;
    using Fanex.NCB.Criteria.SettlementInternalCSD;
    using Fanex.NCB.Criteria.SettlementLicensee;
    using Fanex.NCB.Models.NotifictaionSendMessage;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Extensions;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Models.Settlement;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Newtonsoft.Json;

    public class SettlementInternalLicenseeService : ISettlementInternalLicenseeService
    {
        private readonly IBaseService baseService;
        private readonly IPushSignalRService pushSignalService;
        private readonly IChatbotService chatbotService;
        private readonly ILogService logService;
        private readonly ICommonService commonService;
        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";

        public SettlementInternalLicenseeService(
            IBaseService baseService,
            IPushSignalRService pushSignalService,
            IChatbotService chatbotService,
            ILogService logService,
            ICommonService commonService
        )
        {
            this.baseService = baseService;
            this.pushSignalService = pushSignalService;
            this.chatbotService = chatbotService;
            this.logService = logService;
            this.commonService = commonService;
        }

        public async Task<ServiceSetting> GetServcieSetting(string inServiceName)
        {
            var criteria = new GetServiceSettingCriteria
            {
                InServiceName = inServiceName
            };
            return await baseService.GetDataEntityAsync<ServiceSetting>(criteria);
        }

        protected internal virtual async Task OnAfterImportDataAsync(
            CommandResult commandResult,
            IEnumerable<NotificationMessage> notifications
)
        {
            if (!commandResult.IsSuccessful)
            {
                await chatbotService.NotifyToChatGroupAsync(commandResult.Message);
            }
            else
            {
                if (notifications.Any())
                {
                    await logService.WriteMessageSentryErrorAsync($"Before Send {JsonConvert.SerializeObject(notifications)}");

                    await Task.WhenAll(
                        SendMessageByChannelAsync((byte)ConversationChannel.Skype, notifications),
                        SendMessageByChannelAsync((byte)ConversationChannel.Telegram, notifications)
                    );

                    await logService.WriteMessageSentryErrorAsync($"After Send {JsonConvert.SerializeObject(notifications)}");

                    var notificationIds = string.Join(",", notifications.Select(item => item.NotificationId));
                    await pushSignalService.PushNewNotification(notificationIds);
                }
            }
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

        #region Settlement Internal CSD

        public async Task SettlementInternalCSDAsync()
        {
            var systemSetting = await GetServcieSetting("Settlement - Internal CSD");

            if (systemSetting != null)
            {
                var criteria = new GetMessageSettleInternalCSDCriteria
                {
                    LastMsgTargetId = systemSetting.LastMsgTargetId,
                    BatchSize = systemSetting.BatchSize
                };

                var (messageInfos, messageDetails) = await baseService.FetchMultipleAsync<MessageSettlement, MessageSettlementInfo>(criteria);
                await GetMessageAndImportToChatbotAsync(messageInfos, messageDetails);
            }
        }

        protected internal virtual async Task GetMessageAndImportToChatbotAsync(
            IEnumerable<MessageSettlement> messageInfos,
            IEnumerable<MessageSettlementInfo> messageDetails
        )
        {
            var (messageHeaders, msgs) = await GetEventInfoAsync(messageInfos, messageDetails);
            FormatTelegram(messageHeaders);
            await ImportSettlementInternalCSDToChatbotDBAsync(messageHeaders, msgs);
        }

        private static void FormatTelegram(IEnumerable<MessageSettlementHeader> messageHeaders)
        {
            foreach (var item in messageHeaders)
            {
                item.SportNameEN = TelegramFormatExtensions.EscapeSpecialSyntax(item.SportNameEN);
            }
        }

        protected internal virtual async Task<(IEnumerable<MessageSettlementHeader>, IEnumerable<MessageTarget>)> GetEventInfoAsync(
            IEnumerable<MessageSettlement> messageInfo,
            IEnumerable<MessageSettlementInfo> messageDetail
        )
        {
            var criteria = new GetEventSettleInternalCSDCriteria
            {
                MessageInfo = XmlHelper<MessageSettlement>.Serialize(messageInfo.ToList()),
                MessageDetail = XmlHelper<MessageSettlementInfo>.Serialize(messageDetail.ToList())
            };

            return await baseService.FetchMultipleAsync<MessageSettlementHeader, MessageTarget>(criteria);
        }

        protected internal virtual async Task ImportSettlementInternalCSDToChatbotDBAsync(
            IEnumerable<MessageSettlementHeader> messageHeaders,
            IEnumerable<MessageTarget> msgs
        )
        {
            if (messageHeaders.Any())
            {
                var criteria = new ImportSettleInternalCSDCommand
                {
                    MessageHeader = JsonConvert.SerializeObject(messageHeaders, new JsonSerializerSettings { DateFormatString = DateTimeFormat }),
                    MessageDetail = JsonConvert.SerializeObject(msgs, new JsonSerializerSettings { DateFormatString = DateTimeFormat }),
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

        #endregion Settlement Internal CSD

        #region Settlement Licensee

        public async Task SettlementLicenseeAsync()
        {
            var systemSetting = await GetServcieSetting("Settlement - Licensee");

            if (systemSetting != null)
            {
                var criteria = new GetMessageSettlementLicenseeCriteria
                {
                    LastMsgTargetId = systemSetting.LastMsgTargetId,
                    BatchSize = systemSetting.BatchSize,
                    SiteAsJSON = await commonService.GetJsonDataFromDbACCAsync("Site"),
                    DepCustSAsJSON = await commonService.GetJsonDataFromDbACCAsync("Dep_CustSuper"),
                };

                var (messageInfos, messageDetails, messageTargets) = await baseService.FetchMultipleAsync<MessageSettlement, MessageSettlementDetail, MessageSettlementInfo>(criteria);
                await GetMessageLicenseeAndImportToChatbotAsync(messageInfos, messageDetails, messageTargets);
            }
        }

        protected internal virtual async Task GetMessageLicenseeAndImportToChatbotAsync(
            IEnumerable<MessageSettlement> messageInfos,
            IEnumerable<MessageSettlementDetail> messageDetails,
            IEnumerable<MessageSettlementInfo> messageTargets
        )
        {
            var messageHeaders = await GetLeagueAsync(messageInfos);
            await ImportSettlementLicenseeToChatbotDBAsync(messageHeaders, messageDetails, messageTargets);
        }

        protected internal virtual async Task<IEnumerable<MessageLeagueLicensee>> GetLeagueAsync(
            IEnumerable<MessageSettlement> messageInfo
        )
        {
            var criteria = new GetLeagueSettlementLicenseeCriteria
            {
                Message = XmlHelper<MessageSettlement>.Serialize(messageInfo.ToList())
            };

            return await baseService.GetDataAsync<MessageLeagueLicensee>(criteria);
        }

        protected internal virtual async Task ImportSettlementLicenseeToChatbotDBAsync(
            IEnumerable<MessageLeagueLicensee> messageHeaders,
            IEnumerable<MessageSettlementDetail> messageDetails,
            IEnumerable<MessageSettlementInfo> messageTargets
        )
        {
            if (messageHeaders.Any())
            {
                var criteria = new ImportSettlementLicenseeCommand
                {
                    MessageHeader = JsonConvert.SerializeObject(messageHeaders, new JsonSerializerSettings { DateFormatString = DateTimeFormat }),
                    MessageDetail = JsonConvert.SerializeObject(messageDetails, new JsonSerializerSettings { DateFormatString = DateTimeFormat }),
                    MessageTarget = JsonConvert.SerializeObject(messageTargets, new JsonSerializerSettings { DateFormatString = DateTimeFormat })
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

        #endregion Settlement Licensee
    }
}
