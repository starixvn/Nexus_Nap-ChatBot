namespace Fanex.NCB.Services.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NCB.Criteria.Notification;
    using Fanex.NCB.Models.Common;
    using Fanex.NCB.Models.Notification;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Models.OIC;
    using Fanex.NCB.Shared.Services.Interfaces;
    using NSubstitute;
    using Xunit;

    public class NotificationServiceTests
    {
        private IBaseService baseService;
        private ISendNotificationService sendNotificationService;

        [Fact]
        public async Task CreateAsync_ValidData_ReturnSuccessResult()
        {
            var fixture = new Fixture();
            var model = fixture.Create<CreateNotification>();
            var service = GetService();
            baseService.ExecuteNonQueryAsync(Arg.Any<CreateNewNotificationCommand>())
                .Returns(new CommandResult { Code = 0 });

            var (result, notiIds) = await service.CreateAsync(model);

            Assert.True(result.IsSuccessful);
            Assert.Equal(string.Empty, notiIds);
        }

        [Fact]
        public async Task CreateAsync_SendImmiedatelyIsTrue_SendNotificationImmediatelyIsCalled()
        {
            var service = GetService();
            var model = new CreateNotification
            {
                SendImmediately = true
            };

            var command = new CreateNewNotificationCommand();

            baseService.ExecuteNonQueryAsync(Arg.Do<CreateNewNotificationCommand>(x => command = x))
                .Returns(new CommandResult { Code = 0 })
                .AndDoes(x => command.NotificationIDLst = "[{\"NotificationId\":1},{\"NotificationId\":2}]");

            var (result, notiIds) = await service.CreateAsync(model);

            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public async Task GetRecipientsAsync_ValidData_ReturnExpectedResult()
        {
            var service = GetService();

            var fixture = new Fixture();
            int notificationTypeId = fixture.Create<int>();
            int? productId = fixture.Create<int?>();
            string integrationMethodIds = fixture.Create<string>();
            string channellIds = fixture.Create<string>();
            string platforms = fixture.Create<string>();
            string recipientIds = fixture.Create<string>();

            var recipients = fixture.Create<IEnumerable<Recipient>>();
            var selectedRecipient = fixture.Create<IEnumerable<uint>>();
            var tuple = (recipients, selectedRecipient);

            var criteria = new GetRecipientsCriteria();

            baseService.FetchMultipleAsync<Recipient, uint>(Arg.Do<GetRecipientsCriteria>(x => criteria = x))
                .Returns(tuple);

            var result = await service.GetRecipientsAsync(notificationTypeId, platforms, recipientIds, productId, integrationMethodIds, channellIds);

            Assert.Equal(notificationTypeId, criteria.InNotificationTypeId);
            Assert.Equal(productId, criteria.InProductId);
            Assert.Equal(integrationMethodIds, criteria.InIntegrationMethodIds);
            Assert.Equal(platforms, criteria.InPlatformIds);
            Assert.Equal(channellIds, criteria.InChannelIds);
            Assert.Equal(recipientIds, criteria.InRecipientIds);
            Assert.Equal(recipients.Count(), result.Item1.Count());
            Assert.Equal(recipients, result.Item1);
            Assert.Equal(selectedRecipient, result.Item2);
        }

        [Fact]
        public async Task GetNotificationsAsync_ValidData_ReturnExpectedResult()
        {
            var service = GetService();

            var fixture = new Fixture();
            var model = fixture.Create<GetNotifications>();
            var notifications = fixture.Create<IEnumerable<Notification>>();
            var total = fixture.Create<long>();

            var criteria = new GetNotificationsCriteria();

            baseService.FetchMultipleAsync<long, Notification>(Arg.Do<GetNotificationsCriteria>(x => criteria = x))
                .Returns((new List<long> { total }, notifications));

            var result = await service.GetNotificationsAsync(model);

            Assert.Equal(total, result.Item1);
            Assert.Equal(notifications.Count(), result.Item2.Count());
            Assert.Equal(model.NotificationTypeIds, criteria.InNotificationTypeIds);
            Assert.Equal(model.CreatorId, criteria.InCreatorId);
            Assert.Equal(model.ProductIds, criteria.InProductIds);
            Assert.Equal(model.FromDate, criteria.InConfigSendTimeFrom);
            Assert.Equal(model.ToDate, criteria.InConfigSendTimeTo);
            Assert.Equal(model.NotificationStatusIds, criteria.InNotificationStatusId);
            Assert.Equal(model.RecipientIds, criteria.InRecipientIds);
            Assert.Equal(model.PageIndex, criteria.PageIndex);
            Assert.Equal(model.PageSize, criteria.PageSize);
        }

        [Fact]
        public async Task CancelAsync_ValidData_ReturnSuccessResult()
        {
            var service = GetService();

            var fixture = new Fixture();
            int notificationId = fixture.Create<int>();
            int userId = fixture.Create<int>();

            var command = new CancelNotificationCommand();

            baseService.ExecuteNonQueryAsync(Arg.Do<CancelNotificationCommand>(x => command = x)).Returns(new CommandResult { Code = 0 });

            var result = await service.CancelAsync(notificationId, userId);

            Assert.True(result.IsSuccessful);
            Assert.Equal(notificationId, command.NotificationId);
            Assert.Equal(userId, command.UserId);
        }

        [Fact]
        public async Task GetNotificationErrorAsync_ValidData_ReturnExpectedResult()
        {
            var service = GetService();

            var fixture = new Fixture();
            var notificationId = fixture.Create<int>();

            var report = fixture.Create<NotificationErrorReport>();

            var criteria = new GetNotificationErrorDetailCriteria();

            baseService.FetchMultipleAsync<NotificationErrorReport, string, NotificationErrorDetail>
                (Arg.Do<GetNotificationErrorDetailCriteria>(x => criteria = x))
                .Returns((new List<NotificationErrorReport> { report }, report.SuccessRecipients, report.ErrorDetails));

            var result = await service.GetNotificationErrorAsync(notificationId);

            Assert.Equal(notificationId, criteria.NotificationId);
            Assert.Equal(report.NotificationId, result.NotificationId);
            Assert.Equal(report.ResolvedBy, result.ResolvedBy);
            Assert.Equal(report.ResolvedByUserName, result.ResolvedByUserName);
            Assert.Equal(report.ResolvedRemark, result.ResolvedRemark);
            Assert.Equal(report.StatusId, result.StatusId);
            Assert.Equal(report.Title, result.Title);
            Assert.Equal(report.ConfigSendTime, result.ConfigSendTime);
            Assert.Equal(report.SuccessRecipients, result.SuccessRecipients);
        }

        [Fact]
        public async Task ResolveAsync_ValidData_ReturnSuccessResult()
        {
            var service = GetService();

            var fixture = new Fixture();
            var notificationId = fixture.Create<int>();
            var userId = fixture.Create<int>();
            var resolvedRemark = fixture.Create<string>();

            var command = new ResolveNotificationCommand();

            baseService.ExecuteNonQueryAsync(Arg.Do<ResolveNotificationCommand>(x => command = x)).Returns(new CommandResult { Code = 0 });

            var result = await service.ResolveAsync(notificationId, resolvedRemark, userId);

            Assert.True(result.IsSuccessful);
            Assert.Equal(notificationId, command.NotificationId);
            Assert.Equal(userId, command.UserId);
            Assert.Equal(resolvedRemark, command.ResolvedRemark);
        }

        [Fact]
        public async Task GetEditNotificationInfoAsync_ValidData_ReturnExpectedResult()
        {
            var service = GetService();

            var fixture = new Fixture();
            var notificationId = fixture.Create<int>();

            var expectedNotification = fixture.Create<EditNotificationInfo>();

            var criteria = new GetEditNotificationInfoCriteria();

            baseService.FetchMultipleAsync<EditNotificationInfo, Recipient, Platform>
                (Arg.Do<GetEditNotificationInfoCriteria>(x => criteria = x))
                .Returns((new List<EditNotificationInfo> { expectedNotification }, expectedNotification.Recipients, expectedNotification.Platforms));

            var result = await service.GetEditNotificationInfoAsync(notificationId);

            Assert.Equal(notificationId, criteria.InNotificationId);
            Assert.Equal(expectedNotification.NotificationId, result.NotificationId);
            Assert.Equal(expectedNotification.NotificationTypeId, result.NotificationTypeId);
            Assert.Equal(expectedNotification.Content, result.Content);
            Assert.Equal(expectedNotification.Title, result.Title);
            Assert.Equal(expectedNotification.ConfigSendTime, result.ConfigSendTime);
            Assert.Equal(expectedNotification.Platforms, result.Platforms);
        }

        [Fact]
        public async Task EditAsync_ValidData_ReturnSuccessResult()
        {
            var service = GetService();

            var fixture = new Fixture();
            var model = fixture.Create<EditNotification>();

            var command = new EditNotificationCommand();

            baseService.ExecuteNonQueryAsync(Arg.Do<EditNotificationCommand>(x => command = x)).Returns(new CommandResult { Code = 0 });

            var result = await service.EditAsync(model);

            Assert.True(result.IsSuccessful);
            Assert.Equal(model.NotificationId, command.InNotificationId);
            Assert.Equal(model.RecipientIds, command.InRecipientIds);
            Assert.Equal(model.Title, command.InTitle);
            Assert.Equal(model.Content, command.InContent);
            Assert.Equal(model.UserId, command.InUserId);
            Assert.Equal(model.SendImmediately, command.InSendImmediately);
            Assert.Equal(model.Platforms, command.InPlatformIds);
        }

        [Fact]
        public async Task GetEditNotificationRecipientsAsync_ValidData_ReturnExpectedResult()
        {
            var service = GetService();

            var fixture = new Fixture();
            var notificationId = fixture.Create<int>();
            var notificationTypeId = fixture.Create<int>();
            var productId = fixture.Create<int?>();
            var platforms = fixture.Create<string>();
            var recipientIds = fixture.Create<string>();
            var inIntegrationMethodIds = fixture.Create<string>();

            var recipients = fixture.Create<IEnumerable<Recipient>>();
            var selectedRecipient = fixture.Create<IEnumerable<uint>>();

            var tuple = (recipients, selectedRecipient);

            var criteria = new GetEditNotificationRecipientsCriteria();

            baseService.FetchMultipleAsync<Recipient, uint>(Arg.Do<GetEditNotificationRecipientsCriteria>(x => criteria = x)).Returns(tuple);

            var result = await service.GetEditNotificationRecipientsAsync(notificationId, notificationTypeId, productId, platforms, recipientIds, inIntegrationMethodIds);

            Assert.Equal(notificationId, criteria.InNotificationId);
            Assert.Equal(notificationTypeId, criteria.InNotificationTypeId);
            Assert.Equal(productId, criteria.InProductId);
            Assert.Equal(platforms, criteria.InPlatformIds);
            Assert.Equal(recipients.Count(), result.Item1.Count());
            Assert.Equal(recipientIds, criteria.InRecipientIds);
            Assert.Equal(inIntegrationMethodIds, criteria.InIntegrationMethodIds);
            Assert.Equal(recipients, result.Item1);
            Assert.Equal(selectedRecipient, result.Item2);
        }

        [Fact]
        public async Task GetTemplatesAsync_ValidData_ReturnExpectedResult()
        {
            var service = GetService();

            var fixture = new Fixture();
            var notificationTypeId = fixture.Create<int>();
            var templates = fixture.Create<IEnumerable<Template>>();

            var criteria = new GetTemplatesCriteria();

            baseService.GetDataAsync<Template>(Arg.Do<GetTemplatesCriteria>(x => criteria = x)).Returns(templates);

            var result = await service.GetTemplatesAsync(notificationTypeId);

            Assert.Equal(notificationTypeId, criteria.NotificationTypeId);
            Assert.Equal(templates.Count(), result.Count());
        }

        [Fact]
        public async Task GetResendNotificationInfoAsync_WhenCalled_ReturnExpectedResult()
        {
            var service = GetService();

            var fixture = new Fixture();
            var notificationId = fixture.Create<int>();
            var isSendAll = fixture.Create<bool>();
            var notification = fixture.Create<IEnumerable<ResendNotification>>();
            var recipients = fixture.Create<IEnumerable<Recipient>>();
            var platforms = fixture.Create<IEnumerable<Platform>>();
            var valueTuple = new ValueTuple<IEnumerable<ResendNotification>, IEnumerable<Recipient>, IEnumerable<Platform>>(notification, recipients, platforms);
            var expected = notification.FirstOrDefault();
            expected.Recipients = recipients;
            expected.Platforms = platforms;

            var criteria = new GetResendNotificationInfoCriteria { InNotificationId = notificationId, InIsSendAll = isSendAll };

            baseService.FetchMultipleAsync<ResendNotification, Recipient, Platform>(Arg.Do<GetResendNotificationInfoCriteria>(x => criteria = x)).Returns(valueTuple);

            var result = await service.GetResendNotificationInfoAsync(notificationId, isSendAll);

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task GetUpdatedNotificationAsync_WhenCalled_ReturnExpectedResult()
        {
            var service = GetService();

            var fixture = new Fixture();
            var notifications = fixture.Create<IEnumerable<Notification>>();
            baseService.GetDataAsync<Notification>(Arg.Any<GetUpdatedNotificationCriteria>()).Returns(notifications);

            var result = await service.GetUpdatedNotificationAsync("1,2,3,4");

            Assert.Equal(notifications, result);
        }

        [Fact]
        public async Task GetErrNotificationsAsync_WhenCalled_ReturnExpectedResult()
        {
            var service = GetService();

            var fixture = new Fixture();
            var notifications = fixture.Create<IEnumerable<NotificationPaging>>();
            baseService.GetDataAsync<NotificationPaging>(Arg.Any<GetAllErrNotificationsCriteria>()).Returns(notifications);

            var result = await service.GetErrNotificationsAsync();

            Assert.Equal(notifications, result);
        }

        private NotificationService GetService()
        {
            baseService = Substitute.For<IBaseService>();
            sendNotificationService = Substitute.For<ISendNotificationService>();

            return new NotificationService(baseService, sendNotificationService);
        }
    }
}