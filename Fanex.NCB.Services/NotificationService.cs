namespace Fanex.NCB.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using Core.Extensions;
using Criteria.Notification;
using Models.Common;
using Models.Notification;
using Interfaces;
using Fanex.NCB.Shared.Models;
using Fanex.NCB.Shared.Models.IntegrationMethod;
using Fanex.NCB.Shared.Models.OIC;
using Fanex.NCB.Shared.Services.Interfaces;
using Newtonsoft.Json;

public class NotificationService : INotificationService
{
    private readonly IBaseService baseService;
    private readonly ISendNotificationService sendNotificationService;

    public NotificationService(
        IBaseService baseService,
        ISendNotificationService sendNotificationService)
    {
        this.baseService = baseService;
        this.sendNotificationService = sendNotificationService;
    }

    public async Task<(CommandResult, string)> CreateAsync(CreateNotification model)
    {
        var command = new CreateNewNotificationCommand
        {
            InNotificationTypeId = model.NotificationTypeId,
            InProductId = model.ProductId,
            InRecipientIds = model.RecipientIds,
            InTitle = model.Title,
            InContentLst = model.Content,
            InConfigSendTime = model.SendImmediately ? DateTimeOffset.UtcNow.DateTime : model.SendDate.ConvertToUTC(),
            InSendImmediately = model.SendImmediately,
            InCreatorId = model.CreatorId,
            InPlatformIds = model.Platforms,
                InIntegrationMethodIds = model.IntegrationMethodIds,
                InFileName = model.FileName
        };
        var notificationIds = string.Empty;
        var result = await baseService.ExecuteNonQueryAsync(command);
        if (result.IsSuccessful && !string.IsNullOrEmpty(command.NotificationIDLst))
        {
            var notifications = JsonConvert.DeserializeObject<List<NotificationMessage>>(command.NotificationIDLst);
            foreach (var notification in notifications)
                if (model.SendImmediately)
                    BackgroundJobAfterCreateNotification(notification.NotificationId);

            notificationIds = string.Join(",", notifications.Select(x => x.NotificationId).ToArray());
        }

        return (result, notificationIds);
    }

    public async Task<(IEnumerable<Recipient>, IEnumerable<uint>)> GetRecipientsAsync
    (int notificationTypeId, string platforms, string recipientIds, int? productId = null,
        string integrationMethodIds = null, string channelIds = null)
    {
        return await baseService.FetchMultipleAsync<Recipient, uint>(new GetRecipientsCriteria
        {
            InNotificationTypeId = notificationTypeId,
            InPlatformIds = platforms,
            InProductId = productId,
            InIntegrationMethodIds = integrationMethodIds,
            InChannelIds = channelIds,
            InRecipientIds = recipientIds
        });
    }

    public async Task<(long, IEnumerable<Notification>)> GetNotificationsAsync(GetNotifications model)
    {
        var result = await baseService.FetchMultipleAsync<long, Notification>(new GetNotificationsCriteria
        {
            InNotificationTypeIds = model.NotificationTypeIds,
            InConfigSendTimeFrom = model.FromDate,
            InConfigSendTimeTo = model.ToDate,
            InCreatorId = model.CreatorId,
            InNotificationStatusId = model.NotificationStatusIds,
            InRecipientIds = model.RecipientIds,
            InChannelIds = model.ChannelIds,
            InProductIds = model.ProductIds,
            InMessageId = model.MessageId,
            PageIndex = model.PageIndex,
            PageSize = model.PageSize
        });

        foreach (var notification in result.Item2)
            notification.ConfigSendTime = notification.ConfigSendTime.ConvertFromUTC();

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

        return result;
    }

    public async Task<EditNotificationInfo> GetEditNotificationInfoAsync(int notificationId)
    {
        var criteria = new GetEditNotificationInfoCriteria
        {
            InNotificationId = notificationId
        };

        var result = await FetchEditNotificationInfo(criteria);

        var notification = result.EditNotificationInfos.FirstOrDefault();
        if (notification != null)
        {
            notification.Recipients = result.Recipients;
            notification.Platforms = result.Platforms;
            notification.IntegrationMethods = result.IntegrationMethods;
            notification.ConfigSendTime = notification.ConfigSendTime.ConvertFromUTC();
        }

        return notification;
    }

