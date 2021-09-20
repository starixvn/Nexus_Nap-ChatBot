namespace Fanex.NCB.BotService.Core.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NCB.BotService.Core.Criteria;
    using Fanex.NCB.BotService.Core.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Services.Interfaces;

    public class ConversationService : IConversationService
    {
        private readonly IBaseService baseService;

        public ConversationService(IBaseService baseService)
        {
            this.baseService = baseService;
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
            var conversations = new List<NotificationConversation>();
            foreach (var notification in notifications)
            {
                conversations.AddRange(notification.Recipients.Select(r =>
                new NotificationConversation
                {
                    NotificationId = notification.NotificationId,
                    BotName = r.BotName,
                    BotConversationId = r.BotConversationId,
                    ChannelId = r.ChannelId,
                    ConversationId = r.ConversationId,
                    ServiceUrl = r.ServiceUrl,
                }));
            }

            var distinctConversations = DistinctConversation(conversations);
            var notificationResult = DistinctNotification(notifications);
            foreach (var notification in notificationResult)
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

        private static IEnumerable<NotificationConversation> DistinctConversation(IEnumerable<NotificationConversation> conversations)
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