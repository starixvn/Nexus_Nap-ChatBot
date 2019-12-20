namespace Fanex.NCB.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NCB.Models.NotifictaionSendMessage;
    using Fanex.NCB.Repository.Interfaces;
    using Fanex.NCB.Shared.Models;

    public class SpecialDeleteRepository : ISpecialDeleteRepository
    {
        public async Task<(CommandResult, IEnumerable<NotificationMessage>)> ImportMessageAndGetNotification(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria).ConfigureAwait(false))
                {
                    var command = await multi.ReadFirstAsync<CommandResult>();
                    var recipients = await multi.ReadAsync<NotificationRecipient>();
                    var notifications = await multi.ReadAsync<NotificationSendMessage>();
                    var messages = GenerateNotifificationMessage(notifications, recipients);
                    return new ValueTuple<CommandResult, IEnumerable<NotificationMessage>>(command, messages);
                }
            }
        }

        private IEnumerable<NotificationMessage> GenerateNotifificationMessage(IEnumerable<NotificationSendMessage> notifications, IEnumerable<NotificationRecipient> recipients)
        {
            var messages = new List<NotificationMessage>();
            if (notifications != null && notifications.Any())
            {
                foreach (var noti in notifications)
                {
                    var notification = new NotificationMessage
                    {
                        NotifictaionId = noti.NotificationId,
                        Message = noti.Content,
                        Recipients = recipients.Select(r =>
                            new ConversationInfo
                            {
                                BotConversationId = r.BotConversationId,
                                BotName = r.BotName,
                                ChannelId = r.ChannelId,
                                ConversationId = r.ChatGroupConversationId,
                                ServiceUrl = r.ServiceUrl
                            }
                        )
                    };
                    messages.Add(notification);
                }
            }

            return messages;
        }
    }
}