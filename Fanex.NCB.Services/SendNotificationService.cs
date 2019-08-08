namespace Fanex.NCB.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Fanex.NCB.Core.Constants;
    using Fanex.NCB.Core.Enum;
    using Fanex.NCB.Criteria.SendNotification;
    using Fanex.NCB.Models.NotifictaionSendMessage;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Newtonsoft.Json.Linq;
    using RestSharp;

    public class SendNotificationService : ISendNotificationService
    {
        private readonly IChatbotService chatbotService;
        private readonly IBaseService baseService;

        private const int RetryTimes = 5; //Time to Retry send message if error

        public SendNotificationService(IChatbotService chatbotService, IBaseService baseService)
        {
            this.chatbotService = chatbotService;
            this.baseService = baseService;
        }

        public async Task SendMessageToRecipients(int creatorId)
        {
            var notifications = await GetNotificationIdsToSend().ConfigureAwait(false);
            foreach (var notificationId in notifications)
            {
                var result = await LockForSendingStatus(notificationId, creatorId).ConfigureAwait(false);
                if (result.IsSuccessful)
                {
                    var tuple = await GetNotificationRecipients(notificationId).ConfigureAwait(false);
                    await HandleListRecipients(tuple.Item1, tuple.Item2).ConfigureAwait(false);
                }
            }
        }

        public async Task<CommandResult> LockForSendingStatus(int notificationId, int creatorId)
        {
            var command = new UpdateNotificationSendingStatusCommand
            {
                NotificationId = notificationId,
                Creator = creatorId <= 0 ? Constants.SystemId : creatorId
            };

            return await baseService.ExecuteNonQueryAsync(command).ConfigureAwait(false);
        }

        private async Task<IEnumerable<int>> GetNotificationIdsToSend()
        {
            var criteria = new GetNotificationIdsToSendCriteria();
            return await baseService.GetDataAsync<int>(criteria).ConfigureAwait(false);
        }

        private async Task<(NotificationSendMessage, IEnumerable<NotificationRecipient>)> GetNotificationRecipients(int notificationId)
        {
            var criteria = new GetRecipientsToSendMessageCriteria
            {
                NotifictaionId = notificationId
            };

            var tuple = await baseService.FetchMultipleAsync<NotificationSendMessage, NotificationRecipient>(criteria).ConfigureAwait(false);
            return new ValueTuple<NotificationSendMessage, IEnumerable<NotificationRecipient>>(tuple.Item1.FirstOrDefault(), tuple.Item2);
        }

        private async Task HandleListRecipients(NotificationSendMessage notification, IEnumerable<NotificationRecipient> recipients)
        {
            // Update error if not mapped, bot removed or changed group
            await UpdateRecipientUnableSend(notification.NotificationId, recipients.Where(r => r.RecipientStatus != RecipientStatuses.ReadyToSend)).ConfigureAwait(false);

            //Handle send to recipients
            string message = string.Format("{0}\n{1}", notification.Title, notification.Content);
            var proactiveMessages = MappingProactiveMessage(recipients.Where(r => r.RecipientStatus == RecipientStatuses.ReadyToSend), message);
            await SendMessageToRecipient(proactiveMessages).ConfigureAwait(false);
        }

        private IEnumerable<ProactiveMessage> MappingProactiveMessage(IEnumerable<NotificationRecipient> recipients, string message)
        {
            var proactiveMessages = recipients.Select(r => new ProactiveMessage
            {
                BotConversationId = r.BotConversationId,
                ConversationId = r.ChatGroupConversation,
                BotName = r.BotName,
                ChannelId = r.ChannelId,
                ServiceUrl = r.ServiceUrl,
                Message = message
            });

            return proactiveMessages;
        }

        private async Task SendMessageToRecipient(IEnumerable<ProactiveMessage> proactiveMessages)
        {
            var response = await chatbotService.SendMessageToMultitpleAsync(proactiveMessages).ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                await OnAfterSendMessage(response, proactiveMessages).ConfigureAwait(false);
            }
            else
            {
                await SendMessageToRecipient(proactiveMessages).ConfigureAwait(false);
            }
        }

        private async Task OnAfterSendMessage(IRestResponse response, IEnumerable<ProactiveMessage> proactiveMessages)
        {
            var errorMessages = ConvertJsonResponse(response.Content);
            var successedMsg = proactiveMessages.Where(m => errorMessages.Any(e => e.ErrorCode == 0 && e.ConversationId == m.ConversationId));
            var failedMsg = proactiveMessages.Where(m => errorMessages.Any(e => e.ErrorCode != 0 && e.ConversationId == m.ConversationId));
            await UpdateSuccessSendNotificationRecipient(successedMsg).ConfigureAwait(false);
        }

        private Task UpdateSuccessSendNotificationRecipient(IEnumerable<ProactiveMessage> successedMsg)
        {
            throw new NotImplementedException();
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

        private async Task UpdateRecipientUnableSend(int notifiactionId, IEnumerable<NotificationRecipient> recipients)
        {
            if (recipients.Any())
            {
                foreach (var recipient in recipients)
                {
                    var command = new UpdateRecipientSendNotifiactionStatusCommand
                    {
                        NotificationId = notifiactionId,
                        RecipientConversationId = recipient.ChatGroupConversation,
                        SendNotifiactionStatus = (int)recipient.RecipientStatus
                    };

                    await baseService.ExecuteNonQueryAsync(command).ConfigureAwait(false);
                }
            }
        }
    }
}