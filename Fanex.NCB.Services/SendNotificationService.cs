namespace Fanex.NCB.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NCB.Core.Enum;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Criteria.SendNotification;
    using Fanex.NCB.Models.NotifictaionSendMessage;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json.Linq;

    public class SendNotificationService : ISendNotificationService
    {
        private readonly IChatbotService chatbotService;
        private readonly IBaseService baseService;
        private readonly IOptionsSnapshot<AppSettings> appSettings;

        public SendNotificationService(IChatbotService chatbotService,
            IBaseService baseService,
            IOptionsSnapshot<AppSettings> appSettings)
        {
            this.chatbotService = chatbotService;
            this.baseService = baseService;
            this.appSettings = appSettings;
        }

        public async Task ScanAndSendNotification(int creatorId)
        {
            var notifications = await GetNotificationIdsToSend(creatorId).ConfigureAwait(false);

            await UpdateFailedRecipientStatus(notifications.Item2.Where(r => !r.RecipientStatus.Equals(RecipientStatuses.ReadyToSend)));

            var proactiveMsgs = GenerateProactiveMessage(notifications.Item1, notifications.Item2.Where(r => r.RecipientStatus.Equals(RecipientStatuses.ReadyToSend)));

            await SendNotificationToRecipients(proactiveMsgs).ConfigureAwait(false);
        }

        private async Task SendNotificationToRecipients(IEnumerable<ReciveProactiveMessage> proactiveMsgs)
        {
            int totalBatch = (int)Math.Ceiling((double)proactiveMsgs.Count() / appSettings.Value.BatchSendNotification);
            for (int i = 0; i < totalBatch; i++)
            {
                var currentMsgs = proactiveMsgs.Skip(i * appSettings.Value.BatchSendNotification).Take(appSettings.Value.BatchSendNotification);
                var response = await chatbotService.SendMessageToMultitpleAsync(currentMsgs).ConfigureAwait(false);
            }
        }

        private IEnumerable<ReciveProactiveMessage> GenerateProactiveMessage(IEnumerable<NotificationSendMessage> notifications, IEnumerable<NotificationRecipient> recipients)
        {
            var proactiveMsgs = new List<ReciveProactiveMessage>();
            foreach (var noti in notifications)
            {
                var proactiveMsg = new ReciveProactiveMessage
                {
                    NotifictaionId = noti.NotificationId,
                    Message = string.Format("{0}\n{1}", noti.Title, noti.Content),
                    Recipients = recipients.Where(r => noti.NotificationId == noti.NotificationId)
                            .Select(r => new ProactiveMessage
                            {
                                BotConversationId = r.BotConversationId,
                                ServiceUrl = r.ServiceUrl,
                                ChannelId = r.ChannelId,
                                ConversationId = r.ChatGroupConversation,
                                BotName = r.BotName
                            })
                };
                proactiveMsgs.Add(proactiveMsg);
            }

            return proactiveMsgs;
        }

        private Task UpdateFailedRecipientStatus(IEnumerable<NotificationRecipient> item2)
        {
            throw new NotImplementedException();
        }

        private async Task<(IEnumerable<NotificationSendMessage>, IEnumerable<NotificationRecipient>)> GetNotificationIdsToSend(int creatorId)
        {
            var criteria = new GetNotificationsToSendCriteria { CreatorId = creatorId };
            return await baseService.FetchMultipleAsync<NotificationSendMessage, NotificationRecipient>(criteria).ConfigureAwait(false);
        }

        private IList<ProactiveMessageError> ConvertJsonResponse(string json)
        {
            var messages = new List<ProactiveMessageError>();
            JObject obj = JObject.Parse(json);
            var results = obj["results"];
            foreach (var result in results)
            {
                var proactiveMessage = new ProactiveMessageError
                {
                    ConversationId = (string)result["conversationId"],
                    ErrorCode = (int)result["errorCode"],
                    ErrorMessage = (string)result["errorMessage"]
                };
                messages.Add(proactiveMessage);
            }

            return messages;
        }

        public Task SendNotificationImediately(int notifiactionId, int creatorId)
        {
            throw new NotImplementedException();
        }
    }
}