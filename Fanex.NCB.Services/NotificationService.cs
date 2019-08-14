namespace Fanex.NCB.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NCB.Criteria.Notification;
    using Fanex.NCB.Models.Common;
    using Fanex.NCB.Models.Notification;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Services.Interfaces;

    public class NotificationService : INotificationService
    {
        private readonly IBaseService baseService;
        private readonly ISendNotificationService sendNotificationService;

        public NotificationService(IBaseService baseService, ISendNotificationService sendNotificationService)
        {
            this.baseService = baseService;
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
                Content = model.Content,
                ConfigSendTime = model.SendDate,
                SendImmediately = model.SendImmediately,
                CreatorId = model.CreatorId
            };

            var result = await baseService.ExecuteNonQueryAsync(command).ConfigureAwait(false);

            if (model.SendImmediately)
            {
                SendNotificationImediately(command.NotificationId);
            }

            return result;
        }

        public async Task<IEnumerable<Recipient>> GetRecipientsAsync(int notificationTypeId, int? productId)
        {
            return await baseService.GetDataAsync<Recipient>(new GetRecipientsCriteria
            {
                NotificationTypeId = notificationTypeId,
                ProductId = productId,
            }).ConfigureAwait(false);
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
                PageIndex = model.PageIndex,
                PageSize = model.PageSize
            }).ConfigureAwait(false);

            return (result.Item1.FirstOrDefault(), result.Item2);
        }

        public async Task<CommandResult> CancelAsync(int notificationId, int userId)
        {
            var command = new CancelNotificationCommand
            {
                NotificationId = notificationId,
                UserId = userId
            };

            return await baseService.ExecuteNonQueryAsync(command).ConfigureAwait(false);
        }

        public async Task<NotificationErrorReport> GetNotificationErrorAsync(int notificationId)
        {
            var result = await baseService.FetchMultipleAsync<NotificationErrorReport, string, NotificationErrorDetail>(new GetNotificationErrorDetailCriteria
            {
                NotificationId = notificationId
            }).ConfigureAwait(false);

            var report = result.Item1.FirstOrDefault();

            if (report != null)
            {
                report.SuccessRecipients = result.Item2;
                report.ErrorDetails = result.Item3;
            }

            return report;
        }

        private void SendNotificationImediately(int notificationId)
        {
            Task.Run(
                () => sendNotificationService.SendNotificationImmediately(notificationId)
                ).ConfigureAwait(false);
        }
    }
}