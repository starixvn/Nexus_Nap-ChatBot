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
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;
    using static Fanex.NAP.Core.Business.Constant;

    [NapChatBotSuperAuth(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class NotificationListController : Controller
    {
        private readonly INotificationService notificationService;
        private readonly ISignalRService signalRService;
        private readonly INapSession napSession;
        private readonly IUserService userService;

        public NotificationListController(INotificationService notificationService,
            ISignalRService signalRService,
            INapSession napSession,
            IUserService userService)
        {
            this.notificationService = notificationService;
            this.signalRService = signalRService;
            this.napSession = napSession;
            this.userService = userService;
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

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.NotifiactionList, Policy = Policy.Read, PermissionCache = true)]
        public async Task<IActionResult> GetAllErrorNotifications()
        {
            var result = await notificationService.GetAllErrorNotificationsAsync().ConfigureAwait(false);

            return Json(result);
        }

        [HttpPost]
        [NapChatBotAuth(FunctionKey = FunctionKey.NotifiactionList, Policy = Policy.Host, PermissionCache = true)]
        public async Task<IActionResult> Cancel(int notificationId)
        {
            var result = await notificationService.CancelAsync(notificationId, napSession.UserId).ConfigureAwait(false);
            if (result.IsSuccessful)
            {
                PushSignalRToClient(notificationId.ToString());
            }
            return Json(result);
        }

        [HttpPost]
        [NapChatBotAuth(FunctionKey = FunctionKey.NotifiactionList, Policy = Policy.Host, PermissionCache = true)]
        public async Task<IActionResult> Resolve([FromBody]ResolveNotificationViewModel data)
        {
            var result = await notificationService.ResolveAsync(data.NotificationId, data.ResolvedRemark, napSession.UserId).ConfigureAwait(false);
            if (result.IsSuccessful)
            {
                PushSignalRToClient(data.NotificationId.ToString());
            }
            return Json(result);
        }

        private void PushSignalRToClient(string notificationIds)
        {
            Task.Run(() => signalRService.UpdateNotificationStatus(notificationIds));
        }

        [NapChatBotAuth(FunctionKey = FunctionKey.NotifiactionList, Policy = Policy.Read, PermissionCache = true)]
        public async Task<IActionResult> GetUpdatedNotification(string notificationIds)
        {
            var result = await notificationService.GetUpdatedNotificationAsync(notificationIds);
            return Json(result);
        }
    }
}