    private async Task<EditNotificationRepository> FetchEditNotificationInfo(ICriteria criteria)
    {
        var (info, recipients, platforms, integrationMethods) = await baseService
            .FetchMultipleAsync<EditNotificationInfo, Recipient, Platform, IntegrationMethodInfo>(criteria);
        var data = new EditNotificationRepository
        {
            EditNotificationInfos = info,
            Recipients = recipients,
            Platforms = platforms,
            IntegrationMethods = integrationMethods
        };

        return data;
    }

    public async Task<CommandResult> EditAsync(EditNotification model)
    {
        var command = new EditNotificationCommand
        {
            InNotificationId = model.NotificationId,
            InRecipientIds = model.RecipientIds,
            InTitle = model.Title,
            InContent = model.Content,
            InConfigSendTime = model.SendImmediately ? DateTimeOffset.UtcNow.DateTime : model.SendDate.ConvertToUTC(),
            InSendImmediately = model.SendImmediately,
            InUserId = model.UserId,
            InPlatformIds = model.Platforms,
            InIntegrationMethodIds = model.InIntegrationMethodIds
        };

        var result = await baseService.ExecuteNonQueryAsync(command);
        if (result.IsSuccessful && model.SendImmediately)
            BackgroundJobAfterCreateNotification(command.InNotificationId);

        return result;
    }

    public async Task<(IEnumerable<Recipient>, IEnumerable<uint>)> GetEditNotificationRecipientsAsync
    (int notificationId, int notificationTypeId, int? productId, string platforms, string recipientIds,
        string integrationMethodIds)
    {
        return await baseService.FetchMultipleAsync<Recipient, uint>(new GetEditNotificationRecipientsCriteria
        {
            InNotificationId = notificationId,
            InNotificationTypeId = notificationTypeId,
            InProductId = productId,
            InPlatformIds = platforms,
            InRecipientIds = recipientIds,
            InIntegrationMethodIds = integrationMethodIds
        });
    }

    public async Task<IEnumerable<Template>> GetTemplatesAsync(int notificationTypeId)
    {
        return await baseService.GetDataAsync<Template>(new GetTemplatesCriteria
        {
            NotificationTypeId = notificationTypeId
        });
    }

    private void BackgroundJobAfterCreateNotification(int notificationId)
    {
        Task.Run(() => { sendNotificationService.SendNotificationImmediatelyAsync(notificationId); });
    }

    public async Task<ResendNotification> GetResendNotificationInfoAsync(int notificationId, bool isResendAll)
    {
        var criteria = new GetResendNotificationInfoCriteria
        {
            InNotificationId = notificationId,
            InIsSendAll = isResendAll
        };

        var result = await FetchResendNotification(criteria);
        var notification = result.ResendNotifications.FirstOrDefault();
        if (notification != null)
        {
            notification.Recipients = result.Recipients;
            notification.Platforms = result.Platforms;
            notification.IntegrationMethods = result.IntegrationMethods;
        }

        return notification;
    }

    private async Task<ResendNotificationRepository> FetchResendNotification(ICriteria criteria)
    {
        var (notifications, recipients, platforms, methods) =
            await baseService.FetchMultipleAsync<ResendNotification, Recipient, Platform, IntegrationMethodInfo>(
                criteria);
        var data = new ResendNotificationRepository
        {
            ResendNotifications = notifications,
            Recipients = recipients,
            Platforms = platforms,
            IntegrationMethods = methods
        };

        return data;
    }

    public async Task<IEnumerable<Notification>> GetUpdatedNotificationAsync(string notificationIds)
    {
        var criteria = new GetUpdatedNotificationCriteria
        {
            NotificationIds = notificationIds
        };
        var notifications = await baseService.GetDataAsync<Notification>(criteria);
        foreach (var notification in notifications)
            notification.ConfigSendTime = notification.ConfigSendTime.ConvertFromUTC();

        return notifications;
    }

    public async Task<IEnumerable<NotificationPaging>> GetErrNotificationsAsync()
    {
        var criteria = new GetAllErrNotificationsCriteria();
        var notifications = await baseService.GetDataAsync<NotificationPaging>(criteria);
        foreach (var notification in notifications)
            notification.ConfigSendTime = notification.ConfigSendTime.ConvertFromUTC();

        return notifications;
    }
}