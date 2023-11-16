namespace Fanex.NCB.BotService.Core.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NCB.BotService.Core.Criteria;
    using Fanex.NCB.BotService.Core.Models;
    using Fanex.NCB.BotService.Core.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Services.Interfaces;

    public class ConversationService : IConversationService
    {
        private readonly IBaseService baseService;
        private readonly CheckDuplicatedMessageLogSetting settings;

        public ConversationService(IBaseService baseService,
            CheckDuplicatedMessageLogSetting settings)
        {
            this.baseService = baseService;
            this.settings = settings;
        }

        public async Task<CommandResult> AddBotToGroup(AddBotToGroupCommand command)
        {
            return await baseService.ExecuteNonQueryAsync(command);
        }

        public async Task<CommandResult> RemoveBotOutGroup(RemoveBotOutGroupCommand command)
        {
            return await baseService.ExecuteNonQueryAsync(command);
        }

        public async Task<CommandResult> UpdateGroupTopic(UpdateGroupTopicCommand command)
        {
            return await baseService.ExecuteNonQueryAsync(command);
        }

        public async Task<CommandResult> MigratedToSupergroupAsync(MigratedToSupergroupCommand command)
        {
            return await baseService.ExecuteNonQueryAsync(command);
        }

        public async Task<CommandResult> AddMessageLog(AddMessageLogCommand command)
        {
            return await baseService.ExecuteNonQueryAsync(command);
        }

        public IList<NotificationMessage> GetDistinctNotificationConversation(IList<NotificationMessage> notifications)
        {
            var conversations = new List<ConversationInfo>();
            foreach (var notification in notifications)
            {
                conversations.AddRange(notification.Recipients.Select(r =>
                new ConversationInfo
                {
                    NotificationId = notification.NotificationId,
                    Message = notification.Message,
                    BotName = r.BotName,
                    BotConversationId = r.BotConversationId,
                    ChannelId = r.ChannelId,
                    ConversationId = r.ConversationId,
                    ServiceUrl = r.ServiceUrl,
                }));
            }

            var distinctContents = DistinctConversationIdAndContent(conversations);

            var distinctConversations = DistinctConversationIdAndNotificationId(distinctContents);
            var notificationResult = DistinctNotification(notifications);

            foreach (var notification in notifications)
            {
                notification.Recipients = distinctConversations.Where(conversation => conversation.NotificationId == notification.NotificationId);
            }

            return notificationResult.ToList();
        }

        private static IEnumerable<NotificationMessage> DistinctNotification(IEnumerable<NotificationMessage> notificationMessages)
        {
            List<NotificationMessage> notifications = notificationMessages.GroupBy(g => g.NotificationId).Select(n => n.FirstOrDefault()).ToList();
            return notifications;
        }

        private IEnumerable<ConversationInfo> DistinctConversationIdAndContent(IEnumerable<ConversationInfo> conversations)
        {
            var results = new List<ConversationInfo>();

            foreach (var conversation in conversations)
            {
                if (results.Exists(r => r.Message == conversation.Message && r.ConversationId == conversation.ConversationId && IsNotExceptionGroup(r.ConversationId)))
                {
                    conversation.IsDuplicated = true;
                }
                results.Add(conversation);
            }

            return results;
        }

        private bool IsNotExceptionGroup(string conversationId)
        {
            return !settings.ExceptionConversatiionIDs.ToList().Exists(x => x == conversationId);
        }

        private static IEnumerable<ConversationInfo> DistinctConversationIdAndNotificationId(IEnumerable<ConversationInfo> conversations)
        {
            var disticntConversations = conversations.GroupBy(group => new
            {
                group.NotificationId,
                group.ConversationId
            })
            .Select(conversation => conversation.FirstOrDefault()).ToList();

            return disticntConversations;
        }
    }
}