namespace Fanex.NCB.SkypeBot.Controllers
{
    using System.Threading.Tasks;
    using Fanex.NAP.Client;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Services;
    using Fanex.NCB.Core.Constants;
    using Fanex.NCB.Main.ViewModel.Notification;
    using Fanex.NCB.Models.Notification;
    using Fanex.NCB.NAPResolver.Attributes;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using static Fanex.NAP.Core.Business.Constant;

    [NapChatBotSuperAuth]
    public class NotificationListController : Controller
    {
        private readonly INotificationService notificationService;
        private readonly INapSession napSession;
        private readonly IUserService userService;

        public NotificationListController(INotificationService notificationService, INapSession napSession, IUserService userService)
        {
            this.notificationService = notificationService;
            this.napSession = napSession;
            this.userService = userService;
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.NotifiactionList, Policy = Policy.Read, PermissionCache = true)]
        public IActionResult Index()
        {
            ViewBag.HaveHostPermission = userService.IsHavingPermission(napSession.UserId, NapClientManager.NapSettings.SiteId, FunctionKey.NotifiactionList, Policy.Host);
            ViewBag.HaveEditNotificationPermission = userService.IsHavingPermission(napSession.UserId, NapClientManager.NapSettings.SiteId, FunctionKey.EditNotification, Policy.Host);

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
                PageIndex = filter.PageIndex,
                PageSize = filter.PageSize
            }).ConfigureAwait(false);

            return Json(new { total = result.Item1, data = result.Item2 });
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.NotifiactionList, Policy = Policy.Read, PermissionCache = true)]
        public async Task<IActionResult> GetNotificationError(int notificationId)
        {
            var result = await notificationService.GetNotificationErrorAsync(notificationId).ConfigureAwait(false);

            return Json(result);
        }

        [HttpPost]
        [NapChatBotAuth(FunctionKey = FunctionKey.EditNotification, Policy = Policy.Host, PermissionCache = true)]
        public async Task<IActionResult> Cancel(int notificationId)
        {
            var result = await notificationService.CancelAsync(notificationId, napSession.UserId).ConfigureAwait(false);

            return Json(result);
        }

        [HttpPost]
        [NapChatBotAuth(FunctionKey = FunctionKey.EditNotification, Policy = Policy.Host, PermissionCache = true)]
        public async Task<IActionResult> Resolve([FromBody]ResolveNotificationViewModel data)
        {
            var result = await notificationService.ResolveAsync(data.NotificationId, data.ResolvedRemark, napSession.UserId).ConfigureAwait(false);

            return Json(result);
        }
    }
}