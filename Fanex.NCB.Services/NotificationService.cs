namespace Fanex.NCB.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NCB.Core.Extensions;
    using Fanex.NCB.Criteria.Notification;
    using Fanex.NCB.Models.Common;
    using Fanex.NCB.Models.Notification;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Models.OIC;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Newtonsoft.Json;

    public class NotificationService : INotificationService
    {
        private readonly IBaseService baseService;
        private readonly ISignalRService signalRService;
        private readonly ISendNotificationService sendNotificationService;

        public NotificationService(IBaseService baseService,
            ISignalRService signalRService,
            ISendNotificationService sendNotificationService)
        {
            this.baseService = baseService;
            this.signalRService = signalRService;
            this.sendNotificationService = sendNotificationService;
        }

        public async Task<CommandResult> CreateAsync(CreateNotification model)
        {
            var command = new CreateNewNotificationCommand
            {
                NotificationTypeId = model.NotificationTypeId,
                ProductId = model.ProductId,
                RecipientIDLst = model.RecipientIds,
                Title = model.Title,
                ContentLst = model.Content,
                ConfigSendTime = model.SendImmediately ? DateTime.UtcNow : model.SendDate.ConvertToUTC(),
                SendImmediately = model.SendImmediately,
                CreatorId = model.CreatorId,
                Platforms = model.Platforms
            };

            var result = await baseService.ExecuteNonQueryAsync(command);
            if (result.IsSuccessful)
            {
                var notifications = JsonConvert.DeserializeObject<List<NotificationMessage>>(command.NotificationIDLst);
                foreach (var notification in notifications)
                {
                    if (model.SendImmediately)
                    {
                        BackgroundJobAfterCreateNotification(notification.NotificationId, true);
                    }
                    else
                    {
                        await PushSignalRInsert(notification.NotificationId);
                    }
                }
            }

            return result;
        }

        private async Task PushSignalRInsert(int notifiactionId)
        {
            if (notifiactionId > 0)
            {
                await signalRService.InsertNewNotification(notifiactionId.ToString());
            }
        }

        private async Task PushSignalRUpdate(int notifiactionId)
        {
            if (notifiactionId > 0)
            {
                await signalRService.UpdateNotificationStatus(notifiactionId.ToString());
            }
        }

        public async Task<IEnumerable<Recipient>> GetRecipientsAsync(int notificationTypeId, string platforms, int? productId = null, string channelIds = null)
        {
            return await baseService.GetDataAsync<Recipient>(new GetRecipientsCriteria
            {
                NotificationTypeId = notificationTypeId,
                Platforms = platforms,
                ProductId = productId,
                ChannelIdLst = channelIds
            });
        }

        public async Task<(long, IEnumerable<Notification>)> GetNotificationsAsync(GetNotifications model)
        {
            var result = await baseService.FetchMultipleAsync<long, Notification>(new GetNotificationsCriteria
            {
                NotificationTypeIds = model.NotificationTypeIds,
                ConfigSendTimeFrom = model.FromDate,
                ConfigSendTimeTo = model.ToDate,
                CreatorId = model.CreatorId,
                NotificationStatusIds = model.NotificationStatusIds,
                RecipientIds = model.RecipientIds,
                ChannelIds = model.ChannelIds,
                MessageId = model.MessageId,
                PageIndex = model.PageIndex,
                PageSize = model.PageSize
            });

            foreach (var notification in result.Item2)
            {
                notification.ConfigSendTime = notification.ConfigSendTime.ConvertFromUTC();
            }

            return (result.Item1.FirstOrDefault(), result.Item2);
        }

        public async Task<CommandResult> CancelAsync(int notificationId, int userId)
        {
            var command = new CancelNotificationCommand
            {
                NotificationId = notificationId,
                UserId = userId
            };

            var result = await baseService.ExecuteNonQueryAsync(command);

            if (result.IsSuccessful)
            {
                await PushSignalRUpdate(notificationId);
            }
            return result;
        }

        public async Task<NotificationErrorReport> GetNotificationErrorAsync(int notificationId)
        {
            var result = await baseService.FetchMultipleAsync<NotificationErrorReport, string, NotificationErrorDetail>(
                new GetNotificationErrorDetailCriteria
                {
                    NotificationId = notificationId
                });

            var report = result.Item1.FirstOrDefault();

            if (report != null)
            {
                report.SuccessRecipients = result.Item2;
                report.ErrorDetails = result.Item3;
                report.ConfigSendTime = report.ConfigSendTime.ConvertFromUTC();
            }

            return report;
        }

        public async Task<CommandResult> ResolveAsync(int notificationId, string resolvedRemark, int userId)
        {
            var command = new ResolveNotificationCommand
            {
                NotificationId = notificationId,
                ResolvedRemark = resolvedRemark,
                UserId = userId
            };

            var result = await baseService.ExecuteNonQueryAsync(command);

            if (result.IsSuccessful)
            {
                await PushSignalRUpdate(notificationId);
            }
            return result;
        }

        public async Task<EditNotificationInfo> GetEditNotificationInfoAsync(int notificationId)
        {
            var result = await baseService.FetchMultipleAsync<EditNotificationInfo, Recipient, Platform>(
                new GetEditNotificationInfoCriteria
                {
                    NotificationId = notificationId
                });

            var notification = result.Item1.FirstOrDefault();
            if (notification != null)
            {
                notification.Recipients = result.Item2;
                notification.Platforms = result.Item3;
                notification.ConfigSendTime = notification.ConfigSendTime.ConvertFromUTC();
            }

            return notification;
        }

        public async Task<CommandResult> EditAsync(EditNotification model)
        {
            var command = new EditNotificationCommand
            {
                NotificationId = model.NotificationId,
                RecipientIDLst = model.RecipientIds,
                Title = model.Title,
                Content = model.Content,
                ConfigSendTime = model.SendImmediately ? DateTime.UtcNow : model.SendDate.ConvertToUTC(),
                SendImmediately = model.SendImmediately,
                UserId = model.UserId,
                Platforms = model.Platforms
            };

            var result = await baseService.ExecuteNonQueryAsync(command);
            if (result.IsSuccessful && model.SendImmediately)
            {
                BackgroundJobAfterCreateNotification(command.NotificationId, false);
            }

            if (result.IsSuccessful)
            {
                await PushSignalRUpdate(command.NotificationId);
            }

            return result;
        }

        public async Task<IEnumerable<Recipient>> GetEditNotificationRecipientsAsync(int notificationId, int notificationTypeId, int? productId, string platforms)
        {
            return await baseService.GetDataAsync<Recipient>(new GetEditNotificationRecipientsCriteria
            {
                NotificationId = notificationId,
                NotificationTypeId = notificationTypeId,
                ProductId = productId,
                Platforms = platforms
            });
        }

        public async Task<IEnumerable<Template>> GetTemplatesAsync(int notificationTypeId)
        {
            return await baseService.GetDataAsync<Template>(new GetTemplatesCriteria
            {
                NotificationTypeId = notificationTypeId
            });
        }

        private void BackgroundJobAfterCreateNotification(int notificationId, bool isCreateNotification)
        {
            Task.Run(() => sendNotificationService.SendNotificationImmediatelyAsync(notificationId)
                .ContinueWith(result => isCreateNotification ? signalRService.InsertNewNotification(notificationId.ToString()) :
                        signalRService.UpdateNotificationStatus(notificationId.ToString())));
        }

        public async Task<ResendNotification> GetResendNotificationInfoAsync(int notificationId, bool isResendAll)
        {
            var criteria = new GetResendNotificationInfoCriteria
            {
                NotificationId = notificationId,
                IsSendAll = isResendAll
            };

            var result = await baseService.FetchMultipleAsync<ResendNotification, Recipient, Platform>(criteria);
            var notification = result.Item1.FirstOrDefault();
            if (notification != null)
            {
                notification.Recipients = result.Item2;
                notification.Platforms = result.Item3;
            }

            return notification;
        }

        public async Task<IEnumerable<Notification>> GetUpdatedNotificationAsync(string notificationIds)
        {
            var criteria = new GetUpdatedNotificationCriteria
            {
                NotificationIds = notificationIds,
            };
            var notifications = await baseService.GetDataAsync<Notification>(criteria);
            foreach (var notification in notifications)
            {
                notification.ConfigSendTime = notification.ConfigSendTime.ConvertFromUTC();
            }

            return notifications;
        }

        public async Task<IEnumerable<NotificationPaging>> GetErrNotificationsAsync()
        {
            var criteria = new GetAllErrNotificationsCriteria();
            var notifications = await baseService.GetDataAsync<NotificationPaging>(criteria);
            foreach (var notification in notifications)
            {
                notification.ConfigSendTime = notification.ConfigSendTime.ConvertFromUTC();
            }

            return notifications;
        }
    }
}