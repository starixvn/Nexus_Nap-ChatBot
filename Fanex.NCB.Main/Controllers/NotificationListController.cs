namespace Fanex.NCB.SkypeBot.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NAP.Client;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Services;
    using Fanex.NCB.Core.Constants;
    using Fanex.NCB.Main.Hubs;
    using Fanex.NCB.Main.ViewModel;
    using Fanex.NCB.Main.ViewModel.Notification;
    using Fanex.NCB.Main.ViewModel.NotificationList;
    using Fanex.NCB.Models.Common;
    using Fanex.NCB.Models.Notification;
    using Fanex.NCB.NAPResolver.Attributes;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SignalR;
    using static Fanex.NAP.Core.Business.Constant;

    [NapChatBotSuperAuth(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class NotificationListController : Controller
    {
        private readonly ICommonService commonService;
        private readonly INotificationService notificationService;
        private readonly INapSession napSession;
        private readonly IUserService userService;
        private readonly IHubContext<ChatbotHub, IChatbotClient> chatHubContext;

        public NotificationListController(
            ICommonService commonService,
            INotificationService notificationService,
            INapSession napSession,
            IUserService userService,
            IHubContext<ChatbotHub, IChatbotClient> chatHubContext)
        {
            this.commonService = commonService;
            this.notificationService = notificationService;
            this.napSession = napSession;
            this.userService = userService;
            this.chatHubContext = chatHubContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetSettings()
        {
            IEnumerable<NotificationType> notificationTypes = await commonService.GetNotificationTypesAsync();
            IEnumerable<User> senders = await commonService.GetUsersAsync();
            IEnumerable<NotificationStatus> notificationStatuses = await commonService.GetNotificationStatusesAsync();
            IEnumerable<Recipient> recipients = await commonService.GetRecipientsAsync();
            IEnumerable<ConversationChannel> channels = await commonService.GetConversationChannelAsync();

            var viewModel = new NotificationListFilterViewModel
            {
                NotificationTypeIds = string.Join(",", notificationTypes.Select(item => item.NotificationTypeID)),
                NotificationStatusIds = string.Join(",", notificationStatuses.Select(item => item.Id)),
                RecipientIds = string.Join(",", recipients.Select(item => item.RecipientId)),
                ChannelIds = string.Join(",", channels.Select(item => item.ChannelId)),
                NotificationTypes = notificationTypes.Select(item => new MultiSelectViewModel
                {
                    Id = item.NotificationTypeID,
                    Text = item.NotificationTypeName,
                    Checked = true,
                    IsLeaf = true
                }).ToList(),
                Senders = senders,
                Recipients = BuildTreeRecipient(recipients),
                NotificationStatuses = notificationStatuses.Select(item => new MultiSelectViewModel
                {
                    Id = item.Id,
                    Text = item.Name,
                    Checked = true,
                    IsLeaf = true
                }).ToList(),
                Channels = channels.Select(item => new MultiSelectViewModel
                {
                    Id = item.ChannelId,
                    Text = item.ChannelName,
                    Checked = true,
                    IsLeaf = true
                }).ToList()
            };

            return Json(viewModel);
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.NotifiactionList, Policy = Policy.Read, PermissionCache = true)]
        public IActionResult Index()
        {
            ViewBag.HaveEditNotificationPermission = userService.IsHavingPermission(napSession.UserId, NapClientManager.NapSettings.SiteId, FunctionKey.NotifiactionList, Policy.Host);

            return View("~/Views/SkypeBot/NotificationList/Index.cshtml");
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.NotifiactionList, Policy = Policy.Read, PermissionCache = true)]
        public async Task<IActionResult> Get(GetNotificationViewModel filter)
        {
            var result = await notificationService.GetNotificationsAsync(new GetNotifications
            {
                NotificationTypeIds = filter.NotificationTypeIds,
                FromDate = filter.FromDate,
                ToDate = filter.ToDate,
                CreatorId = filter.CreatorId,
                NotificationStatusIds = filter.NotificationStatusIds,
                RecipientIds = filter.RecipientIds,
                ChannelIds = filter.ChannelIds,
                MessageId = filter.MessageId,
                PageIndex = filter.PageIndex,
                PageSize = filter.PageSize
            });

            return Json(new { total = result.Item1, data = result.Item2 });
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.NotifiactionList, Policy = Policy.Read, PermissionCache = true)]
        public async Task<IActionResult> GetNotificationError(int notificationId)
        {
            var result = await notificationService.GetNotificationErrorAsync(notificationId);

            return Json(result);
        }

        [HttpPost]
        [NapChatBotAuth(FunctionKey = FunctionKey.NotifiactionList, Policy = Policy.Host, PermissionCache = true)]
        public async Task<IActionResult> Cancel(int notificationId)
        {
            var result = await notificationService.CancelAsync(notificationId, napSession.UserId);
            if (result.IsSuccessful)
            {
                PushSignalRUpdateNotificationStatus(notificationId.ToString());
            }

            return Json(result);
        }

        [HttpPost]
        [NapChatBotAuth(FunctionKey = FunctionKey.NotifiactionList, Policy = Policy.Host, PermissionCache = true)]
        public async Task<IActionResult> Resolve([FromBody] ResolveNotificationViewModel data)
        {
            var result = await notificationService.ResolveAsync(data.NotificationId, data.ResolvedRemark, napSession.UserId);

            if (result.IsSuccessful)
            {
                PushSignalRUpdateNotificationStatus(data.NotificationId.ToString());
                var errNotifications = await notificationService.GetErrNotificationsAsync();
                PushSignalUpdateErrNotification(errNotifications);
            }
            return Json(result);
        }

        private void PushSignalRUpdateNotificationStatus(string notificationIds)
        {
            Task.Run(() => chatHubContext.Clients.All.UpdateNotificationStatus(notificationIds));
        }

        private void PushSignalUpdateErrNotification(IEnumerable<Notification> errNotifications)
        {
            Task.Run(() => chatHubContext.Clients.All.UpdateErrNotification(errNotifications));
        }

        [NapChatBotAuth(FunctionKey = FunctionKey.NotifiactionList, Policy = Policy.Read, PermissionCache = true)]
        public async Task<IActionResult> GetUpdatedNotification(string notificationIds)
        {
            var result = await notificationService.GetUpdatedNotificationAsync(notificationIds);
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetErrNotifications()
        {
            var result = await notificationService.GetErrNotificationsAsync();
            return Json(result);
        }

        public IEnumerable<MultiSelectViewModel> BuildTreeRecipient(IEnumerable<Recipient> recipients)
        {
            var result = recipients.GroupBy(x => new { x.RecipientTypeId, x.RecipientTypeName }, x => x, (key, list) =>
            {
                var children = list.Where(x => !x.LicenseeTypeId.HasValue).Select(x => new MultiSelectViewModel
                {
                    Id = x.RecipientId,
                    Text = x.RecipientName,
                    Checked = true,
                    IsLeaf = true
                }).ToList();

                children.AddRange(list.Where(x => x.LicenseeTypeId.HasValue).GroupBy(x => new { x.LicenseeTypeId, x.LicenseeTypeName },
                    x => x,
                    (licensee, childs) =>
                    {
                        return new MultiSelectViewModel
                        {
                            Id = licensee.LicenseeTypeId.Value,
                            Text = licensee.LicenseeTypeName,
                            Checked = true,
                            IsLeaf = false,
                            Items = childs.Select(x => new MultiSelectViewModel
                            {
                                Id = x.RecipientId,
                                Text = x.RecipientName,
                                Checked = true,
                                IsLeaf = true
                            }).ToList()
                        };
                    }));

                return new MultiSelectViewModel
                {
                    Id = key.RecipientTypeId,
                    Text = key.RecipientTypeName,
                    Checked = true,
                    IsLeaf = false,
                    Items = children
                };
            });

            return result;
        }
    }
}