namespace Fanex.NCB.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NCB.Criteria.SiteMessage;
    using Fanex.NCB.Models.NotifictaionSendMessage;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Extensions;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Models.SiteMessage;
    using Fanex.NCB.Shared.Models.SpecialDeleteTicket;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;

    public class SiteMessageService : ISiteMessageService
    {
        private readonly IBaseService baseService;
        private readonly IPushSignalService pushSignalService;
        private readonly IChatbotService chatbotService;
        private readonly string Environment;
        private const int SiteMessageJobIdentify = 3;

        public SiteMessageService(IBaseService baseService,
            IChatbotService chatbotService,
            IPushSignalService pushSignalService,
            IConfiguration configuration)
        {
            this.baseService = baseService;
            this.pushSignalService = pushSignalService;
            this.chatbotService = chatbotService;
            Environment = configuration.GetSection("Environment")?.Value;
        }

        public async Task PullMessageFromSiteMessage()
        {
            SystemSetting systemSettings = await GetSiteMessageSettings();
            if (systemSettings != null)
            {
                var messages = await GetDataFromSiteMessage(systemSettings);
                var matchInfo = await GetMatchInfo(string.Join(",", messages.Item1.Select(m => m.MatchID).Distinct()));

                MappingData(messages.Item1, matchInfo);
                if (messages.Item1.Any() && messages.Item2.Any())
                {
                    var result = await ImportMessageToChatbotDB(messages.Item1, messages.Item2);

                    await OnAfterImportData(result.Item1, result.Item2, messages);
                }
            }
        }

        private async Task<ValueTuple<CommandResult, IEnumerable<NotificationMessage>>> ImportMessageToChatbotDB(IEnumerable<SiteMessageModel> messages, IEnumerable<SiteMessageDetail> messageDetails)
        {
            var criteria = new ImportDataSiteMessageAndGetGroupTestCriteria
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

        private async Task OnAfterImportData(CommandResult commandResult, IEnumerable<NotificationMessage> notifications,
            (IEnumerable<SiteMessageModel>, IEnumerable<SiteMessageDetail>) messages)
        {
            if (!commandResult.IsSuccessful)
            {
                await chatbotService.NotifyToChatGroup(commandResult.Message);
            }
            else
            {
                if (notifications.Any())
                {
                    await InsertMessageHistory(messages.Item2);
                    await chatbotService.SendMessageToMultitpleAsync(notifications);
                    await PushInsertedNotifications(string.Join(",", notifications.Select(item => item.NotificationId)));
                }
            }
        }

        private async Task InsertMessageHistory(IEnumerable<SiteMessageDetail> messageDetails)
        {
            var msgHistory = messageDetails.Select(x => new MessageHistory { MessageID = x.MessageID, TargetID = x.TargetID }).ToList();
            var command = new InsertMessageHistoryCommand
            {
                JobIdentify = SiteMessageJobIdentify,
                MessageIDLst = XmlHelper.Serialize(msgHistory),
                Environment = Environment
            };

            await baseService.ExecuteNonQueryAsync(command);
        }

        private static void MappingData(IEnumerable<SiteMessageModel> messages, IEnumerable<SiteMessageMatchInfoModel> matchInfos)
        {
            foreach (var message in messages)
            {
                var matchInfo = matchInfos.FirstOrDefault(i => i.MatchID == message.MatchID);
                if (matchInfo != null)
                {
                    message.SportType = matchInfo.SportType;
                    message.SportNameEN = matchInfo.SportNameEN;
                    message.SportNameCS = matchInfo.SportNameCS;
                    message.KickOffTime = matchInfo.KickOffTime;
                    message.IsTestMatch = matchInfo.IsTestMatch;
                    message.LeagueNameEN = matchInfo.LeagueNameEN;
                    message.LeagueNameCS = matchInfo.LeagueNameCS;
                    message.HomeNameEN = matchInfo.HomeNameEN;
                    message.HomeNameCS = matchInfo.HomeNameCS;
                    message.AwayNameEN = matchInfo.AwayNameEN;
                    message.AwayNameCS = matchInfo.AwayNameCS;
                    message.IsTest = matchInfo.IsTestMatch || message.IsTestMsg;
                }
            }
        }

        private async Task<IEnumerable<SiteMessageMatchInfoModel>> GetMatchInfo(string matchIds)
        {
            var criteria = new GetEventInfoCriteria
            {
                MatchIDLst = matchIds
            };

            var matchInfo = await baseService.GetDataAsync<SiteMessageMatchInfoModel>(criteria);

            return matchInfo;
        }

        private async Task<ValueTuple<IEnumerable<SiteMessageModel>, IEnumerable<SiteMessageDetail>>> GetDataFromSiteMessage(SystemSetting systemSettings)
        {
            var criteria = new GetMessageInfoFromAdminSite
            {
                MessageID = systemSettings.MaxMessageID,
                BatchSize = systemSettings.BatchSize,
                LastPostTime = systemSettings.LastPostTime,
                MsgContentsTypeLst = systemSettings.MsgContentsTypeLst,
                Environment = Environment
            };
            var result = await baseService.FetchMultipleAsync<SiteMessageModel, SiteMessageDetail>(criteria);

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

        private async Task<SystemSetting> GetSiteMessageSettings()
        {
            return await baseService.GetDataEntityAsync<SystemSetting>(new GetSiteMessageSettingCriteria());
        }

        private async Task PushInsertedNotifications(string notificationIds)
        {
            await pushSignalService.OnInsertNotification(notificationIds);
        }
    }
}